using System;
using Sitko.Blockly.Blocks;
using Sitko.Core.App.Localization;

namespace BioEngine.Admin.Blocks
{
    using Forms;
    using Sitko.Blockly.AntDesignComponents.Blocks;

    public record BioEngineGalleryBlockDescriptor : AntGalleryBlockDescriptor
    {
        public BioEngineGalleryBlockDescriptor(ILocalizationProvider<GalleryBlock> localizationProvider) : base(
            localizationProvider)
        {
        }

        public override Type FormComponent => typeof(BioEngineGalleryBlockForm);
    }
}
