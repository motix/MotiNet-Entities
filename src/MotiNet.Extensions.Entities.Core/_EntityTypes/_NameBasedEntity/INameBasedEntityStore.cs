using System;
using System.Threading;
using System.Threading.Tasks;

namespace MotiNet.Entities
{
    public interface INameBasedEntityStore<TEntity> : IDisposable
        where TEntity : class
    {
        TEntity FindByName(string normalizedName);

        Task<TEntity> FindByNameAsync(string normalizedName, CancellationToken cancellationToken);
    }
}
