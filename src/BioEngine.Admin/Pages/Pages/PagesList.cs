using System;
using BioEngine.Admin.Shared;
using BioEngine.Core.Data.Entities;
using BioEngine.Core.Data.Repositories;

namespace BioEngine.Admin.Pages.Pages
{
    public class PagesList : BaseBioEngineList<Page, Guid, PagesRepository>
    {
    }
}
