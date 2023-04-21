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
        private ILogger<EnsureSeedData> _logger;

        public async Task EnsureSeedDataAsync(IServiceProvider services)
        {
            using var scope = services.GetRequiredService<IServiceScopeFactory>().CreateScope();
            _userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            _roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            _logger = scope.ServiceProvider.GetRequiredService<ILogger<EnsureSeedData>>();

            await EnsureIdentitySeedAsync();
            await EnsureIdentityServerSeedAsync(scope.ServiceProvider);
        }

        private async Task EnsureIdentitySeedAsync()
        {
            if (await _roleManager.FindByNameAsync(AuthorizationConsts.AdministrationRole) == null)
            {
                _logger.LogInformation("初始化默认角色：{Role}", AuthorizationConsts.AdministrationRole);
                var role = new ApplicationRole()
                    { Name = AuthorizationConsts.AdministrationRole, NormalizedName = "Administrator" };
                var result = await _roleManager.CreateAsync(role);
                if (!result.Succeeded)
                {
                    throw new Exception("初始化默认角色失败:" + result.Errors.SelectMany(e => e.Description));
                }
            }

            if (await _userManager.FindByNameAsync(AuthorizationConsts.AdministrationUser) == null)
            {
                _logger.LogInformation("初始化默认用户：{User}", AuthorizationConsts.AdministrationUser);
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

            _logger.LogInformation("初始化Clients");
            if (configurationDbContext.Clients.Any())
            {
                configurationDbContext.Clients.RemoveRange(await configurationDbContext.Clients.ToArrayAsync());
            }

            foreach (var client in IdpConfig.GetClients(configiration))
            {
                await configurationDbContext.Clients.AddAsync(client.ToEntity());
            }

            await configurationDbContext.SaveChangesAsync();

            _logger.LogInformation("初始化ApiResources");
            if (configurationDbContext.ApiResources.Any())
            {
                configurationDbContext.ApiResources.RemoveRange(
                    await configurationDbContext.ApiResources.ToArrayAsync());
            }

            foreach (var api in IdpConfig.GetApiResources())
            {
                await configurationDbContext.ApiResources.AddAsync(api.ToEntity());
            }

            await configurationDbContext.SaveChangesAsync();

            _logger.LogInformation("初始化ApiScopes");
            if (configurationDbContext.ApiScopes.Any())
            {
                configurationDbContext.ApiScopes.RemoveRange(await configurationDbContext.ApiScopes.ToArrayAsync());
            }

            foreach (var api in IdpConfig.GetApiScopes())
            {
                await configurationDbContext.ApiScopes.AddAsync(api.ToEntity());
            }

            await configurationDbContext.SaveChangesAsync();

            _logger.LogInformation("初始化IdentityResources");
            if (configurationDbContext.IdentityResources.Any())
            {
                configurationDbContext.IdentityResources.RemoveRange(await configurationDbContext.IdentityResources
                    .ToArrayAsync());
            }

            foreach (var identity in IdpConfig.GetIdentityResources())
            {
                await configurationDbContext.IdentityResources.AddAsync(identity.ToEntity());
            }

            await configurationDbContext.SaveChangesAsync();
        }
    }
}