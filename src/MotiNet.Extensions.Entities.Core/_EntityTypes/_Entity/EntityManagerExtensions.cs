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

            var entity = manager.EntityStore.FindById(id);
            if (entity != null)
            {
                manager.ExecuteEntityGet(entity);
            }

            return entity;
        }

        public static async Task<TEntity> FindByIdAsync<TEntity>(this IEntityManager<TEntity> manager, object id)
            where TEntity : class
        {
            manager.ThrowIfDisposed();
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            var entity = await manager.EntityStore.FindByIdAsync(id, manager.CancellationToken);
            if (entity != null)
            {
                await manager.ExecuteEntityGetAsync(entity);
            }

            return entity;
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

            var entity = manager.EntityStore.Find(key, spec);
            if (entity != null)
            {
                manager.ExecuteEntityGet(entity);
            }

            return entity;
        }

        public static async Task<TEntity> FindAsync<TEntity>(this IEntityManager<TEntity> manager, object key, IFindSpecification<TEntity> spec)
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

            var entity = await manager.EntityStore.FindAsync(key, spec, manager.CancellationToken);
            if (entity != null)
            {
                await manager.ExecuteEntityGetAsync(entity);
            }

            return entity;
        }

        public static IEnumerable<TEntity> All<TEntity>(this IEntityManager<TEntity> manager)
            where TEntity : class
        {
            manager.ThrowIfDisposed();

            var entities = manager.EntityStore.All();
            manager.ExecuteEntitiesGet(entities);

            return entities;
        }

        public static async Task<IEnumerable<TEntity>> AllAsync<TEntity>(this IEntityManager<TEntity> manager)
            where TEntity : class
        {
            manager.ThrowIfDisposed();

            var entities = await manager.EntityStore.AllAsync(manager.CancellationToken);
            await manager.ExecuteEntitiesGetAsync(entities);

            return entities;
        }

        public static IEnumerable<TEntity> Search<TEntity>(this IEntityManager<TEntity> manager, ISearchSpecification<TEntity> spec)
            where TEntity : class
        {
            manager.ThrowIfDisposed();
            if (spec == null)
            {
                throw new ArgumentNullException(nameof(spec));
            }

            var entities = manager.EntityStore.Search(spec);
            manager.ExecuteEntitiesGet(entities);

            return entities;
        }

        public static async Task<IEnumerable<TEntity>> SearchAsync<TEntity>(this IEntityManager<TEntity> manager, ISearchSpecification<TEntity> spec)
            where TEntity : class
        {
            manager.ThrowIfDisposed();
            if (spec == null)
            {
                throw new ArgumentNullException(nameof(spec));
            }

            var entities = await manager.EntityStore.SearchAsync(spec, manager.CancellationToken);
            await manager.ExecuteEntitiesGetAsync(entities);

            return entities;
        }

        public static PagedSearchResult<TEntity> Search<TEntity>(this IEntityManager<TEntity> manager, IPagedSearchSpecification<TEntity> spec)
            where TEntity : class
        {
            manager.ThrowIfDisposed();
            if (spec == null)
            {
                throw new ArgumentNullException(nameof(spec));
            }

            var result = manager.EntityStore.Search(spec);
            manager.ExecuteEntitiesGet(result.Results);

            return result;
        }

        public static async Task<PagedSearchResult<TEntity>> SearchAsync<TEntity>(this IEntityManager<TEntity> manager, IPagedSearchSpecification<TEntity> spec)
            where TEntity : class
        {
            manager.ThrowIfDisposed();
            if (spec == null)
            {
                throw new ArgumentNullException(nameof(spec));
            }

            var result = await manager.EntityStore.SearchAsync(spec, manager.CancellationToken);
            await manager.ExecuteEntitiesGetAsync(result.Results);

            return result;
        }

        public static async Task<GenericResult> CreateAsync<TEntity>(this IEntityManager<TEntity> manager, TEntity entity)
            where TEntity : class
        {
            manager.ThrowIfDisposed();
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            await manager.ExecuteEntityCreateValidatingAsync(entity);

            var result = await manager.ValidateEntityAsync(entity);
            if (!result.Succeeded)
            {
                return result;
            }

            await manager.ExecuteEntityCreatingAsync(entity);

            await manager.EntityStore.CreateAsync(entity, manager.CancellationToken);

            await manager.ExecuteEntityCreatedAsync(entity);

            return GenericResult.Success;
        }

        public static Task<GenericResult> UpdateAsync<TEntity>(this IEntityManager<TEntity> manager, TEntity entity)
            where TEntity : class
            => UpdateInternal(manager, entity);

        public static Task<GenericResult> UpdateAsync<TEntity>(this IEntityManager<TEntity> manager, TEntity entity, IModifySpecification<TEntity> spec)
            where TEntity : class
            => UpdateInternal(manager, entity, spec);

        public static async Task<GenericResult> DeleteAsync<TEntity>(this IEntityManager<TEntity> manager, TEntity entity)
            where TEntity : class
        {
            manager.ThrowIfDisposed();
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            await manager.EntityStore.DeleteAsync(entity, manager.CancellationToken);

            await manager.ExecuteEntityDeletedAsync(entity);

            return GenericResult.Success;
        }

        private static async Task<GenericResult> UpdateInternal<TEntity>(this IEntityManager<TEntity> manager, TEntity entity, IModifySpecification<TEntity> spec = null)
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

            await manager.ExecuteEntityUpdateValidatingAsync(entity, oldEntity);

            var result = await manager.ValidateEntityAsync(entity);
            if (!result.Succeeded)
            {
                return result;
            }

            await manager.ExecuteEntityUpdatingAsync(entity, oldEntity);

            if (spec == null)
            {
                await manager.EntityStore.UpdateAsync(entity, manager.CancellationToken);
            }
            else
            {
                await manager.EntityStore.UpdateAsync(entity, spec, manager.CancellationToken);
            }

            await manager.ExecuteEntityUpdatedAsync(entity);

            return GenericResult.Success;
        }
    }
}
