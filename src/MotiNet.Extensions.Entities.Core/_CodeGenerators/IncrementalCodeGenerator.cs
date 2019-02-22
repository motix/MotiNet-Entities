using Microsoft.Extensions.Options;
using System;

namespace MotiNet.Entities
{
    public class IncrementalCodeGenerator<TEntity, TEntityManager> : IEntityCodeGenerator<TEntity>
        where TEntity : class
        where TEntityManager : class,
                               ITimeTrackedEntityManager<TEntity>,
                               ICodeBasedEntityManager<TEntity>
    {
        public IncrementalCodeGenerator(
            ICodeBasedEntityAccessor<TEntity> entityCodeAccessor,
            IOptions<IncrementalCodeGeneratorOptions<TEntity>> incrementalCodeGeneratorOptions)
        {
            if (incrementalCodeGeneratorOptions == null)
            {
                throw new ArgumentNullException(nameof(incrementalCodeGeneratorOptions));
            }

            EntityCodeAccessor = entityCodeAccessor ?? throw new ArgumentNullException(nameof(entityCodeAccessor));
            Prefix = incrementalCodeGeneratorOptions.Value.Prefix ?? string.Empty;
        }

        public string Prefix { get; }

        protected ICodeBasedEntityAccessor<TEntity> EntityCodeAccessor { get; }

        public string GenerateCode(object manager, TEntity entity) => GenerateCode(manager, entity, Prefix, true);

        protected string GenerateCode(object manager, TEntity entity, string prefix, bool staticPrefix)
        {
            if (manager == null)
            {
                throw new ArgumentNullException(nameof(manager));
            }
            if (!typeof(TEntityManager).IsAssignableFrom(manager.GetType()))
            {
                // TODO:: Message, resource
                throw new NotSupportedException($"Expected {typeof(TEntityManager).Name}, found {manager.GetType().Name}.");
            }
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (staticPrefix)
            {
                var latestEntity = ((TEntityManager)manager).FindLatest();
                if (latestEntity == null)
                {
                    return $"{prefix}1";
                }

                var latestCode = EntityCodeAccessor.GetCode(latestEntity);
                var latestNumber = int.Parse(latestCode.Substring(prefix.Length));
                return $"{prefix}{latestNumber + 1}";
            }
            else
            {
                var latestNumber = 0;
                string code;
                TEntity currentEntity;
                do
                {
                    latestNumber++;
                    code = $"{prefix}{latestNumber}";
                    currentEntity = ((TEntityManager)manager).FindByCode(code);
                } while (currentEntity != null);
                return code;
            }
        }
    }
}
