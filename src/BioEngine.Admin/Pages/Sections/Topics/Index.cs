using BioEngine.Core.Data.Entities;
using Microsoft.AspNetCore.Components;

namespace BioEngine.Admin.Pages.Sections.Topics
{
    [Route("/Sections/Topics")]
    public class Index : BaseSectionsListPage<Topic>
    {
        protected override string GetUrl(Topic item)
        {
            return $"/Sections/Topics/{item.Id}";
        }
        
        protected override string GetTitle()
        {
            return "Темы";
        }
        
        protected override string GetCreatePageUrl()
        {
            return "/Sections/Topics/Add";
        }
    }
}
