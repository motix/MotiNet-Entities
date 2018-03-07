using System;

namespace MotiNet.Entities
{
    public static class ValidatorExtensions
    {
        public static TManager GetManager<TEntity, TManager>(this IValidator<TEntity> validator, object manager)
            where TEntity : class
            where TManager : class, IManager<TEntity>
        {
            var theManager = manager as TManager;
            if (theManager == null)
            {
                // TODO:: Resource
                throw new ArgumentException($"Variable 'manager' must be of type {typeof(TManager).Name}", nameof(manager));
            }

            return theManager;
        }
    }
}
