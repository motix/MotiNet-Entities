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

            return manager.NameBasedEntityStore.FindByName(NormalizeEntityName(manager, name));
        }

        public static Task<TEntity> FindByNameAsync<TEntity>(this INameBasedEntityManager<TEntity> manager, string name)
            where TEntity : class
        {
            manager.ThrowIfDisposed();
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            return manager.NameBasedEntityStore.FindByNameAsync(NormalizeEntityName(manager, name), manager.CancellationToken);
        }

        public static ManagerEventHandlers<TEntity> GetManagerEventHandlers<TEntity>()
            where TEntity : class
        {
            return new ManagerEventHandlers<TEntity>()
            {
                EntityPreparingForSaving = PrepareEntityForSaving
            };
        }

        private static void PrepareEntityForSaving<TEntity>(object sender, ManagerEventArgs<TEntity> e)
            where TEntity : class
        {
            var manager = (INameBasedEntityManager<TEntity>)sender;

            var name = manager.NameBasedEntityAccessor.GetName(e.Entity);
            var normalizedName = NormalizeEntityName(manager, name);
            manager.NameBasedEntityAccessor.SetNormalizedName(e.Entity, normalizedName);
        }

        private static string NormalizeEntityName<TEntity>(INameBasedEntityManager<TEntity> manager, string name)
            where TEntity : class
        {
            return (manager.NameNormalizer == null) ? name : manager.NameNormalizer.Normalize(name);
        }
    }
}
