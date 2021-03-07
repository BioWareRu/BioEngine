using System;
using System.Linq;
using BioEngine.Core.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Sitko.Core.Repository.EntityFrameworkCore;

namespace BioEngine.Core.Data.Repositories
{
    public class SectionsRepository : SectionRepository<Section>
    {
        public SectionsRepository(EFRepositoryContext<Section, Guid, BioDbContext> repositoryContext) : base(
            repositoryContext)
        {
        }

        protected override IQueryable<Section> AddIncludes(IQueryable<Section> query)
        {
            return base.AddIncludes(query).Include(s => s.Sites);
        }
    }
}
