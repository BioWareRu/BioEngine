using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BioEngine.Admin.Extensions;
using BioEngine.Admin.Helpers;
using BioEngine.Admin.Shared;
using BioEngine.Core;
using BioEngine.Core.Data.Entities;
using BioEngine.Core.Data.Repositories;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Sitko.Blockly.AntDesignComponents.Forms;
using Sitko.Blockly.Blazor.Forms;
using Sitko.Core.Repository;
using Sitko.Core.Storage;

namespace BioEngine.Admin.Pages.Sections
{
    public abstract class BaseSectionForm<TSection, TSectionData, TRepository> :
        BasePublishableRepositoryForm<TSection, Guid, TRepository>
        where TSection : Section<TSectionData>, IEntity<Guid>, new()
        where TSectionData : SectionData, new()
        where TRepository : class, IPublishableEntityRepository<TSection>
    {
        public List<Site> SitesList { get; } = new();
        private string oldTitle = string.Empty;
        public Guid DummySiteId { get; set; }
        public AntDesignBlocklyFormOptions BlocksOptions { get; private set; } = null!;
        public IStorage Storage => GetRequiredService<IStorage>();

        protected override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            using var scope = CreateServicesScope();
            SitesList.AddRange((await scope.ServiceProvider.GetRequiredService<SitesRepository>().GetAllAsync()).items);
            var imagesStorageOptions = new BlockFormStorageOptions
            {
                UploadPath = $"sections/{typeof(TSection).Name.ToLowerInvariant()}",
                MaxAllowedFiles = 10,
                MaxFileSize = 2 * 1024 * 1024, // 2Mb
                GenerateMetadata = StorageUploadExtensions.GenerateMetadataAsync
            };
            var filesStorageOptions = new BlockFormStorageOptions
            {
                UploadPath = $"sections/{typeof(TSection).Name.ToLowerInvariant()}",
                MaxAllowedFiles = 10,
                MaxFileSize = 100 * 1024 * 1024, // 100Mb
                GenerateMetadata = StorageUploadExtensions.GenerateMetadataAsync
            };
            BlocksOptions = new AntDesignBlocklyFormOptions
            {
                Storage = GetService<IStorage<BRCStorageConfig>>(),
                ImagesOptions = imagesStorageOptions,
                FilesOptions = filesStorageOptions
            };
        }

        protected override async Task InitializeEntityAsync(TSection entity)
        {
            await base.InitializeEntityAsync(entity);
            oldTitle = entity.Title;
        }

        public IEnumerable<Guid> SiteIds
        {
            get => Entity.Sites.Select(s => s.Id);
            set => Entity.Sites = SitesList.Where(s => value.Contains(s.Id)).ToList();
        }

        public void TitleChanged(KeyboardEventArgs args)
        {
            var title = Entity.Title + args.Key;
            if (!string.IsNullOrEmpty(title))
            {
                Entity.Url = SlugifyHelper.Slugify(title, oldTitle, Entity.Url);
                oldTitle = Entity.Title;
            }
            else
            {
                Entity.Url = string.Empty;
            }

            NotifyChange(new FieldIdentifier(this, nameof(Entity.Url)));
        }
    }
}
