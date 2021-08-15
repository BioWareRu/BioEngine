using System;
using BioEngine.Core.Data.Entities.Abstractions;
using Sitko.Core.Repository;
using Sitko.Core.Repository.EntityFrameworkCore;

namespace BioEngine.Core.Data.Repositories
{
    public abstract class ContentItemRepository<TEntity> : SectionEntityRepository<TEntity>
        where TEntity : class, IContentItem, IEntity, ISectionEntity
    {
        protected ContentItemRepository(EFRepositoryContext<TEntity, Guid, BioDbContext> repositoryContext) :
            base(repositoryContext)
        {
        }
    }
}
