using Identity.Api.Middleware;

namespace Identity.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddCustomIdentityServer(builder.Configuration);

            builder.Services.AddMasaMinimalAPIs(options =>
            {
                options.PluralizeServiceName = false;
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseCustomIdentityServer();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.MapMasaMinimalAPIs();

            app.Run();
        }
    }
}