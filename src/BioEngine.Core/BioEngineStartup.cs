using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Sitko.Core.Blazor.AntDesignComponents;

namespace BioEngine.Core
{
    public class BioEngineStartup : AntBlazorStartup
    {
        public BioEngineStartup(IConfiguration configuration, IHostEnvironment environment) : base(
            configuration, environment)
        {
        }
    }
}
