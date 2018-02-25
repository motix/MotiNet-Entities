using System.Collections.Generic;
using System.Threading.Tasks;

namespace MotiNet.Entities
{
    public static class GroupedEntityManagerExtensions
    {
        public static Task<IEnumerable<TEntityGroup>> AllGroupsAsync<TEntity, TEntityGroup>(this IGroupedEntityManager<TEntity, TEntityGroup> manager)
            where TEntity : class
            where TEntityGroup : class
        {
            manager.ThrowIfDisposed();

            return manager.GroupedEntityStore.AllGroupsAsync(manager.CancellationToken);
        }

        public static Task<IEnumerable<TEntityGroup>> AllNonEmptyGroupsAsync<TEntity, TEntityGroup>(this IGroupedEntityManager<TEntity, TEntityGroup> manager)
            where TEntity : class
            where TEntityGroup : class
        {
            manager.ThrowIfDisposed();

            return manager.GroupedEntityStore.AllNonEmptyGroupsAsync(manager.CancellationToken);
        }
    }
}
