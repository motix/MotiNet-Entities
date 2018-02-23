using Microsoft.Data.Sqlite;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MotiNet.Entities.EntityFrameworkCore.Test
{
    public class NameBasedEntityStore
    {
        [Fact(DisplayName = "NameBasedEntityStore.FindsEntityByName")]
        public async Task FindsEntityByName()
        {
            var testName = "Tech";

            // In-memory database only exists while the connection is open
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            try
            {
                // Run the test against one instance of the context
                using (var dbContext = DbContextHelper.InitBloggingDbContext(connection).dbContext)
                {
                    var store = new CategoryStore(dbContext);
                    var entity = await store.FindByNameAsync(testName.ToUpper(), CancellationToken.None);
                    var expected = dbContext.Categories.Single(x => x.NormalizedName == testName.ToUpper()).Id;

                    Assert.Equal(expected, entity.Id);
                }
            }
            finally
            {
                connection.Close();
            }
        }

        public class CategoryStore : StoreBase<Category, BloggingDbContext>, INameBasedEntityStoreMarker<Category, BloggingDbContext>
        {
            public CategoryStore(BloggingDbContext dbContext) : base(dbContext) { }

            public Category FindByName(string normalizedName)
            {
                return NameBasedEntityStoreHelper.FindByName(this, normalizedName, x => x.UrlFriendlyName);
            }

            public Task<Category> FindByNameAsync(string normalizedName, CancellationToken cancellationToken)
            {
                return NameBasedEntityStoreHelper.FindByNameAsync(this, normalizedName, cancellationToken);
            }
        }
    }
}
