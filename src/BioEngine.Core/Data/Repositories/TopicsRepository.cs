using System;
using BioEngine.Core.Data.Entities;
using Sitko.Core.Repository.EntityFrameworkCore;

namespace BioEngine.Core.Data.Repositories
{
    public class TopicsRepository : SectionRepository<Topic>
    {
        public TopicsRepository(EFRepositoryContext<Topic, Guid, BioDbContext> repositoryContext) : base(
            repositoryContext)
        {
        }
    }
}
