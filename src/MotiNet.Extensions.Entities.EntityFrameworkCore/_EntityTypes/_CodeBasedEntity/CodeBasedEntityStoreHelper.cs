using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace MotiNet.Entities.EntityFrameworkCore
{
    public static class CodeBasedEntityStoreHelper
    {
        public static TEntity FindByCode<TEntity, TDbContext>(
            ICodeBasedEntityStoreMarker<TEntity, TDbContext> store,
            string normalizedCode, Expression<Func<TEntity, string>> codeSelector)
            where TEntity : class
            where TDbContext : DbContext
        {
            store.ThrowIfDisposed();

            return store.DbContext.Set<TEntity>().SingleOrDefault(codeSelector, normalizedCode);
        }

        public static TEntity FindByCode<TEntity, TDbContext>(
            ICodeBasedEntityStoreMarker<TEntity, TDbContext> store,
            string normalizedCode)
            where TEntity : class, ICodeWiseEntity
            where TDbContext : DbContext
        {
            store.ThrowIfDisposed();

            return store.DbContext.Set<TEntity>().SingleOrDefault(x => x.Code == normalizedCode);
        }

        public static Task<TEntity> FindByCodeAsync<TEntity, TDbContext>(
            ICodeBasedEntityStoreMarker<TEntity, TDbContext> store,
            string normalizedCode,
            Expression<Func<TEntity, string>> codeSelector,
            CancellationToken cancellationToken)
            where TEntity : class
            where TDbContext : DbContext
        {
            cancellationToken.ThrowIfCancellationRequested();
            store.ThrowIfDisposed();

            return store.DbContext.Set<TEntity>().SingleOrDefaultAsync(codeSelector, normalizedCode, cancellationToken);
        }

        public static Task<TEntity> FindByCodeAsync<TEntity, TDbContext>(
            ICodeBasedEntityStoreMarker<TEntity, TDbContext> store,
            string normalizedCode,
            CancellationToken cancellationToken)
            where TEntity : class, ICodeWiseEntity
            where TDbContext : DbContext
        {
            cancellationToken.ThrowIfCancellationRequested();
            store.ThrowIfDisposed();

            return store.DbContext.Set<TEntity>().SingleOrDefaultAsync(x => x.Code == normalizedCode, cancellationToken);
        }
    }
}
