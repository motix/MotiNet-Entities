using System;
using System.Threading.Tasks;

namespace MotiNet.Entities
{
    public static class DeleteMarkEntityManagerExtensions
    {
        public static Task<GenericResult> MarkDeletedAsync<TEntity>(this IDeleteMarkEntityManager<TEntity> manager, TEntity entity)
            where TEntity : class
            => manager.MarkDeletedAsync(entity, null);

        public static async Task<GenericResult> MarkDeletedAsync<TEntity>(
            this IDeleteMarkEntityManager<TEntity> manager,
            TEntity entity,
            Action<TEntity> entityAction)
            where TEntity : class
        {
            manager.ThrowIfDisposed();
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var id = manager.EntityAccessor.GetId(entity);
            // Get old entity without passing through ExecuteEntityGetAsync
            var oldEntity = await manager.EntityStore.FindByIdAsync(id, manager.CancellationToken);

            if (oldEntity == null)
            {
                return GenericResult.Failed();
            }

            entityAction?.Invoke(oldEntity);

            manager.DeleteMarkEntityAccessor.MarkDeleted(oldEntity);

            await manager.EntityStore.UpdateAsync(oldEntity, manager.CancellationToken);

            return GenericResult.Success;
        }

        public static Task<GenericResult> UnmarkDeletedAsync<TEntity>(this IDeleteMarkEntityManager<TEntity> manager, TEntity entity)
            where TEntity : class
            => manager.UnmarkDeletedAsync(entity, null);


        public static async Task<GenericResult> UnmarkDeletedAsync<TEntity>(
            this IDeleteMarkEntityManager<TEntity> manager,
            TEntity entity,
            Action<TEntity> entityAction)
            where TEntity : class
        {
            manager.ThrowIfDisposed();
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var id = manager.EntityAccessor.GetId(entity);
            // Get old entity without passing through ExecuteEntityGetAsync
            var oldEntity = await manager.EntityStore.FindByIdAsync(id, manager.CancellationToken);

            if (oldEntity == null)
            {
                return GenericResult.Failed();
            }

            entityAction?.Invoke(oldEntity);

            manager.DeleteMarkEntityAccessor.UnmarkDeleted(oldEntity);

            await manager.EntityStore.UpdateAsync(oldEntity, manager.CancellationToken);

            return GenericResult.Success;
        }
    }
}
