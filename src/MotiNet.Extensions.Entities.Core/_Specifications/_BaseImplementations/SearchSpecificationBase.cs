using System;
using System.Linq.Expressions;

namespace MotiNet.Entities
{
    public abstract class SearchSpecificationBase<TEntity> : GetSpecificationBase<TEntity>, ISearchSpecification<TEntity>
        where TEntity : class
    {
        public abstract Expression<Func<TEntity, bool>> Criteria { get; }
    }
}
