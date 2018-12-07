using System;
using System.Linq.Expressions;

namespace MotiNet.Entities
{
    public class SearchSpecification<TEntity> : SearchSpecificationBase<TEntity>
        where TEntity : class
    {
        public override Expression<Func<TEntity, bool>> Criteria { get; }

        public SearchSpecification(Expression<Func<TEntity, bool>> criteria) => Criteria = criteria;
    }
}
