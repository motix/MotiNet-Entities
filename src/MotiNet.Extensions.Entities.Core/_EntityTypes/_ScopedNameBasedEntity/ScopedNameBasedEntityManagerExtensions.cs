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

            var entity = manager.ScopedNameBasedEntityStore.FindByName(NormalizeEntityName(manager, name), scope);
            if (entity != null)
            {
                manager.ExecuteEntityGet(entity);
            }

            return entity;
        }

        public static async Task<TEntity> FindByNameAsync<TEntity, TEntityScope>(this IScopedNameBasedEntityManager<TEntity, TEntityScope> manager, string name, TEntityScope scope)
            where TEntity : class
            where TEntityScope : class
        {
            manager.ThrowIfDisposed();
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            var entity = await manager.ScopedNameBasedEntityStore.FindByNameAsync(NormalizeEntityName(manager, name), scope, manager.CancellationToken);
            if (entity != null)
            {
                await manager.ExecuteEntityGetAsync(entity);
            }

            return entity;
        }

        public static ManagerTasks<TEntity, TEntityScope> GetManagerTasks<TEntity, TEntityScope>()
            where TEntity : class
            where TEntityScope : class
        {
            return new ManagerTasks<TEntity, TEntityScope>()
            {
                EntityWithSubEntityCreateValidatingAsync = EntityCreateValidatingAsync,
                EntityWithSubEntityUpdateValidatingAsync = EntityUpdateValidatingAsync,
                EntityWithSubEntitySavingAsync = EntitySavingAsync
            };
        }

        private static async Task EntityCreateValidatingAsync<TEntity, TEntityScope>(IManager<TEntity, TEntityScope> manager, ManagerTaskArgs<TEntity> taskArgs)
            where TEntity : class
            where TEntityScope : class
        {
            var scopedNameBasedManager = (IScopedNameBasedEntityManager<TEntity, TEntityScope>)manager;

            var scopeId = scopedNameBasedManager.ScopedNameBasedEntityAccessor.GetScopeId(taskArgs.Entity);
            var scope = await scopedNameBasedManager.ScopedNameBasedEntityStore.FindScopeByIdAsync(scopeId, manager.CancellationToken);
            scopedNameBasedManager.ScopedNameBasedEntityAccessor.SetScope(taskArgs.Entity, scope);
        }

        private static Task EntityUpdateValidatingAsync<TEntity, TEntityScope>(IManager<TEntity, TEntityScope> manager, ManagerUpdatingTaskArgs<TEntity> taskArgs)
            where TEntity : class
            where TEntityScope : class
        {
            var scopedNameBasedManager = (IScopedNameBasedEntityManager<TEntity, TEntityScope>)manager;

            var oldScopeId = scopedNameBasedManager.ScopedNameBasedEntityAccessor.GetScopeId(taskArgs.OldEntity);
            scopedNameBasedManager.ScopedNameBasedEntityAccessor.SetScopeId(taskArgs.Entity, oldScopeId);

            return EntityCreateValidatingAsync(manager, taskArgs);
        }

        private static Task EntitySavingAsync<TEntity, TEntityScope>(IManager<TEntity, TEntityScope> manager, ManagerTaskArgs<TEntity> taskArgs)
            where TEntity : class
            where TEntityScope : class
        {
            var scopedNameBasedManager = (IScopedNameBasedEntityManager<TEntity, TEntityScope>)manager;

            scopedNameBasedManager.ScopedNameBasedEntityAccessor.SetScope(taskArgs.Entity, null);

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
