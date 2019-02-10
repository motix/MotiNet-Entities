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

            var entity = manager.TimeTrackedEntityStore.FindLatest();
            if (entity != null)
            {
                manager.ExecuteEntityGet(entity);
            }

            return entity;
        }

        public static async Task<TEntity> FindLatestAsync<TEntity>(this ITimeTrackedEntityManager<TEntity> manager)
            where TEntity : class
        {
            manager.ThrowIfDisposed();

            var entity = await manager.TimeTrackedEntityStore.FindLatestAsync(manager.CancellationToken);
            if (entity != null)
            {
                await manager.ExecuteEntityGetAsync(entity);
            }

            return entity;
        }
    }

    public static class TimeTrackedEntityManagerExtensions<TEntity>
        where TEntity : class
    {
        public static ManagerTasks<TEntity> GetManagerTasks()
        {
            return new ManagerTasks<TEntity>()
            {
                EntityCreatingAsync = EntityCreatingAsync,
                EntityUpdatingAsync = EntityUpdatingAsync
            };
        }

        private static Task EntityCreatingAsync(IManager<TEntity> manager, ManagerTaskArgs<TEntity> taskArgs)
        {
            var timeTrackedManager = (ITimeTrackedEntityManager<TEntity>)manager;

            var now = DateTime.Now;
            timeTrackedManager.TimeTrackedEntityAccessor.SetDataCreateDate(taskArgs.Entity, now);
            timeTrackedManager.TimeTrackedEntityAccessor.SetDataLastModifyDate(taskArgs.Entity, now);

            return Task.FromResult(0);
        }

        private static Task EntityUpdatingAsync(IManager<TEntity> manager, ManagerUpdatingTaskArgs<TEntity> taskArgs)
        {
            var timeTrackedManager = (ITimeTrackedEntityManager<TEntity>)manager;

            var oldCreateDate = timeTrackedManager.TimeTrackedEntityAccessor.GetDataCreateDate(taskArgs.OldEntity);
            timeTrackedManager.TimeTrackedEntityAccessor.SetDataCreateDate(taskArgs.Entity, oldCreateDate);
            timeTrackedManager.TimeTrackedEntityAccessor.SetDataLastModifyDate(taskArgs.Entity, DateTime.Now);

            return Task.FromResult(0);
        }
    }
}
