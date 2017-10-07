using System;
using System.Linq.Expressions;

namespace MotiNet.Entities
{
    public abstract class FindSpecificationBase<TEntity, TKey> : GetSpecificationBase<TEntity>, IFindSpecification<TEntity, TKey>
        where TEntity : class
        where TKey : IEquatable<TKey>
    {
        public abstract Expression<Func<TEntity, TKey>> IdExpression { get; }
    }
}
