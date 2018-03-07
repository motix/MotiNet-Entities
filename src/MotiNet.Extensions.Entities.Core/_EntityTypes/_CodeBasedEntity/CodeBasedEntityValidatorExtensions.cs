using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MotiNet.Entities
{
    public static class CodeBasedEntityValidatorExtensions
    {
        public static async Task ValidateCodeAsync<TEntity, TEntityManager, TEntityAccessor>(
            this IValidator<TEntity> validator,
            TEntityManager manager,
            TEntityAccessor accessor,
            TEntity entity,
            List<GenericError> errors,
            Func<string, GenericError> invalidCode,
            Func<string, GenericError> duplicateCode)
            where TEntity : class
            where TEntityManager : class, ICodeBasedEntityManager<TEntity>
            where TEntityAccessor : ICodeBasedEntityAccessor<TEntity>
        {
            var name = accessor.GetCode(entity);
            if (string.IsNullOrWhiteSpace(name))
            {
                errors.Add(invalidCode(name));
            }
            var existingEntity = await manager.FindByCodeAsync(name);
            if (existingEntity != null)
            {
                errors.Add(duplicateCode(name));
            }
        }
    }
}
