using System;
using System.Threading.Tasks;

namespace MotiNet.Entities
{
    public static class ScopedNameBasedEntityManagerExtensions
    {
        public static TEntity FindByName<TEntity, TEntityScope>(this IScopedNameBasedEntityManager<TEntity, TEntityScope> manager, string name, TEntityScope scope)
            where TEntity : class
            where TEntityScope : class
        {
            manager.ThrowIfDisposed();
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            return manager.ScopedNameBasedEntityStore.FindByName(NormalizeEntityName(manager, name), scope);
        }

        public static Task<TEntity> FindByNameAsync<TEntity, TEntityScope>(this IScopedNameBasedEntityManager<TEntity, TEntityScope> manager, string name, TEntityScope scope)
            where TEntity : class
            where TEntityScope : class
        {
            manager.ThrowIfDisposed();
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            return manager.ScopedNameBasedEntityStore.FindByNameAsync(NormalizeEntityName(manager, name), scope, manager.CancellationToken);
        }

        public static ManagerTasks<TEntity, TEntityScope> GetManagerTasks<TEntity, TEntityScope>()
            where TEntity : class
            where TEntityScope : class
        {
            return new ManagerTasks<TEntity, TEntityScope>()
            {
                EntityWithSubEntityValidatingAsync = EntityValidatingAsync,
                EntityWithSubEntitySavingAsync = EntitySavingAsync
            };
        }

        private static async Task EntityValidatingAsync<TEntity, TEntityScope>(IManager<TEntity, TEntityScope> manager, ManagerTaskArgs<TEntity> taskArgs)
            where TEntity : class
            where TEntityScope : class
        {
            var scopedNameBasedManager = (IScopedNameBasedEntityManager<TEntity, TEntityScope>)manager;

            var scopeId = scopedNameBasedManager.ScopedNameBasedEntityAccessor.GetScopeId(taskArgs.Entity);
            var scope = await scopedNameBasedManager.ScopedNameBasedEntityStore.FindScopeByIdAsync(scopeId, manager.CancellationToken);
            scopedNameBasedManager.ScopedNameBasedEntityAccessor.SetScope(taskArgs.Entity, scope);
        }

        private static Task EntitySavingAsync<TEntity, TEntityScope>(IManager<TEntity, TEntityScope> manager, ManagerTaskArgs<TEntity> taskArgs)
            where TEntity : class
            where TEntityScope : class
        {
            var scopedNameBasedManager = (IScopedNameBasedEntityManager<TEntity, TEntityScope>)manager;

            var name = scopedNameBasedManager.ScopedNameBasedEntityAccessor.GetName(taskArgs.Entity);
            var normalizedName = NormalizeEntityName(scopedNameBasedManager, name);
            scopedNameBasedManager.ScopedNameBasedEntityAccessor.SetNormalizedName(taskArgs.Entity, normalizedName);

            return Task.FromResult(0);
        }

        private static string NormalizeEntityName<TEntity, TEntityScope>(IScopedNameBasedEntityManager<TEntity, TEntityScope> manager, string name)
            where TEntity : class
            where TEntityScope : class
        {
            return (manager.NameNormalizer == null) ? name : manager.NameNormalizer.Normalize(name);
        }
    }
}
