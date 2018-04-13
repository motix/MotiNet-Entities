using System;
using System.Threading;
using System.Threading.Tasks;

namespace MotiNet.Entities
{
    public interface IChildEntityStore<TEntity, TEntityParent> : IDisposable
        where TEntity : class
        where TEntityParent : class
    {
        TEntityParent FindParentById(object id);

        Task<TEntityParent> FindParentByIdAsync(object id, CancellationToken cancellationToken);
    }
}
