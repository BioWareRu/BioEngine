using BioEngine.Core.Data.Entities;
using Microsoft.AspNetCore.Components;

namespace BioEngine.Admin.Pages.Sections.Developers
{
    [Route("/Sections/Developers")]
    public class Index : BaseSectionsListPage<Developer>
    {
        protected override string GetUrl(Developer item)
        {
            return $"/Sections/Developers/{item.Id}";
        }

        protected override string GetTitle()
        {
            return "Разработчики";
        }
        
        protected override string GetCreatePageUrl()
        {
            return "/Sections/Developers/Add";
        }
    }
}
