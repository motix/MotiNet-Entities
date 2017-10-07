using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace MotiNet.Entities
{
    public interface ISearchSpecification<TEntity> : IGetSpecification<TEntity>
        where TEntity : class
    {
        Expression<Func<TEntity, bool>> Criteria { get; }

        ICollection<OrderSpecification<TEntity>> Orders { get; }

        void AddOrder(Expression<Func<TEntity, object>> orderExpression, bool isDescending);
    }
}
