using IdentityServer4.Models;
using System.Collections.Generic;

namespace com.b_velop.Identity.Server
{
    public class Config
    {
        public static IEnumerable<ApiResource> Apis =>
             new List<ApiResource>
             {
                    new ApiResource{
                        Name = "slipways.api",
                        DisplayName = "Slipways API",

                        Scopes =
                        {
                            new Scope
                            {
                                Name = "slipways.api.reader",
                                DisplayName = "Slipways API reader"
                            },
                            new Scope
                            {
                                Name = "slipways.api.allaccess",
                                DisplayName = "Slipways API all access"
                            }
                        }
                    }
             };

        public static IEnumerable<Client> Clients =>
        new List<Client>
        {
            new Client{
                ClientId = "slipways.ios",
                ClientName = "slipways ios appication",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                AllowedScopes = { "slipways.api.reader" }
            },
            new Client{
                ClientId = "slipways.web",
                ClientName = "slipways web client",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                AllowedScopes = { "slipways.api.allaccess" }
            },
            new Client{
                ClientId = "slipways.android",
                ClientName = "slipways android client",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                AllowedScopes = { "slipways.api.reader" }
            },
            new Client{
                ClientId = "slipways.graphql",
                ClientName = "slipways graphql client",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                AllowedScopes = { "slipways.api.reader" }
            },
        };

        public static IEnumerable<IdentityResource> Ids =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };
    }
}
