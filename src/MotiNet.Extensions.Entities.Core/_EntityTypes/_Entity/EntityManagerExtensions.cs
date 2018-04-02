using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MotiNet.Entities
{
    public static class EntityManagerExtensions
    {
        public static TEntity FindById<TEntity>(this IEntityManager<TEntity> manager, object id)
            where TEntity : class
        {
            manager.ThrowIfDisposed();
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            return manager.EntityStore.FindById(id);
        }

        public static Task<TEntity> FindByIdAsync<TEntity>(this IEntityManager<TEntity> manager, object id)
            where TEntity : class
        {
            manager.ThrowIfDisposed();
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            return manager.EntityStore.FindByIdAsync(id, manager.CancellationToken);
        }

        public static TEntity Find<TEntity>(this IEntityManager<TEntity> manager, object key, IFindSpecification<TEntity> spec)
            where TEntity : class
        {
            manager.ThrowIfDisposed();
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (spec == null)
            {
                throw new ArgumentNullException(nameof(spec));
            }

            return manager.EntityStore.Find(key, spec);
        }

        public static Task<TEntity> FindAsync<TEntity>(this IEntityManager<TEntity> manager, object key, IFindSpecification<TEntity> spec)
            where TEntity : class
        {
            manager.ThrowIfDisposed();
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (spec == null)
            {
                throw new ArgumentNullException(nameof(spec));
            }

            return manager.EntityStore.FindAsync(key, spec, manager.CancellationToken);
        }

        public static IEnumerable<TEntity> All<TEntity>(this IEntityManager<TEntity> manager)
            where TEntity : class
        {
            manager.ThrowIfDisposed();

            return manager.EntityStore.All();
        }

        public static Task<IEnumerable<TEntity>> AllAsync<TEntity>(this IEntityManager<TEntity> manager)
            where TEntity : class
        {
            manager.ThrowIfDisposed();

            return manager.EntityStore.AllAsync(manager.CancellationToken);
        }

        public static IEnumerable<TEntity> Search<TEntity>(this IEntityManager<TEntity> manager, ISearchSpecification<TEntity> spec)
            where TEntity : class
        {
            manager.ThrowIfDisposed();
            if (spec == null)
            {
                throw new ArgumentNullException(nameof(spec));
            }

            return manager.EntityStore.Search(spec);
        }

        public static Task<IEnumerable<TEntity>> SearchAsync<TEntity>(this IEntityManager<TEntity> manager, ISearchSpecification<TEntity> spec)
            where TEntity : class
        {
            manager.ThrowIfDisposed();
            if (spec == null)
            {
                throw new ArgumentNullException(nameof(spec));
            }

            return manager.EntityStore.SearchAsync(spec, manager.CancellationToken);
        }

        public static PagedSearchResult<TEntity> Search<TEntity>(this IEntityManager<TEntity> manager, IPagedSearchSpecification<TEntity> spec)
            where TEntity : class
        {
            manager.ThrowIfDisposed();
            if (spec == null)
            {
                throw new ArgumentNullException(nameof(spec));
            }

            return manager.EntityStore.Search(spec);
        }

        public static Task<PagedSearchResult<TEntity>> SearchAsync<TEntity>(this IEntityManager<TEntity> manager, IPagedSearchSpecification<TEntity> spec)
            where TEntity : class
        {
            manager.ThrowIfDisposed();
            if (spec == null)
            {
                throw new ArgumentNullException(nameof(spec));
            }

            return manager.EntityStore.SearchAsync(spec, manager.CancellationToken);
        }

        public static async Task<GenericResult> CreateAsync<TEntity>(this IEntityManager<TEntity> manager, TEntity entity)
            where TEntity : class
        {
            manager.ThrowIfDisposed();
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            await manager.ExecuteEntityValidatingAsync(entity);

            var result = await manager.ValidateEntityAsync(entity);
            if (!result.Succeeded)
            {
                return result;
            }

            await manager.ExecuteEntityCreatingAsync(entity);

            await manager.EntityStore.CreateAsync(entity, manager.CancellationToken);

            return GenericResult.Success;
        }

        public static async Task<GenericResult> UpdateAsync<TEntity>(this IEntityManager<TEntity> manager, TEntity entity)
            where TEntity : class
        {
            manager.ThrowIfDisposed();
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            await manager.ExecuteEntityValidatingAsync(entity);

            var result = await manager.ValidateEntityAsync(entity);
            if (!result.Succeeded)
            {
                return result;
            }

            await manager.ExecuteEntityUpdatingAsync(entity);

            await manager.EntityStore.UpdateAsync(entity, manager.CancellationToken);

            return GenericResult.Success;
        }

        public static async Task<GenericResult> DeleteAsync<TEntity>(this IEntityManager<TEntity> manager, TEntity entity)
            where TEntity : class
        {
            manager.ThrowIfDisposed();
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            await manager.EntityStore.DeleteAsync(entity, manager.CancellationToken);

            return GenericResult.Success;
        }
    }
}
