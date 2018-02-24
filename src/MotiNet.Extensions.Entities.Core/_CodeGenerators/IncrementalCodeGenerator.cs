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

        public string GenerateCode(object manager, TEntity entity)
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

            var latestEntity = ((TEntityManager)manager).FindLatest();
            if (latestEntity == null)
            {
                return $"{Prefix}1";
            }

            var latestCode = EntityCodeAccessor.GetCode(latestEntity);
            var latestNumber = int.Parse(latestCode.Substring(Prefix.Length));
            return $"{Prefix}{latestNumber + 1}";
        }
    }
}
