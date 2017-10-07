using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace MotiNet.Entities
{
    // TODO:: Support ThenInclude
    public interface IGetSpecification<TEntity>
        where TEntity : class
    {
        ICollection<Expression<Func<TEntity, object>>> Includes { get; }

        void AddInclude(Expression<Func<TEntity, object>> includeExpression);
    }
}
