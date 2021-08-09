using BioEngine.Core.Data.Entities;
using BioEngine.Core.Data.Repositories;
using Microsoft.AspNetCore.Components;

namespace BioEngine.Admin.Pages.Sections.Topics
{
    [Route("/Sections/Topics")]
    public class Index : BaseSectionsListPage<Topic, TopicData, TopicsRepository>
    {
        protected override string GetUrl(Topic item) => $"/Sections/Topics/{item.Id}";

        protected override string Title => "Темы";

        protected override string CreatePageUrl => "/Sections/Topics/Add";
    }
}
