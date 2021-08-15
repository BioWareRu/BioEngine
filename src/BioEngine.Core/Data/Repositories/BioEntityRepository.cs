using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BioEngine.Core.Data.Entities.Abstractions;
using FluentValidation.Results;
using Sitko.Core.Repository.EntityFrameworkCore;

namespace BioEngine.Core.Data.Repositories
{
    public abstract class BioEntityRepository<TEntity> : EFRepository<TEntity, Guid, BioDbContext>
        where TEntity : class, IBioEntity
    {
        protected BioEntityRepository(EFRepositoryContext<TEntity, Guid, BioDbContext> repositoryContext) : base(
            repositoryContext)
        {
        }

        protected override async Task<bool> BeforeSaveAsync(TEntity item,
            (bool isValid, IList<ValidationFailure> errors) validationResult, bool isNew,
            CancellationToken cancellationToken = default)
        {
            if (await base.BeforeSaveAsync(item, validationResult, isNew, cancellationToken))
            {
                item.DateUpdated = DateTimeOffset.UtcNow;
                return true;
            }

            return false;
        }
    }
}
