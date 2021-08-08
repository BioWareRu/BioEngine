using System;
using System.Linq;
using System.Threading.Tasks;
using AntDesign;
using BioEngine.Core.Data;
using BioEngine.Core.Data.Entities;
using BioEngine.Core.Users;
using Flurl;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sitko.Core.Blazor.AntDesignComponents.Components;
using Sitko.Core.Repository;
using Tag = BioEngine.Core.Data.Entities.Tag;

namespace BioEngine.Admin.Pages.Posts
{
    public partial class Index
    {
        protected override string CreatePageUrl => "/Posts/Add";
        protected override string Title => "Посты";
        private TableFilter<string>[] authorsFilter = Array.Empty<TableFilter<string>>();

        private Guid? siteId;
        private Guid? sectionId;
        private Guid? tagId;
        public AntRepositoryList<Post, Guid>? ListTable { get; set; }

        protected override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            NavigationManager.LocationChanged += HandleLocationChanged;
            SetFilterParameters();
            using var scope = CreateServicesScope();
            var authorIds = await scope.ServiceProvider.GetRequiredService<BioDbContext>().Posts.Select(a => a.AuthorId)
                .Distinct().ToArrayAsync();
            var usersProvider = scope.ServiceProvider.GetRequiredService<IUserDataProvider>();
            var usersData = await usersProvider.GetDataAsync(authorIds);
            authorsFilter = usersData.OrderBy(u => u.Name)
                .Select(x => new TableFilter<string> { Text = x.Name, Value = x.Id }).ToArray();
        }

        private void HandleLocationChanged(object? sender, LocationChangedEventArgs e)
        {
            SetFilterParameters();
            ListTable?.RefreshAsync();
        }

        private void SetFilterParameters()
        {
            Url url = new Uri(NavigationManager.Uri);
            siteId = GetParamValue(url, nameof(siteId));
            sectionId = GetParamValue(url, nameof(sectionId));
            tagId = GetParamValue(url, nameof(tagId));
        }

        private static Guid? GetParamValue(Url url, string paramName)
        {
            if (url.QueryParams.TryGetFirst(paramName, out var paramString) &&
                Guid.TryParse(paramString.ToString(), out var paramValue))
            {
                return paramValue;
            }

            return null;
        }

        private Task ConfigureQueryAsync(IRepositoryQuery<Post> query)
        {
            if (siteId is not null)
            {
                query.Where(p => p.Sites.Any(site => site.Id == siteId));
            }

            if (sectionId is not null)
            {
                query.Where(p => p.Sections.Any(section => section.Id == sectionId));
            }

            if (tagId is not null)
            {
                query.Where(p => p.Tags.Any(tag => tag.Id == tagId));
            }

            return Task.CompletedTask;
        }

        private string GetFilterUrl(Guid? siteId, Guid? sectionId, Guid? tagId)
        {
            Url url = new Uri(NavigationManager.Uri);
            if (siteId is not null)
            {
                url = url.SetQueryParam(nameof(this.siteId), siteId);
            }

            if (sectionId is not null)
            {
                url = url.SetQueryParam(nameof(this.sectionId), sectionId);
            }

            if (tagId is not null)
            {
                url = url.SetQueryParam(nameof(this.tagId), tagId);
            }

            return url.ToString();
        }

        private string GetSiteFilterUrl(Site site) => GetFilterUrl(site.Id, sectionId, tagId);

        private string GetSectionFilterUrl(Section section) => GetFilterUrl(siteId, section.Id, tagId);

        private string GetTagFilterUrl(Tag tag) => GetFilterUrl(siteId, sectionId, tag.Id);
    }
}
