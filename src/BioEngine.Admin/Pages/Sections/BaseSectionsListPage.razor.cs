using System;
using BioEngine.Core;
using BioEngine.Core.Data.Entities;
using Microsoft.AspNetCore.Components;
using Sitko.Core.Storage.ImgProxy;

namespace BioEngine.Admin.Pages.Sections
{
    public abstract partial class BaseSectionsListPage<TItem, TData>
        where TItem : Section<TData>, new() where TData : SectionData, new()
    {
        protected abstract string GetUrl(TItem item);
        [Inject] private IImgProxyUrlGenerator<BRCStorageConfig> ImgProxyUrlGenerator { get; set; }
    }
}
