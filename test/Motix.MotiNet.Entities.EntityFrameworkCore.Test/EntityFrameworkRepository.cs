using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MotiNet.Entities.EntityFrameworkCore.Test
{
    public class EntityFrameworkRepository
    {
        private class FindArticleByIdSpecification : FindSpecificationBase<Article>
        {
            public override Expression<Func<Article, object>> KeyExpression => x => x.Id;
        }

        private class UpdateArticleSpecification : ModifySpecificationBase<Article> { }

        [Fact]
        public async Task MatchesEntityWithGivenId()
        {
            var testId = 1;

            // In-memory database only exists while the connection is open
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            try
            {
                var options = new DbContextOptionsBuilder<BloggingDbContext>()
                    .UseSqlite(connection)
                    .Options;

                // Create the schema in the database
                using (var dbContext = new BloggingDbContext(options))
                {
                    dbContext.Database.EnsureCreated();
                    BloggingDbContextSeeder.Seed(dbContext);
                }

                // Run the test against one instance of the context
                using (var dbContext = new BloggingDbContext(options))
                {
                    var repository = new EntityFrameworkRepository<Article, BloggingDbContext>(dbContext);
                    var article = await repository.FindByIdAsync(testId, CancellationToken.None);

                    Assert.Equal(testId, article.Id);
                }
            }
            finally
            {
                connection.Close();
            }
        }

        [Fact]
        public async Task IncludesOneToManyRelationshipsInFoundEntity()
        {
            var testId = 1;

            // In-memory database only exists while the connection is open
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            try
            {
                var options = new DbContextOptionsBuilder<BloggingDbContext>()
                    .UseSqlite(connection)
                    .Options;

                // Create the schema in the database
                using (var dbContext = new BloggingDbContext(options))
                {
                    dbContext.Database.EnsureCreated();
                    BloggingDbContextSeeder.Seed(dbContext);
                }

                // Run the test against one instance of the context
                using (var dbContext = new BloggingDbContext(options))
                {
                    var repository = new EntityFrameworkRepository<Article, BloggingDbContext>(dbContext);
                    var spec = new FindArticleByIdSpecification();
                    spec.AddInclude(x => x.Author);
                    var article = await repository.FindByIdAsync(testId, spec, CancellationToken.None);

                    Assert.NotNull(article.Author);
                    Assert.Equal(article.AuthorId, article.Author.Id);
                }
            }
            finally
            {
                connection.Close();
            }
        }

        [Fact]
        public async Task IncludesManyToManyRelationshipsAndTheirChildRelationshipsInFoundEntity()
        {
            var testId = 1;
            var categoriesCount = 2;
            var testCategoryId = 1;
            var testCategoryParentId = 6;
            var siblingCount = 3;
            var testAuthorId = 1;

            // In-memory database only exists while the connection is open
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            try
            {
                var options = new DbContextOptionsBuilder<BloggingDbContext>()
                    .UseSqlite(connection)
                    .Options;

                // Create the schema in the database
                using (var dbContext = new BloggingDbContext(options))
                {
                    dbContext.Database.EnsureCreated();
                    BloggingDbContextSeeder.Seed(dbContext);
                }

                // Run the test against one instance of the context
                using (var dbContext = new BloggingDbContext(options))
                {
                    var repository = new EntityFrameworkRepository<Article, BloggingDbContext>(dbContext);
                    var articleCategoryInclude = new ManyToManyIncludeSpecification<Article>(
                        thisIdExpression: x => x.Id,
                        otherType: typeof(Category),
                        otherIdExpression: x => ((Category)x).Id,
                        othersExpression: x => x.Categories,
                        linkType: typeof(ArticleCategory),
                        linkForeignKeyToThisExpression: x => ((ArticleCategory)x).ArticleId,
                        linkForeignKeyToOtherExpression: x => ((ArticleCategory)x).CategoryId);
                    articleCategoryInclude.ChildIncludes.Add(x => ((Category)x).Parent);
                    var categoryArticleInclude = new ManyToManyIncludeSpecification<object>(
                        thisIdExpression: x => ((Category)x).Id,
                        otherType: typeof(Article),
                        otherIdExpression: x => ((Article)x).Id,
                        othersExpression: x => ((Category)x).Articles,
                        linkType: typeof(ArticleCategory),
                        linkForeignKeyToThisExpression: x => ((ArticleCategory)x).CategoryId,
                        linkForeignKeyToOtherExpression: x => ((ArticleCategory)x).ArticleId);
                    categoryArticleInclude.ChildIncludes.Add(x => ((Article)x).Author);
                    articleCategoryInclude.ChildManyToManyIncludes.Add(categoryArticleInclude);
                    var spec = new FindArticleByIdSpecification();
                    spec.AddInclude(articleCategoryInclude);
                    var article = await repository.FindByIdAsync(testId, spec, CancellationToken.None);

                    Assert.Equal(categoriesCount, article.Categories.Count);
                    Assert.Equal(testCategoryParentId, article.Categories.Single(x => x.Id == testCategoryId).ParentId);
                    Assert.Equal(testCategoryParentId, article.Categories.Single(x => x.Id == testCategoryId).Parent.Id);
                    Assert.Equal(siblingCount, article.Categories.Single(x => x.Id == testCategoryId).Articles.Count);
                    Assert.Equal(testAuthorId, article.Categories.Single(x => x.Id == testCategoryId).Articles.Single(x => x.Id == testId).AuthorId);
                    Assert.Equal(testAuthorId, article.Categories.Single(x => x.Id == testCategoryId).Articles.Single(x => x.Id == testId).Author.Id);
                }
            }
            finally
            {
                connection.Close();
            }
        }

        [Fact]
        public async Task UpdatesOneToManyRelationshipToParentIdWithParentAndNoForeignKey()
        {
            var testId = 1;
            var newParentId = 2;

            // In-memory database only exists while the connection is open
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            try
            {
                var options = new DbContextOptionsBuilder<BloggingDbContext>()
                    .UseSqlite(connection)
                    .Options;

                // Create the schema in the database
                using (var dbContext = new BloggingDbContext(options))
                {
                    dbContext.Database.EnsureCreated();
                    BloggingDbContextSeeder.Seed(dbContext);
                }

                // Run the test against one instance of the context
                using (var dbContext = new BloggingDbContext(options))
                {
                    var repository = new EntityFrameworkRepository<Article, BloggingDbContext>(dbContext);
                    var article = dbContext.Articles.AsNoTracking().Include(x => x.Author).Single(x => x.Id == testId);

                    Assert.Equal(testId, article.AuthorId);
                    Assert.Equal(testId, article.Author.Id);

                    article.AuthorId = 0;
                    article.Author = dbContext.Authors.AsNoTracking().Single(x => x.Id == newParentId);
                    var spec = new UpdateArticleSpecification();
                    spec.AddOneToManyRelationship(
                        foreignKeyExpression: x => x.AuthorId,
                        parentExpression: x => x.Author,
                        parentIdExpression: x => ((Author)x).Id);
                    await repository.UpdateAsync(article, spec, CancellationToken.None);
                }

                // Use a separate instance of the context to verify correct data was saved to database
                using (var dbContext = new BloggingDbContext(options))
                {
                    var article = dbContext.Articles.AsNoTracking().Include(x => x.Author).Single(x => x.Id == testId);

                    Assert.Equal(newParentId, article.AuthorId);
                    Assert.Equal(newParentId, article.Author.Id);
                }
            }
            finally
            {
                connection.Close();
            }
        }

        [Fact]
        public async Task UpdatesOneToManyRelationshipToForeignKeyIfForeignKeyPresentsAndParentHasDifferentId()
        {
            var testId = 1;
            var newForeignKeyId = 2;
            var newParentId = 3;

            // In-memory database only exists while the connection is open
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            try
            {
                var options = new DbContextOptionsBuilder<BloggingDbContext>()
                    .UseSqlite(connection)
                    .Options;

                // Create the schema in the database
                using (var dbContext = new BloggingDbContext(options))
                {
                    dbContext.Database.EnsureCreated();
                    BloggingDbContextSeeder.Seed(dbContext);
                }

                // Run the test against one instance of the context
                using (var dbContext = new BloggingDbContext(options))
                {
                    var repository = new EntityFrameworkRepository<Article, BloggingDbContext>(dbContext);
                    var article = dbContext.Articles.AsNoTracking().Include(x => x.Author).Single(x => x.Id == testId);

                    Assert.Equal(testId, article.AuthorId);
                    Assert.Equal(testId, article.Author.Id);

                    article.AuthorId = newForeignKeyId;
                    article.Author = dbContext.Authors.AsNoTracking().Single(x => x.Id == newParentId);
                    var spec = new UpdateArticleSpecification();
                    spec.AddOneToManyRelationship(
                        foreignKeyExpression: x => x.AuthorId,
                        parentExpression: x => x.Author,
                        parentIdExpression: x => ((Author)x).Id);
                    await repository.UpdateAsync(article, spec, CancellationToken.None);
                }

                // Use a separate instance of the context to verify correct data was saved to database
                using (var dbContext = new BloggingDbContext(options))
                {
                    var article = dbContext.Articles.AsNoTracking().Include(x => x.Author).Single(x => x.Id == testId);

                    Assert.Equal(newForeignKeyId, article.AuthorId);
                    Assert.Equal(newForeignKeyId, article.Author.Id);
                }
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
