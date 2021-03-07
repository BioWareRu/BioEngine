using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sitko.Core.App.Web;

namespace BioEngine.Admin
{
    public class Startup : BaseStartup<Core.BioEngineApp>
    {
        public Startup(IConfiguration configuration, IHostEnvironment environment) : base(configuration, environment)
        {
        }

        protected override void ConfigureAppServices(IServiceCollection services)
        {
            base.ConfigureAppServices(services);
            services.AddRazorPages(options =>
            {
                options.Conventions.AuthorizeFolder("/");
            });
            services.AddServerSideBlazor().AddCircuitOptions(options =>
            {
                options.DetailedErrors = Environment.IsDevelopment();
            });
            services.AddAntDesign();
            services.AddValidatorsFromAssemblyContaining<Program>();
            services.AddValidatorsFromAssemblyContaining<Core.BioEngineApp>();
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
            endpoints.MapFallbackToPage("/_Host");
        }
    }
}
