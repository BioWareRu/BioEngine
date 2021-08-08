using System;
using System.Linq;
using BioEngine.Core.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Sitko.Core.Repository.EntityFrameworkCore;

namespace BioEngine.Core.Data.Repositories
{
    public abstract class SectionRepository<TEntity> : PublishableEntityRepository<TEntity> where TEntity : Section
    {
        protected SectionRepository(EFRepositoryContext<TEntity, Guid, BioDbContext> repositoryContext) : base(repositoryContext)
        {
        }

        protected override IQueryable<TEntity> AddIncludes(IQueryable<TEntity> query)
        {
            return base.AddIncludes(query).Include(s => s.Sites);
        }
    }
}
