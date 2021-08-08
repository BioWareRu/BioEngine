using System;
using System.Threading.Tasks;
using BioEngine.Core.Data.Entities;
using BioEngine.Core.Data.Repositories;
using Microsoft.AspNetCore.Components;
using Sitko.Core.Storage;

namespace BioEngine.Admin.Pages.Sections.Topics
{
    public partial class Edit
    {
        protected override string Title => Form.IsNew ? "Новая тема" : Form.Entity.Title;
    }

    public class TopicForm : BaseSectionForm<Topic, TopicData, TopicsRepository>
    {
        [Parameter] public RenderFragment<TopicForm> ChildContent { get; set; } = null!;

        protected override RenderFragment ChildContentFragment => ChildContent(this);

        protected override async Task OnCreatedAsync(Topic entity)
        {
            await base.OnCreatedAsync(entity);
            NavigationManager.NavigateTo($"/Sections/Topics/{entity.Id}");
        }
    }
}
