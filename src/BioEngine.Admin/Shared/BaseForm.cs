using System.Threading.Tasks;
using BioEngine.Core.Users;
using KellermanSoftware.CompareNetObjects;
using Sitko.Blockly.Blazor.Extensions;
using Sitko.Core.Blazor.AntDesignComponents.Components;
using Sitko.Core.Repository;

namespace BioEngine.Admin.Shared
{
    public abstract class BaseForm<TEntity, TEntityPk, TRepository> : BaseAntRepositoryForm<TEntity, TEntityPk,
        TRepository>
        where TEntity : class, IEntity<TEntityPk>, new()
        where TRepository : class, IRepository<TEntity, TEntityPk>
    {
        protected override void ConfigureComparer(ComparisonConfig comparisonConfig)
        {
            base.ConfigureComparer(comparisonConfig);
            comparisonConfig.AddBlocklyCollectionMapping();
        }

        protected override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            var user = await GetRequiredService<ICurrentUserProvider>().GetCurrentUserAsync();
            if (user?.IsAdmin == true)
            {
                Debug = true;
            }
        }
    }
}
