using System.IdentityModel.Tokens.Jwt;

namespace AppCollect.Api.Middleware;

public static class AuthenticationMiddleware
{
    public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        // prevent from mapping "sub" claim to nameidentifier.
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");
        var identityUrl = configuration.GetValue<string>("IdentityUrl");
        services.AddAuthentication("Bearer").AddJwtBearer(options =>
        {
            options.Authority = identityUrl;
            options.RequireHttpsMetadata = false;
            options.Audience = "appcollect";
            options.TokenValidationParameters.ValidateAudience = false;
        });
        return services;
    }
}