using System.Threading;
using System.Threading.Tasks;

namespace MotiNet.Entities
{
    public interface IScopedNameBasedEntityStore<TEntity, TEntityScope>
        where TEntity : class
        where TEntityScope : class
    {
        TEntity FindByName(string normalizedName, TEntityScope scope);

        Task<TEntity> FindByNameAsync(string normalizedName, TEntityScope scope, CancellationToken cancellationToken);

        TEntityScope FindScopeById(object id, CancellationToken cancellationToken);

        Task<TEntityScope> FindScopeByIdAsync(object id, CancellationToken cancellationToken);

    }
}
