using System.Threading.Tasks;
using BioEngine.Core.Data.Entities;
using BioEngine.Core.Data.Repositories;
using Microsoft.AspNetCore.Components;

namespace BioEngine.Admin.Pages.Sections.Developers
{
    public partial class Edit
    {
        protected override string Title =>
            Form.IsNew ? "Новый разработчик" : Form.Entity.Title;
    }

    public class DeveloperForm : BaseSectionForm<Developer, DeveloperData, DevelopersRepository>
    {
        [Parameter] public RenderFragment<DeveloperForm> ChildContent { get; set; } = null!;

        protected override RenderFragment ChildContentFragment => ChildContent(this);

        protected override async Task OnCreatedAsync(Developer entity)
        {
            await base.OnCreatedAsync(entity);
            NavigationManager.NavigateTo($"/Sections/Developers/{entity.Id}");
        }
    }
}
