using System.Collections.Generic;
using System.Linq;
using BioEngine.Core.IPB.Users;

namespace BioEngine.Core.IPB
{
    public static class BioEngineExtensions
    {
        public static BioEngineApp AddIpbUsers(this BioEngineApp application, bool isAdmin = false)
        {
            application.AddModule<IPBUsersModule, IPBUsersModuleConfig>((configuration, env, moduleConfig) =>
            {
                bool.TryParse(configuration["BE_IPB_API_DEV_MODE"] ?? "", out var devMode);
                int.TryParse(configuration["BE_IPB_API_ADMIN_GROUP_ID"], out var adminGroupId);
                int.TryParse(configuration["BE_IPB_API_SITE_TEAM_GROUP_ID"], out var siteTeamGroupId);

                var additionalGroupIds = new List<int> {siteTeamGroupId};
                if (!string.IsNullOrEmpty(configuration["BE_IPB_API_ADDITIONAL_GROUP_IDS"]))
                {
                    var ids = configuration["BE_IPB_API_ADDITIONAL_GROUP_IDS"].Split(',');
                    foreach (var id in ids)
                    {
                        if (int.TryParse(id, out var parsedId))
                        {
                            additionalGroupIds.Add(parsedId);
                        }
                    }
                }

                moduleConfig.DevMode = devMode;
                moduleConfig.AdminGroupId = adminGroupId;
                moduleConfig.AdditionalGroupIds = additionalGroupIds.Distinct().ToArray();
                moduleConfig.CallbackPath = "/login/ipb";
                if (isAdmin)
                {
                    moduleConfig.ApiClientId = configuration["BE_IPB_ADMIN_OAUTH_CLIENT_ID"];
                    moduleConfig.ApiClientSecret = configuration["BE_IPB_ADMIN_OAUTH_CLIENT_SECRET"];
                }
                else
                {
                    moduleConfig.ApiClientId = configuration["BE_IPB_OAUTH_CLIENT_ID"];
                    moduleConfig.ApiClientSecret = configuration["BE_IPB_OAUTH_CLIENT_SECRET"];
                }

                moduleConfig.AuthorizationEndpoint = configuration["BE_IPB_AUTHORIZATION_ENDPOINT"];
                moduleConfig.TokenEndpoint = configuration["BE_IPB_TOKEN_ENDPOINT"];
                moduleConfig.DataProtectionPath = configuration["BE_IPB_DATA_PROTECTION_PATH"];
            });

            return application;
        }
    }
}
