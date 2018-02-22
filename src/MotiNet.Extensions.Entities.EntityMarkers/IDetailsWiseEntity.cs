using System.Collections.Generic;

namespace MotiNet.Entities
{
    public interface IDetailsWiseEntity<TEntityDetail>
        where TEntityDetail : class
    {
        ICollection<TEntityDetail> Details { get; set; }
    }
}
