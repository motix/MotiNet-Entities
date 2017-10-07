using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MotiNet.Entities
{
    public interface IRepository<TEntity> : IDisposable
        where TEntity : class
    {
        Task<TEntity> FindByIdAsync<TKey>(TKey id, CancellationToken cancellationToken)
            where TKey : IEquatable<TKey>;

        Task<TEntity> FindByIdAsync<TKey>(TKey id, IFindSpecification<TEntity, TKey> spec, CancellationToken cancellationToken)
            where TKey : IEquatable<TKey>;

        Task<IEnumerable<TEntity>> AllAsync(CancellationToken cancellationToken);

        Task<IEnumerable<TEntity>> SearchAsync(ISearchSpecification<TEntity> spec, CancellationToken cancellationToken);

        Task<PagedSearchResult<TEntity>> SearchAsync(IPagedSearchSpecification<TEntity> spec, CancellationToken cancellationToken);

        Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken);

        Task<TEntity> AddAsync<TKey>(TEntity entity, IModifySpecification<TEntity, TKey> spec, CancellationToken cancellationToken)
            where TKey : IEquatable<TKey>;

        Task UpdateAsync(TEntity entity, CancellationToken cancellationToken);

        Task UpdateAsync<TKey>(TEntity entity, IModifySpecification<TEntity, TKey> spec, CancellationToken cancellationToken)
            where TKey : IEquatable<TKey>;

        Task DeleteAsync(TEntity entity, CancellationToken cancellationToken);
    }
}
