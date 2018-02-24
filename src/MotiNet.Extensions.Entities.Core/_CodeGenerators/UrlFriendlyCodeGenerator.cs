using Microsoft.Extensions.Options;
using System;

namespace MotiNet.Entities
{
    public class UrlFriendlyCodeGenerator<TEntity, TEntityManager> : IEntityCodeGenerator<TEntity>
        where TEntity : class
        where TEntityManager : class
    {
        public UrlFriendlyCodeGenerator(
            INameBasedEntityAccessor<TEntity> entityNameAccessor,
            IOptions<UrlFriendlyCodeGeneratorOptions<TEntity>> urlFriendlyCodeGeneratorOptions)
        {
            if (urlFriendlyCodeGeneratorOptions == null)
            {
                throw new ArgumentNullException(nameof(urlFriendlyCodeGeneratorOptions));
            }

            EntityNameAccessor = entityNameAccessor ?? throw new ArgumentNullException(nameof(entityNameAccessor));
        }

        protected INameBasedEntityAccessor<TEntity> EntityNameAccessor { get; }

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

            var name = EntityNameAccessor.GetName(entity);
            return name.ToUrlFriendly();
        }
    }
}
