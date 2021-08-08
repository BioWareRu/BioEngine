using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using AntDesign;
using AntDesign.TableModels;
using Sitko.Core.Storage;

namespace BioEngine.Admin.Extensions
{
    public static class AntExtensions
    {
        public static IOrderedQueryable<TItem> ThenSort<TItem>(this ITableSortModel sortModel,
            IOrderedQueryable<TItem> source)
        {
            if (sortModel.Sort is null)
            {
                return source;
            }

            var propertyInfo = typeof(TItem).GetProperty(sortModel.FieldName);
            var method = typeof(AntExtensions).GetMethod(nameof(ThenSortByField));
            var genericMethod = method.MakeGenericMethod(typeof(TItem), propertyInfo.PropertyType);
            return genericMethod.Invoke(null, new object[] {sortModel, source, propertyInfo}) as
                IOrderedQueryable<TItem> ?? source;
        }

        public static IOrderedQueryable<TItem> ThenSortByField<TItem, TField>(ITableSortModel sortModel,
            IOrderedQueryable<TItem> source, PropertyInfo propertyInfo)
        {
            var sourceExpression = Expression.Parameter(typeof(TItem));
            var propertyExpression =
                Expression.Property(sourceExpression, propertyInfo);

            var lambda = Expression.Lambda<Func<TItem, TField>>(propertyExpression, sourceExpression);

            if (sortModel.Sort == SortDirection.Ascending.Name)
            {
                return source.ThenBy(lambda);
            }

            return source.ThenByDescending(lambda);
        }
    }
}
