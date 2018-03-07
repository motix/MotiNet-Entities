using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MotiNet.Entities
{
    public static class NameBasedEntityValidatorExtensions
    {
        public static async Task ValidateNameAsync<TEntity, TEntityManager, TEntityAccessor>(
            this IValidator<TEntity> validator,
            TEntityManager manager,
            TEntityAccessor accessor,
            TEntity entity,
            List<GenericError> errors,
            Func<string, GenericError> invalidName,
            Func<string, GenericError> duplicateName)
            where TEntity : class
            where TEntityManager : class, INameBasedEntityManager<TEntity>
            where TEntityAccessor : INameBasedEntityAccessor<TEntity>
        {
            var name = accessor.GetName(entity);
            if (string.IsNullOrWhiteSpace(name))
            {
                errors.Add(invalidName(name));
            }
            var existingEntity = await manager.FindByNameAsync(name);
            if (existingEntity != null)
            {
                errors.Add(duplicateName(name));
            }
        }
    }
}
