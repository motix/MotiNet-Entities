using System;
using System.Linq.Expressions;

namespace MotiNet.Entities
{
    public interface IFindSpecification<TEntity, TKey> : IGetSpecification<TEntity>
        where TEntity : class
        where TKey : IEquatable<TKey>
    {
        Expression<Func<TEntity, TKey>> IdExpression { get; }
    }
}
