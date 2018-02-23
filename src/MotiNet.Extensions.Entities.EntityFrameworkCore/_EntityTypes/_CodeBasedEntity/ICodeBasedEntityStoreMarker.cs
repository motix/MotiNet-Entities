using Microsoft.EntityFrameworkCore;

namespace MotiNet.Entities.EntityFrameworkCore
{
    public interface ICodeBasedEntityStoreMarker<TEntity, TDbContext> : IStore<TEntity, TDbContext>, ICodeBasedEntityStore<TEntity>
        where TEntity : class
        where TDbContext : DbContext
    { }
}
