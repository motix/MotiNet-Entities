using System;
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

        public int? PageSize { get; }

        public int? PageNumber { get; }
    }
}
