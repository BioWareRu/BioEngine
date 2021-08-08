using BioEngine.Admin.Old;
using BioEngine.Core.IPB;
using BioEngine.Core.IPB.Users;
using Microsoft.Extensions.DependencyInjection;
using Sitko.Blockly.AntDesignComponents;
using Sitko.Blockly.AntDesignComponents.Blocks;
using Sitko.Blockly.Blocks;
using Sitko.Core.Db.Postgres;

namespace BioEngine.Admin
{
    using Blocks;

    public class AdminApplication : Core.BioEngineApp<Startup>
    {
        public AdminApplication(string[] args) : base(args)
        {
            AddPostgresDb(typeof(AdminApplication).Assembly)
                .AddS3Storage()
                .AddModule<IPBAdminModule, IpbAdminModuleOptions>();
            AddModule<IPBUsersModule<IpbAdminModuleOptions>, IPBUsersModuleOptions>();
            this.AddPostgresDatabase<OldBrcContext>(options =>
            {
                options.EnableSensitiveLogging = false;
            });
            AddModule<AntDesignBlocklyModule, AntDesignBlocklyModuleOptions>(
                (_, _, moduleConfig) =>
                {
                    moduleConfig.AddBlock<AntTextBlockDescriptor, TextBlock>();
                    moduleConfig.AddBlock<AntCutBlockDescriptor, CutBlock>();
                    moduleConfig.AddBlock<AntIframeBlockDescriptor, IframeBlock>();
                    moduleConfig.AddBlock<AntQuoteBlockDescriptor, QuoteBlock>();
                    moduleConfig.AddBlock<AntTextBlockDescriptor, TextBlock>();
                    moduleConfig.AddBlock<AntTwitchBlockDescriptor, TwitchBlock>();
                    moduleConfig.AddBlock<AntTwitterBlockDescriptor, TwitterBlock>();
                    moduleConfig.AddBlock<AntYoutubeBlockDescriptor, YoutubeBlock>();
                    moduleConfig.AddBlock<BioEngineGalleryBlockDescriptor, GalleryBlock>();
                    moduleConfig.AddBlock<BioEngineFilesBlockDescriptor, FilesBlock>();
                });
            ConfigureServices((_, _, services) =>
            {
                services.AddScoped<BrcConverter>();
            });
        }
    }
}
