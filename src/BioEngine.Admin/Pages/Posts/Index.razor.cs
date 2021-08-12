using System;
using System.Linq;
using System.Threading.Tasks;
using AntDesign;
using BioEngine.Admin.Shared;
using BioEngine.Core.Data;
using BioEngine.Core.Data.Entities;
using BioEngine.Core.Data.Repositories;
using BioEngine.Core.Users;
using Flurl;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sitko.Core.Blazor.AntDesignComponents.Components;
using Sitko.Core.Repository;
using Tag = BioEngine.Core.Data.Entities.Tag;

namespace BioEngine.Admin.Pages.Posts
{
    public partial class Index
    {
        private TableFilter<string>[] authorsFilter = Array.Empty<TableFilter<string>>();
        private Guid filterSectionId;
        private Guid filterSiteId;
        private Guid filterTagId;
        protected override string CreatePageUrl => "/Posts/Add";
        protected override string Title => "Посты";

        protected override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            SetFilterParameters();
            using var scope = CreateServicesScope();
            var authorIds = await scope.ServiceProvider.GetRequiredService<BioDbContext>().Posts.Select(a => a.AuthorId)
                .Distinct().ToArrayAsync();
            var usersProvider = scope.ServiceProvider.GetRequiredService<IUserDataProvider>();
            var usersData = await usersProvider.GetDataAsync(authorIds);
            authorsFilter = usersData.OrderBy(u => u.Name)
                .Select(x => new TableFilter<string> { Text = x.Name, Value = x.Id }).ToArray();
        }

        protected override async Task OnLocationChangeAsync(string location, bool isNavigationIntercepted)
        {
            await base.OnLocationChangeAsync(location, isNavigationIntercepted);
            SetFilterParameters();
            await List.RefreshAsync();
        }

        private void SetFilterParameters()
        {
            filterSiteId = GetQueryString<Guid>(nameof(filterSiteId));
            filterSectionId = GetQueryString<Guid>(nameof(filterSectionId));
            filterTagId = GetQueryString<Guid>(nameof(filterTagId));
        }

        private Task ConfigureQueryAsync(IRepositoryQuery<Post> query)
        {
            if (filterSiteId != default)
            {
                query.Where(p => p.Sites.Any(site => site.Id == filterSiteId));
            }

            if (filterSectionId != default)
            {
                query.Where(p => p.Sections.Any(section => section.Id == filterSectionId));
            }

            if (filterTagId != default)
            {
                query.Where(p => p.Tags.Any(tag => tag.Id == filterTagId));
            }

            query.Include(p => p.Sites).Include(p => p.Sections).Include(p => p.Tags);
            return Task.CompletedTask;
        }

        private string GetFilterUrl(Guid? siteId, Guid? sectionId, Guid? tagId)
        {
            Url url = new Uri(NavigationManager.Uri);
            if (siteId is not null)
            {
                url = url.SetQueryParam(nameof(this.filterSiteId), siteId);
            }

            if (sectionId is not null)
            {
                url = url.SetQueryParam(nameof(this.filterSectionId), sectionId);
            }

            if (tagId is not null)
            {
                url = url.SetQueryParam(nameof(this.filterTagId), tagId);
            }

            return url.ToString();
        }

        private string GetSiteFilterUrl(Site site) => GetFilterUrl(site.Id, filterSectionId, filterTagId);

        private string GetSectionFilterUrl(Section section) => GetFilterUrl(filterSiteId, section.Id, filterTagId);

        private string GetTagFilterUrl(Tag tag) => GetFilterUrl(filterSiteId, filterSectionId, tag.Id);
    }

    public class PostsList : BaseBioEngineList<Post, Guid, PostsRepository>
    {
        public override Task DeleteAsync(Post entity) => throw new NotImplementedException();
    }
}
