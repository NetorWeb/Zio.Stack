using AppCollect.Api.Infrastructure.Filters;
using Microsoft.OpenApi.Models;

namespace AppCollect.Api.Middleware;

public static class SwaggerMiddleware
{
    public static IServiceCollection AddCustomSwagger(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1",new OpenApiInfo()
            {
                Title = "Zio.Stack - AppCollect HTTP API",
                Version = "v1",
                // Description = "The AppCollect Service HTTP API"
            });
            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme()
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows()
                {
                    Implicit = new OpenApiOAuthFlow()
                    {
                        AuthorizationUrl = new Uri($"{configuration.GetValue<string>("IdentityUrl")}/connect/authorize"),
                        TokenUrl = new Uri($"{configuration.GetValue<string>("IdentityUrl")}/connect/token"),
                        Scopes = new Dictionary<string, string>()
                        {
                            {"appcollect", "App Collect Service"}
                        }
                    }
                }
            });
            options.OperationFilter<AuthorizeCheckOperationFilter>();
        });
        return services;
    }
}