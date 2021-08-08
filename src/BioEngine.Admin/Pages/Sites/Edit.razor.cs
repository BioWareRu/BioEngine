using System;
using System.Threading.Tasks;
using BioEngine.Core.Data.Entities;
using BioEngine.Core.Data.Repositories;
using Microsoft.AspNetCore.Components;
using Sitko.Core.Blazor.AntDesignComponents.Components;

namespace BioEngine.Admin.Pages.Sites
{
    public partial class Edit
    {
        protected override string Title => Form.IsNew ? "Новый сайт" : Form.Entity.Title;
    }

    public class SiteForm : BaseAntRepositoryForm<Site, Guid, SitesRepository>
    {
        [Parameter] public RenderFragment<SiteForm> ChildContent { get; set; } = null!;
        protected override RenderFragment ChildContentFragment => ChildContent(this);

        protected override async Task OnCreatedAsync(Site entity)
        {
            await base.OnCreatedAsync(entity);
            NavigationManager.NavigateTo($"/Sites/{entity.Id}");
        }
    }
}
