using System;
using System.Reflection;
using System.Security.Claims;
using Amazon;
using BioEngine.Core.Data;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Sitko.Backup;
using Sitko.Core.App.Logging;
using Sitko.Core.App.Web;
using Sitko.Core.Db.Postgres;
using Sitko.Core.Repository.EntityFrameworkCore;
using Sitko.Core.Storage;
using Sitko.Core.Storage.ImgProxy;
using Sitko.Core.Storage.Metadata.Postgres;
using Sitko.Core.Storage.S3;

namespace BioEngine.Core
{
    public abstract class BioEngineApp : WebApplication<BioEngineApp>
    {
        public static readonly string AdminRoleName = "admin";

        public static readonly AuthorizationPolicy AdminPolicy = new AuthorizationPolicyBuilder()
            .RequireClaim(ClaimTypes.Role, AdminRoleName)
            .Build();

        public static readonly string SiteTeamRoleName = "siteTeam";

        public static readonly AuthorizationPolicy SiteTeamPolicy = new AuthorizationPolicyBuilder()
            .RequireClaim(ClaimTypes.Role, SiteTeamRoleName)
            .Build();

        protected BioEngineApp(string[] args) : base(args)
        {
        }

        protected override void ConfigureLogging(LoggerConfiguration loggerConfiguration,
            LogLevelSwitcher logLevelSwitcher)
        {
            base.ConfigureLogging(loggerConfiguration, logLevelSwitcher);
            ConfigureLogLevel("Microsoft.AspNetCore", LogEventLevel.Warning);
            ConfigureLogLevel("Sitko.Core.Search.ElasticSearch.ElasticSearcher", LogEventLevel.Warning);
            ConfigureLogLevel("Microsoft.EntityFrameworkCore", LogEventLevel.Warning);
        }

        public BioEngineApp AddPostgresDb(bool enablePooling = true, Assembly? migrationsAssembly = null)
        {
            return AddModule<PostgresModule<BioDbContext>, PostgresDatabaseModuleConfig<BioDbContext>>(
                    (configuration, env, moduleConfig) =>
                    {
                        var builder = configuration.GetPostgresConnectionStringBuilder();
                        moduleConfig.Host = builder.Host!;
                        moduleConfig.Username = builder.Username!;
                        moduleConfig.Password = builder.Password!;
                        moduleConfig.Port = builder.Port;
                        moduleConfig.Database = builder.Database!;
                        moduleConfig.MigrationsAssembly = migrationsAssembly;
                        moduleConfig.EnableNpgsqlPooling = env.IsDevelopment();
                        moduleConfig.EnableSensitiveLogging = env.IsDevelopment();
                    }
                )
                .AddModule<EFRepositoriesModule<BioEngineApp>, EFRepositoriesModuleConfig>(
                    (configuration, environment, moduleConfig) =>
                    {
                        moduleConfig.EnableThreadSafeOperations = false;
                    });
        }

        public BioEngineApp AddS3Storage()
        {
            return AddModule<S3StorageModule<BRCStorageConfig>, BRCStorageConfig>((configuration, env, moduleConfig) =>
            {
                var uri = configuration["BE_STORAGE_PUBLIC_URI"];
                var success = Uri.TryCreate(uri, UriKind.Absolute, out var publicUri);
                if (!success)
                {
                    throw new ArgumentException($"URI {uri} is not proper URI");
                }

                var serverUriStr = configuration["BE_STORAGE_S3_SERVER_URI"];
                success = Uri.TryCreate(serverUriStr, UriKind.Absolute, out var serverUri);
                if (!success || serverUri is null)
                {
                    throw new ArgumentException($"S3 server URI {serverUriStr} is not proper URI");
                }

                moduleConfig.Name = "brc";
                moduleConfig.PublicUri = publicUri;
                moduleConfig.Server = serverUri;
                moduleConfig.Bucket = configuration["BE_STORAGE_S3_BUCKET"];
                moduleConfig.SecretKey = configuration["BE_STORAGE_S3_SECRET_KEY"];
                moduleConfig.AccessKey = configuration["BE_STORAGE_S3_ACCESS_KEY"];
                moduleConfig.Region = RegionEndpoint.GetBySystemName(configuration["BE_STORAGE_S3_REGION"]);
                moduleConfig.BucketPath = "storage";

                moduleConfig
                    .UseMetadata<PostgresStorageMetadataProvider<BRCStorageConfig>,
                        PostgresStorageMetadataProviderOptions>(options =>
                    {
                        var builder = configuration.GetPostgresConnectionStringBuilder();

                        builder.SearchPath = "storage,public";
                        options.ConnectionString = builder.ConnectionString;
                        options.Schema = "storage";
                    });
            }).AddModule<StorageImgProxyModule<BRCStorageConfig>, StorageImgProxyModuleConfig<BRCStorageConfig>>(
                (configuration, environment, moduleConfig) =>
                {
                    moduleConfig.Host = configuration["BE_IMGPROXY_HOST"];
                    moduleConfig.Key = configuration["BE_IMGPROXY_KEY"];
                    moduleConfig.Salt = configuration["BE_IMGPROXY_SALT"];
                    moduleConfig.EncodeUrls = true;
                });
        }
    }

    public class BRCStorageConfig : StorageOptions, IS3StorageOptions
    {
        public Uri Server { get; set; } = new Uri("http://localhost");
        public string Bucket { get; set; } = "brc";
        public string AccessKey { get; set; } = string.Empty;
        public string SecretKey { get; set; } = String.Empty;
        public RegionEndpoint Region { get; set; }
        public string BucketPath { get; set; }
        public override string Name { get; set; }
    }
}
