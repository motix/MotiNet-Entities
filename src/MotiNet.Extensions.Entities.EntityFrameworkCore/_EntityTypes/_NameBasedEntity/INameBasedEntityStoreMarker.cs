using Microsoft.EntityFrameworkCore;

namespace MotiNet.Entities.EntityFrameworkCore
{
    public interface INameBasedEntityStoreMarker<TEntity, TDbContext> : IStore<TEntity, TDbContext>, INameBasedEntityStore<TEntity>
        where TEntity : class
        where TDbContext : DbContext
    { }
}
