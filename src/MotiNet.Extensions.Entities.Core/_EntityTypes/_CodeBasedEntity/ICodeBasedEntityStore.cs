using System;
using System.Threading;
using System.Threading.Tasks;

namespace MotiNet.Entities
{
    public interface ICodeBasedEntityStore<TEntity> : IDisposable
        where TEntity : class
    {
        TEntity FindByCode(string normalizedCode);

        Task<TEntity> FindByCodeAsync(string normalizedCode, CancellationToken cancellationToken);
    }
}
