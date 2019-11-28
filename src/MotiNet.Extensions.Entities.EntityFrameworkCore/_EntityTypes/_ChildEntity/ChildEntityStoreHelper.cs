using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MotiNet.Entities.EntityFrameworkCore
{
    public static class ChildEntityStoreHelper
    {
        public static TEntityParent FindParentById<TEntity, TEntityParent, TDbContext>(
            IChildEntityStoreMarker<TEntity, TEntityParent, TDbContext> store,
            object id)
            where TEntity : class
            where TEntityParent : class
            where TDbContext : DbContext
        {
            store.ThrowIfDisposed();
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            return store.DbContext.Set<TEntityParent>().Find(new object[] { id });
        }

        public static Task<TEntityParent> FindParentByIdAsync<TEntity, TEntityParent, TDbContext>(
            IChildEntityStoreMarker<TEntity, TEntityParent, TDbContext> store,
            object id,
            CancellationToken cancellationToken)
            where TEntity : class
            where TEntityParent : class
            where TDbContext : DbContext
        {
            cancellationToken.ThrowIfCancellationRequested();
            store.ThrowIfDisposed();
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            return store.DbContext.Set<TEntityParent>().FindAsync(new object[] { id }, cancellationToken).AsTask();
        }
    }
}
