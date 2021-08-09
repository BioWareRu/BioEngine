using System;
using BioEngine.Admin.Shared;
using BioEngine.Core.Data.Entities;
using BioEngine.Core.Data.Repositories;

namespace BioEngine.Admin.Pages.Sites
{
    public class SitesList : BaseBioEngineList<Site, Guid, SitesRepository>
    {
    }
}
