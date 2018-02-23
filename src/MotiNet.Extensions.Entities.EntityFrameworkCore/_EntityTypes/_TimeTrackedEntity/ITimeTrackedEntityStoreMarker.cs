using Microsoft.EntityFrameworkCore;

namespace MotiNet.Entities.EntityFrameworkCore
{
    public interface ITimeTrackedEntityStoreMarker<TEntity, TDbContext> : IStore<TEntity, TDbContext>, ITimeTrackedEntityStore<TEntity>
        where TEntity : class
        where TDbContext : DbContext
    { }
}
