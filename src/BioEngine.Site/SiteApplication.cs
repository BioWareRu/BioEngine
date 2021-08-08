using System;
using BioEngine.Core.IPB;

namespace BioEngine.Site
{
    public class SiteApplication : Core.BioEngineApp
    {
        public SiteApplication(string[] args) : base(args)
        {
            AddPostgresDb()
                .AddS3Storage()
                .AddIpbUsers(true)
                .AddModule<IPBSiteModule, IPBSiteModuleConfig>((configuration, env, moduleConfig) =>
                {
                    if (!Uri.TryCreate(configuration["BE_IPB_URL"], UriKind.Absolute, out var ipbUrl))
                    {
                        throw new ArgumentException($"Can't parse IPB url; {configuration["BE_IPB_URL"]}");
                    }

                    moduleConfig.Url = ipbUrl;


                    moduleConfig.ApiReadonlyKey = configuration["BE_IPB_API_READONLY_KEY"];
                });
        }
    }
}
