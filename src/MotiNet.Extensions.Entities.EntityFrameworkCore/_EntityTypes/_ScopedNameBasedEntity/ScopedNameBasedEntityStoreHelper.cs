using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace MotiNet.Entities.EntityFrameworkCore
{
    public static class ScopedNameBasedEntityStoreHelper
    {
        public static TEntity FindEntityByName<TEntity, TEntityScope, TKey, TDbContext>(
            IScopedNameBasedEntityStoreMarker<TEntity, TEntityScope, TDbContext> store,
            string normalizedName, TEntityScope scope,
            Expression<Func<TEntity, TKey>> scopeIdSelector,
            Expression<Func<TEntity, string>> normalizedNameSelector,
            Expression<Func<TEntityScope, TKey>> idSelector)
            where TEntity : class
            where TEntityScope : class
            where TKey : IEquatable<TKey>
            where TDbContext : DbContext
        {
            store.ThrowIfDisposed();
            if (normalizedName == null)
            {
                throw new ArgumentNullException(nameof(normalizedName));
            }
            if (scope == null)
            {
                throw new ArgumentNullException(nameof(scope));
            }
            if (scopeIdSelector == null)
            {
                throw new ArgumentNullException(nameof(scopeIdSelector));
            }
            if (normalizedNameSelector == null)
            {
                throw new ArgumentNullException(nameof(normalizedNameSelector));
            }
            if (idSelector == null)
            {
                throw new ArgumentNullException(nameof(idSelector));
            }

            var scopeId = idSelector.Compile()(scope);
            var entities = GetScopedEntities(store, scopeIdSelector, scopeId);
            return entities.SingleOrDefault(normalizedNameSelector, normalizedName);
        }

        public static TEntity FindEntityByName<TEntity, TEntityScope, TKey, TDbContext>(
            IScopedNameBasedEntityStoreMarker<TEntity, TEntityScope, TDbContext> store,
            string normalizedName, TEntityScope scope,
            Expression<Func<TEntity, TKey>> scopeIdSelector)
            where TEntity : class, INameWiseEntity
            where TEntityScope : class, IIdWiseEntity<TKey>
            where TKey : IEquatable<TKey>
            where TDbContext : DbContext
        {
            store.ThrowIfDisposed();
            if (normalizedName == null)
            {
                throw new ArgumentNullException(nameof(normalizedName));
            }
            if (scope == null)
            {
                throw new ArgumentNullException(nameof(scope));
            }
            if (scopeIdSelector == null)
            {
                throw new ArgumentNullException(nameof(scopeIdSelector));
            }

            var scopeId = scope.Id;
            var entities = GetScopedEntities(store, scopeIdSelector, scopeId);
            return entities.SingleOrDefault(x => x.NormalizedName == normalizedName);
        }

        public static Task<TEntity> FindEntityByNameAsync<TEntity, TEntityScope, TKey, TDbContext>(
            IScopedNameBasedEntityStoreMarker<TEntity, TEntityScope, TDbContext> store,
            string normalizedName, TEntityScope scope,
            Expression<Func<TEntity, TKey>> scopeIdSelector,
            Expression<Func<TEntity, string>> normalizedNameSelector,
            Expression<Func<TEntityScope, TKey>> idSelector,
            CancellationToken cancellationToken)
            where TEntity : class
            where TEntityScope : class
            where TKey : IEquatable<TKey>
            where TDbContext : DbContext
        {
            cancellationToken.ThrowIfCancellationRequested();
            store.ThrowIfDisposed();
            if (normalizedName == null)
            {
                throw new ArgumentNullException(nameof(normalizedName));
            }
            if (scope == null)
            {
                throw new ArgumentNullException(nameof(scope));
            }
            if (scopeIdSelector == null)
            {
                throw new ArgumentNullException(nameof(scopeIdSelector));
            }
            if (normalizedNameSelector == null)
            {
                throw new ArgumentNullException(nameof(normalizedNameSelector));
            }
            if (idSelector == null)
            {
                throw new ArgumentNullException(nameof(idSelector));
            }

            var scopeId = idSelector.Compile()(scope);
            var entities = GetScopedEntities(store, scopeIdSelector, scopeId);
            return entities.SingleOrDefaultAsync(normalizedNameSelector, normalizedName, cancellationToken);
        }

        public static Task<TEntity> FindEntityByNameAsync<TEntity, TEntityScope, TKey, TDbContext>(
            IScopedNameBasedEntityStoreMarker<TEntity, TEntityScope, TDbContext> store,
            string normalizedName, TEntityScope scope,
            Expression<Func<TEntity, TKey>> scopeIdSelector,
            CancellationToken cancellationToken)
            where TEntity : class, INameWiseEntity
            where TEntityScope : class, IIdWiseEntity<TKey>
            where TKey : IEquatable<TKey>
            where TDbContext : DbContext
        {
            cancellationToken.ThrowIfCancellationRequested();
            store.ThrowIfDisposed();
            if (normalizedName == null)
            {
                throw new ArgumentNullException(nameof(normalizedName));
            }
            if (scope == null)
            {
                throw new ArgumentNullException(nameof(scope));
            }
            if (scopeIdSelector == null)
            {
                throw new ArgumentNullException(nameof(scopeIdSelector));
            }

            var scopeId = scope.Id;
            var entities = GetScopedEntities(store, scopeIdSelector, scopeId);
            return entities.SingleOrDefaultAsync(x => x.NormalizedName == normalizedName, cancellationToken);
        }

        public static TEntityScope FindScopeById<TEntity, TEntityScope, TDbContext>(
            IScopedNameBasedEntityStoreMarker<TEntity, TEntityScope, TDbContext> store,
            object id)
            where TEntity : class
            where TEntityScope : class
            where TDbContext : DbContext
        {
            store.ThrowIfDisposed();
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            return store.DbContext.Set<TEntityScope>().Find(new object[] { id });
        }

        public static Task<TEntityScope> FindScopeByIdAsync<TEntity, TEntityScope, TDbContext>(
            IScopedNameBasedEntityStoreMarker<TEntity, TEntityScope, TDbContext> store,
            object id,
            CancellationToken cancellationToken)
            where TEntity : class
            where TEntityScope : class
            where TDbContext : DbContext
        {
            cancellationToken.ThrowIfCancellationRequested();
            store.ThrowIfDisposed();
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            return store.DbContext.Set<TEntityScope>().FindAsync(new object[] { id }, cancellationToken).AsTask();
        }

        private static IQueryable<TEntity> GetScopedEntities<TEntity, TEntityScope, TKey, TDbContext>(
            IScopedNameBasedEntityStoreMarker<TEntity, TEntityScope, TDbContext> store,
            Expression<Func<TEntity, TKey>> scopeIdSelector,
            TKey scopeId)
            where TEntity : class
            where TEntityScope : class
            where TKey : IEquatable<TKey>
            where TDbContext : DbContext
        {
            var param = Expression.Parameter(typeof(TEntity), "x");
            var scopeIdMember = Expression.Property(param, scopeIdSelector.GetPropertyAccess().Name);
            var scopeIdExpression = Expression.Equal(scopeIdMember, Expression.Constant(scopeId, typeof(TKey)));
            var predicate = Expression.Lambda<Func<TEntity, bool>>(scopeIdExpression, param);
            return store.DbContext.Set<TEntity>().Where(predicate);
        }
    }
}
