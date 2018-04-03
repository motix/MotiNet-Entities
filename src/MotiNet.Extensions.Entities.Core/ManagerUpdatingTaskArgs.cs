using System;

namespace MotiNet.Entities
{
    public class ManagerUpdatingTaskArgs<TEntity> : ManagerTaskArgs<TEntity>
        where TEntity : class
    {
        public TEntity OldEntity { get; }

        public ManagerUpdatingTaskArgs(TEntity entity, TEntity oldEntity)
            : base(entity)
        {
            OldEntity = oldEntity ?? throw new ArgumentNullException(nameof(oldEntity));
        }
    }
}
