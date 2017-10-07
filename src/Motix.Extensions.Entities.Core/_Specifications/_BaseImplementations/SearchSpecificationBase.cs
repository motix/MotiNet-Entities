using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace MotiNet.Entities
{
    public abstract class SearchSpecificationBase<TEntity> : GetSpecificationBase<TEntity>, ISearchSpecification<TEntity>
        where TEntity : class
    {
        public abstract Expression<Func<TEntity, bool>> Criteria { get; }

        public virtual ICollection<OrderSpecification<TEntity>> Orders { get; }
            = new List<OrderSpecification<TEntity>>();

        public virtual void AddOrder(Expression<Func<TEntity, object>> orderExpression, bool isDescending)
        {
            Orders.Add(new OrderSpecification<TEntity>(orderExpression, isDescending));
        }
    }
}
