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
        public static TEntity FindByName<TEntity, TDbContext>(
            INameBasedEntityStoreMarker<TEntity, TDbContext> store,
            string normalizedName, Expression<Func<TEntity, string>> normalizedNameSelector)
            where TEntity : class
            where TDbContext : DbContext
        {
            store.ThrowIfDisposed();

            return store.DbContext.Set<TEntity>().SingleOrDefault(normalizedNameSelector, normalizedName);
        }

        public static TEntity FindByName<TEntity, TDbContext>(
            INameBasedEntityStoreMarker<TEntity, TDbContext> store,
            string normalizedName)
            where TEntity : class, INameWiseEntity
            where TDbContext : DbContext
        {
            store.ThrowIfDisposed();

            return store.DbContext.Set<TEntity>().SingleOrDefault(x => x.NormalizedName == normalizedName);
        }

        public static Task<TEntity> FindByNameAsync<TEntity, TDbContext>(
            INameBasedEntityStoreMarker<TEntity, TDbContext> store,
            string normalizedName,
            Expression<Func<TEntity, string>> normalizedNameSelector,
            CancellationToken cancellationToken)
            where TEntity : class
            where TDbContext : DbContext
        {
            cancellationToken.ThrowIfCancellationRequested();
            store.ThrowIfDisposed();

            return store.DbContext.Set<TEntity>().SingleOrDefaultAsync(normalizedNameSelector, normalizedName, cancellationToken);
        }

        public static Task<TEntity> FindByNameAsync<TEntity, TDbContext>(
            INameBasedEntityStoreMarker<TEntity, TDbContext> store,
            string normalizedName,
            CancellationToken cancellationToken)
            where TEntity : class, INameWiseEntity
            where TDbContext : DbContext
        {
            cancellationToken.ThrowIfCancellationRequested();
            store.ThrowIfDisposed();

            return store.DbContext.Set<TEntity>().SingleOrDefaultAsync(x => x.NormalizedName == normalizedName, cancellationToken);
        }
    }
}
