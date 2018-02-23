using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MotiNet.Entities.EntityFrameworkCore
{
    public class EntityFrameworkRepository<TEntity, TDbContext> : IRepository<TEntity>, IAsyncRepository<TEntity>
        where TEntity : class
        where TDbContext : DbContext
    {
        #region Fields

        private readonly TDbContext _dbContext;

        #endregion

        #region Constructors

        public EntityFrameworkRepository(TDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        protected EntityFrameworkRepository() { }

        #endregion

        #region Public Operations

        #region FindById

        public virtual TEntity FindById(object id)
        {
            ThrowIfDisposed();
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            return _dbContext.Set<TEntity>().Find(new object[] { id });
        }

        public virtual Task<TEntity> FindByIdAsync(object id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            return _dbContext.Set<TEntity>().FindAsync(new object[] { id }, cancellationToken);
        }

        #endregion

        #region Find

        public virtual TEntity Find(object key, IFindSpecification<TEntity> spec)
        {
            ThrowIfDisposed();
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var entities = _dbContext.Set<TEntity>().AsQueryable();

            entities = StoreHelper.Include(entities, spec);

            if (spec.AdditionalCriteria != null)
            {
                entities = entities.Where(spec.AdditionalCriteria);
            }

            var result = entities.SingleOrDefault(x => Equals(StoreHelper.GetPropertyValue(x, spec.KeyExpression), key));

            if (spec.ManyToManyIncludes != null)
            {
                StoreHelper.FillManyToManyRelationships(result, _dbContext, spec.ManyToManyIncludes);
            }

            return result;
        }

        public virtual async Task<TEntity> FindAsync(object key, IFindSpecification<TEntity> spec, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var entities = _dbContext.Set<TEntity>().AsQueryable();

            entities = StoreHelper.Include(entities, spec);

            if (spec.AdditionalCriteria != null)
            {
                entities = entities.Where(spec.AdditionalCriteria);
            }

            var result = await entities.SingleOrDefaultAsync(x => Equals(StoreHelper.GetPropertyValue(x, spec.KeyExpression), key), cancellationToken);

            if (spec.ManyToManyIncludes != null)
            {
                await StoreHelper.FillManyToManyRelationshipsAsync(result, _dbContext, spec.ManyToManyIncludes, cancellationToken);
            }

            return result;
        }

        #endregion

        #region All

        public virtual IEnumerable<TEntity> All()
        {
            ThrowIfDisposed();

            return _dbContext.Set<TEntity>().ToList();
        }

        public virtual async Task<IEnumerable<TEntity>> AllAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            return await _dbContext.Set<TEntity>().ToListAsync(cancellationToken);
        }

        #endregion

        #region Search

        public virtual IEnumerable<TEntity> Search(ISearchSpecification<TEntity> spec)
        {
            ThrowIfDisposed();
            if (spec == null)
            {
                throw new ArgumentNullException(nameof(spec));
            }

            var entities = _dbContext.Set<TEntity>().AsQueryable();

            entities = StoreHelper.Include(entities, spec);

            if (spec.Criteria != null)
            {
                entities = entities.Where(spec.Criteria);
            }

            var result = entities.ToList();

            if (spec.ManyToManyIncludes != null)
            {
                StoreHelper.FillManyToManyRelationships(result, _dbContext, spec.ManyToManyIncludes);
            }

            return result;
        }

        public virtual async Task<IEnumerable<TEntity>> SearchAsync(ISearchSpecification<TEntity> spec, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (spec == null)
            {
                throw new ArgumentNullException(nameof(spec));
            }

            var entities = _dbContext.Set<TEntity>().AsQueryable();

            entities = StoreHelper.Include(entities, spec);

            if (spec.Criteria != null)
            {
                entities = entities.Where(spec.Criteria);
            }

            var result = await entities.ToListAsync(cancellationToken);

            if (spec.ManyToManyIncludes != null)
            {
                await StoreHelper.FillManyToManyRelationshipsAsync(result, _dbContext, spec.ManyToManyIncludes, cancellationToken);
            }

            return result;
        }

        public virtual PagedSearchResult<TEntity> Search(IPagedSearchSpecification<TEntity> spec)
        {
            ThrowIfDisposed();
            if (spec == null)
            {
                throw new ArgumentNullException(nameof(spec));
            }

            var entities = _dbContext.Set<TEntity>().AsQueryable();

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
                StoreHelper.FillManyToManyRelationships(result, _dbContext, spec.ManyToManyIncludes);
            }

            return new PagedSearchResult<TEntity>(totalCount, resultCount, result);
        }

        public virtual async Task<PagedSearchResult<TEntity>> SearchAsync(IPagedSearchSpecification<TEntity> spec, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (spec == null)
            {
                throw new ArgumentNullException(nameof(spec));
            }

            var entities = _dbContext.Set<TEntity>().AsQueryable();

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
                await StoreHelper.FillManyToManyRelationshipsAsync(result, _dbContext, spec.ManyToManyIncludes, cancellationToken);
            }

            return new PagedSearchResult<TEntity>(totalCount, resultCount, result);
        }

        #endregion

        #region Create

        public virtual TEntity Create(TEntity entity)
        {
            ThrowIfDisposed();
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            _dbContext.Set<TEntity>().Add(entity);
            _dbContext.SaveChanges();

            return entity;
        }

        public virtual async Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            _dbContext.Set<TEntity>().Add(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return entity;
        }

        public virtual TEntity Create(TEntity entity, IModifySpecification<TEntity> spec)
        {
            ThrowIfDisposed();
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            if (spec == null)
            {
                throw new ArgumentNullException(nameof(spec));
            }

            StoreHelper.PrepareOneToManyRelationships(entity, spec);

            _dbContext.Set<TEntity>().Add(entity);
            _dbContext.SaveChanges();

            StoreHelper.AddManyToManyRelationships(entity, _dbContext, spec);

            return entity;
        }

        public virtual async Task<TEntity> CreateAsync(TEntity entity, IModifySpecification<TEntity> spec, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            if (spec == null)
            {
                throw new ArgumentNullException(nameof(spec));
            }

            StoreHelper.PrepareOneToManyRelationships(entity, spec);

            _dbContext.Set<TEntity>().Add(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);

            await StoreHelper.AddManyToManyRelationshipsAsync(entity, _dbContext, spec, cancellationToken);

            return entity;
        }

        #endregion

        #region Update

        public virtual void Update(TEntity entity)
        {
            ThrowIfDisposed();
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            _dbContext.Entry(entity).State = EntityState.Modified;
            _dbContext.SaveChanges();
        }

        public virtual async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public virtual void Update(TEntity entity, IModifySpecification<TEntity> spec)
        {
            ThrowIfDisposed();
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            if (spec == null)
            {
                throw new ArgumentNullException(nameof(spec));
            }

            StoreHelper.PrepareOneToManyRelationships(entity, spec);

            StoreHelper.UpdateManyToManyRelationships(entity, _dbContext, spec);

            _dbContext.Entry(entity).State = EntityState.Modified;
            _dbContext.SaveChanges();
        }

        public virtual async Task UpdateAsync(TEntity entity, IModifySpecification<TEntity> spec, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            if (spec == null)
            {
                throw new ArgumentNullException(nameof(spec));
            }

            StoreHelper.PrepareOneToManyRelationships(entity, spec);

            await StoreHelper.UpdateManyToManyRelationshipsAsync(entity, _dbContext, spec, cancellationToken);

            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        #endregion

        #region Delete

        public virtual void Delete(TEntity entity)
        {
            ThrowIfDisposed();
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            _dbContext.Set<TEntity>().Remove(entity);
            _dbContext.SaveChanges();
        }

        public virtual async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            _dbContext.Set<TEntity>().Remove(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        #endregion

        #endregion

        #region IDisposable Support

        private bool _disposed = false; // To detect redundant calls

        protected void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    _dbContext.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                _disposed = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~EfRepository() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        #endregion
    }
}
