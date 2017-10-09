using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MotiNet.Entities
{
    public interface IAsyncRepository<TEntity> : IDisposable
        where TEntity : class
    {
        Task<TEntity> FindByIdAsync(object id, CancellationToken cancellationToken);

        Task<TEntity> FindByIdAsync(object id, IFindSpecification<TEntity> spec, CancellationToken cancellationToken);

        Task<IEnumerable<TEntity>> AllAsync(CancellationToken cancellationToken);

        Task<IEnumerable<TEntity>> SearchAsync(ISearchSpecification<TEntity> spec, CancellationToken cancellationToken);

        Task<PagedSearchResult<TEntity>> SearchAsync(IPagedSearchSpecification<TEntity> spec, CancellationToken cancellationToken);

        Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken);

        Task<TEntity> AddAsync(TEntity entity, IModifySpecification<TEntity> spec, CancellationToken cancellationToken);

        Task UpdateAsync(TEntity entity, CancellationToken cancellationToken);

        Task UpdateAsync(TEntity entity, IModifySpecification<TEntity> spec, CancellationToken cancellationToken);

        Task DeleteAsync(TEntity entity, CancellationToken cancellationToken);
    }
}
