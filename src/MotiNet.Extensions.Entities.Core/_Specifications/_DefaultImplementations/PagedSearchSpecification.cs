using System;
using System.Linq.Expressions;

namespace MotiNet.Entities
{
    public class PagedSearchSpecification<TEntity> : PagedSearchSpecificationBase<TEntity>
        where TEntity : class
    {
        public override Expression<Func<TEntity, bool>> Criteria { get; }

        public override Expression<Func<TEntity, bool>> ScopeCriteria { get; }

        public PagedSearchSpecification(int? pageSize, int? pageNumber, Expression<Func<TEntity, bool>> criteria)
            : this(pageSize, pageNumber, criteria, null) { }

        public PagedSearchSpecification(int? pageSize, int? pageNumber,
            Expression<Func<TEntity, bool>> criteria, Expression<Func<TEntity, bool>> scopeCriteria)
            : base(pageSize, pageNumber)
        {
            Criteria = criteria;
            ScopeCriteria = scopeCriteria;
        }
    }
}
