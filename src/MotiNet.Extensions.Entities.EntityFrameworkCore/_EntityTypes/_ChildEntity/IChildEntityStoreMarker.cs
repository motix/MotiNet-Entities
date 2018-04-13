using Microsoft.EntityFrameworkCore;

namespace MotiNet.Entities.EntityFrameworkCore
{
    public interface IChildEntityStoreMarker<TEntity, TEntityParent, TDbContext> : IStore<TEntity, TDbContext>, IChildEntityStore<TEntity, TEntityParent>
        where TEntity : class
        where TEntityParent : class
        where TDbContext : DbContext
    { }
}
