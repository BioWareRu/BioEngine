using BioEngine.Core.IPB.Users;
using BioEngine.Core.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sitko.Core.App;

namespace BioEngine.Core.IPB
{
    public class IPBUsersModule : BaseApplicationModule<IPBUsersModuleConfig>
    {
        public IPBUsersModule(IPBUsersModuleConfig config, Application application) : base(config, application)
        {
        }

        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration,
            IHostEnvironment environment)
        {
            base.ConfigureServices(services, configuration, environment);

            services.AddScoped<IUserDataProvider, IPBUserDataProvider>();
            services.AddScoped<ICurrentUserProvider, IPBCurrentUserProvider>();
            
            services.AddAuthorization(options =>
            {
                options.AddPolicy(BioPolicies.User, builder => builder.RequireAuthenticatedUser());
                foreach (var policy in Config.Policies)
                {
                    options.AddPolicy(policy.Key, policy.Value);
                }
            });
            
            services.AddSingleton(typeof(IPBUsersModuleConfig), Config);
            services.AddSingleton(Config);
            services.AddHttpClient();
            services.AddHttpContextAccessor();

            services.AddIpbOauthAuthentication(Config, environment);
        }
    }
}
