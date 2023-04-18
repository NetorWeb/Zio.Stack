namespace AppCollect.Api.Middleware;

public static class ConfigurationMiddleware
{
    public static void AddCustomConfiguration(this WebApplicationBuilder builder)
    {
        builder.Configuration.AddConfiguration(GetConfiguration()).Build();
    }

    static IConfiguration GetConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();

        return builder.Build();
    }
}