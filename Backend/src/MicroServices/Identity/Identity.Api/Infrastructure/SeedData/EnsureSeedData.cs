using Identity.Api.Configuration;
using Identity.Api.Entities;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Identity.Api.Infrastructure.SeedData
{
    public class EnsureSeedData
    {
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<ApplicationRole> _roleManager;

        public async Task EnsureSeedDataAsync(IServiceProvider services)
        {
            using var scope = services.GetRequiredService<IServiceScopeFactory>().CreateScope();
            _userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            _roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

            await EnsureIdentitySeedAsync();
            await EnsureIdentityServerSeedAsync(scope.ServiceProvider);
        }

        private async Task EnsureIdentitySeedAsync()
        {
            if (await _roleManager.FindByNameAsync(AuthorizationConsts.AdministrationRole) == null)
            {
                var role = new ApplicationRole() { Name = AuthorizationConsts.AdministrationRole, NormalizedName = "Administrator" };
                var result = await _roleManager.CreateAsync(role);
                if (!result.Succeeded)
                {
                    throw new Exception("初始化默认角色失败:" + result.Errors.SelectMany(e => e.Description));
                }
            }

            if (await _userManager.FindByNameAsync(AuthorizationConsts.AdministrationUser) == null)
            {
                var defaultUser = new ApplicationUser
                {
                    UserName = AuthorizationConsts.AdministrationUser,
                    Name = "admin",
                    SecurityStamp = "admin",
                };

                var result = await _userManager.CreateAsync(defaultUser, "admin@123...");

                if (!result.Succeeded)

                {
                    throw new Exception("初始默认用户失败");
                }
                await _userManager.AddToRoleAsync(defaultUser, AuthorizationConsts.AdministrationRole);
            }
        }

        private async Task EnsureIdentityServerSeedAsync(IServiceProvider services)
        {
            var configiration = services.GetService<IConfiguration>();

            services.GetRequiredService<CustomPersistedGrantDbContext>().Database.Migrate();
            var configurationDbContext = services.GetRequiredService<CustomConfigurationDbContext>();

            if (!configurationDbContext.Clients.Any())
            {
                foreach (var client in IdpConfig.GetClients(configiration))
                {
                    await configurationDbContext.Clients.AddAsync(client.ToEntity());
                }
                await configurationDbContext.SaveChangesAsync();
            }

            if (!configurationDbContext.ApiResources.Any())
            {
                foreach (var api in IdpConfig.GetApiResources())
                {
                    await configurationDbContext.ApiResources.AddAsync(api.ToEntity());
                }
                await configurationDbContext.SaveChangesAsync();
            }

            if (!configurationDbContext.ApiScopes.Any())
            {
                foreach (var api in IdpConfig.GetApiScopes())
                {
                    await configurationDbContext.ApiScopes.AddAsync(api.ToEntity());
                }
                await configurationDbContext.SaveChangesAsync();
            }

            if (!configurationDbContext.IdentityResources.Any())
            {
                foreach (var identity in IdpConfig.GetIdentityResources())
                {
                    await configurationDbContext.IdentityResources.AddAsync(identity.ToEntity());
                }
                await configurationDbContext.SaveChangesAsync();
            }
        }
    }
}