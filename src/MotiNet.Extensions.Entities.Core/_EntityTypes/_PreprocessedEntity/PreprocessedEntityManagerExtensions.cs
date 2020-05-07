using System.Threading.Tasks;

namespace MotiNet.Entities
{
    public static class PreprocessedEntityManagerExtensions<TEntity>
        where TEntity : class
    {
        public static ManagerTasks<TEntity> GetManagerTasks()
        {
            return new ManagerTasks<TEntity>()
            {
                EntityGet = EntityGet,
                EntityGetAsync = EntityGetAsync,
                EntityUpdateValidatingAsync = EntityUpdateValidatingAsync
            };
        }

        private static void EntityGet(IManager<TEntity> manager, ManagerTaskArgs<TEntity> taskArgs)
        {
            var preprocessedEntityManager = (IPreprocessedEntityManager<TEntity>)manager;

            if (preprocessedEntityManager.EntityPreprocessor?.Disabled == false)
            {
                preprocessedEntityManager.EntityPreprocessor.PreprocessEntityForGet(taskArgs.Entity);
            }
        }

        private static Task EntityGetAsync(IManager<TEntity> manager, ManagerTaskArgs<TEntity> taskArgs)
        {
            var preprocessedEntityManager = (IPreprocessedEntityManager<TEntity>)manager;

            if (preprocessedEntityManager.EntityPreprocessor?.Disabled == false)
            {
                preprocessedEntityManager.EntityPreprocessor.PreprocessEntityForGet(taskArgs.Entity);
            }

            return Task.FromResult(0);
        }

        private static Task EntityUpdateValidatingAsync(IManager<TEntity> manager, ManagerUpdatingTaskArgs<TEntity> taskArgs)
        {
            var preprocessedEntityManager = (IPreprocessedEntityManager<TEntity>)manager;

            if (preprocessedEntityManager.EntityPreprocessor?.Disabled == false)
            {
                preprocessedEntityManager.EntityPreprocessor.PreprocessEntityForUpdate(taskArgs.OldEntity, taskArgs.Entity);
            }

            return Task.FromResult(0);
        }
    }
}
