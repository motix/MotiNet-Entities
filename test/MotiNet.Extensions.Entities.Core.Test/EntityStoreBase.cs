using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MotiNet.Entities.Test
{
    public class EntityStoreBase<TEntity> : IEntityStore<TEntity>
        where TEntity : class
    {
        public virtual TEntity FindById(object id)
        {
            throw new NotImplementedException();
        }

        public virtual Task<TEntity> FindByIdAsync(object id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public TEntity Find(object key, IFindSpecification<TEntity> spec)
        {
            throw new NotImplementedException();
        }

        public virtual Task<TEntity> FindAsync(object key, IFindSpecification<TEntity> spec, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TEntity> All()
        {
            throw new NotImplementedException();
        }

        public virtual Task<IEnumerable<TEntity>> AllAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TEntity> Search(ISearchSpecification<TEntity> spec)
        {
            throw new NotImplementedException();
        }

        public virtual Task<IEnumerable<TEntity>> SearchAsync(ISearchSpecification<TEntity> spec, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public PagedSearchResult<TEntity> Search(IPagedSearchSpecification<TEntity> spec)
        {
            throw new NotImplementedException();
        }

        public virtual Task<PagedSearchResult<TEntity>> SearchAsync(IPagedSearchSpecification<TEntity> spec, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public virtual Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public virtual Task<TEntity> CreateAsync(TEntity entity, IModifySpecification<TEntity> spec, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public virtual Task UpdateAsync(TEntity entity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public virtual Task UpdateAsync(TEntity entity, IModifySpecification<TEntity> spec, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public virtual Task DeleteAsync(TEntity entity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public virtual void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
