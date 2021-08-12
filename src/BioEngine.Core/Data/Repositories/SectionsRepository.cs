using System;
using System.Linq;
using BioEngine.Core.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Sitko.Core.Repository.EntityFrameworkCore;

namespace BioEngine.Core.Data.Repositories
{
    public class SectionsRepository : SectionRepository<Section>
    {
        public SectionsRepository(EFRepositoryContext<Section, Guid, BioDbContext> repositoryContext) : base(repositoryContext)
        {
        }
    }
}
