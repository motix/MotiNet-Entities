﻿using System;
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

        public static ManagerTasks<TEntity> GetManagerTasks<TEntity>()
            where TEntity : class
        {
            return new ManagerTasks<TEntity>()
            {
                EntitySavingAsync = EntitySavingAsync
            };
        }

        private static Task EntitySavingAsync<TEntity>(IManager<TEntity> manager, ManagerTaskArgs<TEntity> taskArgs)
            where TEntity : class
        {
            var nameBasedManager = (INameBasedEntityManager<TEntity>)manager;

            var name = nameBasedManager.NameBasedEntityAccessor.GetName(taskArgs.Entity);
            var normalizedName = NormalizeEntityName(nameBasedManager, name);
            nameBasedManager.NameBasedEntityAccessor.SetNormalizedName(taskArgs.Entity, normalizedName);

            return Task.FromResult(0);
        }

        private static string NormalizeEntityName<TEntity>(INameBasedEntityManager<TEntity> manager, string name)
            where TEntity : class
        {
            return (manager.NameNormalizer == null) ? name : manager.NameNormalizer.Normalize(name);
        }
    }
}