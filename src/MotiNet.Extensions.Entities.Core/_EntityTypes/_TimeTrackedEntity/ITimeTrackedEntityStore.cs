using System;
using System.Threading;
using System.Threading.Tasks;

namespace MotiNet.Entities
{
    public interface ITimeTrackedEntityStore<TEntity> : IDisposable
        where TEntity : class
    {
        TEntity FindLatest();

        Task<TEntity> FindLatestAsync(CancellationToken cancellationToken);
    }
}
