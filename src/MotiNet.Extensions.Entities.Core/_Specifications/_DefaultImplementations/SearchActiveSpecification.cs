using System;
using System.Linq.Expressions;

namespace MotiNet.Entities
{
    public class SearchActiveSpecification<TEntity> : SearchSpecificationBase<TEntity>
        where TEntity : class, IIsActiveWiseEntity
    {
        public override Expression<Func<TEntity, bool>> Criteria => x => x.IsActive;
    }
}
