using System;
using System.Threading.Tasks;

namespace MotiNet.Entities
{
    public static class CodeBasedEntityManagerExtensions
    {
        public static TEntity FindByCode<TEntity>(this ICodeBasedEntityManager<TEntity> manager, string code)
            where TEntity : class
        {
            manager.ThrowIfDisposed();
            if (code == null)
            {
                throw new ArgumentNullException(nameof(code));
            }

            var entity = manager.CodeBasedEntityStore.FindByCode(NormalizeEntityCode(manager, code));
            if (entity != null)
            {
                manager.ExecuteEntityGet(entity);
            }

            return entity;
        }

        public static async Task<TEntity> FindByCodeAsync<TEntity>(this ICodeBasedEntityManager<TEntity> manager, string code)
            where TEntity : class
        {
            manager.ThrowIfDisposed();
            if (code == null)
            {
                throw new ArgumentNullException(nameof(code));
            }

            var entity = await manager.CodeBasedEntityStore.FindByCodeAsync(NormalizeEntityCode(manager, code), manager.CancellationToken);
            if (entity != null)
            {
                await manager.ExecuteEntityGetAsync(entity);
            }

            return entity;
        }

        internal static string NormalizeEntityCode<TEntity>(ICodeBasedEntityManager<TEntity> manager, string code)
            where TEntity : class
        {
            return (manager.CodeNormalizer == null) ? code : manager.CodeNormalizer.Normalize(code);
        }
    }

    public static class CodeBasedEntityManagerExtensions<TEntity>
        where TEntity : class
    {
        public static ManagerTasks<TEntity> GetManagerTasks()
        {
            return new ManagerTasks<TEntity>()
            {
                EntityCreateValidatingAsync = EntityValidatingAsync,
                EntityUpdateValidatingAsync = EntityValidatingAsync,
                EntitySavingAsync = EntitySavingAsync
            };
        }

        private static Task EntityValidatingAsync(IManager<TEntity> manager, ManagerTaskArgs<TEntity> taskArgs)
        {
            var codeBasedManager = (ICodeBasedEntityManager<TEntity>)manager;

            if (codeBasedManager.CodeBasedEntityAccessor.GetCode(taskArgs.Entity) == null && codeBasedManager.CodeGenerator != null)
            {
                var code = codeBasedManager.CodeGenerator.GenerateCode(manager, taskArgs.Entity);
                codeBasedManager.CodeBasedEntityAccessor.SetCode(taskArgs.Entity, code);
            }

            return Task.FromResult(0);
        }

        private static Task EntitySavingAsync(IManager<TEntity> manager, ManagerTaskArgs<TEntity> taskArgs)
        {
            var codeBasedManager = (ICodeBasedEntityManager<TEntity>)manager;

            var code = codeBasedManager.CodeBasedEntityAccessor.GetCode(taskArgs.Entity);
            var normalizedCode = CodeBasedEntityManagerExtensions.NormalizeEntityCode(codeBasedManager, code);
            codeBasedManager.CodeBasedEntityAccessor.SetCode(taskArgs.Entity, normalizedCode);

            return Task.FromResult(0);
        }
    }
}
