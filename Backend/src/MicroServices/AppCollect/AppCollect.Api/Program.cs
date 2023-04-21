using AppCollect.Api.Infrastructure.AutofacModules;
using AppCollect.Api.Middleware;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using HealthChecks.UI.Client;
using Log.Serilog.Middleware;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;


var builder = WebApplication.CreateBuilder(args);

builder.AddCustomConfiguration();
var log = builder.AddCustomSerilog();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddCustomMvc();
builder.Services.AddCustomHealthChecks(builder.Configuration);
builder.Services.AddCustomSwagger(builder.Configuration);
builder.Services.AddCustomAuthentication(builder.Configuration);
builder.Services.AddCustomAuthorization(builder.Configuration);
builder.Services.AddCustomConfigurationOption(builder.Configuration);

var container = new ContainerBuilder();
container.Populate(builder.Services);
container.RegisterModule(new MediatorModule());
container.RegisterModule(new ApplicationModule(builder.Configuration["ConnectionString"]));

var app = builder.Build();

// Configure the HTTP request pipeline.
var pathBase = builder.Configuration["PATH_BASE"];
if (!string.IsNullOrEmpty(pathBase))
{
    log.Debug("Using PATH BASE '{pathBase}'", pathBase);
    app.UsePathBase(pathBase);
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint($"{(!string.IsNullOrEmpty(pathBase) ? pathBase : string.Empty)}/swagger/v1/swagger.json",
        "AppCollect.API V1");
    c.OAuthClientId("appcollectswaggerui");
    c.OAuthAppName("AppCollect Swagger UI");
});

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapDefaultControllerRoute();
app.MapControllers();
app.MapHealthChecks("/hc", new HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.MapHealthChecks("/liveness", new HealthCheckOptions()
{
    Predicate = r => r.Name.Contains("self")
});


app.Run();