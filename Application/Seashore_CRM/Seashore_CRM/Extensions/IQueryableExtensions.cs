using System;
using System.Linq;
using System.Linq.Expressions;

namespace Seashore_CRM.Extensions
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> OrderByDynamic<T>(this IQueryable<T> query, string sortColumn, bool ascending)
        {
            if (string.IsNullOrWhiteSpace(sortColumn)) return query;

            var param = Expression.Parameter(typeof(T), "x");
            Expression prop;
            try
            {
                prop = sortColumn.Split('.').Aggregate<string, Expression>(param, Expression.PropertyOrField);
            }
            catch
            {
                return query; // invalid column
            }

            var lambda = Expression.Lambda(prop, param);
            string methodName = ascending ? "OrderBy" : "OrderByDescending";
            var result = Expression.Call(
                typeof(Queryable),
                methodName,
                new Type[] { typeof(T), prop.Type },
                query.Expression,
                Expression.Quote(lambda));
            return query.Provider.CreateQuery<T>(result);
        }
    }
}
