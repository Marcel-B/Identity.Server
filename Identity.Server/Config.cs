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
    }
}
