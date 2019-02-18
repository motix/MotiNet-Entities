using System.Threading.Tasks;

namespace MotiNet.Entities
{
    public static class ReadableIdEntityManagerExtensions<TEntity>
        where TEntity : class
    {
        public static ManagerTasks<TEntity> GetManagerTasks()
        {
            return new ManagerTasks<TEntity>()
            {
                EntityCreatingAsync = EntityCreatingAsync
            };
        }

        private static Task EntityCreatingAsync(IManager<TEntity> manager, ManagerTaskArgs<TEntity> taskArgs)
        {
            var readableIdManager = (IReadableIdEntityManager<TEntity>)manager;

            var idSource = readableIdManager.ReadableIdEntityAccessor.GetIdSource(taskArgs.Entity);
            if (idSource != null)
            {
                var id = idSource.ToString().ToUrlFriendly();
                readableIdManager.ReadableIdEntityAccessor.SetId(taskArgs.Entity, id);
            }

            return Task.FromResult(0);
        }
    }
}
