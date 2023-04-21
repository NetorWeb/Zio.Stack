namespace AppCollect.Api.Middleware;

public static class AuthorizationMiddleware
{
    public static IServiceCollection AddCustomAuthorization(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("ApiScope", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim("scope", "appcollect");
            });
        });
        return services;
    }
}