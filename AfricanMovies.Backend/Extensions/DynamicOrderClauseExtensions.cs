using System;
using System.Linq;
using System.Linq.Expressions;
using AfricanMovies.Backend.ViewModels;

namespace AfricanMovies.Backend.Extensions
{
    public static class DynamicOrderClauseExtensions
    {
        public static IOrderedQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> source, string orderByProperty,
            OrderDirectionEnum direction = OrderDirectionEnum.Ascending)
        {
            var command = direction == OrderDirectionEnum.Descending ? "OrderByDescending" : "OrderBy";

            var type = typeof(TEntity);

            var parameter = Expression.Parameter(type, "p");
            Expression expr = parameter;

            foreach (var prop in orderByProperty.Split('.'))
            {
                var pi = type.GetProperty(prop);
                expr = Expression.Property(expr, pi);
                type = pi.PropertyType;
            }

            var delegateType = typeof(Func<,>).MakeGenericType(typeof(TEntity), type);
            var lambda = Expression.Lambda(delegateType, expr, parameter);


            var result = typeof(Queryable).GetMethods().Single(
                method => method.Name == command
                          && method.IsGenericMethodDefinition
                          && method.GetGenericArguments().Length == 2
                          && method.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(TEntity), type)
                .Invoke(null, new object[] { source, lambda });
            return result as IOrderedQueryable<TEntity>;
        }
    }
}