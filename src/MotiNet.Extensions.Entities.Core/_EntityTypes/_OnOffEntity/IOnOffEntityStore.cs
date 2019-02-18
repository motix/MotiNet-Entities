using System;

namespace MotiNet.Entities
{
    public interface IOnOffEntityStore<TEntity> : IDisposable
        where TEntity : class
    {
        ISearchSpecification<TEntity> SearchActiveEntitiesSpecification { get; }
    }
}
