using System;

namespace MotiNet.Entities
{
    public interface ITimeTrackedEntityAccessor<TEntity>
        where TEntity : class
    {
        void SetDataCreateDate(TEntity entity, DateTime dataCreateDate);

        void SetDataLastModifyDate(TEntity entity, DateTime dataLastModifyDate);
    }
}
