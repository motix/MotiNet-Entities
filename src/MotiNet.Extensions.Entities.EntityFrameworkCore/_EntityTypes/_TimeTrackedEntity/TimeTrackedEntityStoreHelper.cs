using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace MotiNet.Entities.EntityFrameworkCore
{
    public static class TimeTrackedEntityStoreHelper
    {
        public static TEntity FindLatest<TEntity, TDbContext>(
            ITimeTrackedEntityStoreMarker<TEntity, TDbContext> store,
            Expression<Func<TEntity, DateTime>> dataCreateDateSelector)
            where TEntity : class
            where TDbContext : DbContext
        {
            store.ThrowIfDisposed();

            return store.DbContext.Set<TEntity>().OrderByDescending(dataCreateDateSelector)
                                                 .FirstOrDefault();
        }

        public static TEntity FindLatest<TEntity, TDbContext>(
            ITimeTrackedEntityStoreMarker<TEntity, TDbContext> store)
            where TEntity : class, ITimeWiseEntity
            where TDbContext : DbContext
        {
            store.ThrowIfDisposed();

            return store.DbContext.Set<TEntity>().OrderByDescending(x => x.DataCreateDate)
                                                 .FirstOrDefault();
        }

        public static Task<TEntity> FindLatestAsync<TEntity, TDbContext>(
            ITimeTrackedEntityStoreMarker<TEntity, TDbContext> store,
            Expression<Func<TEntity, DateTime>> dataCreateDateSelector,
            CancellationToken cancellationToken)
            where TEntity : class
            where TDbContext : DbContext
        {
            cancellationToken.ThrowIfCancellationRequested();
            store.ThrowIfDisposed();

            return store.DbContext.Set<TEntity>().OrderByDescending(dataCreateDateSelector)
                                                 .FirstOrDefaultAsync(cancellationToken);
        }

        public static Task<TEntity> FindLatestAsync<TEntity, TDbContext>(
            ITimeTrackedEntityStoreMarker<TEntity, TDbContext> store,
            CancellationToken cancellationToken)
            where TEntity : class, ITimeWiseEntity
            where TDbContext : DbContext
        {
            cancellationToken.ThrowIfCancellationRequested();
            store.ThrowIfDisposed();

            return store.DbContext.Set<TEntity>().OrderByDescending(x => x.DataCreateDate)
                                                 .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
