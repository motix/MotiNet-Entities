using System;
using System.Linq.Expressions;

namespace MotiNet.Entities
{
    public interface IPagedSearchSpecification<TEntity> : ISearchSpecification<TEntity>
        where TEntity : class
    {
        Expression<Func<TEntity, bool>> ScopeCriteria { get; }

        int? PageSize { get; }

        int? PageNumber { get; }
    }
}
