using Identity.Api.Infrastructure.Filters;
using Microsoft.OpenApi.Models;

namespace Identity.Api.Middleware;

public static class SwaggerMiddleware
{
    public static IServiceCollection AddCustomSwagger(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1",new OpenApiInfo()
            {
                Title = "Identity HTTP API",
                Version = "v1",
                Description = "The Identity Service HTTP API"
            });
            options.AddSecurityDefinition("oauth2",new OpenApiSecurityScheme()
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows()
                {
                    Implicit = new OpenApiOAuthFlow()
                    {
                        AuthorizationUrl = new Uri($"{configuration["IdentityServer4:authUrls"]}/connect/authorize"),
                        TokenUrl = new Uri($"{configuration["IdentityServer4:authUrls"]}/connect/token"),
                        Scopes = new Dictionary<string, string>()
                        {
                            {"identityapi", "IdentityServer Manager"}
                        }
                    }
                },
            });
            options.OperationFilter<AuthorizeCheckOperationFilter>();
        });
        
        return services;
    }
}