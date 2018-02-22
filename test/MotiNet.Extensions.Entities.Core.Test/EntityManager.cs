using Microsoft.Extensions.Logging;
using Moq;
using MotiNet.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MotiNet.Extensions.Entities.Core.Test
{
    public class EntityManager
    {
        private ArticleManager Manager { get; } = new ArticleManager();

        private ArticleStore Store => Manager.Store as ArticleStore;

        private CancellationToken CancellationToken => CancellationToken.None;

        [Fact]
        public async void FindsEntityById()
        {
            var testId = 2;

            var entity = await Manager.FindByIdAsync(testId, CancellationToken);

            Assert.Equal(testId, entity.Id);
        }

        [Fact]
        public async void FindsEntityByPriority()
        {
            var testPriority = 2;

            var entity = await Manager.FindAsync(testPriority, new FindArticleByPrioritySpecification(), CancellationToken);

            Assert.Equal(testPriority, entity.Priority);
        }

        [Fact]
        public async void GetsAllEntities()
        {
            var entities = await Manager.AllAsync(CancellationToken);
            var expected = Store.Data.Count;

            Assert.Equal(expected, entities.Count());
        }

        [Fact]
        public async void SearchesEntitiesByTitle()
        {
            var testTitle = "Title 3";

            var entities = await Manager.SearchAsync(new SearchArticleSpecification(testTitle), CancellationToken);
            var expected = Store.Data.Count(x => x.Title == testTitle);

            Assert.Equal(expected, entities.Count());
        }

        [Fact]
        public async void SearchesEntitiesByTitleWithPaging()
        {
            var testTitle = "Title 3";

            var result = await Manager.SearchAsync(new PagedSearchArticleSpecification(testTitle, 10, 1), CancellationToken);
            var expected = Store.Data.Count(x => x.Title == testTitle);

            Assert.Equal(expected, result.ResultCount);
        }

        [Fact]
        public async void CreatesEntity()
        {
            var newEntity = new Article { Id = 4, Title = "Title 4" };

            var initialCount = Store.Data.Count;
            var result = await Manager.CreateAsync(newEntity, CancellationToken);
            var finalCount = Store.Data.Count;
            var addedEntity = Store.Data.Single(x => x.Id == newEntity.Id);

            Assert.True(result.Succeeded);
            Assert.Equal(finalCount, initialCount + 1);
            Assert.Equal(newEntity.Id, addedEntity.Id);
        }

        [Fact]
        public async void UpdatesEntity()
        {
            var testId = 1;
            var newTitle = "Modified title";

            var currentEntity = Store.Data.Single(x => x.Id == testId);
            var newEntity = new Article { Id = testId, Title = newTitle };

            Assert.NotEqual(newTitle, currentEntity.Title);

            var result = await Manager.UpdateAsync(newEntity, CancellationToken);

            Assert.True(result.Succeeded);
            Assert.Equal(newTitle, currentEntity.Title);
        }

        [Fact]
        public async void DeletesEntity()
        {
            var testId = 1;
            var currentEntity = new Article { Id = testId };

            var initialCount = Store.Data.Count;
            var result = await Manager.DeleteAsync(currentEntity, CancellationToken);
            var finalCount = Store.Data.Count;
            var deletedEntity = Store.Data.SingleOrDefault(x => x.Id == testId);

            Assert.True(result.Succeeded);
            Assert.Equal(finalCount, initialCount - 1);
            Assert.Null(deletedEntity);
        }

        public class ArticleStore : IEntityStore<Article>
        {
            internal List<Article> Data { get; } = new List<Article>()
            {
                new Article() { Id = 1, Priority = 1, Title = "Title 1", UrlFriendlyTitle = "title-1", Content = "Content 1", AuthorId = 1 },
                new Article() { Id = 2, Priority = 2, Title = "Title 2", UrlFriendlyTitle = "title-2", Content = "Content 2", AuthorId = 2 },
                new Article() { Id = 3, Priority = 3, Title = "Title 3", UrlFriendlyTitle = "title-3", Content = "Content 3", AuthorId = 3 }
            };

            public Task<Article> FindByIdAsync(object id, CancellationToken cancellationToken)
            {
                var result = Data.SingleOrDefault(x => x.Id.Equals(id));
                return Task.FromResult(result);
            }

            public Task<Article> FindAsync(object key, IFindSpecification<Article> spec, CancellationToken cancellationToken)
            {
                var entities = Data.AsQueryable();

                if (spec.AdditionalCriteria != null)
                {
                    entities = entities.Where(spec.AdditionalCriteria);
                }

                var result = entities.SingleOrDefault(x => Equals(GetPropertyValue(x, spec.KeyExpression), key));
                return Task.FromResult(result);
            }

            public Task<IEnumerable<Article>> AllAsync(CancellationToken cancellationToken)
            {
                var result = Data.AsEnumerable();
                return Task.FromResult(result);
            }

            public Task<IEnumerable<Article>> SearchAsync(ISearchSpecification<Article> spec, CancellationToken cancellationToken)
            {
                var entities = Data.AsQueryable();

                if (spec.Criteria != null)
                {
                    entities = entities.Where(spec.Criteria);
                }

                var result = entities.AsEnumerable();
                return Task.FromResult(result);
            }

            public Task<PagedSearchResult<Article>> SearchAsync(IPagedSearchSpecification<Article> spec, CancellationToken cancellationToken)
            {
                var entities = Data.AsQueryable();

                if (spec.ScopeCriteria != null)
                {
                    entities = entities.Where(spec.ScopeCriteria);
                }

                var totalCount = entities.Count();

                if (spec.Criteria != null)
                {
                    entities = entities.Where(spec.Criteria);
                }

                var resultCount = entities.Count();

                var result = entities.ToList();

                return Task.FromResult(new PagedSearchResult<Article>(totalCount, resultCount, result));
            }

            public Task<Article> CreateAsync(Article entity, CancellationToken cancellationToken)
            {
                Data.Add(entity);
                return Task.FromResult(entity);
            }

            public Task<Article> CreateAsync(Article entity, IModifySpecification<Article> spec, CancellationToken cancellationToken)
            {
                Data.Add(entity);
                return Task.FromResult(entity);
            }

            public Task UpdateAsync(Article entity, CancellationToken cancellationToken)
            {
                var oldEntity = Data.Single(x => x.Id == entity.Id);
                oldEntity.Priority = entity.Priority;
                oldEntity.Title = entity.Title;
                oldEntity.UrlFriendlyTitle = entity.UrlFriendlyTitle;
                oldEntity.Content = entity.Content;
                oldEntity.AuthorId = entity.AuthorId;
                return Task.FromResult(0);
            }

            public Task UpdateAsync(Article entity, IModifySpecification<Article> spec, CancellationToken cancellationToken)
            {
                var oldEntity = Data.Single(x => x.Id == entity.Id);
                oldEntity.Priority = entity.Priority;
                oldEntity.Title = entity.Title;
                oldEntity.UrlFriendlyTitle = entity.UrlFriendlyTitle;
                oldEntity.Content = entity.Content;
                oldEntity.AuthorId = entity.AuthorId;
                return Task.FromResult(0);
            }

            public Task DeleteAsync(Article entity, CancellationToken cancellationToken)
            {
                Data.Remove(Data.Single(x => x.Id == entity.Id));
                return Task.FromResult(0);
            }

            public void Dispose() { }

            #region Helpers

            private TResult GetPropertyValue<T, TResult>(T obj, Expression<Func<T, TResult>> expression)
            {
                var body = expression.Body;
                MemberExpression propertySelector = body as MemberExpression ?? (MemberExpression)((UnaryExpression)body).Operand;
                var property = (PropertyInfo)propertySelector.Member;
                return (TResult)property.GetValue(obj);
            }

            #endregion
        }

        public class ArticleManager : ManagerBase<Article>, IEntityManager<Article>
        {
            public ArticleManager()
                : base(
                      store: new ArticleStore(),
                      entityAccessor: null,
                      entityValidators: null,
                      logger: new Mock<ILogger<ArticleManager>>().Object)
            { }

            public IEntityStore<Article> EntityStore => Store as IEntityStore<Article>;
        }

        public class FindArticleByPrioritySpecification : FindSpecificationBase<Article>
        {
            public override Expression<Func<Article, object>> KeyExpression => x => x.Priority;
        }

        public class SearchArticleSpecification : SearchSpecificationBase<Article>
        {
            public SearchArticleSpecification(string title)
            {
                Title = title ?? throw new ArgumentNullException(nameof(title));
            }

            public string Title { get; set; }

            public override Expression<Func<Article, bool>> Criteria => x => x.Title == Title;
        }

        public class PagedSearchArticleSpecification : PagedSearchSpecificationBase<Article>
        {
            public PagedSearchArticleSpecification(string title, int? pageSize, int? pageNumber) : base(pageSize, pageNumber)
            {
                Title = title ?? throw new ArgumentNullException(nameof(title));
            }

            public string Title { get; set; }

            public override Expression<Func<Article, bool>> ScopeCriteria => x => true;

            public override Expression<Func<Article, bool>> Criteria => x => x.Title == Title;
        }
    }
}
