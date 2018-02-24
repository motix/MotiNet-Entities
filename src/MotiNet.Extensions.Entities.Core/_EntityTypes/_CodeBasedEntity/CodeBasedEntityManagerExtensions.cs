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

            return manager.CodeBasedEntityStore.FindByCode(NormalizeEntityCode(manager, code));
        }

        public static Task<TEntity> FindByCodeAsync<TEntity>(this ICodeBasedEntityManager<TEntity> manager, string code)
            where TEntity : class
        {
            manager.ThrowIfDisposed();
            if (code == null)
            {
                throw new ArgumentNullException(nameof(code));
            }

            return manager.CodeBasedEntityStore.FindByCodeAsync(NormalizeEntityCode(manager, code), manager.CancellationToken);
        }

        public static ManagerTasks<TEntity> GetManagerTasks<TEntity>()
            where TEntity : class
        {
            return new ManagerTasks<TEntity>()
            {
                EntityValidatingAsync = EntityValidatingAsync,
                EntitySavingAsync = EntitySavingAsync
            };
        }

        private static Task EntityValidatingAsync<TEntity>(IManager<TEntity> manager, ManagerTaskArgs<TEntity> taskArgs)
            where TEntity : class
        {
            var codeBasedManager = (ICodeBasedEntityManager<TEntity>)manager;

            if (codeBasedManager.CodeBasedEntityAccessor.GetCode(taskArgs.Entity) == null && codeBasedManager.CodeGenerator != null)
            {
                var code = codeBasedManager.CodeGenerator.GenerateCode(manager, taskArgs.Entity);
                codeBasedManager.CodeBasedEntityAccessor.SetCode(taskArgs.Entity, code);
            }

            return Task.FromResult(0);
        }

        private static Task EntitySavingAsync<TEntity>(IManager<TEntity> manager, ManagerTaskArgs<TEntity> taskArgs)
            where TEntity : class
        {
            var codeBasedManager = (ICodeBasedEntityManager<TEntity>)manager;

            var code = codeBasedManager.CodeBasedEntityAccessor.GetCode(taskArgs.Entity);
            var normalizedCode = NormalizeEntityCode(codeBasedManager, code);
            codeBasedManager.CodeBasedEntityAccessor.SetCode(taskArgs.Entity, normalizedCode);

            return Task.FromResult(0);
        }

        private static string NormalizeEntityCode<TEntity>(ICodeBasedEntityManager<TEntity> manager, string code)
            where TEntity : class
        {
            return (manager.CodeNormalizer == null) ? code : manager.CodeNormalizer.Normalize(code);
        }
    }
}
