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
    public class NameBasedEntityManager
    {
        private CategoryManager Manager { get; } = new CategoryManager();

        private CategoryStore Store => Manager.Store as CategoryStore;

        [Fact(DisplayName = "NameBasedEntityManager.AutoNormalizesNameWhenSavingAnEntity")]
        public async void AutoNormalizesNameWhenSavingAnEntity()
        {
            var testName = "test name";
            var newEntity = new Category { Id = 4, Name = testName };

            var result = await Manager.CreateAsync(newEntity);
            var addedEntity = Store.Data.Single(x => x.Id == newEntity.Id);

            Assert.True(result.Succeeded);
            Assert.Equal(testName.ToUpper(), addedEntity.NormalizedName);
        }

        [Fact(DisplayName = "NameBasedEntityManager.FindsEntityByName")]
        public async void FindsEntityByName()
        {
            var testName = "Name 1";
            var entity = await Manager.FindByNameAsync(testName);
            var expected = Store.Data.Single(x => x.NormalizedName == testName.ToUpper()).Id;

            Assert.Equal(expected, entity.Id);
        }

        public class CategoryStore : EntityStoreBase<Category>, INameBasedEntityStore<Category>
        {
            internal List<Category> Data { get; } = new List<Category>()
            {
                new Category() { Id = 1, NormalizedName = "NAME 1" },
                new Category() { Id = 2, NormalizedName = "NAME 2" },
                new Category() { Id = 3, NormalizedName = "NAME 3" },
            };

            public override Task<Category> CreateAsync(Category entity, CancellationToken cancellationToken)
            {
                Data.Add(entity);
                return Task.FromResult(entity);
            }

            public Category FindByName(string normalizedName)
            {
                throw new NotImplementedException();
            }

            public Task<Category> FindByNameAsync(string normalizedName, CancellationToken cancellationToken)
            {
                var entities = Data.AsQueryable();

                var result = entities.SingleOrDefault(x => x.NormalizedName == normalizedName);
                return Task.FromResult(result);
            }
        }

        public class CategoryAccessor : INameBasedEntityAccessor<Category>
        {
            public string GetName(Category entity)
            {
                return entity.Name;
            }

            public void SetNormalizedName(Category entity, string normalizedName)
            {
                entity.NormalizedName = normalizedName;
            }
        }

        public class CategoryManager : ManagerBase<Category>, IEntityManager<Category>, INameBasedEntityManager<Category>
        {
            public CategoryManager()
                : base(
                      store: new CategoryStore(),
                      entityAccessor: new CategoryAccessor(),
                      entityValidators: null,
                      logger: new Mock<ILogger<CategoryManager>>().Object)
            {
                NameNormalizer = new UpperInvariantLookupNormalizer();
            }

            public IEntityStore<Category> EntityStore => Store as IEntityStore<Category>;

            public INameBasedEntityStore<Category> NameBasedEntityStore => Store as INameBasedEntityStore<Category>;

            public INameBasedEntityAccessor<Category> NameBasedEntityAccessor => Accessor as INameBasedEntityAccessor<Category>;

            public ILookupNormalizer NameNormalizer { get; }
        }
    }
}
