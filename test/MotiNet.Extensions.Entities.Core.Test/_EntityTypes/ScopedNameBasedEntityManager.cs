using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace MotiNet.Entities.Test
{
    public class ScopedNameBasedEntityManager
    {
        private CityManager Manager { get; } = new CityManager();

        private CityStore Store => Manager.Store as CityStore;

        [Fact(DisplayName = "ScopedNameBasedEntityManager.AutoNormalizesNameWhenSavingAnEntity")]
        public async void AutoNormalizesNameWhenSavingAnEntity()
        {
            var testName = "test name";
            var newEntity = new City { Id = 6, Name = testName };

            var result = await Manager.CreateAsync(newEntity);
            var addedEntity = Store.Data.Single(x => x.Id == newEntity.Id);

            Assert.True(result.Succeeded);
            Assert.Equal(testName.ToUpper(), addedEntity.NormalizedName);
        }

        [Fact(DisplayName = "ScopedNameBasedEntityManager.FindsEntityByName")]
        public async void FindsEntityByName()
        {
            var testName = "City 1";
            var testScopeId = 1;
            var entity = await Manager.FindByNameAsync(testName, new Country() { Id = testScopeId });
            var expected = Store.Data.Single(x => x.NormalizedName == testName.ToUpper() && x.CountryId == testScopeId).Id;

            Assert.Equal(expected, entity.Id);
        }

        public class CityStore : EntityStoreBase<City>, IScopedNameBasedEntityStore<City, Country>
        {
            internal List<City> Data { get; } = new List<City>()
            {
                new City() { Id = 1, CountryId = 1, NormalizedName = "CITY 1" },
                new City() { Id = 2, CountryId = 1, NormalizedName = "CITY 2" },
                new City() { Id = 3, CountryId = 1, NormalizedName = "CITY 3" },
                new City() { Id = 4, CountryId = 2, NormalizedName = "CITY 1" },
                new City() { Id = 5, CountryId = 2, NormalizedName = "CITY 2" },
            };

            public override Task<City> CreateAsync(City entity, CancellationToken cancellationToken)
            {
                Data.Add(entity);
                return Task.FromResult(entity);
            }

            public City FindByName(string normalizedName, Country scope) => throw new NotImplementedException();

            public Task<City> FindByNameAsync(string normalizedName, Country scope, CancellationToken cancellationToken)
            {
                var entities = Data.AsQueryable();

                var result = entities.SingleOrDefault(x => x.NormalizedName == normalizedName && x.CountryId == scope.Id);
                return Task.FromResult(result);
            }

            public Country FindScopeById(object id) => throw new NotImplementedException();

            public Task<Country> FindScopeByIdAsync(object id, CancellationToken cancellationToken)
            {
                var result = Data.Select(x => x.CountryId)
                                 .Distinct()
                                 .Select(x => new Country() { Id = x })
                                 .SingleOrDefault(x => x.Id.Equals(id));
                return Task.FromResult(result);
            }
        }

        public class CityAccessor : IScopedNameBasedEntityAccessor<City, Country>
        {
            public object GetId(City entity) => entity.Id;

            public string GetName(City entity) => entity.Name;

            public void SetNormalizedName(City entity, string normalizedName) => entity.NormalizedName = normalizedName;

            public object GetScopeId(City entity) => entity.CountryId;

            public void SetScopeId(City entity, object scopeId) => entity.CountryId = (int)scopeId;

            public Country GetScope(City entity) => entity.Country;

            public void SetScope(City entity, Country scope) => entity.Country = scope;
        }

        public class CityManager : ManagerBase<City, Country>, IEntityManager<City>, IScopedNameBasedEntityManager<City, Country>
        {
            public CityManager()
                : base(
                      store: new CityStore(),
                      entityAccessor: new CityAccessor(),
                      entityValidators: null,
                      logger: new Mock<ILogger<CityManager>>().Object)
            {
                NameNormalizer = new UpperInvariantLookupNormalizer();
            }

            public IEntityStore<City> EntityStore => Store as IEntityStore<City>;

            public IEntityAccessor<City> EntityAccessor => throw new NotImplementedException();

            public IScopedNameBasedEntityStore<City, Country> ScopedNameBasedEntityStore => Store as IScopedNameBasedEntityStore<City, Country>;

            public IScopedNameBasedEntityAccessor<City, Country> ScopedNameBasedEntityAccessor => Accessor as IScopedNameBasedEntityAccessor<City, Country>;

            public ILookupNormalizer NameNormalizer { get; }
        }
    }
}
