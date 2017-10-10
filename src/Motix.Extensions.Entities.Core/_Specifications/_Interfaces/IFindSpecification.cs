using System;
using System.Linq.Expressions;

namespace MotiNet.Entities
{
    public interface IFindSpecification<TEntity> : IGetSpecification<TEntity>
        where TEntity : class
    {
        Expression<Func<TEntity, object>> KeyExpression { get; }

        Expression<Func<TEntity, bool>> AdditionalCriteria { get; }
    }
}
