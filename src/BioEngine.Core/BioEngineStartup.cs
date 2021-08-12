using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Sitko.Core.Blazor.AntDesignComponents;
using StackExchange.Redis;

namespace BioEngine.Core
{
    public class BioEngineStartup : AntBlazorStartup
    {
        public BioEngineStartup(IConfiguration configuration, IHostEnvironment environment) : base(
            configuration, environment)
        {
        }

        protected override IDataProtectionBuilder ConfigureDataProtection(IDataProtectionBuilder dataProtectionBuilder)
        {
            base.ConfigureDataProtection(dataProtectionBuilder);
            var redisConfig = new RedisOptions();
            Configuration.GetSection("Redis").Bind(redisConfig);
            if (!string.IsNullOrEmpty(redisConfig.Host))
            {
                dataProtectionBuilder.PersistKeysToStackExchangeRedis(() =>
                {
                    var redis = ConnectionMultiplexer.Connect(redisConfig.ConnectionString);
                    return redis.GetDatabase(redisConfig.Db);
                }, $"{Environment.ApplicationName}-DP");
            }

            return dataProtectionBuilder;
        }
    }

    public class RedisOptions
    {
        public string Host { get; set; } = "";
        public int Port { get; set; } = 6379;
        public int Db { get; set; } = -1;

        public string ConnectionString => $"{Host}:{Port}";
    }
}
