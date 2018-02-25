using Microsoft.Data.Sqlite;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MotiNet.Entities.EntityFrameworkCore.Test
{
    public class TimeTrackedEntityStore
    {
        [Fact(DisplayName = "TimeTrackedEntityStore.FindsLatestEntity")]
        public async Task FindsLatestEntity()
        {
            // In-memory database only exists while the connection is open
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            try
            {
                // Run the test against one instance of the context
                using (var dbContext = DbContextHelper.InitBloggingDbContext(connection).dbContext)
                {
                    var store = new ArticleStore(dbContext);
                    var entity = await store.FindLatestAsync(CancellationToken.None);
                    var expected = dbContext.Articles.OrderByDescending(x => x.DataCreateDate).First().Id;

                    Assert.Equal(expected, entity.Id);
                }
            }
            finally
            {
                connection.Close();
            }
        }

        public class ArticleStore : StoreBase<Article, BloggingDbContext>, ITimeTrackedEntityStoreMarker<Article, BloggingDbContext>
        {
            public ArticleStore(BloggingDbContext dbContext) : base(dbContext) { }

            public Article FindLatest()
            {
                return TimeTrackedEntityStoreHelper.FindLatestEntity(this);
            }

            public Task<Article> FindLatestAsync(CancellationToken cancellationToken)
            {
                return TimeTrackedEntityStoreHelper.FindLatestEntityAsync(this, cancellationToken);
            }
        }
    }
}
