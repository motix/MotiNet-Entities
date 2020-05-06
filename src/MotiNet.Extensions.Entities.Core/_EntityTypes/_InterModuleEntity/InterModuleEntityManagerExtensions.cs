using System.Threading.Tasks;

namespace MotiNet.Entities
{
    public static class InterModuleEntityManagerExtensions<TEntity>
        where TEntity : class
    {
        public static ManagerTasks<TEntity> GetManagerTasks()
        {
            return new ManagerTasks<TEntity>()
            {
                EntityCreatedAsync = EntityCreatedAsync,
                EntityUpdatedAsync = EntityUpdatedAsync,
                EntityDeletedAsync = EntityDeletedAsync
            };
        }

        private static Task EntityCreatedAsync(IManager<TEntity> manager, ManagerTaskArgs<TEntity> taskArgs)
        {
            var interModuleEntityManager = (IInterModuleEntityManager<TEntity>)manager;

            return interModuleEntityManager.EntityAdapter.OnCreatedAsync(manager, taskArgs);
        }

        private static Task EntityUpdatedAsync(IManager<TEntity> manager, ManagerTaskArgs<TEntity> taskArgs)
        {
            var interModuleEntityManager = (IInterModuleEntityManager<TEntity>)manager;

            return interModuleEntityManager.EntityAdapter.OnUpdatedAsync(manager, taskArgs);
        }

        private static Task EntityDeletedAsync(IManager<TEntity> manager, ManagerTaskArgs<TEntity> taskArgs)
        {
            var interModuleEntityManager = (IInterModuleEntityManager<TEntity>)manager;

            return interModuleEntityManager.EntityAdapter.OnDeletedAsync(manager, taskArgs);
        }
    }
}
