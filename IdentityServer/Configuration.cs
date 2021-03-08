using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer
{
    public static class Configuration
    {
        public static IEnumerable<ApiResource> GetApis() =>
            new List<ApiResource> { 
                new ApiResource
                {
                    Name = "ApiOne",
                    Scopes= { "ApiOne" },
                    UserClaims = { "LDAP" }
                    
                } 
            };

        public static IEnumerable<Client> GetClients() =>
            new List<Client>
            {
                new Client
                {
                    ClientId = "angular",
                    RedirectUris = { "http://localhost:4200" },
                    AllowedGrantTypes = GrantTypes.Code,
                    RequireClientSecret = false,
                    AllowedScopes = { IdentityServerConstants.StandardScopes.OpenId , "ApiOne" },
                    AllowAccessTokensViaBrowser = true,
                    AllowedCorsOrigins = { "http://localhost:4200" },
                    RequireConsent = false,
                    AccessTokenLifetime = 1,
                    PostLogoutRedirectUris = { "http://localhost:4200" },
                    RequirePkce = true,

                }
            };

        public static IEnumerable<ApiScope> GetScopes() =>
            new List<ApiScope> {
                new ApiScope("ApiOne"),
            };

        public static IEnumerable<IdentityResource> GetIdentityResources() =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResource
                {
                    Name = "rc.scope",
                    UserClaims =
                    {
                        "rc.grandma"
                    }
                }
            };
    }
}
