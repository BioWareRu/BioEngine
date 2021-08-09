using BioEngine.Core.Data.Entities;
using BioEngine.Core.Data.Repositories;
using Microsoft.AspNetCore.Components;

namespace BioEngine.Admin.Pages.Sections.Games
{
    [Route("/Sections/Games")]
    public class Index : BaseSectionsListPage<Game, GameData, GamesRepository>
    {
        protected override string GetUrl(Game item) => $"/Sections/Games/{item.Id}";

        protected override string Title => "Игры";

        protected override string CreatePageUrl => "/Sections/Games/Add";
    }
}
