using System;
using System.Linq.Expressions;

namespace MotiNet.Entities
{
    public abstract class FindSpecificationBase<TEntity> : GetSpecificationBase<TEntity>, IFindSpecification<TEntity>
        where TEntity : class
    {
        public abstract Expression<Func<TEntity, object>> KeyExpression { get; }

        public virtual Expression<Func<TEntity, bool>> AdditionalCriteria => null;
    }
}
