using Microsoft.Data.Sqlite;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MotiNet.Entities.EntityFrameworkCore.Test
{
    public class CodeBasedEntityStore
    {
        [Fact(DisplayName = "CodeBasedEntityStore.FindsEntityByCode")]
        public async Task FindsEntityByCode()
        {
            var testCode = "di-del-gli-la-tale-d'esse-ingannati-noi-al-ignoranza";

            // In-memory database only exists while the connection is open
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            try
            {
                // Run the test against one instance of the context
                using (var dbContext = DbContextHelper.InitBloggingDbContext(connection).dbContext)
                {
                    var store = new ArticleStore(dbContext);
                    var entity = await store.FindByCodeAsync(testCode, CancellationToken.None);
                    var expected = dbContext.Articles.Single(x => x.UrlFriendlyTitle == testCode).Id;

                    Assert.Equal(expected, entity.Id);
                }
            }
            finally
            {
                connection.Close();
            }
        }

        public class ArticleStore : StoreBase<Article, BloggingDbContext>, ICodeBasedEntityStoreMarker<Article, BloggingDbContext>
        {
            public ArticleStore(BloggingDbContext dbContext) : base(dbContext) { }

            public Article FindByCode(string normalizedCode)
            {
                return CodeBasedEntityStoreHelper.FindByCode(this, normalizedCode, x => x.UrlFriendlyTitle);
            }

            public Task<Article> FindByCodeAsync(string normalizedCode, CancellationToken cancellationToken)
            {
                return CodeBasedEntityStoreHelper.FindByCodeAsync(this, normalizedCode, x => x.UrlFriendlyTitle, cancellationToken);
            }
        }
    }
}
