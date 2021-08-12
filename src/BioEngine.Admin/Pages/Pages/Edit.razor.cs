using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BioEngine.Admin.Extensions;
using BioEngine.Admin.Shared;
using BioEngine.Core;
using BioEngine.Core.Data.Entities;
using BioEngine.Core.Data.Repositories;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Sitko.Blockly.AntDesignComponents.Forms;
using Sitko.Blockly.Blazor.Forms;
using Sitko.Core.Blazor.AntDesignComponents.Components;
using Sitko.Core.Repository;
using Sitko.Core.Storage;

namespace BioEngine.Admin.Pages.Pages
{
    public partial class Edit
    {
        protected override string Title => Form.IsNew ? "Новая страница" : Form.Entity.Title;
    }

    public class PageForm : BaseForm<Page, Guid, PagesRepository>
    {
        [Parameter] public RenderFragment<PageForm> ChildContent { get; set; } = null!;
        protected override RenderFragment ChildContentFragment => ChildContent(this);

        public Guid DummySiteId { get; set; }
        public List<Site> SitesList { get; private set; } = new();

        public IEnumerable<Guid> SiteIds
        {
            get => Entity.Sites.Select(s => s.Id);
            set
            {
                Entity.Sites = SitesList.Where(s => value.Contains(s.Id)).ToList();
                NotifyChange(FieldIdentifier.Create(() => Entity.Sites));
            }
        }

        public AntDesignBlocklyFormOptions BlocksOptions { get; private set; } = null!;

        protected override async Task OnCreatedAsync(Page entity)
        {
            await base.OnCreatedAsync(entity);
            NavigationManager.NavigateTo($"/Pages/{entity.Id}");
        }

        protected override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            var imagesStorageOptions = new BlockFormStorageOptions
            {
                UploadPath = $"pages/{DateTimeOffset.UtcNow:dd/MM/yyyy}",
                MaxAllowedFiles = 10,
                MaxFileSize = 2 * 1024 * 1024, // 2Mb
                GenerateMetadata = StorageUploadExtensions.GenerateMetadataAsync
            };
            var filesStorageOptions = new BlockFormStorageOptions
            {
                UploadPath = $"pages/{DateTimeOffset.UtcNow:dd/MM/yyyy}",
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
            using var scope = CreateServicesScope();
            SitesList = (await scope.ServiceProvider.GetRequiredService<SitesRepository>().GetAllAsync()).items
                .ToList();
        }

        protected override async Task ConfigureQueryAsync(IRepositoryQuery<Page> query)
        {
            await base.ConfigureQueryAsync(query);
            query.Include(q => q.Sites);
        }
    }
}
