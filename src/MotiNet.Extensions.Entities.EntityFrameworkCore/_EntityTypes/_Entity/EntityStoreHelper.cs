using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MotiNet.Entities.EntityFrameworkCore
{
    public static class EntityStoreHelper
    {
        public static TEntity FindEntityById<TEntity, TDbContext>(
            IEntityStoreMarker<TEntity, TDbContext> store,
            object id)
            where TEntity : class
            where TDbContext : DbContext
        {
            store.ThrowIfDisposed();
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            return store.DbContext.Set<TEntity>().Find(new object[] { id });
        }

        public static Task<TEntity> FindEntityByIdAsync<TEntity, TDbContext>(
            IEntityStoreMarker<TEntity, TDbContext> store,
            object id,
            CancellationToken cancellationToken)
            where TEntity : class
            where TDbContext : DbContext
        {
            cancellationToken.ThrowIfCancellationRequested();
            store.ThrowIfDisposed();
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            return store.DbContext.Set<TEntity>().FindAsync(new object[] { id }, cancellationToken).AsTask();
        }

        public static TEntity FindEntity<TEntity, TDbContext>(
            IEntityStoreMarker<TEntity, TDbContext> store,
            object key, IFindSpecification<TEntity> spec)
            where TEntity : class
            where TDbContext : DbContext
        {
            store.ThrowIfDisposed();
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (spec == null)
            {
                throw new ArgumentNullException(nameof(spec));
            }

            var entities = store.DbContext.Set<TEntity>().AsQueryable();

            entities = StoreHelper.Include(entities, spec);

            if (spec.AdditionalCriteria != null)
            {
                entities = entities.Where(spec.AdditionalCriteria);
            }

            var result = entities.SingleOrDefault(x => Equals(StoreHelper.GetPropertyValue(x, spec.KeyExpression), key));

            if (result != null && spec.ManyToManyIncludes != null)
            {
                StoreHelper.FillManyToManyRelationships(result, store.DbContext, spec.ManyToManyIncludes);
            }

            return result;
        }

        public static async Task<TEntity> FindEntityAsync<TEntity, TDbContext>(
            IEntityStoreMarker<TEntity, TDbContext> store,
            object key, IFindSpecification<TEntity> spec,
            CancellationToken cancellationToken)
            where TEntity : class
            where TDbContext : DbContext
        {
            cancellationToken.ThrowIfCancellationRequested();
            store.ThrowIfDisposed();
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (spec == null)
            {
                throw new ArgumentNullException(nameof(spec));
            }

            var entities = store.DbContext.Set<TEntity>().AsQueryable();

            entities = StoreHelper.Include(entities, spec);

            if (spec.AdditionalCriteria != null)
            {
                entities = entities.Where(spec.AdditionalCriteria);
            }

            var result = await entities.SingleOrDefaultAsync(x => Equals(StoreHelper.GetPropertyValue(x, spec.KeyExpression), key), cancellationToken);

            if (result != null && spec.ManyToManyIncludes != null)
            {
                await StoreHelper.FillManyToManyRelationshipsAsync(result, store.DbContext, spec.ManyToManyIncludes, cancellationToken);
            }

            return result;
        }

        public static IEnumerable<TEntity> AllEntities<TEntity, TDbContext>(
            IEntityStoreMarker<TEntity, TDbContext> store)
            where TEntity : class
            where TDbContext : DbContext
        {
            store.ThrowIfDisposed();

            return store.DbContext.Set<TEntity>().ToList();
        }

        public static async Task<IEnumerable<TEntity>> AllEntitiesAsync<TEntity, TDbContext>(
            IEntityStoreMarker<TEntity, TDbContext> store,
            CancellationToken cancellationToken)
            where TEntity : class
            where TDbContext : DbContext
        {
            cancellationToken.ThrowIfCancellationRequested();
            store.ThrowIfDisposed();

            return await store.DbContext.Set<TEntity>().ToListAsync(cancellationToken);
        }

        public static IEnumerable<TEntity> SearchEntities<TEntity, TDbContext>(
            IEntityStoreMarker<TEntity, TDbContext> store,
            ISearchSpecification<TEntity> spec)
            where TEntity : class
            where TDbContext : DbContext
        {
            store.ThrowIfDisposed();
            if (spec == null)
            {
                throw new ArgumentNullException(nameof(spec));
            }

            var entities = store.DbContext.Set<TEntity>().AsQueryable();

            entities = StoreHelper.Include(entities, spec);

            if (spec.Criteria != null)
            {
                entities = entities.Where(spec.Criteria);
            }

            var result = entities.ToList();

            if (spec.ManyToManyIncludes != null)
            {
                StoreHelper.FillManyToManyRelationships(result, store.DbContext, spec.ManyToManyIncludes);
            }

            return result;
        }

        public static async Task<IEnumerable<TEntity>> SearchEntitiesAsync<TEntity, TDbContext>(
            IEntityStoreMarker<TEntity, TDbContext> store,
            ISearchSpecification<TEntity> spec,
            CancellationToken cancellationToken)
            where TEntity : class
            where TDbContext : DbContext
        {
            cancellationToken.ThrowIfCancellationRequested();
            store.ThrowIfDisposed();
            if (spec == null)
            {
                throw new ArgumentNullException(nameof(spec));
            }

            var entities = store.DbContext.Set<TEntity>().AsQueryable();

            entities = StoreHelper.Include(entities, spec);

            if (spec.Criteria != null)
            {
                entities = entities.Where(spec.Criteria);
            }

            var result = await entities.ToListAsync(cancellationToken);

            if (spec.ManyToManyIncludes != null)
            {
                await StoreHelper.FillManyToManyRelationshipsAsync(result, store.DbContext, spec.ManyToManyIncludes, cancellationToken);
            }

            return result;
        }

        public static PagedSearchResult<TEntity> SearchEntities<TEntity, TDbContext>(
            IEntityStoreMarker<TEntity, TDbContext> store,
            IPagedSearchSpecification<TEntity> spec)
            where TEntity : class
            where TDbContext : DbContext
        {
            store.ThrowIfDisposed();
            if (spec == null)
            {
                throw new ArgumentNullException(nameof(spec));
            }

            var entities = store.DbContext.Set<TEntity>().AsQueryable();

            entities = StoreHelper.Include(entities, spec);

            if (spec.ScopeCriteria != null)
            {
                entities = entities.Where(spec.ScopeCriteria);
            }

            var totalCount = entities.Count();

            if (spec.Criteria != null)
            {
                entities = entities.Where(spec.Criteria);
            }

            var resultCount = entities.Count();

            entities = StoreHelper.Order(entities, spec);

            entities = StoreHelper.Page(entities, spec);

            var result = entities.ToList();

            if (spec.ManyToManyIncludes != null)
            {
                StoreHelper.FillManyToManyRelationships(result, store.DbContext, spec.ManyToManyIncludes);
            }

            return new PagedSearchResult<TEntity>(totalCount, resultCount, result);
        }

        public static async Task<PagedSearchResult<TEntity>> SearchEntitiesAsync<TEntity, TDbContext>(
            IEntityStoreMarker<TEntity, TDbContext> store,
            IPagedSearchSpecification<TEntity> spec,
            CancellationToken cancellationToken)
            where TEntity : class
            where TDbContext : DbContext
        {
            cancellationToken.ThrowIfCancellationRequested();
            store.ThrowIfDisposed();
            if (spec == null)
            {
                throw new ArgumentNullException(nameof(spec));
            }

            var entities = store.DbContext.Set<TEntity>().AsQueryable();

            entities = StoreHelper.Include(entities, spec);

            if (spec.ScopeCriteria != null)
            {
                entities = entities.Where(spec.ScopeCriteria);
            }

            var totalCount = await entities.CountAsync(cancellationToken);

            if (spec.Criteria != null)
            {
                entities = entities.Where(spec.Criteria);
            }

            var resultCount = await entities.CountAsync(cancellationToken);

            entities = StoreHelper.Order(entities, spec);

            entities = StoreHelper.Page(entities, spec);

            var result = await entities.ToListAsync(cancellationToken);

            if (spec.ManyToManyIncludes != null)
            {
                await StoreHelper.FillManyToManyRelationshipsAsync(result, store.DbContext, spec.ManyToManyIncludes, cancellationToken);
            }

            return new PagedSearchResult<TEntity>(totalCount, resultCount, result);
        }

        public static async Task<TEntity> CreateEntityAsync<TEntity, TDbContext>(
            IEntityStoreMarker<TEntity, TDbContext> store,
            TEntity entity,
            CancellationToken cancellationToken)
            where TEntity : class
            where TDbContext : DbContext
        {
            cancellationToken.ThrowIfCancellationRequested();
            store.ThrowIfDisposed();
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            store.DbContext.Set<TEntity>().Add(entity);
            await store.DbContext.SaveChangesAsync(cancellationToken);

            return entity;
        }

        public static async Task<TEntity> CreateEntityAsync<TEntity, TDbContext>(
            IEntityStoreMarker<TEntity, TDbContext> store,
            TEntity entity, IModifySpecification<TEntity> spec,
            CancellationToken cancellationToken)
            where TEntity : class
            where TDbContext : DbContext
        {
            cancellationToken.ThrowIfCancellationRequested();
            store.ThrowIfDisposed();
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            if (spec == null)
            {
                throw new ArgumentNullException(nameof(spec));
            }

            StoreHelper.PrepareOneToManyRelationships(entity, spec);

            store.DbContext.Set<TEntity>().Add(entity);
            await store.DbContext.SaveChangesAsync(cancellationToken);

            await StoreHelper.AddManyToManyRelationshipsAsync(entity, store.DbContext, spec, cancellationToken);

            return entity;
        }

        public static Task UpdateEntityAsync<TEntity, TDbContext>(
            IEntityStoreMarker<TEntity, TDbContext> store,
            TEntity entity,
            CancellationToken cancellationToken)
            where TEntity : class
            where TDbContext : DbContext
        {
            cancellationToken.ThrowIfCancellationRequested();
            store.ThrowIfDisposed();
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            store.DbContext.Entry(entity).State = EntityState.Modified;
            return store.DbContext.SaveChangesAsync(cancellationToken);
        }

        public static async Task UpdateEntityAsync<TEntity, TDbContext>(
            IEntityStoreMarker<TEntity, TDbContext> store,
            TEntity entity, IModifySpecification<TEntity> spec,
            CancellationToken cancellationToken)
            where TEntity : class
            where TDbContext : DbContext
        {
            cancellationToken.ThrowIfCancellationRequested();
            store.ThrowIfDisposed();
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            if (spec == null)
            {
                throw new ArgumentNullException(nameof(spec));
            }

            StoreHelper.PrepareOneToManyRelationships(entity, spec);

            await StoreHelper.UpdateManyToManyRelationshipsAsync(entity, store.DbContext, spec, cancellationToken);

            store.DbContext.Entry(entity).State = EntityState.Modified;
            await store.DbContext.SaveChangesAsync(cancellationToken);
        }

        public static Task DeleteEntityAsync<TEntity, TDbContext>(
            IEntityStoreMarker<TEntity, TDbContext> store,
            TEntity entity,
            CancellationToken cancellationToken)
            where TEntity : class
            where TDbContext : DbContext
        {
            cancellationToken.ThrowIfCancellationRequested();
            store.ThrowIfDisposed();
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            store.DbContext.Set<TEntity>().Remove(entity);
            return store.DbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
