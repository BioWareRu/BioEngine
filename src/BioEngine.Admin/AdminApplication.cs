using System;
using BioEngine.Admin.Old;
using BioEngine.Core.IPB;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sitko.Backup;
using Sitko.Core.Db.Postgres;

namespace BioEngine.Admin
{
    public class AdminApplication : Core.BioEngineApp
    {
        public AdminApplication(string[] args) : base(args)
        {
            AddPostgresDb(true, typeof(AdminApplication).Assembly)
                .AddS3Storage()
                .AddModule<IPBAdminModule, IPBAdminModuleConfig>((configuration, env, moduleConfig) =>
                {
                    if (!Uri.TryCreate(configuration["BE_IPB_URL"], UriKind.Absolute, out var ipbUrl))
                    {
                        throw new ArgumentException($"Can't parse IPB url; {configuration["BE_IPB_URL"]}");
                    }

                    moduleConfig.Url = ipbUrl;


                    moduleConfig.ApiReadonlyKey = configuration["BE_IPB_API_READONLY_KEY"];
                    moduleConfig.ApiPublishKey = configuration["BE_IPB_API_PUBLISH_KEY"];
                })
                .AddIpbUsers(true)
                .AddModule<PostgresModule<OldBrcContext>, PostgresDatabaseModuleConfig<OldBrcContext>>(
                    (configuration, env, moduleConfig) =>
                    {
                        var builder = configuration.GetPostgresConnectionStringBuilder();
                        moduleConfig.Host = builder.Host!;
                        moduleConfig.Username = builder.Username!;
                        moduleConfig.Password = builder.Password!;
                        moduleConfig.Port = builder.Port;
                        moduleConfig.Database = "brc";
                        moduleConfig.MigrationsAssembly = null;
                        moduleConfig.EnableNpgsqlPooling = env.IsDevelopment();
                        moduleConfig.EnableSensitiveLogging = env.IsDevelopment();
                    }
                )
                .ConfigureServices(services => services.AddScoped<BrcConverter>());
        }
    }
}
