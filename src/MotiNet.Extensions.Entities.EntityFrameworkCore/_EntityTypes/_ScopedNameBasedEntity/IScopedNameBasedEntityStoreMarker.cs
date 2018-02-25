using Microsoft.EntityFrameworkCore;

namespace MotiNet.Entities.EntityFrameworkCore
{
    public interface IScopedNameBasedEntityStoreMarker<TEntity, TEntityScope, TDbContext> : IStore<TEntity, TDbContext>, IScopedNameBasedEntityStore<TEntity, TEntityScope>
        where TEntity : class
        where TEntityScope : class
        where TDbContext : DbContext
    { }
}
