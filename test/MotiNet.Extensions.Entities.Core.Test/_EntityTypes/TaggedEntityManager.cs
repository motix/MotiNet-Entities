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
    public class TaggedEntityManager
    {
        private ArticleManager Manager { get; } = new ArticleManager();

        private ArticleStore Store => Manager.Store as ArticleStore;

        [Fact(DisplayName = "TaggedEntityManager.AutoNormalizesTagsWhenSavingAnEntity")]
        public async Task AutoNormalizesTagsWhenSavingAnEntity()
        {
            var testTags = "A B-C";
            var newEntity = new Article { Id = 4, Tags = testTags };

            var result = await Manager.CreateAsync(newEntity);
            var addedEntity = Store.Data.Single(x => x.Id == newEntity.Id);
            var expected = new TagProcessor().NormalizeTags(testTags);

            Assert.True(result.Succeeded);
            Assert.Equal(expected, addedEntity.Tags);
        }

        public class ArticleStore : EntityStoreBase<Article>
        {
            internal List<Article> Data { get; } = new List<Article>()
            {
                new Article() { Id = 1, UrlFriendlyTitle = "title-1" },
                new Article() { Id = 2, UrlFriendlyTitle = "title-2" },
                new Article() { Id = 3, UrlFriendlyTitle = "title-3" },
            };
            public override Task<Article> CreateAsync(Article entity, CancellationToken cancellationToken)
            {
                Data.Add(entity);
                return Task.FromResult(entity);
            }
        }

        public class ArticleAccessor : ITaggedEntityAccessor<Article>
        {
            public string GetTags(Article entity) => entity.Tags;

            public void SetTags(Article entity, string tags) => entity.Tags = tags;
        }

        public class ArticleManager : ManagerBase<Article>, IEntityManager<Article>, ITaggedEntityManager<Article>
        {
            public ArticleManager()
                : base(
                      store: new ArticleStore(),
                      entityAccessor: new ArticleAccessor(),
                      entityValidators: null,
                      logger: new Mock<ILogger<ArticleManager>>().Object)
            {
                TagProcessor = new TagProcessor();
            }

            public IEntityStore<Article> EntityStore => Store as IEntityStore<Article>;

            public IEntityAccessor<Article> EntityAccessor => throw new NotImplementedException();

            public ITaggedEntityAccessor<Article> TaggedEntityAccessor => Accessor as ITaggedEntityAccessor<Article>;

            public ITagProcessor TagProcessor { get; }
        }
    }
}
