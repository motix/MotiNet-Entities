using System;
using System.Threading.Tasks;

namespace MotiNet.Entities
{
    public static class TimeTrackedEntityManagerExtensions
    {
        public static TEntity FindLatest<TEntity>(this ITimeTrackedEntityManager<TEntity> manager)
            where TEntity : class
        {
            manager.ThrowIfDisposed();

            return manager.TimeTrackedEntityStore.FindLatest();
        }

        public static Task<TEntity> FindLatestAsync<TEntity>(this ITimeTrackedEntityManager<TEntity> manager)
            where TEntity : class
        {
            manager.ThrowIfDisposed();

            return manager.TimeTrackedEntityStore.FindLatestAsync(manager.CancellationToken);
        }

        public static ManagerTasks<TEntity> GetManagerTasks<TEntity>()
            where TEntity : class
        {
            return new ManagerTasks<TEntity>()
            {
                EntityCreatingAsync = EntityCreatingAsync,
                EntityUpdatingAsync = EntityUpdatingAsync
            };
        }

        private static Task EntityCreatingAsync<TEntity>(IManager<TEntity> manager, ManagerTaskArgs<TEntity> taskArgs)
            where TEntity : class
        {
            var timeTrackedManager = (ITimeTrackedEntityManager<TEntity>)manager;

            var date = DateTime.Now;
            timeTrackedManager.TimeTrackedEntityAccessor.SetDataCreateDate(taskArgs.Entity, date);
            timeTrackedManager.TimeTrackedEntityAccessor.SetDataLastModifyDate(taskArgs.Entity, date);

            return Task.FromResult(0);
        }

        private static Task EntityUpdatingAsync<TEntity>(IManager<TEntity> manager, ManagerTaskArgs<TEntity> taskArgs)
            where TEntity : class
        {
            var timeTrackedManager = (ITimeTrackedEntityManager<TEntity>)manager;

            var date = DateTime.Now;
            timeTrackedManager.TimeTrackedEntityAccessor.SetDataLastModifyDate(taskArgs.Entity, DateTime.Now);

            return Task.FromResult(0);
        }
    }
}
