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



            services.AddDbContext<ApplicationDbContext>(opt =>
            {
                opt.UseMySql(configuration["ConnectionString"], new MySqlServerVersion(new Version(8, 0, 31)));
            });

            services.AddIdentity<ApplicationUser, IdentityRole>(opt =>
            {
                opt.Password.RequireLowercase = false;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireUppercase = false;
                opt.Password.RequiredLength = 6;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddInMemoryIdentityResources(IdpConfig.GetIdentityResources())
                .AddInMemoryClients(IdpConfig.GetClients(configuration))
                .AddInMemoryApiScopes(IdpConfig.GetScope())
                .AddInMemoryApiResources(IdpConfig.GetApiResources())    
                .AddResourceOwnerValidator<MyResourceOwnerPasswordValidator<ApplicationUser>>() //这句可以打开自主验证登录用户
                //.AddProfileService<MyProfileService>()
                .AddAspNetIdentity<ApplicationUser>()
                
                //.AddTestUsers(IdpConfig.GetUsers().ToList())
                ;

            return services;
        }


        public static void UseCustomIdentityServer(this WebApplication app)
        {
            app.UseIdentityServer();

        }

    }
}