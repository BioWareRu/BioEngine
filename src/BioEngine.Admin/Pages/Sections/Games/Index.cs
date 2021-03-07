using BioEngine.Core.Data.Entities;
using Microsoft.AspNetCore.Components;

namespace BioEngine.Admin.Pages.Sections.Games
{
    [Route("/Sections/Games")]
    public class Index : BaseSectionsListPage<Game>
    {
        protected override string GetUrl(Game item)
        {
            return $"/Sections/Games/{item.Id}";
        }
        
        protected override string GetTitle()
        {
            return "Игры";
        }
        
        protected override string GetCreatePageUrl()
        {
            return "/Sections/Games/Add";
        }
    }
}
