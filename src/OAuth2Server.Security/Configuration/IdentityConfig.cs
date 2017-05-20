using System.Collections.Generic;
using IdentityServer4.Models;

namespace OAuth2Server.Security.Configuration
{
    public class IdentityConfig
    {
        public static List<Client> GetClients(string redirect, string logoutRedirect)
        {
            return new List<Client>
            {
                new Client
                {
                    ClientName = "Static Client",
                    ClientId = "Example",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    RequireConsent = false,
                    AccessTokenLifetime = 1296000, // 15 days
                    AllowAccessTokensViaBrowser = true,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    RedirectUris = new List<string>
                    {
                        redirect
                    },
                    PostLogoutRedirectUris = new List<string>
                    {
                        logoutRedirect
                    },
                    AllowedCorsOrigins = new List<string>
                    {
                        "http://localhost:4200", // front end Angular 2 portal
                    },
                    AllowedScopes = new List<string>
                    {
                        "openid",
                        "accounts",
                        "accountService",
                        "accountservicescope",
                        "role",
                        "profile",
                        "email",
                    }
                }
            };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            // standard OpenID Connect scopes
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResource("accountservicescope",new []{ "role", "accounts", "accountService"} )
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("accountService")
                {
                    ApiSecrets =
                    {
                        new Secret("accountServiceSecret".Sha256())
                    },
                    Scopes =
                    {
                        new Scope
                        {
                            Name = "accountservicescope",
                            DisplayName = "Manage & view Authorized Users and registered Clients"
                        }
                    },
                    UserClaims = { "role", "accounts", "accountService"}
                }
            };
        }
    }
}