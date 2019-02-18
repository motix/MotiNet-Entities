using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MotiNet.Entities
{
    public static class OnOffEntityManagerExtensions
    {
        public static Task<IEnumerable<TEntity>> GetAllActiveAsync<TEntity>(this IOnOffEntityManager<TEntity> manager)
            where TEntity : class
            => manager.GetAllActiveAsync(null);

        public static Task<IEnumerable<TEntity>> GetAllActiveAsync<TEntity>(this IOnOffEntityManager<TEntity> manager,
            Action<ISearchSpecification<TEntity>> specificationAction)
            where TEntity : class
        {
            manager.ThrowIfDisposed();

            var spec = manager.OnOffEntityStore.SearchActiveEntitiesSpecification;
            specificationAction?.Invoke(spec);

            return manager.SearchAsync(spec);
        }
    }
}
