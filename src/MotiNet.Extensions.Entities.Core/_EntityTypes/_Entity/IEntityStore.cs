using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MotiNet.Entities
{
    public interface IEntityStore<TEntity> : IDisposable
        where TEntity : class
    {
        TEntity FindById(object id);

        Task<TEntity> FindByIdAsync(object id, CancellationToken cancellationToken);

        TEntity Find(object key, IFindSpecification<TEntity> spec);

        Task<TEntity> FindAsync(object key, IFindSpecification<TEntity> spec, CancellationToken cancellationToken);

        IEnumerable<TEntity> All();

        Task<IEnumerable<TEntity>> AllAsync(CancellationToken cancellationToken);

        IEnumerable<TEntity> Search(ISearchSpecification<TEntity> spec);

        Task<IEnumerable<TEntity>> SearchAsync(ISearchSpecification<TEntity> spec, CancellationToken cancellationToken);

        PagedSearchResult<TEntity> Search(IPagedSearchSpecification<TEntity> spec);

        Task<PagedSearchResult<TEntity>> SearchAsync(IPagedSearchSpecification<TEntity> spec, CancellationToken cancellationToken);

        Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken);

        Task<TEntity> CreateAsync(TEntity entity, IModifySpecification<TEntity> spec, CancellationToken cancellationToken);

        Task UpdateAsync(TEntity entity, CancellationToken cancellationToken);

        Task UpdateAsync(TEntity entity, IModifySpecification<TEntity> spec, CancellationToken cancellationToken);

        Task DeleteAsync(TEntity entity, CancellationToken cancellationToken);
    }
}
