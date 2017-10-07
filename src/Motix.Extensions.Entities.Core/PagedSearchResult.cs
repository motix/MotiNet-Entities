using System.Collections.Generic;

namespace MotiNet.Entities
{
    public class PagedSearchResult<TEntity>
        where TEntity : class
    {
        public PagedSearchResult(int totalCount, int resultCount, IEnumerable<TEntity> results)
        {
            TotalCount = totalCount;
            ResultCount = resultCount;
            Results = results;
        }

        public int TotalCount { get; }

        public int ResultCount { get; }

        public IEnumerable<TEntity> Results { get; }
    }
}
