using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sitko.Core.App;

namespace BioEngine.Core.IPB
{
    public class IPBAdminModule : IPBModule<IPBAdminModuleConfig>
    {
        public IPBAdminModule(IPBAdminModuleConfig config, Application application) : base(config, application)
        {
        }

        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration,
            IHostEnvironment environment)
        {
            base.ConfigureServices(services, configuration, environment);

            // services.AddScoped<IContentPublisher<IPBPublishConfig>, IPBContentPublisher>();
            // services.AddScoped<IPBContentPublisher>();
            // services.AddScoped<IPropertiesOptionsResolver, IPBSectionPropertiesOptionsResolver>();
        }
    }

    public class IPBAdminModuleConfig : IPBModuleConfig
    {
    }
}
