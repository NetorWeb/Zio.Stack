using Identity.Api.Configuration.Consts;
using Identity.Api.Infrastructure.SeedData;
using Identity.Api.Middleware;
using Log.Serilog.Middleware;
using Microsoft.OpenApi.Models;

namespace Identity.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            builder.AddCustomConfiguration();

            var log = builder.AddCustomSerilog();

            // Add services to the container.

            //builder.Services.AddCustomFluentValidatation();
            //注册认证策略
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy(PolicyConst.Admin,
                        policy => policy.RequireAssertion(c => c.User.IsInRole("Administrator")));
                options.AddPolicy(PolicyConst.Manager,
                    policy => policy.RequireAuthenticatedUser());
            });

            builder.Services.AddSameSiteCookiePolicy();

            builder.Services.AddCustomIdentity(builder.Configuration);

            builder.Services.AddCustomIdentityServer(builder.Configuration);

            builder.Services.AddControllersWithViews();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            //添加认证过滤
            builder.Services.ConfigAuthentication(builder.Configuration);

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseStaticFiles();

            app.UseCookiePolicy();

            app.UseRouting();

            //app.UseHttpsRedirection();

            app.UseCors("AllowAnyDomain");


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

            app.MapControllers();

            try
            {
                var seed = args.Contains("seed");
                if (seed)
                {
                    log.Information("添加种子数据...");
                    //args = args.Except(new[] { "seed" }).ToArray();

                    var seedData = new EnsureSeedData();
                    seedData.EnsureSeedDataAsync(app.Services).Wait();
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "添加种子数据出错!");
                throw;
            }


            app.Run();
        }
    }
}