using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace MotiNet.Entities
{
    public abstract class PagedSearchSpecificationBase<TEntity> : SearchSpecificationBase<TEntity>, IPagedSearchSpecification<TEntity>
        where TEntity : class
    {
        public PagedSearchSpecificationBase(int? pageSize, int? pageNumber)
        {
            PageSize = pageSize;
            PageNumber = pageNumber;
        }

        public abstract Expression<Func<TEntity, bool>> ScopeCriteria { get; }

        public virtual ICollection<OrderSpecification<TEntity>> Orders { get; }
            = new List<OrderSpecification<TEntity>>();

        public int? PageSize { get; }

        public int? PageNumber { get; }

        public virtual void AddOrder(Expression<Func<TEntity, object>> orderExpression, bool isDescending)
        {
            Orders.Add(new OrderSpecification<TEntity>(orderExpression, isDescending));
        }
    }
}
