using BioEngine.Core.Data.Entities.Abstractions;
using BioEngine.Core.Data.Repositories;
using KellermanSoftware.CompareNetObjects;
using Sitko.Blockly.Blazor.Extensions;
using Sitko.Core.Blazor.AntDesignComponents.Components;
using Sitko.Core.Repository;

namespace BioEngine.Admin.Shared
{
    public abstract class BaseForm<TEntity, TEntityPk, TRepository> : BaseAntRepositoryForm<TEntity, TEntityPk,
        TRepository>
        where TEntity : class, IEntity<TEntityPk>, IPublishable, new()
        where TRepository : class, IRepository<TEntity, TEntityPk>, IPublishableEntityRepository<TEntity>
    {
        protected override void ConfigureComparer(ComparisonConfig comparisonConfig)
        {
            base.ConfigureComparer(comparisonConfig);
            comparisonConfig.AddBlocklyCollectionMapping();
        }
    }
}
