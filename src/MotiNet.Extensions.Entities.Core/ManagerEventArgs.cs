using System;

namespace MotiNet.Entities
{
    public class ManagerEventArgs<TEntity> : EventArgs
        where TEntity : class
    {
        public TEntity Entity { get; set; }
    }
}
