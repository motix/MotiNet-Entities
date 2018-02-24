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

        public static ManagerEventHandlers<TEntity> GetManagerEventHandlers<TEntity>()
            where TEntity : class
        {
            return new ManagerEventHandlers<TEntity>()
            {
                EntityPreparingForCreating = PrepareEntityForCreating,
                EntityPreparingForUpdating = PrepareEntityForUpdating
            };
        }

        private static void PrepareEntityForCreating<TEntity>(object sender, ManagerEventArgs<TEntity> e)
            where TEntity : class
        {
            var manager = (ITimeTrackedEntityManager<TEntity>)sender;

            var date = DateTime.Now;
            manager.TimeTrackedEntityAccessor.SetDataCreateDate(e.Entity, date);
            manager.TimeTrackedEntityAccessor.SetDataLastModifyDate(e.Entity, date);
        }

        private static void PrepareEntityForUpdating<TEntity>(object sender, ManagerEventArgs<TEntity> e)
            where TEntity : class
        {
            var manager = (ITimeTrackedEntityManager<TEntity>)sender;

            manager.TimeTrackedEntityAccessor.SetDataLastModifyDate(e.Entity, DateTime.Now);
        }
    }
}
