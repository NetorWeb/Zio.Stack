using AppCollect.Api.Controllers;
using AppCollect.Api.Infrastructure.Filters;

namespace AppCollect.Api.Middleware;

public static class MvcMiddleware
{
    public static IServiceCollection AddCustomMvc(this IServiceCollection services)
    {
        services.AddControllers(options => { options.Filters.Add<HttpGlobalExceptionFilter>(); })
            .AddApplicationPart(typeof(AppManagerController).Assembly)
            .AddJsonOptions(options => options.JsonSerializerOptions.WriteIndented = true);

        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy",
                builder =>
                    builder
                        .SetIsOriginAllowed((host) => true)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials());
        });
        return services;
    }
}