using Identity.Api.Configuration;
using Identity.Api.Entities;
using Identity.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Identity.Api.Middleware
{
    public static class IdentityServerMiddleware
    {
        public static IServiceCollection AddCustomIdentityServer(this IServiceCollection services, IConfiguration configuration)
        {
            var migrationAssembly = typeof(Program).GetTypeInfo().Assembly.GetName().Name;

            services.AddIdentityServer(options =>
            {

                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
            })
                .AddDeveloperSigningCredential()

                .AddConfigurationStore<CustomConfigurationDbContext>(opt =>
                {
                    opt.ConfigureDbContext = builder =>
                    {
                        builder.UseMySql(configuration["ConnectionString"], new MySqlServerVersion(new Version(8, 0, 31)),
                            sql =>
                            {
                                sql.MigrationsAssembly(migrationAssembly);
                            });
                    };
                })
                .AddOperationalStore<CustomPersistedGrantDbContext>(opt =>
                {
                    opt.ConfigureDbContext = builder =>
                    {
                        builder.UseMySql(configuration["ConnectionString"], new MySqlServerVersion(new Version(8, 0, 31)), sql =>
                        {
                            sql.MigrationsAssembly(migrationAssembly);
                        });
                    };
                    opt.EnableTokenCleanup = true;
                })
                .AddResourceOwnerValidator<CustomResourceOwnerPasswordValidator<ApplicationUser, ApplicationRole>>() //这句可以打开自主验证登录用户
                .AddProfileService<CustomProfileService<ApplicationUser>>()
                .AddAspNetIdentity<ApplicationUser>()

                ;

            return services;
        }

        public static void UseCustomIdentityServer(this WebApplication app)
        {
            app.UseIdentityServer();
        }
    }
}