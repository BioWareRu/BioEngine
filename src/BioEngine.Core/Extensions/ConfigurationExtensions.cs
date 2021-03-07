using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Sitko.Backup
{
    public static class ConfigurationExtensions
    {
        public static NpgsqlConnectionStringBuilder GetPostgresConnectionStringBuilder(
            this IConfiguration configuration,
            string? databaseName = null,
            bool useContextPooling = true)
        {
            return new NpgsqlConnectionStringBuilder()
            {
                Host = (configuration["BE_POSTGRES_HOST"] ?? "localhost"),
                Port = GetIntValue(configuration, "BE_POSTGRES_PORT", 5432),
                Username = (configuration["BE_POSTGRES_USERNAME"] ?? "postgres"),
                Password = (configuration["BE_POSTGRES_PASSWORD"] ?? ""),
                Database = (databaseName ?? configuration["BE_POSTGRES_DATABASE"] ?? "postgres"),
                Pooling = useContextPooling
            };
        }

        public static string GetPostgresConnectionString(
            this IConfiguration configuration,
            string? databaseName = null,
            bool useContextPooling = true)
        {
            return configuration.GetPostgresConnectionStringBuilder(databaseName, useContextPooling).ConnectionString;
        }

        public static int GetIntValue(this IConfiguration configuration, string key, int defaultValue = 0)
        {
            if (!string.IsNullOrEmpty(configuration[key]) &&
                int.TryParse(configuration[key], out var intValue))
            {
                return intValue;
            }

            return defaultValue;
        }
    }
}
