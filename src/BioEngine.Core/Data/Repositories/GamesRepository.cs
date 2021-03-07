using System;
using BioEngine.Core.Data.Entities;
using Sitko.Core.Repository.EntityFrameworkCore;

namespace BioEngine.Core.Data.Repositories
{
    public sealed class GamesRepository : SectionRepository<Game>
    {
        public GamesRepository(EFRepositoryContext<Game, Guid, BioDbContext> repositoryContext) : base(
            repositoryContext)
        {
        }
    }
}
