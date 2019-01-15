using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MotiNet.Entities.Test
{
    public class TimeTrackedEntityManager
    {
        private ArticleManager Manager { get; } = new ArticleManager();

        private ArticleStore Store => Manager.Store as ArticleStore;

        [Fact(DisplayName = "TimeTrackedEntityManager.AutoSetsDatesWhenCreatingANewEntity")]
        public async Task AutoSetsDatesWhenCreatingANewEntity()
        {
            var newEntity = new Article { Id = 4 };
            var oldDataCreateDate = newEntity.DataCreateDate;
            var oldDataLastModifyDate = newEntity.DataLastModifyDate;

            var result = await Manager.CreateAsync(newEntity);
            var addedEntity = Store.Data.Single(x => x.Id == newEntity.Id);

            Assert.True(result.Succeeded);
            Assert.True(addedEntity.DataCreateDate > oldDataCreateDate);
            Assert.True(addedEntity.DataLastModifyDate > oldDataLastModifyDate);
        }

        [Fact(DisplayName = "TimeTrackedEntityManager.AutoUpdatesDataLastModifyDateWhenUpdatingAnEntity")]
        public async Task AutoUpdatesDataLastModifyDateWhenUpdatingAnEntity()
        {
            var testId = 1;

            var currentEntity = Store.Data.Single(x => x.Id == testId);
            var oldDataLastModifyDate = currentEntity.DataLastModifyDate;

            var result = await Manager.UpdateAsync(currentEntity);
            var updatedEntity = Store.Data.Single(x => x.Id == testId);

            Assert.True(result.Succeeded);
            Assert.True(updatedEntity.DataLastModifyDate > oldDataLastModifyDate);
        }

        [Fact(DisplayName = "TimeTrackedEntityManager.FindsLatestEntity")]
        public async Task FindsLatestEntity()
        {
            var entity = await Manager.FindLatestAsync();
            var expected = Store.Data.OrderByDescending(x => x.DataCreateDate).First().Id;

            Assert.Equal(expected, entity.Id);
        }

        public class ArticleStore : EntityStoreBase<Article>, ITimeTrackedEntityStore<Article>
        {
            internal List<Article> Data { get; } = new List<Article>()
            {
                new Article() { Id = 1, DataCreateDate = DateTime.Now.AddDays(-1), DataLastModifyDate = DateTime.Now },
                new Article() { Id = 2, DataCreateDate = DateTime.Now, DataLastModifyDate = DateTime.Now },
                new Article() { Id = 3, DataCreateDate = DateTime.Now.AddDays(-1), DataLastModifyDate = DateTime.Now }
            };

            public override Task<Article> FindByIdAsync(object id, CancellationToken cancellationToken)
            {
                return Task.FromResult(Data.SingleOrDefault(x => x.Id == (int)id));
            }

            public override Task<Article> CreateAsync(Article entity, CancellationToken cancellationToken)
            {
                Data.Add(entity);
                return Task.FromResult(entity);
            }

            public override Task UpdateAsync(Article entity, CancellationToken cancellationToken)
            {
                var oldEntity = Data.Single(x => x.Id == entity.Id);
                oldEntity.DataCreateDate = entity.DataCreateDate;
                oldEntity.DataLastModifyDate = entity.DataLastModifyDate;
                return Task.FromResult(0);
            }

            public Article FindLatest() => throw new NotImplementedException();

            public Task<Article> FindLatestAsync(CancellationToken cancellationToken)
            {
                var entities = Data.AsQueryable();

                var result = entities.OrderByDescending(x => x.DataCreateDate).First();
                return Task.FromResult(result);
            }
        }

        public class ArticleAccessor
            : IEntityAccessor<Article>,
              ITimeTrackedEntityAccessor<Article>
        {
            public object GetId(Article entity) => entity.Id;

            public DateTime GetDataCreateDate(Article entity) => entity.DataCreateDate;

            public void SetDataCreateDate(Article entity, DateTime dataCreateDate) => entity.DataCreateDate = dataCreateDate;

            public void SetDataLastModifyDate(Article entity, DateTime dataLastModifyDate) => entity.DataLastModifyDate = dataLastModifyDate;
        }

        public class ArticleManager : ManagerBase<Article>, IEntityManager<Article>, ITimeTrackedEntityManager<Article>
        {
            public ArticleManager()
                : base(
                      store: new ArticleStore(),
                      entityAccessor: new ArticleAccessor(),
                      entityValidators: null,
                      logger: new Mock<ILogger<ArticleManager>>().Object)
            { }

            public IEntityStore<Article> EntityStore => Store as IEntityStore<Article>;

            public IEntityAccessor<Article> EntityAccessor => Accessor as IEntityAccessor<Article>;

            public ITimeTrackedEntityStore<Article> TimeTrackedEntityStore => Store as ITimeTrackedEntityStore<Article>;

            public ITimeTrackedEntityAccessor<Article> TimeTrackedEntityAccessor => Accessor as ITimeTrackedEntityAccessor<Article>;
        }
    }
}
