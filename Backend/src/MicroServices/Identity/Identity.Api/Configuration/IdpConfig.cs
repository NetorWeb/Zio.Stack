using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;

namespace Identity.Api.Configuration
{
    public static class IdpConfig
    {
        /// <summary>
        /// 定义哪些用户将使用这个IdentityServer
        /// </summary>
        /// <returns></returns>
        //public static IEnumerable<TestUser> GetUsers()
        //{
        //    return new[]
        //    {
        //        new TestUser
        //        {
        //             SubjectId="10000",
        //            Username = "ZongYu1119",
        //            Password = "12345"
        //        },
        //        new TestUser
        //        {
        //             SubjectId="10001",
        //             Username ="Admin",
        //              Password ="12345"
        //        }
        //    };
        //}

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
                new ApiResource("appcollect", "App Collect Service" , new List<string> {JwtClaimTypes.Role})
                {
                    Scopes = { "appcollect" }
                }
            };
        }

        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new ApiScope[] {
                new ApiScope("appcollect", "App Collect Service")
            };
        }

        public static IEnumerable<Client> GetClients(IConfiguration configuration)
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "client",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = { "appcollect",
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.Address,
                        IdentityServerConstants.StandardScopes.Phone,
                        IdentityServerConstants.StandardScopes.Profile,
                    }
                },

                new Client
                {
                    ClientId = "ro.client",
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