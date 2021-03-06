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

        [Fact(DisplayName = "EntityFrameworkRepository.FindsEntityById")]
        public async Task FindsEntityById()
        {
            var testId = 1;

            // In-memory database only exists while the connection is open
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            try
            {
                // Run the test against one instance of the context
                using var dbContext = DbContextHelper.InitBloggingDbContext(connection).dbContext;
                var repository = new EntityFrameworkRepository<Article, BloggingDbContext>(dbContext);
                var entity = await repository.FindByIdAsync(testId, CancellationToken.None);

                Assert.Equal(testId, entity.Id);
            }
            finally
            {
                connection.Close();
            }
        }

        [Fact(DisplayName = "EntityFrameworkRepository.FindsEntity")]
        public async Task FindsEntity()
        {
            var testId = 1;

            // In-memory database only exists while the connection is open
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            try
            {
                // Run the test against one instance of the context
                using var dbContext = DbContextHelper.InitBloggingDbContext(connection).dbContext;
                var repository = new EntityFrameworkRepository<Article, BloggingDbContext>(dbContext);
                var spec = new FindArticleByIdSpecification();
                var entity = await repository.FindAsync(testId, spec, CancellationToken.None);

                Assert.Equal(testId, entity.Id);
            }
            finally
            {
                connection.Close();
            }
        }

        [Fact(DisplayName = "EntityFrameworkRepository.IncludesOneToManyRelationshipsInFoundEntity")]
        public async Task IncludesOneToManyRelationshipsInFoundEntity()
        {
            var testId = 1;

            // In-memory database only exists while the connection is open
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            try
            {
                // Run the test against one instance of the context
                using var dbContext = DbContextHelper.InitBloggingDbContext(connection).dbContext;
                var repository = new EntityFrameworkRepository<Article, BloggingDbContext>(dbContext);
                var spec = new FindArticleByIdSpecification();
                spec.AddInclude(x => x.Author);
                var entity = await repository.FindAsync(testId, spec, CancellationToken.None);

                Assert.NotNull(entity.Author);
                Assert.Equal(entity.AuthorId, entity.Author.Id);
            }
            finally
            {
                connection.Close();
            }
        }

        [Fact(DisplayName = "EntityFrameworkRepository.IncludesManyToManyRelationshipsAndTheirChildRelationshipsInFoundEntity")]
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
                // Run the test against one instance of the context
                using var dbContext = DbContextHelper.InitBloggingDbContext(connection).dbContext;
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
                var entity = await repository.FindAsync(testId, spec, CancellationToken.None);

                Assert.Equal(categoriesCount, entity.Categories.Count);
                Assert.Equal(testCategoryParentId, entity.Categories.Single(x => x.Id == testCategoryId).ParentId);
                Assert.NotNull(entity.Categories.Single(x => x.Id == testCategoryId).Parent);
                Assert.Equal(testCategoryParentId, entity.Categories.Single(x => x.Id == testCategoryId).Parent.Id);
                Assert.Equal(siblingCount, entity.Categories.Single(x => x.Id == testCategoryId).Articles.Count);
                Assert.Equal(testAuthorId, entity.Categories.Single(x => x.Id == testCategoryId).Articles.Single(x => x.Id == testId).AuthorId);
                Assert.NotNull(entity.Categories.Single(x => x.Id == testCategoryId).Articles.Single(x => x.Id == testId).Author);
                Assert.Equal(testAuthorId, entity.Categories.Single(x => x.Id == testCategoryId).Articles.Single(x => x.Id == testId).Author.Id);
            }
            finally
            {
                connection.Close();
            }
        }

        [Fact(DisplayName = "EntityFrameworkRepository.UpdatesOneToManyRelationshipRespectingParentIdWhenParentPresentsAndForeignKeyNotPresent")]
        public async Task UpdatesOneToManyRelationshipRespectingParentIdWhenParentPresentsAndForeignKeyNotPresent()
        {
            var testId = 1;
            var testParentId = 1;
            var newParentId = 2;

            // In-memory database only exists while the connection is open
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            try
            {
                // Run the test against one instance of the context
                var db = DbContextHelper.InitBloggingDbContext(connection);
                using (var dbContext = db.dbContext)
                {
                    var repository = new EntityFrameworkRepository<Article, BloggingDbContext>(dbContext);
                    var entity = await dbContext.Articles.AsNoTracking().Include(x => x.Author).SingleAsync(x => x.Id == testId);

                    Assert.Equal(testParentId, entity.AuthorId);
                    Assert.Equal(testParentId, entity.Author.Id);

                    entity.AuthorId = 0;
                    entity.Author = dbContext.Authors.AsNoTracking().Single(x => x.Id == newParentId);
                    var spec = new UpdateArticleSpecification();
                    spec.AddOneToManyRelationship(
                        foreignKeyExpression: x => x.AuthorId,
                        parentExpression: x => x.Author,
                        parentIdExpression: x => ((Author)x).Id);
                    await repository.UpdateAsync(entity, spec, CancellationToken.None);
                }

                // Use a separate instance of the context to verify correct data was saved to database
                using (var dbContext = new BloggingDbContext(db.options))
                {
                    var entity = await dbContext.Articles.AsNoTracking().Include(x => x.Author).SingleAsync(x => x.Id == testId);

                    Assert.Equal(newParentId, entity.AuthorId);
                    Assert.Equal(newParentId, entity.Author.Id);
                }
            }
            finally
            {
                connection.Close();
            }
        }

        [Fact(DisplayName = "EntityFrameworkRepository.UpdatesOneToManyRelationshipRespectingForeignKeyWhenForeignKeyPresentsAndParentHasDifferentId")]
        public async Task UpdatesOneToManyRelationshipRespectingForeignKeyWhenForeignKeyPresentsAndParentHasDifferentId()
        {
            var testId = 1;
            var testParentId = 1;
            var newForeignKeyId = 2;
            var newParentId = 3;

            // In-memory database only exists while the connection is open
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            try
            {
                // Run the test against one instance of the context
                var db = DbContextHelper.InitBloggingDbContext(connection);
                using (var dbContext = db.dbContext)
                {
                    var repository = new EntityFrameworkRepository<Article, BloggingDbContext>(dbContext);
                    var entity = await dbContext.Articles.AsNoTracking().Include(x => x.Author).SingleAsync(x => x.Id == testId);

                    Assert.Equal(testParentId, entity.AuthorId);
                    Assert.Equal(testParentId, entity.Author.Id);

                    entity.AuthorId = newForeignKeyId;
                    entity.Author = dbContext.Authors.AsNoTracking().Single(x => x.Id == newParentId);
                    var spec = new UpdateArticleSpecification();
                    spec.AddOneToManyRelationship(
                        foreignKeyExpression: x => x.AuthorId,
                        parentExpression: x => x.Author,
                        parentIdExpression: x => ((Author)x).Id);
                    await repository.UpdateAsync(entity, spec, CancellationToken.None);
                }

                // Use a separate instance of the context to verify correct data was saved to database
                using (var dbContext = new BloggingDbContext(db.options))
                {
                    var entity = await dbContext.Articles.AsNoTracking().Include(x => x.Author).SingleAsync(x => x.Id == testId);

                    Assert.Equal(newForeignKeyId, entity.AuthorId);
                    Assert.Equal(newForeignKeyId, entity.Author.Id);
                }
            }
            finally
            {
                connection.Close();
            }
        }

        [Fact(DisplayName = "EntityFrameworkRepository.UpdatesManyToManyRelationship")]
        public async Task UpdatesManyToManyRelationship()
        {
            var testId = 1;
            var testRelationshipCount = 2;
            var testRelationship1Id = 1;
            var testRelationship2Id = 5;
            var newRelationshipId = 2;

            // In-memory database only exists while the connection is open
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            try
            {
                // Run the test against one instance of the context
                var db = DbContextHelper.InitBloggingDbContext(connection);
                using (var dbContext = db.dbContext)
                {
                    var repository = new EntityFrameworkRepository<Article, BloggingDbContext>(dbContext);
                    var entity = await dbContext.Articles.AsNoTracking().SingleAsync(x => x.Id == testId);
                    var categoryIds = await dbContext.ArticleCategories.AsNoTracking().Where(x => x.ArticleId == testId)
                                                                                      .Select(x => x.CategoryId)
                                                                                      .ToListAsync();
                    entity.Categories = await dbContext.Categories.AsNoTracking().Where(x => categoryIds.Contains(x.Id))
                                                                                 .ToListAsync();

                    Assert.Equal(testRelationshipCount, entity.Categories.Count);
                    Assert.Contains(entity.Categories, x => x.Id == testRelationship1Id);
                    Assert.Contains(entity.Categories, x => x.Id == testRelationship2Id);

                    entity.Categories.Add(new Category { Id = newRelationshipId });
                    var spec = new UpdateArticleSpecification();
                    spec.AddManyToManyRelationship(
                        thisIdExpression: x => x.Id,
                        otherIdExpression: x => ((Category)x).Id,
                        othersExpression: x => x.Categories,
                        linkType: typeof(ArticleCategory),
                        linkForeignKeyToThisExpression: x => ((ArticleCategory)x).ArticleId,
                        linkForeignKeyToOtherExpression: x => ((ArticleCategory)x).CategoryId);
                    await repository.UpdateAsync(entity, spec, CancellationToken.None);
                }

                // Use a separate instance of the context to verify correct data was saved to database
                using (var dbContext = new BloggingDbContext(db.options))
                {
                    var entity = await dbContext.Articles.AsNoTracking().SingleAsync(x => x.Id == testId);
                    var categoryIds = await dbContext.ArticleCategories.AsNoTracking().Where(x => x.ArticleId == testId)
                                                                                      .Select(x => x.CategoryId)
                                                                                      .ToListAsync();
                    entity.Categories = await dbContext.Categories.AsNoTracking().Where(x => categoryIds.Contains(x.Id))
                                                                                 .ToListAsync();

                    Assert.Equal(testRelationshipCount + 1, entity.Categories.Count);
                    Assert.Contains(entity.Categories, x => x.Id == testRelationship1Id);
                    Assert.Contains(entity.Categories, x => x.Id == testRelationship2Id);
                    Assert.Contains(entity.Categories, x => x.Id == newRelationshipId);
                }
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
