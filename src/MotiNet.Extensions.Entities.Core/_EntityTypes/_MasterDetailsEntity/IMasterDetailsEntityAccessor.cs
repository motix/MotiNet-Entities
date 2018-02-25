using System.Collections.Generic;

namespace MotiNet.Entities
{
    public interface IMasterDetailsEntityAccessor<TEntity, TEntityDetail>
        where TEntity : class
        where TEntityDetail : class
    {
        ICollection<TEntityDetail> GetDetails(TEntity entity);
    }
}
