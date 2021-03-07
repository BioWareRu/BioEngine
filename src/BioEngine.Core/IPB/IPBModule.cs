using System;
using BioEngine.Core.IPB.Api;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sitko.Core.App;

namespace BioEngine.Core.IPB
{
    public abstract class IPBModule<TConfig> : BaseApplicationModule<TConfig> where TConfig : IPBModuleConfig, new()
    {
        protected IPBModule(TConfig config, Application application) : base(config, application)
        {
        }

        public override void CheckConfig()
        {
            if (Config.Url == null)
            {
                throw new ArgumentException("IPB url is not set");
            }
        }

        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration,
            IHostEnvironment environment)
        {
            services.AddSingleton(typeof(IPBModuleConfig), Config);
            services.AddSingleton(Config);
            services.AddSingleton<IPBApiClientFactory>();
            services.AddHttpClient();
        }
    }

    public abstract class IPBModuleConfig
    {
        public Uri? Url { get; set; } = null;
        public Uri ApiUrl => new Uri($"{Url!}/api");
        public string ApiReadonlyKey { get; set; } = "";
        public string ApiPublishKey { get; set; } = "";
    }
}
