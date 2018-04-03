using System;

namespace MotiNet.Entities
{
    public interface ITimeTrackedEntityAccessor<TEntity>
        where TEntity : class
    {
        DateTime GetDataCreateDate(TEntity entity);

        void SetDataCreateDate(TEntity entity, DateTime dataCreateDate);

        void SetDataLastModifyDate(TEntity entity, DateTime dataLastModifyDate);
    }
}
