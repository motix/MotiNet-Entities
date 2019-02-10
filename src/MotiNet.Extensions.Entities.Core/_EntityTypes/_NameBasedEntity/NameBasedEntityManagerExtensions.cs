using System;
using System.Threading.Tasks;

namespace MotiNet.Entities
{
    public static class NameBasedEntityManagerExtensions
    {
        public static TEntity FindByName<TEntity>(this INameBasedEntityManager<TEntity> manager, string name)
            where TEntity : class
        {
            manager.ThrowIfDisposed();
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            var entity = manager.NameBasedEntityStore.FindByName(NormalizeEntityName(manager, name));
            if (entity != null)
            {
                manager.ExecuteEntityGet(entity);
            }

            return entity;
        }

        public static async Task<TEntity> FindByNameAsync<TEntity>(this INameBasedEntityManager<TEntity> manager, string name)
            where TEntity : class
        {
            manager.ThrowIfDisposed();
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            var entity = await manager.NameBasedEntityStore.FindByNameAsync(NormalizeEntityName(manager, name), manager.CancellationToken);
            if (entity != null)
            {
                await manager.ExecuteEntityGetAsync(entity);
            }

            return entity;
        }

        internal static string NormalizeEntityName<TEntity>(INameBasedEntityManager<TEntity> manager, string name)
            where TEntity : class
        {
            return (manager.NameNormalizer == null) ? name : manager.NameNormalizer.Normalize(name);
        }
    }

    public static class NameBasedEntityManagerExtensions<TEntity>
        where TEntity : class
    {
        public static ManagerTasks<TEntity> GetManagerTasks()
        {
            return new ManagerTasks<TEntity>()
            {
                EntitySavingAsync = EntitySavingAsync
            };
        }

        private static Task EntitySavingAsync(IManager<TEntity> manager, ManagerTaskArgs<TEntity> taskArgs)
        {
            var nameBasedManager = (INameBasedEntityManager<TEntity>)manager;

            var name = nameBasedManager.NameBasedEntityAccessor.GetName(taskArgs.Entity);
            var normalizedName = NameBasedEntityManagerExtensions.NormalizeEntityName(nameBasedManager, name);
            nameBasedManager.NameBasedEntityAccessor.SetNormalizedName(taskArgs.Entity, normalizedName);

            return Task.FromResult(0);
        }
    }
}
