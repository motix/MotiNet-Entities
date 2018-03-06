﻿using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace System.Linq
{
    public static class QueryableExtensions
    {
        public static TSource SingleOrDefault<TSource, TProperty>(
            this IQueryable<TSource> source,
            Expression<Func<TSource, TProperty>> propertySelector,
            TProperty propertyValue)
            where TProperty : IEquatable<TProperty>
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (propertySelector == null)
            {
                throw new ArgumentNullException(nameof(propertySelector));
            }

            var value = Expression.Constant(propertyValue);
            var body = Expression.Equal(propertySelector.Body, value);
            var lambda = Expression.Lambda(body, propertySelector.Parameters.First());

            var method = typeof(Queryable).GetMethods()
                .Single(m => m.Name == nameof(Queryable.SingleOrDefault) && m.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(TSource));

            return (TSource)method.Invoke(null, new object[] { source, lambda });
        }

        public static Task<TSource> SingleOrDefaultAsync<TSource, TProperty>(
            this IQueryable<TSource> source,
            Expression<Func<TSource, TProperty>> propertySelector,
            TProperty propertyValue,
            CancellationToken cancellationToken = default(CancellationToken))
            where TProperty : IEquatable<TProperty>
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (propertySelector == null)
            {
                throw new ArgumentNullException(nameof(propertySelector));
            }

            var value = Expression.Constant(propertyValue);
            var body = Expression.Equal(propertySelector.Body, value);
            var lambda = Expression.Lambda(body, propertySelector.Parameters.First());

            var method = typeof(EntityFrameworkQueryableExtensions).GetMethods()
                .Single(m => m.Name == nameof(EntityFrameworkQueryableExtensions.SingleOrDefaultAsync) && m.GetParameters().Length == 3)
                .MakeGenericMethod(typeof(TSource));

            return (Task<TSource>)method.Invoke(null, new object[] { source, lambda, cancellationToken });
        }
    }
}
