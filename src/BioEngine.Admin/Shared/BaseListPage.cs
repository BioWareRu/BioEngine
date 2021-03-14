using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AntDesign;
using AntDesign.TableModels;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Sitko.Core.Repository;

namespace BioEngine.Admin.Shared
{
    public abstract class BaseListPage<TItem, TItemPk> : BasePage where TItem : class, IEntity<TItemPk>
    {
        [Inject] protected NavigationManager NavigationManager { get; set; }

        protected IEnumerable<TItem> Items;
        protected int Count;
        protected AntDesign.ITable table;
        protected int _pageIndex = 1;
        protected int _pageSize = 20;

        protected async Task OnChange(QueryModel<TItem> queryModel)
        {
            var orderBy = string.Join(",",
                queryModel.SortModel
                    .Where(s => s.Sort is not null)
                    .OrderBy(s => s.Priority)
                    .Select(s =>
                        $"{(s.Sort == SortDirection.Descending.Name ? "-" : "")}{s.FieldName.ToLowerInvariant()}"));
            await LoadData(string.IsNullOrEmpty(orderBy) ? "-title" : orderBy,
                queryModel.PageSize * (queryModel.PageIndex - 1), queryModel.PageSize);
        }

        private async Task LoadData(string orderBy, int skip = 0, int take = 20)
        {
            using var scope = ScopedServices.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IRepository<TItem, TItemPk>>();
            var results = await repository.GetAllAsync(query =>
                query.Take(take)
                    .Skip(skip)
                    .OrderByString(orderBy)
            );
            Items = results.items;
            Count = results.itemsCount;
        }

        protected void ToFormPage()
        {
            NavigationManager.NavigateTo(GetCreatePageUrl());
        }

        protected abstract string GetCreatePageUrl();
        protected abstract string GetTitle();
    }
}
