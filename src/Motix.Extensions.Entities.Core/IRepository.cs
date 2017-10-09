using System;
using System.Collections.Generic;

namespace MotiNet.Entities
{
    public interface IRepository<TEntity> : IDisposable
        where TEntity : class
    {
        TEntity FindById(object id);

        TEntity FindById(object id, IFindSpecification<TEntity> spec);

        IEnumerable<TEntity> All();

        IEnumerable<TEntity> Search(ISearchSpecification<TEntity> spec);

        PagedSearchResult<TEntity> Search(IPagedSearchSpecification<TEntity> spec);

        TEntity Add(TEntity entity);

        TEntity Add(TEntity entity, IModifySpecification<TEntity> spec);

        void Update(TEntity entity);

        void Update(TEntity entity, IModifySpecification<TEntity> spec);

        void Delete(TEntity entity);
    }
}
