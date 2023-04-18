using Microsoft.AspNetCore.Builder;
using Serilog;

namespace Log.Serilog.Middleware
{
    public static class SerilogMiddleware
    {
        public static ILogger AddCustomSerilog(this WebApplicationBuilder builder)
        {
            var appName = builder.Configuration["AppName"];
            var seqServerUrl = builder.Configuration["Serilog:SeqServerUrl"];
            var logstashUrl = builder.Configuration["Serilog:LogstashUrl"];

            global::Serilog.Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.WithProperty("ApplicationContext", appName)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.Seq(string.IsNullOrWhiteSpace(seqServerUrl) ? "http://seq" : seqServerUrl)
                .WriteTo.Http(string.IsNullOrWhiteSpace(logstashUrl) ? "http://localhost:8080" : logstashUrl, null)
                .ReadFrom.Configuration(builder.Configuration)
                .CreateLogger();

            builder.Host.UseSerilog();

            return global::Serilog.Log.Logger;
        }
    }
}
