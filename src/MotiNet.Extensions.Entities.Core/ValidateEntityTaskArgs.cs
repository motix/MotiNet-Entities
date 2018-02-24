using System;
using System.Collections.Generic;

namespace MotiNet.Entities
{
    public class ValidateEntityTaskArgs<TEntity> : ManagerTaskArgs<TEntity>
        where TEntity : class
    {
        public List<GenericError> Errors { get; }

        public ValidateEntityTaskArgs(TEntity entity, List<GenericError> errors) : base(entity)
        {
            Errors = errors ?? throw new ArgumentNullException(nameof(errors));
        }
    }
}
