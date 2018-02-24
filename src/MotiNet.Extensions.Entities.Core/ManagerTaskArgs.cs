using System;

namespace MotiNet.Entities
{
    public class ManagerTaskArgs<TEntity>
        where TEntity : class
    {
        public TEntity Entity { get; }

        public ManagerTaskArgs(TEntity entity)
        {
            Entity = entity ?? throw new ArgumentNullException(nameof(entity));
        }
    }
}
