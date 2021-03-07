using System;
using BioEngine.Core.Data.Entities;
using Sitko.Core.Repository.EntityFrameworkCore;

namespace BioEngine.Core.Data.Repositories
{
    public class PagesRepository : PublishableEntityRepository<Page>
    {
        public PagesRepository(EFRepositoryContext<Page, Guid, BioDbContext> repositoryContext) : base(
            repositoryContext)
        {
        }
    }
}
