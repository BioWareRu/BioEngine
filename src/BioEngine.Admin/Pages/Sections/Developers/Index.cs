using BioEngine.Core.Data.Entities;
using Microsoft.AspNetCore.Components;

namespace BioEngine.Admin.Pages.Sections.Developers
{
    [Route("/Sections/Developers")]
    public class Index : BaseSectionsListPage<Developer, DeveloperData>
    {
        protected override string GetUrl(Developer item) => $"/Sections/Developers/{item.Id}";

        protected override string Title => "Разработчики";

        protected override string CreatePageUrl => "/Sections/Developers/Add";
    }
}
