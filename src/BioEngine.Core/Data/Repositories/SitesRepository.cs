using System;
using BioEngine.Core.Data.Entities;
using Microsoft.Extensions.Options;
using Sitko.Core.Repository.EntityFrameworkCore;

namespace BioEngine.Core.Data.Repositories
{
    public class SitesRepository : EFRepository<Site, Guid, BioDbContext>
    {
        public SitesRepository(EFRepositoryContext<Site, Guid, BioDbContext> repositoryContext) : base(repositoryContext)
        {
        }
    }
}
