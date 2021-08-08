using AntDesign.ProLayout;
using BioEngine.Core;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sitko.Core.App.Web;

namespace BioEngine.Admin
{
    public class Startup : BioEngineStartup
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
            services.Configure<ProSettings>(settings =>
            {
                settings.NavTheme = "dark";
                settings.Title = "BRCGames";
                settings.FixedHeader = true;
            });
            services.AddValidatorsFromAssemblyContaining<Program>();
            services.AddValidatorsFromAssemblyContaining<BioEngineApp<Startup>>();
        }

        protected override void ConfigureAfterRoutingMiddleware(IApplicationBuilder app)
        {
            base.ConfigureAfterRoutingMiddleware(app);
            app.ConfigureLocalization("ru-RU");
            app.UseAuthentication();
            app.UseAuthorization();
        }
    }
}
