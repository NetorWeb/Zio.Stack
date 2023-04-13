using Identity.Api.Configuration;
using Identity.Api.Infrastructure;
using Identity.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Identity.Api.Middleware
{
    public static class IdentityServerMiddleware
    {
        public static IServiceCollection AddCustomIdentityServer(this IServiceCollection services, IConfiguration configuration)
        {
            //var migrationAssembly = typeof(Program).GetTypeInfo().Assembly.GetName().Name;

            services.AddAuthentication();
            services.AddAuthorization();

            services.AddDbContext<ApplicationDbContext>(opt =>
            {
                opt.UseMySql(configuration["ConnectionString"], new MySqlServerVersion(new Version(8, 0, 31)));
            });

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddUserManager<MyUserManager>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddIdentityServer(options =>
            {
                options.Authentication.CookieLifetime = TimeSpan.FromHours(2);

                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
            })
                .AddDeveloperSigningCredential()
                .AddInMemoryIdentityResources(IdpConfig.GetIdentityResources())
                .AddInMemoryClients(IdpConfig.GetClients(configuration))
                .AddInMemoryApiScopes(IdpConfig.GetScope())
                .AddInMemoryApiResources(IdpConfig.GetApiResources())    
                //.AddResourceOwnerValidator<MyResourceOwnerPasswordValidator>() //这句可以打开自主验证登录用户
                //.AddProfileService<MyProfileService>()
                .AddAspNetIdentity<ApplicationUser>()
    ;

            return services;
        }


        public static void UseCustomIdentityServer(this WebApplication app)
        {
            app.UseIdentityServer();
            app.UseAuthorization();
        }

    }
}