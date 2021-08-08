using System;
using System.Threading.Tasks;
using BioEngine.Core.Data.Entities;
using BioEngine.Core.Data.Repositories;
using Microsoft.AspNetCore.Components;
using Sitko.Core.Blazor.AntDesignComponents.Components;

namespace BioEngine.Admin.Pages.Tags
{
    public partial class Edit
    {
        protected override string Title => Form.IsNew ? "Новый тэг" : Form.Entity.Title;
    }

    public class TagForm : BaseAntRepositoryForm<Tag, Guid, TagsRepository>
    {
        [Parameter] public RenderFragment<TagForm> ChildContent { get; set; } = null!;

        protected override RenderFragment ChildContentFragment => ChildContent(this);

        protected override async Task OnCreatedAsync(Tag entity)
        {
            await base.OnCreatedAsync(entity);
            NavigationManager.NavigateTo($"/Tags/{entity.Id}");
        }
    }
}
