using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace MotiNet.Entities
{
    public abstract class GetSpecificationBase<TEntity> : IGetSpecification<TEntity>
        where TEntity : class
    {
        public virtual ICollection<Expression<Func<TEntity, object>>> Includes { get; }
            = new List<Expression<Func<TEntity, object>>>();

        public virtual void AddInclude(Expression<Func<TEntity, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }
    }
}
