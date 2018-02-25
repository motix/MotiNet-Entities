using System;
using System.Collections.Generic;

namespace MotiNet.Entities
{
    public class ValidateEntityTaskArgs<TEntity, TSubEntity> : ManagerTaskArgs<TEntity>
        where TEntity : class
        where TSubEntity : class
    {
        IEnumerable<IEntityValidator<TEntity, TSubEntity>> EntityValidators { get; }

        public List<GenericError> Errors { get; }

        public ValidateEntityTaskArgs(
            TEntity entity,
            IEnumerable<IEntityValidator<TEntity, TSubEntity>> entityValidators,
            List<GenericError> errors)
            : base(entity)
        {
            EntityValidators = entityValidators ?? throw new ArgumentNullException(nameof(entityValidators));
            Errors = errors ?? throw new ArgumentNullException(nameof(errors));
        }
    }

    public class ValidateEntityTaskArgs<TEntity> : ManagerTaskArgs<TEntity>
        where TEntity : class
    {
        IEnumerable<IEntityValidator<TEntity>> EntityValidators { get; }

        public List<GenericError> Errors { get; }

        public ValidateEntityTaskArgs(
            TEntity entity,
            IEnumerable<IEntityValidator<TEntity>> entityValidators,
            List<GenericError> errors)
            : base(entity)
        {
            EntityValidators = entityValidators ?? throw new ArgumentNullException(nameof(entityValidators));
            Errors = errors ?? throw new ArgumentNullException(nameof(errors));
        }
    }
}
