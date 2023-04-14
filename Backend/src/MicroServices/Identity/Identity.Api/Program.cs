using FluentValidation;
using FluentValidation.AspNetCore;
using Identity.Api.Configuration.Consts;
using Identity.Api.Infrastructure.SeedData;
using Identity.Api.Middleware;
using Identity.Api.Services.Account;
using Microsoft.OpenApi.Models;

namespace Identity.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();
            builder.Services.AddScoped<IValidator<LoginInputModel>, LoginInputModelValidator>();
            //注册认证策略
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy(PolicyConst.Admin,
                        policy => policy.RequireAssertion(c => c.User.IsInRole("Administrator")));
                options.AddPolicy(PolicyConst.Manager,
                    policy => policy.RequireAuthenticatedUser());
            });

            builder.Services.AddCustomIdentity(builder.Configuration);

            builder.Services.AddCustomIdentityServer(builder.Configuration);

            builder.Services.AddMasaMinimalAPIs(options =>
            {
                options.PluralizeServiceName = false;
                options.RouteHandlerBuilder = builder =>
                {
                    builder.RequireAuthorization();
                };
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            //添加认证过滤
            builder.Services.ConfigAuthentication(builder.Configuration);

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseCookiePolicy();

            app.UseHttpsRedirection();

            app.UseCustomIdentityServer();

            app.UseAuthentication();
            app.UseAuthorization();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger(c => {

                    c.PreSerializeFilters.Add((swagger, httpReq) =>
                    {
                        swagger.Servers = new List<OpenApiServer> { new OpenApiServer { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}" } };
                    });
                });
                app.UseSwaggerUI(c =>
                {

                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "KnowledgeBaseAPI");
                    //c.IndexStream = () => typeof(Program).GetTypeInfo().Assembly.GetManifestResourceStream("swaggerIndex.html");
                    c.RoutePrefix = "doc";
                    c.OAuthClientId("IdentityServer4");
                });
            }

            app.MapMasaMinimalAPIs();

            try
            {
                var seed = args.Contains("seed");
                if (seed)
                {
                    args = args.Except(new[] { "seed" }).ToArray();

                    var seedData = new EnsureSeedData();
                    seedData.EnsureSeedDataAsync(app.Services).Wait();
                }
            }
            catch (Exception)
            {

                throw;
            }


            app.Run();
        }
    }
}