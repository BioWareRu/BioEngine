using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sitko.Core.Blazor.AntDesignComponents;
using Toolbelt.Blazor.Extensions.DependencyInjection;

namespace BioEngine.Core
{
    public class BioEngineStartup : AntBlazorStartup
    {
        public BioEngineStartup(IConfiguration configuration, IHostEnvironment environment) : base(
            configuration, environment)
        {
        }

        protected override void ConfigureAppServices(IServiceCollection services)
        {
            base.ConfigureAppServices(services);
            services.AddHeadElementHelper();
        }

        protected override void ConfigureBeforeRoutingModulesHook(IApplicationBuilder app)
        {
            app.UseHeadElementServerPrerendering();
            base.ConfigureBeforeRoutingModulesHook(app);
        }
    }
}
