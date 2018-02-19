using System;
using System.Linq.Expressions;

namespace MotiNet.Entities
{
    public class OrderSpecification<TEntity>
        where TEntity : class
    {
        public OrderSpecification(Expression<Func<TEntity, object>> orderExpression, bool isDescending)
        {
            OrderExpression = orderExpression ?? throw new ArgumentNullException(nameof(orderExpression));
            IsDescending = isDescending;
        }

        public Expression<Func<TEntity, object>> OrderExpression { get; }

        public bool IsDescending { get; }
    }
}
