using System;
using BioEngine.Core.Data.Entities;
using Sitko.Core.Repository.EntityFrameworkCore;

namespace BioEngine.Core.Data.Repositories
{
    public class DevelopersRepository : SectionRepository<Developer>
    {
        public DevelopersRepository(EFRepositoryContext<Developer, Guid, BioDbContext> repositoryContext) : base(
            repositoryContext)
        {
        }
    }
}
