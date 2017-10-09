using System;
using System.Linq.Expressions;

namespace MotiNet.Entities
{
    public interface ISearchSpecification<TEntity> : IGetSpecification<TEntity>
        where TEntity : class
    {
        Expression<Func<TEntity, bool>> Criteria { get; }
    }
}
