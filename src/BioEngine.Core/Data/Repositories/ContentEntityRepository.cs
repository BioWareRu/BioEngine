using System;
using System.Threading.Tasks;
using BioEngine.Core.Data.Entities.Abstractions;
using Sitko.Core.Repository;
using Sitko.Core.Repository.EntityFrameworkCore;

namespace BioEngine.Core.Data.Repositories
{
    public interface IPublishableEntityRepository<TEntity> : IExternalRepository<TEntity, Guid>
        where TEntity : class, IPublishable, IBioEntity
    {
        Task PublishAsync(TEntity item, TEntity? snapshot = null);

        Task UnPublishAsync(TEntity item, TEntity? snapshot = null);
    }

    public abstract class PublishableEntityRepository<TEntity> : SiteEntityRepository<TEntity>,
        IPublishableEntityRepository<TEntity>
        where TEntity : class, IPublishable, ISiteEntity, IBioEntity
    {
        protected PublishableEntityRepository(EFRepositoryContext<TEntity, Guid, BioDbContext> repositoryContext) :
            base(repositoryContext)
        {
        }

        public virtual async Task PublishAsync(TEntity item, TEntity? snapshot = null)
        {
            item.IsPublished = true;
            item.DatePublished = DateTimeOffset.UtcNow;
            await UpdateExternalAsync(item, snapshot);
        }

        public virtual async Task UnPublishAsync(TEntity item, TEntity? snapshot = null)
        {
            item.IsPublished = false;
            item.DatePublished = null;
            await UpdateExternalAsync(item, snapshot);
        }
    }
}
