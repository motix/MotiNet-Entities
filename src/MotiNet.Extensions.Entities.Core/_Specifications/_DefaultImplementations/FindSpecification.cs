using System;
using System.Linq.Expressions;

namespace MotiNet.Entities
{
    public class FindSpecification<TEntity> : FindSpecificationBase<TEntity>
        where TEntity : class
    {
        public override Expression<Func<TEntity, object>> KeyExpression { get; }

        public override Expression<Func<TEntity, bool>> AdditionalCriteria { get; }

        public FindSpecification(Expression<Func<TEntity, object>> keyExpression) => KeyExpression = keyExpression;

        public FindSpecification(
            Expression<Func<TEntity, object>> keyExpression,
            Expression<Func<TEntity, bool>> additionalCriteria)
            : this(keyExpression)
            => AdditionalCriteria = additionalCriteria;
    }
}
