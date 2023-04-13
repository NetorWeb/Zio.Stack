using IdentityServer4;
using IdentityServer4.Models;

namespace Identity.Api.Configuration
{
    public static class IdpConfig
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Address(),
                new IdentityResources.Phone(),
                new IdentityResources.Email(),
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new[]
            {
                new ApiResource("appcollect", "App Collect Service")
            };
        }

        public static IEnumerable<ApiScope> GetScope()
        {
            return new ApiScope[] {
                new ApiScope("appcollect", "App Collect Service")
            };
        }

        public static IEnumerable<Client> GetClients(IConfiguration configuration)
        {
            return new[]
            {
                new Client
                {
                    ClientId = "client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = //允许当访问的资源
                    {
                        "appcollect",
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.Address,
                        IdentityServerConstants.StandardScopes.Phone,
                        IdentityServerConstants.StandardScopes.Profile,
                    }
                }
            };
        }
    }
}
