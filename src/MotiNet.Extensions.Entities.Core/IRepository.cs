using System;
using System.Collections.Generic;

namespace MotiNet.Entities
{
    public interface IRepository<TEntity> : IDisposable
        where TEntity : class
    {
        TEntity FindById(object id);

        TEntity Find(object key, IFindSpecification<TEntity> spec);

        IEnumerable<TEntity> All();

        IEnumerable<TEntity> Search(ISearchSpecification<TEntity> spec);

        PagedSearchResult<TEntity> Search(IPagedSearchSpecification<TEntity> spec);

        TEntity Create(TEntity entity);

        TEntity Create(TEntity entity, IModifySpecification<TEntity> spec);

        void Update(TEntity entity);

        void Update(TEntity entity, IModifySpecification<TEntity> spec);

        void Delete(TEntity entity);
    }
}
