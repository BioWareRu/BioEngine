using System;
using System.Threading.Tasks;
using BioEngine.Admin.Shared;
using BioEngine.Core;
using BioEngine.Core.Data.Entities;
using ImgProxy;
using Sitko.Core.Repository;
using Sitko.Core.Storage.ImgProxy;
using ResizeOption = Sitko.Core.Storage.ImgProxy.ResizeOption;

namespace BioEngine.Admin.Pages.Sections
{
    public abstract partial class BaseSectionsListPage<TItem, TData, TRepository>
        where TItem : Section<TData>, new()
        where TData : SectionData, new()
        where TRepository : IRepository<TItem, Guid>
    {
        protected abstract string GetUrl(TItem item);

        private string GetSectionImageUrl(TItem section) => section.Data.HeaderPicture is not null
            ? GetRequiredService<IImgProxyUrlGenerator<BRCStorageConfig>>().Build(section.Data.HeaderPicture,
                proxyBuilder => proxyBuilder.WithOptions(new ResizeOption("fill", 0, 32), new GravityOption("ce")))
            : "";
    }

    public class SectionsList<TItem, TRepository> : BaseBioEngineList<TItem, Guid, TRepository>
        where TItem : Section, IEntity<Guid>, new() where TRepository : IRepository<TItem, Guid>
    {
        protected override async Task ConfigureQueryAsync(IRepositoryQuery<TItem> query)
        {
            await base.ConfigureQueryAsync(query);
            query.Include(s => s.Sites);
        }
    }
}
