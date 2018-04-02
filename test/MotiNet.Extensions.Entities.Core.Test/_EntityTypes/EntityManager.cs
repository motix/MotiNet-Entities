using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MotiNet.Entities.Test
{
    public class EntityManager
    {
        private ArticleManager Manager { get; } = new ArticleManager();

        private ArticleStore Store => Manager.Store as ArticleStore;

        [Fact(DisplayName = "EntityManager.FindsEntityById")]
        public async void FindsEntityById()
        {
            var testId = 2;

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await Manager.FindByIdAsync(null));
            var entity = await Manager.FindByIdAsync(testId);

            Assert.Equal(testId, entity.Id);
        }

        [Fact(DisplayName = "EntityManager.FindsEntityByPriority")]
        public async void FindsEntityByPriority()
        {
            var testPriority = 2;

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await Manager.FindAsync(null, null));
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await Manager.FindAsync(string.Empty, null));
            var entity = await Manager.FindAsync(testPriority, new FindArticleByPrioritySpecification());

            Assert.Equal(testPriority, entity.Priority);
        }

        [Fact(DisplayName = "EntityManager.GetsAllEntities")]
        public async void GetsAllEntities()
        {
            var entities = await Manager.AllAsync();
            var expected = Store.Data.Count;

            Assert.Equal(expected, entities.Count());
        }

        [Fact(DisplayName = "EntityManager.SearchesEntitiesByTitle")]
        public async void SearchesEntitiesByTitle()
        {
            var testTitle = "Title 3";

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await Manager.SearchAsync((ISearchSpecification<Article>)null));
            var entities = await Manager.SearchAsync(new SearchArticleSpecification(testTitle));
            var expected = Store.Data.Count(x => x.Title == testTitle);

            Assert.Equal(expected, entities.Count());
        }

        [Fact(DisplayName = "EntityManager.SearchesEntitiesByTitleWithPaging")]
        public async void SearchesEntitiesByTitleWithPaging()
        {
            var testTitle = "Title 3";

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await Manager.SearchAsync(null));
            var result = await Manager.SearchAsync(new PagedSearchArticleSpecification(testTitle, 10, 1));
            var expected = Store.Data.Count(x => x.Title == testTitle);

            Assert.Equal(expected, result.ResultCount);
        }

        [Fact(DisplayName = "EntityManager.ValidatesEntityWhenCreating")]
        public async void ValidatesEntityWhenCreating()
        {
            var newEntity = new Article { Id = 4 };

            var initialCount = Store.Data.Count;
            var result = await Manager.CreateAsync(newEntity);
            var finalCount = Store.Data.Count;

            Assert.False(result.Succeeded);
            Assert.Equal(finalCount, initialCount);
        }

        [Fact(DisplayName = "EntityManager.CreatesEntity")]
        public async void CreatesEntity()
        {
            var newEntity = new Article { Id = 4, Title = "Title 4" };

            var initialCount = Store.Data.Count;
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await Manager.CreateAsync(null));
            var result = await Manager.CreateAsync(newEntity);
            var finalCount = Store.Data.Count;
            var addedEntity = Store.Data.Single(x => x.Id == newEntity.Id);

            Assert.True(result.Succeeded);
            Assert.Equal(finalCount, initialCount + 1);
            Assert.Equal(newEntity.Id, addedEntity.Id);
        }

        [Fact(DisplayName = "EntityManager.ValidatesEntityWhenUpdating")]
        public async void ValidatesEntityWhenUpdating()
        {
            var testId = 1;

            var currentEntity = Store.Data.Single(x => x.Id == testId);
            var oldTitle = currentEntity.Title;
            var newEntity = new Article { Id = testId, Title = null };

            Assert.NotEqual(newEntity.Title, oldTitle);

            var result = await Manager.UpdateAsync(newEntity);
            var updatedEntity = Store.Data.Single(x => x.Id == testId);

            Assert.False(result.Succeeded);
            Assert.Equal(oldTitle, updatedEntity.Title);
        }

        [Fact(DisplayName = "EntityManager.UpdatesEntity")]
        public async void UpdatesEntity()
        {
            var testId = 1;
            var newTitle = "Modified title";

            var currentEntity = Store.Data.Single(x => x.Id == testId);
            var newEntity = new Article { Id = testId, Title = newTitle };

            Assert.NotEqual(newTitle, currentEntity.Title);

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await Manager.UpdateAsync(null));
            var result = await Manager.UpdateAsync(newEntity);
            var updatedEntity = Store.Data.Single(x => x.Id == testId);

            Assert.True(result.Succeeded);
            Assert.Equal(newTitle, updatedEntity.Title);
        }

        [Fact(DisplayName = "EntityManager.DeletesEntity")]
        public async void DeletesEntity()
        {
            var testId = 1;
            var currentEntity = new Article { Id = testId };

            var initialCount = Store.Data.Count;
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await Manager.DeleteAsync(null));
            var result = await Manager.DeleteAsync(currentEntity);
            var finalCount = Store.Data.Count;
            var deletedEntity = Store.Data.SingleOrDefault(x => x.Id == testId);

            Assert.True(result.Succeeded);
            Assert.Equal(finalCount, initialCount - 1);
            Assert.Null(deletedEntity);
        }

        public class ArticleValidator : IValidator<Article>
        {
            public Task<GenericResult> ValidateAsync(object manager, Article entity)
            {
                var result = string.IsNullOrWhiteSpace(entity.Title) ?
                    GenericResult.Failed(new GenericError()) :
                    GenericResult.Success;
                return Task.FromResult(result);
            }   
        }

        public class ArticleStore : IEntityStore<Article>
        {
            internal List<Article> Data { get; } = new List<Article>()
            {
                new Article() { Id = 1, Priority = 1, Title = "Title 1" },
                new Article() { Id = 2, Priority = 2, Title = "Title 2" },
                new Article() { Id = 3, Priority = 3, Title = "Title 3" }
            };

            public Article FindById(object id)
            {
                return Data.SingleOrDefault(x => x.Id.Equals(id));
            }

            public Task<Article> FindByIdAsync(object id, CancellationToken cancellationToken)
            {
                var result = Data.SingleOrDefault(x => x.Id.Equals(id));
                return Task.FromResult(result);
            }

            public Article Find(object key, IFindSpecification<Article> spec)
            {
                var entities = Data.AsQueryable();

                if (spec.AdditionalCriteria != null)
                {
                    entities = entities.Where(spec.AdditionalCriteria);
                }

                return entities.SingleOrDefault(x => Equals(GetPropertyValue(x, spec.KeyExpression), key));
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

            public IEnumerable<Article> All()
            {
                return Data.AsEnumerable();
            }

            public Task<IEnumerable<Article>> AllAsync(CancellationToken cancellationToken)
            {
                var result = Data.AsEnumerable();
                return Task.FromResult(result);
            }

            public IEnumerable<Article> Search(ISearchSpecification<Article> spec)
            {
                var entities = Data.AsQueryable();

                if (spec.Criteria != null)
                {
                    entities = entities.Where(spec.Criteria);
                }

                return entities.AsEnumerable();
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

            public PagedSearchResult<Article> Search(IPagedSearchSpecification<Article> spec)
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

                return new PagedSearchResult<Article>(totalCount, resultCount, result);
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
                oldEntity.Title = entity.Title;
                return Task.FromResult(0);
            }

            public Task UpdateAsync(Article entity, IModifySpecification<Article> spec, CancellationToken cancellationToken)
            {
                var oldEntity = Data.Single(x => x.Id == entity.Id);
                oldEntity.Title = entity.Title;
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
                      entityValidators: new List<ArticleValidator>() { new ArticleValidator() },
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
