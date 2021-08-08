using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Routing;
using Sitko.Core.App.Web;

namespace BioEngine.Site
{
    public class Startup : BaseStartup<Core.BioEngineApp>
    {
        public Startup(IConfiguration configuration, IHostEnvironment environment) : base(configuration, environment)
        {
        }

        protected override void ConfigureAppServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor().AddCircuitOptions(options =>
            {
                options.DetailedErrors = Environment.IsDevelopment();
            });
            services.AddAntDesign();
        }

        protected override void ConfigureAfterRoutingMiddleware(IApplicationBuilder app)
        {
            base.ConfigureAfterRoutingMiddleware(app);
            app.UseAuthentication();
            app.UseAuthorization();
        }

        protected override void ConfigureEndpoints(IApplicationBuilder app, IEndpointRouteBuilder endpoints)
        {
            base.ConfigureEndpoints(app, endpoints);
            endpoints.MapBlazorHub();
            endpoints.MapControllers();
            endpoints.MapFallbackToPage("/_Host");
        }
    }

}
