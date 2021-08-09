using BioEngine.Core.Data.Entities;
using BioEngine.Core.Data.Repositories;
using Microsoft.AspNetCore.Components;

namespace BioEngine.Admin.Pages.Sections.Developers
{
    [Route("/Sections/Developers")]
    public class Index : BaseSectionsListPage<Developer, DeveloperData, DevelopersRepository>
    {
        protected override string GetUrl(Developer item) => $"/Sections/Developers/{item.Id}";

        protected override string Title => "Разработчики";

        protected override string CreatePageUrl => "/Sections/Developers/Add";
    }
}
