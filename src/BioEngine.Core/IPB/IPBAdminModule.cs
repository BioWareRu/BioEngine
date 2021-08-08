using Microsoft.Extensions.DependencyInjection;
using Sitko.Core.App;

namespace BioEngine.Core.IPB
{
    public class IPBAdminModule : IPBModule<IpbAdminModuleOptions>
    {
        public override string OptionsKey => "Ipb:Admin";

        public override void ConfigureServices(ApplicationContext context, IServiceCollection services,
            IpbAdminModuleOptions startupOptions)
        {
            base.ConfigureServices(context, services, startupOptions);

            // services.AddScoped<IContentPublisher<IPBPublishConfig>, IPBContentPublisher>();
            // services.AddScoped<IPBContentPublisher>();
            // services.AddScoped<IPropertiesOptionsResolver, IPBSectionPropertiesOptionsResolver>();
        }
    }

    public class IpbAdminModuleOptions : IPBModuleOptions
    {
    }
}
