using System;
using System.Threading.Tasks;
using BioEngine.Core.Data.Entities.Abstractions;
using Sitko.Core.Repository;
using Sitko.Core.Repository.EntityFrameworkCore;

namespace BioEngine.Core.Data.Repositories
{
    public interface IPublishableEntityRepository<TEntity> : IRepository<TEntity, Guid>
        where TEntity : class, IPublishable, IBioEntity
    {
        Task PublishAsync(TEntity item);

        Task UnPublishAsync(TEntity item);
    }

    public abstract class PublishableEntityRepository<TEntity> : SiteEntityRepository<TEntity>,
        IPublishableEntityRepository<TEntity>
        where TEntity : class, IPublishable, ISiteEntity, IBioEntity
    {
        protected PublishableEntityRepository(EFRepositoryContext<TEntity, Guid, BioDbContext> repositoryContext) : base(
            repositoryContext)
        {
        }

        public virtual async Task PublishAsync(TEntity item)
        {
            item.IsPublished = true;
            item.DatePublished = DateTimeOffset.UtcNow;
            await UpdateAsync(item);
        }

        public virtual async Task UnPublishAsync(TEntity item)
        {
            item.IsPublished = false;
            item.DatePublished = null;
            await UpdateAsync(item);
        }
    }
}
