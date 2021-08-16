using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BioEngine.Admin.Shared;
using BioEngine.Core.Data.Entities;
using BioEngine.Core.Data.Repositories;
using BioEngine.Core.IPB;
using BioEngine.Core.IPB.Api;
using BioEngine.Core.IPB.Models;
using Microsoft.AspNetCore.Components;
using Sitko.Core.Blazor.AntDesignComponents.Components;

namespace BioEngine.Admin.Pages.Sites
{
    public partial class Edit
    {
        protected override string Title => Form.IsNew ? "Новый сайт" : Form.Entity.Title;
    }

    public class SiteForm : BaseForm<Site, Guid, SitesRepository>
    {
        [Parameter] public RenderFragment<SiteForm> ChildContent { get; set; } = null!;
        protected override RenderFragment ChildContentFragment => ChildContent(this);
        public IEnumerable<Forum> Forums { get; set; }


        protected override async Task OnCreatedAsync(Site entity)
        {
            await base.OnCreatedAsync(entity);
            NavigationManager.NavigateTo($"/Sites/{entity.Id}");
        }

        protected override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            var factory = GetRequiredService<IPBApiClientFactory<IpbAdminModuleOptions>>();
            var client = factory.GetReadOnlyClient();
            var allForums = await client.GetForumsAsync();
            var forums = new List<Forum>();
            var roots = allForums.Where(f => f.ParentId < 1).OrderBy(f => f.Position).ToList();
            foreach (var forum in roots)
            {
                FillTree(forum, forums, allForums);
            }

            Forums = forums;
        }

        private void FillTree(Forum forum, List<Forum> forums, List<Forum> allForums, Forum? parent = null)
        {
            var name = forum.Name;
            if (parent is not null)
            {
                name = parent.Name + " > " + name;
            }

            var current = new Forum { Id = forum.Id, Name = name };
            forums.Add(current);
            var children = allForums.Where(f => f.ParentId == current.Id).OrderBy(f => f.Position).ToList();
            foreach (var child in children)
            {
                FillTree(child, forums, allForums, current);
            }
        }
    }
}
