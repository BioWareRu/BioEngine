using System.Threading.Tasks;
using Sitko.Core.Blazor.AntDesignComponents.Components;
using Sitko.Core.Repository;

namespace BioEngine.Admin.Shared
{
    public abstract class BaseListPage<TEntity, TEntityPk, TRepository> : BasePage
        where TEntity : class, IEntity<TEntityPk>, new() where TRepository : IExternalRepository<TEntity, TEntityPk>
    {
        protected void ToFormPage() => NavigationManager.NavigateTo(CreatePageUrl);

        protected abstract string CreatePageUrl { get; }

        public BaseBioEngineList<TEntity, TEntityPk, TRepository> List { get; set; } = null!;
    }

    public abstract class
        BaseBioEngineList<TEntity, TEntityPk, TRepository> : BaseAntRepositoryList<TEntity, TEntityPk, TRepository>
        where TEntity : class, IEntity<TEntityPk>, new() where TRepository : IExternalRepository<TEntity, TEntityPk>
    {
        public virtual async Task DeleteAsync(TEntity entity)
        {
            await ExecuteRepositoryOperation(repository => repository.DeleteExternalAsync(entity));
            await RefreshAsync();
        }
    }
}
