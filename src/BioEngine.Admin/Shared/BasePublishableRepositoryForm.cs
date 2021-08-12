using System;
using System.Threading.Tasks;
using BioEngine.Core.Data.Entities.Abstractions;
using BioEngine.Core.Data.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Sitko.Core.Blazor.AntDesignComponents.Components;
using Sitko.Core.Repository;

namespace BioEngine.Admin.Shared
{
    public abstract class
        BasePublishableRepositoryForm<TEntity, TEntityPk, TRepository> : BaseAntRepositoryForm<TEntity, TEntityPk,
            TRepository>
        where TEntity : class, IEntity<TEntityPk>, IPublishable, new()
        where TRepository : class, IRepository<TEntity, TEntityPk>, IPublishableEntityRepository<TEntity>
    {
        public bool CanPublish()
        {
            if (IsNew)
            {
                return false;
            }

            if (HasChanges)
            {
                return false;
            }

            if (!IsValid)
            {
                return false;
            }

            return !Entity.IsPublished;
        }

        public bool CanUnPublish()
        {
            if (IsNew)
            {
                return false;
            }

            if (HasChanges)
            {
                return false;
            }

            if (!IsValid)
            {
                return false;
            }

            return Entity.IsPublished;
        }

        public async Task PublishAsync()
        {
            if (CanPublish())
            {
                await StartLoadingAsync();
                try
                {
                    using var scope = CreateServicesScope();
                    var repository = scope.ServiceProvider.GetRequiredService<TRepository>();
                    await repository.PublishAsync(Entity, EntitySnapshot);
                    await StopLoadingAsync();
                    await MessageService.Success("Запись успешно опубликована");
                }
                catch (Exception ex)
                {
                    await StopLoadingAsync();
                    await MessageService.Error(ex.ToString());
                }
            }
        }

        public async Task UnPublishAsync()
        {
            if (CanUnPublish())
            {
                await StartLoadingAsync();
                try
                {
                    using var scope = CreateServicesScope();
                    var repository = scope.ServiceProvider.GetRequiredService<TRepository>();
                    await repository.UnPublishAsync(Entity);
                    await StopLoadingAsync();
                    await MessageService.Success("Запись успешно спрятана");
                }
                catch (Exception ex)
                {
                    await StopLoadingAsync();
                    await MessageService.Error(ex.ToString());
                }
            }
        }
    }
}
