using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MotiNet.Entities
{
    public static class ScopedNameBasedEntityValidatorExtensions
    {
        public static async Task ValidateNameAsync<TEntity, TEntityScope, TEntityManager, TEntityAccessor>(
            this IValidator<TEntity, TEntityScope> validator,
            TEntityManager manager,
            TEntityAccessor accessor,
            TEntity entity,
            List<GenericError> errors,
            Func<string, GenericError> invalidName,
            Func<string, GenericError> duplicateName)
            where TEntity : class
            where TEntityScope : class
            where TEntityManager : class, IScopedNameBasedEntityManager<TEntity, TEntityScope>
            where TEntityAccessor : IScopedNameBasedEntityAccessor<TEntity, TEntityScope>
        {
            var name = accessor.GetName(entity);
            if (string.IsNullOrWhiteSpace(name))
            {
                errors.Add(invalidName(name));
            }
            else
            {
                var scope = accessor.GetScope(entity);
                var existingEntity = await manager.FindByNameAsync(name, scope);
                if (existingEntity != null && !Equals(accessor.GetId(existingEntity), accessor.GetId(entity)))
                {
                    errors.Add(duplicateName(name));
                }
            }
        }
    }
}
