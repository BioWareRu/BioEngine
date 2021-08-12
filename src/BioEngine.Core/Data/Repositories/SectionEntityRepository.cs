using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BioEngine.Core.Data.Entities.Abstractions;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Sitko.Core.Repository.EntityFrameworkCore;

namespace BioEngine.Core.Data.Repositories
{
    public abstract class SectionEntityRepository<TEntity> : PublishableEntityRepository<TEntity>
        where TEntity : class, ISectionEntity, IContentEntity
    {
        protected SectionEntityRepository(EFRepositoryContext<TEntity, Guid, BioDbContext> repositoryContext) :
            base(repositoryContext)
        {
        }
    }
}
