using Microsoft.EntityFrameworkCore;

namespace MotiNet.Entities.EntityFrameworkCore
{
    public interface IEntityStoreMarker<TEntity, TDbContext> : IStore<TEntity, TDbContext>, IEntityStore<TEntity>
        where TEntity : class
        where TDbContext : DbContext
    { }
}
