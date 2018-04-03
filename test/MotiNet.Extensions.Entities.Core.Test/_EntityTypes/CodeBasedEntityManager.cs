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
    public class CodeBasedEntityManager
    {
        private ArticleManager Manager { get; } = new ArticleManager();

        private ArticleStore Store => Manager.Store as ArticleStore;

        [Fact(DisplayName = "CodeBasedEntityManager.AutoGenerateCodeWhenCreatingANewEntityWithEmptyCode")]
        public async void AutoGenerateCodeWhenCreatingANewEntityWithEmptyCode()
        {
            var newEntity = new Article { Id = 4 };
            Assert.Null(newEntity.UrlFriendlyTitle);

            var result = await Manager.CreateAsync(newEntity);
            var addedEntity = Store.Data.Single(x => x.Id == newEntity.Id);

            Assert.True(result.Succeeded);
            Assert.NotNull(addedEntity.UrlFriendlyTitle);
        }

        [Fact(DisplayName = "CodeBasedEntityManager.AutoNormalizesCodeWhenSavingAnEntity")]
        public async void AutoNormalizesCodeWhenSavingAnEntity()
        {
            var testCode = "A";
            var newEntity = new Article { Id = 4, UrlFriendlyTitle = testCode };

            var result = await Manager.CreateAsync(newEntity);
            var addedEntity = Store.Data.Single(x => x.Id == newEntity.Id);

            Assert.True(result.Succeeded);
            Assert.Equal(testCode.ToLower(), addedEntity.UrlFriendlyTitle);
        }

        [Fact(DisplayName = "CodeBasedEntityManager.AutoNormalizesCodeButDoesNothingWhenNoCodeNormalizerPresents")]
        public async void AutoNormalizesCodeButDoesNothingWhenNoCodeNormalizerPresents()
        {
            var testCode = "A";
            var newEntity = new Article { Id = 4, UrlFriendlyTitle = testCode };

            var codeNormalizer = Manager.CodeNormalizer;
            Manager.CodeNormalizer = null;
            var result = await Manager.CreateAsync(newEntity);
            Manager.CodeNormalizer = codeNormalizer;
            var addedEntity = Store.Data.Single(x => x.Id == newEntity.Id);

            Assert.True(result.Succeeded);
            Assert.Equal(testCode, addedEntity.UrlFriendlyTitle);
        }

        [Fact(DisplayName = "CodeBasedEntityManager.FindsEntityByCode")]
        public async void FindsEntityByCode()
        {
            var testCode = "title-1";
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await Manager.FindByCodeAsync(null));
            var entity = await Manager.FindByCodeAsync(testCode);
            var expected = Store.Data.Single(x => x.UrlFriendlyTitle == testCode).Id;

            Assert.Equal(expected, entity.Id);
        }

        public class ArticleStore : EntityStoreBase<Article>, ICodeBasedEntityStore<Article>
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

            public Article FindByCode(string normalizedCode) => throw new NotImplementedException();

            public Task<Article> FindByCodeAsync(string normalizedCode, CancellationToken cancellationToken)
            {
                var entities = Data.AsQueryable();

                var result = entities.SingleOrDefault(x => x.UrlFriendlyTitle == normalizedCode);
                return Task.FromResult(result);
            }
        }

        public class ArticleAccessor : ICodeBasedEntityAccessor<Article>
        {
            public object GetId(Article entity) => entity.Id;

            public string GetCode(Article entity) => entity.UrlFriendlyTitle;

            public void SetCode(Article entity, string code) => entity.UrlFriendlyTitle = code;
        }

        public class ArticleCodeGenerator : IEntityCodeGenerator<Article>
        {
            public string GenerateCode(object manager, Article entity) => Guid.NewGuid().ToString();
        }

        public class ArticleManager : ManagerBase<Article>, IEntityManager<Article>, ICodeBasedEntityManager<Article>
        {
            public ArticleManager()
                : base(
                      store: new ArticleStore(),
                      entityAccessor: new ArticleAccessor(),
                      entityValidators: null,
                      logger: new Mock<ILogger<ArticleManager>>().Object)
            {
                CodeNormalizer = new LowerInvariantLookupNormalizer();
                CodeGenerator = new ArticleCodeGenerator();
            }

            public IEntityStore<Article> EntityStore => Store as IEntityStore<Article>;

            public IEntityAccessor<Article> EntityAccessor => throw new NotImplementedException();

            public ICodeBasedEntityStore<Article> CodeBasedEntityStore => Store as ICodeBasedEntityStore<Article>;

            public ICodeBasedEntityAccessor<Article> CodeBasedEntityAccessor => Accessor as ICodeBasedEntityAccessor<Article>;

            public ILookupNormalizer CodeNormalizer { get; internal set; }

            public IEntityCodeGenerator<Article> CodeGenerator { get; }
        }
    }
}
