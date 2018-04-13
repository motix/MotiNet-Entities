using System.Threading.Tasks;

namespace MotiNet.Entities
{
    public static class ChildEntityManagerExtensions
    {
        public static ManagerTasks<TEntity, TEntityParent> GetManagerTasks<TEntity, TEntityParent>()
            where TEntity : class
            where TEntityParent : class
        {
            return new ManagerTasks<TEntity, TEntityParent>()
            {
                EntityWithSubEntityCreateValidatingAsync = EntityCreateValidatingAsync,
                EntityWithSubEntityUpdateValidatingAsync = EntityUpdateValidatingAsync,
                EntityWithSubEntitySavingAsync = EntitySavingAsync
            };
        }

        private static async Task EntityCreateValidatingAsync<TEntity, TEntityParent>(IManager<TEntity, TEntityParent> manager, ManagerTaskArgs<TEntity> taskArgs)
            where TEntity : class
            where TEntityParent : class
        {
            var childManager = (IChildEntityManager<TEntity, TEntityParent>)manager;

            var parentId = childManager.ChildEntityAccessor.GetParentId(taskArgs.Entity);
            var parent = await childManager.ChildEntityStore.FindParentByIdAsync(parentId, manager.CancellationToken);
            childManager.ChildEntityAccessor.SetParent(taskArgs.Entity, parent);
        }

        private static Task EntityUpdateValidatingAsync<TEntity, TEntityParent>(IManager<TEntity, TEntityParent> manager, ManagerUpdatingTaskArgs<TEntity> taskArgs)
            where TEntity : class
            where TEntityParent : class
        {
            var childManager = (IChildEntityManager<TEntity, TEntityParent>)manager;

            var oldParentId = childManager.ChildEntityAccessor.GetParentId(taskArgs.OldEntity);
            childManager.ChildEntityAccessor.SetParentId(taskArgs.Entity, oldParentId);

            return EntityCreateValidatingAsync(manager, taskArgs);
        }

        private static Task EntitySavingAsync<TEntity, TEntityParent>(IManager<TEntity, TEntityParent> manager, ManagerTaskArgs<TEntity> taskArgs)
            where TEntity : class
            where TEntityParent : class
        {
            var childManager = (IChildEntityManager<TEntity, TEntityParent>)manager;

            childManager.ChildEntityAccessor.SetParent(taskArgs.Entity, null);

            return Task.FromResult(0);
        }
    }
}
