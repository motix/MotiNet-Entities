using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace MotiNet.Entities.EntityFrameworkCore
{
    public static class NameBasedEntityStoreHelper
    {
        public static TEntity FindEntityByName<TEntity, TDbContext>(
            INameBasedEntityStoreMarker<TEntity, TDbContext> store,
            string normalizedName,
            Expression<Func<TEntity, string>> normalizedNameSelector)
            where TEntity : class
            where TDbContext : DbContext
        {
            store.ThrowIfDisposed();
            if (normalizedName == null)
            {
                throw new ArgumentNullException(nameof(normalizedName));
            }
            if (normalizedNameSelector == null)
            {
                throw new ArgumentNullException(nameof(normalizedNameSelector));
            }

            return store.DbContext.Set<TEntity>().SingleOrDefault(normalizedNameSelector, normalizedName);
        }

        public static TEntity FindEntityByName<TEntity, TDbContext>(
            INameBasedEntityStoreMarker<TEntity, TDbContext> store,
            string normalizedName)
            where TEntity : class, INameWiseEntity
            where TDbContext : DbContext
        {
            store.ThrowIfDisposed();
            if (normalizedName == null)
            {
                throw new ArgumentNullException(nameof(normalizedName));
            }

            return store.DbContext.Set<TEntity>().SingleOrDefault(x => x.NormalizedName == normalizedName);
        }

        public static Task<TEntity> FindEntityByNameAsync<TEntity, TDbContext>(
            INameBasedEntityStoreMarker<TEntity, TDbContext> store,
            string normalizedName,
            Expression<Func<TEntity, string>> normalizedNameSelector,
            CancellationToken cancellationToken)
            where TEntity : class
            where TDbContext : DbContext
        {
            cancellationToken.ThrowIfCancellationRequested();
            store.ThrowIfDisposed();
            if (normalizedName == null)
            {
                throw new ArgumentNullException(nameof(normalizedName));
            }
            if (normalizedNameSelector == null)
            {
                throw new ArgumentNullException(nameof(normalizedNameSelector));
            }

            return store.DbContext.Set<TEntity>().SingleOrDefaultAsync(normalizedNameSelector, normalizedName, cancellationToken);
        }

        public static Task<TEntity> FindEntityByNameAsync<TEntity, TDbContext>(
            INameBasedEntityStoreMarker<TEntity, TDbContext> store,
            string normalizedName,
            CancellationToken cancellationToken)
            where TEntity : class, INameWiseEntity
            where TDbContext : DbContext
        {
            cancellationToken.ThrowIfCancellationRequested();
            store.ThrowIfDisposed();
            if (normalizedName == null)
            {
                throw new ArgumentNullException(nameof(normalizedName));
            }

            return store.DbContext.Set<TEntity>().SingleOrDefaultAsync(x => x.NormalizedName == normalizedName, cancellationToken);
        }
    }
}
