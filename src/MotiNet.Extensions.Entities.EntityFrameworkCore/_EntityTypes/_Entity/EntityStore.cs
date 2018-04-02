using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MotiNet.Entities.EntityFrameworkCore
{
    public class EntityStore<TEntity, TDbContext> : StoreBase<TEntity, TDbContext>, IEntityStoreMarker<TEntity, TDbContext>
        where TEntity : class
        where TDbContext : DbContext
    {
        #region Constructors

        public EntityStore(TDbContext dbContext) : base(dbContext) { }

        protected EntityStore() { }

        #endregion

        #region Public Operations

        public virtual TEntity FindById(object id)
        {
            return EntityStoreHelper.FindEntityById(this, id);
        }

        public virtual Task<TEntity> FindByIdAsync(object id, CancellationToken cancellationToken)
        {
            return EntityStoreHelper.FindEntityByIdAsync(this, id, cancellationToken);
        }

        public virtual TEntity Find(object key, IFindSpecification<TEntity> spec)
        {
            return EntityStoreHelper.FindEntity(this, key, spec);
        }

        public virtual Task<TEntity> FindAsync(object key, IFindSpecification<TEntity> spec, CancellationToken cancellationToken)
        {
            return EntityStoreHelper.FindEntityAsync(this, key, spec, cancellationToken);
        }

        public virtual IEnumerable<TEntity> All()
        {
            return EntityStoreHelper.AllEntities(this);
        }

        public virtual Task<IEnumerable<TEntity>> AllAsync(CancellationToken cancellationToken)
        {
            return EntityStoreHelper.AllEntitiesAsync(this, cancellationToken);
        }

        public virtual IEnumerable<TEntity> Search(ISearchSpecification<TEntity> spec)
        {
            return EntityStoreHelper.SearchEntities(this, spec);
        }

        public virtual Task<IEnumerable<TEntity>> SearchAsync(ISearchSpecification<TEntity> spec, CancellationToken cancellationToken)
        {
            return EntityStoreHelper.SearchEntitiesAsync(this, spec, cancellationToken);
        }

        public virtual PagedSearchResult<TEntity> Search(IPagedSearchSpecification<TEntity> spec)
        {
            return EntityStoreHelper.SearchEntities(this, spec);
        }

        public virtual Task<PagedSearchResult<TEntity>> SearchAsync(IPagedSearchSpecification<TEntity> spec, CancellationToken cancellationToken)
        {
            return EntityStoreHelper.SearchEntitiesAsync(this, spec, cancellationToken);
        }

        public virtual Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken)
        {
            return EntityStoreHelper.CreateEntityAsync(this, entity, cancellationToken);
        }

        public virtual Task<TEntity> CreateAsync(TEntity entity, IModifySpecification<TEntity> spec, CancellationToken cancellationToken)
        {
            return EntityStoreHelper.CreateEntityAsync(this, entity, spec, cancellationToken);
        }

        public virtual Task UpdateAsync(TEntity entity, CancellationToken cancellationToken)
        {
            return EntityStoreHelper.UpdateEntityAsync(this, entity, cancellationToken);
        }

        public virtual Task UpdateAsync(TEntity entity, IModifySpecification<TEntity> spec, CancellationToken cancellationToken)
        {
            return EntityStoreHelper.UpdateEntityAsync(this, entity, spec, cancellationToken);
        }

        public virtual Task DeleteAsync(TEntity entity, CancellationToken cancellationToken)
        {
            return EntityStoreHelper.DeleteEntityAsync(this, entity, cancellationToken);
        }

        #endregion
    }
}
