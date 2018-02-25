using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MotiNet.Entities
{
    public interface IGroupedEntityStore<TEntity, TEntityGroup> : IDisposable
        where TEntity : class
        where TEntityGroup : class
    {
        Task<IEnumerable<TEntityGroup>> AllGroupsAsync(CancellationToken cancellationToken);

        Task<IEnumerable<TEntityGroup>> AllNonEmptyGroupsAsync(CancellationToken cancellationToken);
    }
}
