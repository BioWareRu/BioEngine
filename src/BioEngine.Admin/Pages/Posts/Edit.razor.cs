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
using BioEngine.Core.Users;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Sitko.Blockly.AntDesignComponents.Forms;
using Sitko.Blockly.Blazor.Forms;
using Sitko.Core.Repository;
using Sitko.Core.Storage;

namespace BioEngine.Admin.Pages.Posts
{
    public partial class Edit
    {
        protected override string Title => Form.IsNew ? "Новый пост" : Form.Entity.Title;
    }

    public class PostForm : BasePublishableRepositoryForm<Post, Guid, PostsRepository>
    {
        [Parameter] public RenderFragment<PostForm> ChildContent { get; set; } = null!;
        protected override RenderFragment ChildContentFragment => ChildContent(this);
        private string oldTitle = string.Empty;
        public readonly List<Section> SectionsList = new();
        public readonly List<Tag> TagsList = new();
        public AntDesignBlocklyFormOptions BlocksOptions { get; private set; } = null!;
        public Guid DummySectionId { get; set; }
        public Guid DummyTagId { get; set; }

        public IEnumerable<Guid> SectionIds
        {
            get => Entity.Sections.Select(s => s.Id);
            set
            {
                Entity.Sections = SectionsList.Where(s => value.Contains(s.Id)).ToList();
                Entity.Sites = Entity.Sections.SelectMany(s => s.Sites).Distinct().ToList();
                NotifyChange(FieldIdentifier.Create(() => Entity.Sections));
            }
        }

        public IEnumerable<Guid> TagIds
        {
            get => Entity.Tags.Select(s => s.Id);
            set
            {
                Entity.Tags = TagsList.Where(s => value.Contains(s.Id)).ToList();
                NotifyChange(FieldIdentifier.Create(() => Entity.Tags));
            }
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
        }

        protected override async Task InitializeEntityAsync(Post entity)
        {
            await base.InitializeEntityAsync(entity);
            oldTitle = entity.Title;
            if (IsNew)
            {
                var user = await GetRequiredService<ICurrentUserProvider>().GetCurrentUserAsync();
                entity.AuthorId = user!.Id;
            }
        }

        protected override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            var imagesStorageOptions = new BlockFormStorageOptions
            {
                UploadPath = $"posts/{DateTimeOffset.UtcNow:dd/MM/yyyy}",
                MaxAllowedFiles = 10,
                MaxFileSize = 2 * 1024 * 1024, // 2Mb
                GenerateMetadata = StorageUploadExtensions.GenerateMetadataAsync
            };
            var filesStorageOptions = new BlockFormStorageOptions
            {
                UploadPath = $"posts/{DateTimeOffset.UtcNow:dd/MM/yyyy}",
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
            SectionsList.AddRange((await scope.ServiceProvider.GetRequiredService<SectionsRepository>()
                .GetAllAsync(q => q.Where(s => s.IsPublished))).items);
            TagsList.AddRange((await scope.ServiceProvider.GetRequiredService<TagsRepository>().GetAllAsync()).items);
        }

        protected override async Task OnCreatedAsync(Post entity)
        {
            await base.OnCreatedAsync(entity);
            NavigationManager.NavigateTo($"/Posts/{entity.Id}");
        }

        protected override async Task ConfigureQueryAsync(IRepositoryQuery<Post> query)
        {
            await base.ConfigureQueryAsync(query);
            query.Include(p => p.Sites).Include(p => p.Sections).Include(p => p.Tags);
        }
    }
}
