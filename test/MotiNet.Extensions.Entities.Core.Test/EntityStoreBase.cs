using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MotiNet.Entities.Test
{
    public class EntityStoreBase<TEntity> : IEntityStore<TEntity>
        where TEntity : class
    {
        public virtual Task<TEntity> FindByIdAsync(object id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public virtual Task<TEntity> FindAsync(object key, IFindSpecification<TEntity> spec, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public virtual Task<IEnumerable<TEntity>> AllAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public virtual Task<IEnumerable<TEntity>> SearchAsync(ISearchSpecification<TEntity> spec, CancellationToken cancellationToken)
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
