using System.Threading.Tasks;
using BioEngine.Core.Data.Entities;
using BioEngine.Core.Data.Repositories;
using Microsoft.AspNetCore.Components;

namespace BioEngine.Admin.Pages.Sections.Games
{
    public partial class Edit
    {
        protected override string Title =>
            Form.IsNew ? "Новая игра" : Form.Entity.Title;
    }

    public class GameForm : BaseSectionForm<Game, GameData, GamesRepository>
    {
        [Parameter] public RenderFragment<GameForm> ChildContent { get; set; } = null!;

        protected override RenderFragment ChildContentFragment => ChildContent(this);

        protected override async Task OnCreatedAsync(Game entity)
        {
            await base.OnCreatedAsync(entity);
            NavigationManager.NavigateTo($"/Sections/Games/{entity.Id}");
        }
    }
}
