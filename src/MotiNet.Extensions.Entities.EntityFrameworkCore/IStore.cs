using Microsoft.EntityFrameworkCore;

namespace MotiNet.Entities.EntityFrameworkCore
{
    public interface IStore<TEntity, TDbContext>
        where TEntity : class
        where TDbContext : DbContext
    {
        TDbContext DbContext { get; }

        void ThrowIfDisposed();
    }
}
