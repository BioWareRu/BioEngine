using System.Reflection;
using System.Security.Claims;
using BioEngine.Core.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Sitko.Core.App;
using Sitko.Core.App.Logging;
using Sitko.Core.Blazor.AntDesignComponents;
using Sitko.Core.Configuration.Vault;
using Sitko.Core.Db.Postgres;
using Sitko.Core.ElasticStack;
using Sitko.Core.Repository.EntityFrameworkCore;
using Sitko.Core.Storage.ImgProxy;
using Sitko.Core.Storage.Metadata.Postgres;
using Sitko.Core.Storage.S3;

namespace BioEngine.Core
{
    using Sitko.Core.App.Localization;

    public abstract class BioEngineApp<TStartup> : AntBlazorApplication<TStartup> where TStartup : AntBlazorStartup
    {
        public static readonly AuthorizationPolicy AdminPolicy = new AuthorizationPolicyBuilder()
            .RequireClaim(ClaimTypes.Role, Roles.AdminRoleName)
            .Build();

        public static readonly AuthorizationPolicy SiteTeamPolicy = new AuthorizationPolicyBuilder()
            .RequireClaim(ClaimTypes.Role, Roles.SiteTeamRoleName)
            .Build();

        protected BioEngineApp(string[] args) : base(args) =>
            this.AddJsonLocalization().AddVaultConfiguration().AddElasticStack();

        protected override void ConfigureLogging(ApplicationContext applicationContext,
            LoggerConfiguration loggerConfiguration,
            LogLevelSwitcher logLevelSwitcher)
        {
            base.ConfigureLogging(applicationContext, loggerConfiguration, logLevelSwitcher);
            ConfigureLogLevel("Microsoft.AspNetCore", LogEventLevel.Warning);
            ConfigureLogLevel("Sitko.Core.Search.ElasticSearch.ElasticSearcher", LogEventLevel.Warning);
            ConfigureLogLevel("Microsoft.EntityFrameworkCore", LogEventLevel.Warning);
        }

        public BioEngineApp<TStartup> AddPostgresDb(Assembly? migrationsAssembly = null)
        {
            this.AddPostgresDatabase<BioDbContext>((_, environment, options) =>
            {
                options.MigrationsAssembly = migrationsAssembly;
                options.EnableNpgsqlPooling = environment.IsDevelopment();
                options.EnableSensitiveLogging = environment.IsDevelopment();
            }).AddEFRepositories<BioDbContext>();
            return this;
        }

        public BioEngineApp<TStartup> AddS3Storage()
        {
            this.AddS3Storage<BRCStorageConfig>()
                .AddPostgresStorageMetadata<BRCStorageConfig>()
                .AddImgProxyStorage<BRCStorageConfig>();
            return this;
        }
    }

    public class BRCStorageConfig : S3StorageOptions
    {
    }
}
