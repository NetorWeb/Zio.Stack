using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AppCollect.Api.Middleware;

public static class HealthChecksMiddleware
{
    public static IServiceCollection AddCustomHealthChecks(this IServiceCollection services,
        IConfiguration configuration)
    {
        var hcBuilder = services.AddHealthChecks();

        hcBuilder.AddCheck("self", () => HealthCheckResult.Healthy());

        return services;
    }
}