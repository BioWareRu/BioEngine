using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BioEngine.Admin
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var application = CreateApplication(args);
            application.GetHostBuilder().ConfigureWebHost(builder =>
            {
                builder.ConfigureKestrel(options =>
                {
                    options.Limits.MaxRequestBodySize = 1 * 1024 * 1024 * 1024; // 1 gb
                });
            });
            await application.RunAsync();
        }

        // need for migrations
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            CreateApplication(args).GetHostBuilder();

        public static AdminApplication CreateApplication(string[] args) => new(args);
    }
}
