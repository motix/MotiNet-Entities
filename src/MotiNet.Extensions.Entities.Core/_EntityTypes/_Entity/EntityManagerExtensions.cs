using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MotiNet.Entities
{
    public static class EntityManagerExtensions
    {
        public static Task<TEntity> FindByIdAsync<TEntity>(this IEntityManager<TEntity> manager,
            object id, CancellationToken cancellationToken)
            where TEntity : class
        {
            manager.ThrowIfDisposed();
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            return manager.EntityStore.FindByIdAsync(id, cancellationToken);
        }

        public static Task<TEntity> FindAsync<TEntity>(this IEntityManager<TEntity> manager,
            object key, IFindSpecification<TEntity> spec, CancellationToken cancellationToken)
            where TEntity : class
        {
            manager.ThrowIfDisposed();
            if (spec == null)
            {
                throw new ArgumentNullException(nameof(spec));
            }

            return manager.EntityStore.FindAsync(key, spec, cancellationToken);
        }

        public static Task<IEnumerable<TEntity>> AllAsync<TEntity>(this IEntityManager<TEntity> manager,
            CancellationToken cancellationToken)
            where TEntity : class
        {
            manager.ThrowIfDisposed();

            return manager.EntityStore.AllAsync(cancellationToken);
        }

        public static Task<IEnumerable<TEntity>> SearchAsync<TEntity>(this IEntityManager<TEntity> manager,
            ISearchSpecification<TEntity> spec, CancellationToken cancellationToken)
            where TEntity : class
        {
            manager.ThrowIfDisposed();
            if (spec == null)
            {
                throw new ArgumentNullException(nameof(spec));
            }

            return manager.EntityStore.SearchAsync(spec, cancellationToken);
        }

        public static Task<PagedSearchResult<TEntity>> SearchAsync<TEntity>(this IEntityManager<TEntity> manager,
            IPagedSearchSpecification<TEntity> spec, CancellationToken cancellationToken)
            where TEntity : class
        {
            manager.ThrowIfDisposed();
            if (spec == null)
            {
                throw new ArgumentNullException(nameof(spec));
            }

            return manager.EntityStore.SearchAsync(spec, cancellationToken);
        }

        public static async Task<GenericResult> CreateAsync<TEntity>(this IEntityManager<TEntity> manager,
            TEntity entity, CancellationToken cancellationToken)
            where TEntity : class
        {
            manager.ThrowIfDisposed();
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            manager.RaiseEntityPreparingForValidation(entity);

            var result = await manager.ValidateEntityAsync(entity);
            if (!result.Succeeded)
            {
                return result;
            }

            manager.RaiseEntityPreparingForCreating(entity);

            await manager.EntityStore.CreateAsync(entity, cancellationToken);

            return GenericResult.Success;
        }

        public static async Task<GenericResult> UpdateAsync<TEntity>(this IEntityManager<TEntity> manager,
            TEntity entity, CancellationToken cancellationToken)
            where TEntity : class
        {
            manager.ThrowIfDisposed();
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            manager.RaiseEntityPreparingForValidation(entity);

            var result = await manager.ValidateEntityAsync(entity);
            if (!result.Succeeded)
            {
                return result;
            }

            manager.RaiseEntityPreparingForUpdating(entity);

            await manager.EntityStore.UpdateAsync(entity, cancellationToken);

            return GenericResult.Success;
        }

        public static async Task<GenericResult> DeleteAsync<TEntity>(this IEntityManager<TEntity> manager,
            TEntity entity, CancellationToken cancellationToken)
            where TEntity : class
        {
            manager.ThrowIfDisposed();
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            await manager.EntityStore.DeleteAsync(entity, cancellationToken);

            return GenericResult.Success;
        }
    }
}
