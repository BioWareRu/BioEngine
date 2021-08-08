using System;
using Sitko.Blockly.AntDesignComponents.Blocks;
using Sitko.Blockly.Blocks;
using Sitko.Core.App.Localization;

namespace BioEngine.Admin.Blocks
{
    using Forms;

    public record BioEngineFilesBlockDescriptor : AntFilesBlockDescriptor
    {
        public BioEngineFilesBlockDescriptor(ILocalizationProvider<FilesBlock> localizationProvider) : base(
            localizationProvider)
        {
        }

        public override Type FormComponent => typeof(BioEngineFilesBlockForm);
    }
}
