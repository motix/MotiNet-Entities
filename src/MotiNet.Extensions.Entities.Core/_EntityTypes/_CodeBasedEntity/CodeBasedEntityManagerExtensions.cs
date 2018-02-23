using System;
using System.Threading;
using System.Threading.Tasks;

namespace MotiNet.Entities
{
    public static class CodeBasedEntityManagerExtensions
    {
        public static TEntity FindByCode<TEntity>(this ICodeBasedEntityManager<TEntity> manager,
            string code)
            where TEntity : class
        {
            manager.ThrowIfDisposed();
            if (code == null)
            {
                throw new ArgumentNullException(nameof(code));
            }

            return manager.CodeBasedEntityStore.FindByCode(NormalizeEntityCode(manager, code));
        }

        public static Task<TEntity> FindByCodeAsync<TEntity>(this ICodeBasedEntityManager<TEntity> manager,
            string code, CancellationToken cancellationToken)
            where TEntity : class
        {
            manager.ThrowIfDisposed();
            if (code == null)
            {
                throw new ArgumentNullException(nameof(code));
            }

            return manager.CodeBasedEntityStore.FindByCodeAsync(NormalizeEntityCode(manager, code), cancellationToken);
        }

        public static ManagerEventHandlers<TEntity> GetManagerEventHandlers<TEntity>()
            where TEntity : class
        {
            return new ManagerEventHandlers<TEntity>()
            {
                EntityPreparingForValidation = PrepareEntityForValidation,
                EntityPreparingForSaving = PrepareEntityForSaving
            };
        }

        private static void PrepareEntityForValidation<TEntity>(object sender, ManagerEventArgs<TEntity> e)
            where TEntity : class
        {
            var manager = (ICodeBasedEntityManager<TEntity>)sender;

            if (manager.CodeBasedEntityAccessor.GetCode(e.Entity) == null && manager.CodeGenerator != null)
            {
                var code = manager.CodeGenerator.GenerateCode(manager, e.Entity);
                manager.CodeBasedEntityAccessor.SetCode(e.Entity, code);
            }
        }

        private static void PrepareEntityForSaving<TEntity>(object sender, ManagerEventArgs<TEntity> e)
            where TEntity : class
        {
            var manager = (ICodeBasedEntityManager<TEntity>)sender;

            var code = manager.CodeBasedEntityAccessor.GetCode(e.Entity);
            var normalizedCode = NormalizeEntityCode(manager, code);
            manager.CodeBasedEntityAccessor.SetCode(e.Entity, normalizedCode);
        }

        private static string NormalizeEntityCode<TEntity>(ICodeBasedEntityManager<TEntity> manager, string code)
            where TEntity : class
        {
            return (manager.CodeNormalizer == null) ? code : manager.CodeNormalizer.Normalize(code);
        }
    }
}
