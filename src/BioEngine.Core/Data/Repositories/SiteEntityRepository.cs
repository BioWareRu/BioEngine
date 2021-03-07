using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BioEngine.Core.Data.Entities;
using BioEngine.Core.Data.Entities.Abstractions;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Sitko.Core.Repository.EntityFrameworkCore;

namespace BioEngine.Core.Data.Repositories
{
    public abstract class SiteEntityRepository<TEntity> : EFRepository<TEntity, Guid, BioDbContext>
        where TEntity : class, ISiteEntity, IBioEntity
    {
        protected SiteEntityRepository(EFRepositoryContext<TEntity, Guid, BioDbContext> repositoryContext)
            : base(repositoryContext)
        {
        }

        protected override async Task<bool> BeforeValidateAsync(TEntity item,
            (bool isValid, IList<ValidationFailure> errors) validationResult, bool isNew,
            CancellationToken cancellationToken = default)
        {
            item.DateUpdated = DateTimeOffset.UtcNow;
            if (!item.Sites.Any())
            {
                var sites = await Set<Site>().ToListAsync(cancellationToken);
                if (sites.Count == 1)
                {
                    item.Sites = new List<Site> {sites.First()};
                }
            }

            return await base.BeforeValidateAsync(item, validationResult, isNew, cancellationToken);
        }

        protected override IQueryable<TEntity> AddIncludes(IQueryable<TEntity> query)
        {
            return base.AddIncludes(query).Include(e => e.Sites);
        }
    }
}
