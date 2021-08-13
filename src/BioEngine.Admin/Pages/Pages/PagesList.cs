using System;
using System.Threading.Tasks;
using BioEngine.Admin.Shared;
using BioEngine.Core.Data.Entities;
using BioEngine.Core.Data.Repositories;
using Sitko.Core.Repository;

namespace BioEngine.Admin.Pages.Pages
{
    public class PagesList : BaseBioEngineList<Page, Guid, PagesRepository>
    {
        protected override async Task ConfigureQueryAsync(IRepositoryQuery<Page> query)
        {
            await base.ConfigureQueryAsync(query);
            query.Include(p => p.Sites);
        }
    }
}
