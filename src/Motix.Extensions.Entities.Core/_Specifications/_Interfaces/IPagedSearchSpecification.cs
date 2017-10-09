using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace MotiNet.Entities
{
    public interface IPagedSearchSpecification<TEntity> : ISearchSpecification<TEntity>
        where TEntity : class
    {
        Expression<Func<TEntity, bool>> ScopeCriteria { get; }

        ICollection<OrderSpecification<TEntity>> Orders { get; }

        int? PageSize { get; }

        int? PageNumber { get; }

        void AddOrder(Expression<Func<TEntity, object>> orderExpression, bool isDescending);
    }
}
