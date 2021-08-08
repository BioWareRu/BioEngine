namespace BioEngine.Admin.Blocks.Forms
{
    using System.Linq;
    using System.Threading.Tasks;
    using AntDesign;
    using Extensions;
    using Microsoft.AspNetCore.Components;
    using Sitko.Blockly.AntDesignComponents.Forms.Blocks;
    using Sitko.Core.App.Collections;
    using Sitko.Core.App.Localization;
    using Sitko.Core.Blazor.AntDesignComponents.Components;
    using Sitko.Core.Storage;

    public partial class BioEngineGalleryBlockForm
    {
        [Inject] private ILocalizationProvider<AntCutBlockForm> LocalizationProvider { get; set; } = null!;
        [Inject] private ModalService ModalService { get; set; } = null!;

        private async Task<ValueCollection<StorageItem>> SelectImagesAsync(
            BaseAntStorageInput<ValueCollection<StorageItem>> arg)
        {
            var collection = new ValueCollection<StorageItem>(Block.Pictures.ToList());
            var result = await ModalService.SelectStorageItemsAsync(isMultiple: true);
            if (result.Any())
            {
                foreach (var item in result)
                {
                    collection.Add(item);
                }
            }

            return collection;
        }
    }
}
