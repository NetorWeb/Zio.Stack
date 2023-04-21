using IdentityModel;
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
                new IdentityResources.Profile()
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new[]
            {
                new ApiResource("identityapi","IdentityServer Manager")
                {
                    Scopes = {"identityapi"}
                },
                new ApiResource("appcollect", "AppCollect Service")
                {
                    Scopes = {"appcollect"}
                }
            };
        }

        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new ApiScope[]
            {
                new ApiScope("identityapi", "IdentityServer Manager"),
                new ApiScope("appcollect", "App Collect Service")
            };
        }

        public static IEnumerable<Client> GetClients(IConfiguration configuration)
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "spa",
                    ClientName = "SPA OpenId Client",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,
                    RedirectUris = { $"{configuration["SpaClient"]}/" },
                    RequireConsent = false,
                    PostLogoutRedirectUris = { $"{configuration["SpaClient"]}/" },
                    AllowedCorsOrigins = { $"{configuration["SpaClient"]}/" },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "appcollect"
                    }
                },
                new Client
                {
                    ClientId = "webrazorpage",
                    ClientName = "Web RazorPage client",
                    ClientSecrets = new List<Secret>()
                    {
                        new Secret("secret".Sha256())
                    },
                    ClientUri = $"{configuration["WebRazorPageClient"]}",
                    AllowedGrantTypes = GrantTypes.Code,
                    AllowAccessTokensViaBrowser = false,
                    RequireConsent = false,
                    AllowOfflineAccess = true,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    RequirePkce = false,
                    RedirectUris = new List<string>()
                    {
                        $"{configuration["WebRazorPageClient"]}/signin-oidc"
                    },
                    PostLogoutRedirectUris = new List<string>()
                    {
                        $"{configuration["WebRazorPageClient"]}/signout-callback-oidc"
                    },
                    AllowedScopes = new List<string>()
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "appcollect"
                    }
                },
                new Client
                {
                    ClientId = "app",
                    ClientName = "App OpenId Client",
                    AllowedGrantTypes = GrantTypes.Hybrid,
                    ClientSecrets = new List<Secret>()
                    {
                        new Secret("secret".Sha256())
                    },
                    RedirectUris = { configuration["AppClientCallback"] },
                    RequireConsent = false,
                    RequirePkce = true,
                    PostLogoutRedirectUris = { $"{configuration["AppClientCallback"]}/Account/Redirecting" },
                    AllowedScopes = new List<string>()
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "appcollect"
                    },
                    AllowAccessTokensViaBrowser = true,
                    AllowOfflineAccess = true
                },
                new Client
                {
                    ClientId = "appcollectswaggerui",
                    ClientName = "AppCollect Swagger UI",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,
                    RedirectUris = { $"{configuration["AppCollectApiClient"]}/swagger/oauth2-redirect.html" },
                    PostLogoutRedirectUris = { $"{configuration["AppCollectApiClient"]}/swagger/" },
                    AllowedScopes =
                    {
                        "appcollect"
                    }
                },
                new Client
                {
                    ClientId = "identityswaggerui",
                    ClientName = "Identity Swagger UI",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,
                    RedirectUris = { $"{configuration["IdentityServer4:authUrls"]}/doc/oauth2-redirect.html" },
                    PostLogoutRedirectUris = { $"{configuration["IdentityServer4:authUrls"]}/doc/" },
                    AllowedScopes =
                    {
                        "identityapi"
                    }
                }
            };
        }
    }
}