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
    public class MasterDetailsEntityManager
    {
        private CountryManager Manager { get; } = new CountryManager();

        private CountryStore Store => Manager.Store as CountryStore;

        [Fact(DisplayName = "MasterDetailsEntityManager.ValidatesDetailsWhenCreating")]
        public async void ValidatesDetailsWhenCreating()
        {
            var newEntity = new Country { Id = 4, Name = "Country 4", Cities = new List<City>() { new City() { Id = 1 } } };

            var initialCount = Store.Data.Count;
            var result = await Manager.CreateAsync(newEntity);
            var finalCount = Store.Data.Count;

            Assert.False(result.Succeeded);
            Assert.Equal(finalCount, initialCount);

            newEntity = new Country { Id = 4, Name = "Country 4", Cities = new List<City>() { new City() { Id = 1, Name = "City 1" } } };

            initialCount = Store.Data.Count;
            result = await Manager.CreateAsync(newEntity);
            finalCount = Store.Data.Count;
            var addedEntity = Store.Data.Single(x => x.Id == newEntity.Id);

            Assert.True(result.Succeeded);
            Assert.Equal(finalCount, initialCount + 1);
            Assert.Equal(newEntity.Id, addedEntity.Id);
        }

        public class CountryValidator : IValidator<Country, City>
        {
            public Task<GenericResult> ValidateAsync(object manager, Country entity)
            {
                var result = string.IsNullOrWhiteSpace(entity.Name) ?
                    GenericResult.Failed(new GenericError()) :
                    GenericResult.Success;
                return Task.FromResult(result);
            }

            public Task<GenericResult> ValidateAsync(object manager, City subEntity)
            {
                var result = string.IsNullOrWhiteSpace(subEntity.Name) ?
                    GenericResult.Failed(new GenericError()) :
                    GenericResult.Success;
                return Task.FromResult(result);
            }
        }

        public class CountryStore : EntityStoreBase<Country>
        {
            internal List<Country> Data { get; } = new List<Country>()
            {
                new Country() { Id = 1, Name = "Country 1" },
                new Country() { Id = 2, Name = "Country 2" },
                new Country() { Id = 3, Name = "Country 3" }
            };

            public override Task<Country> CreateAsync(Country entity, CancellationToken cancellationToken)
            {
                Data.Add(entity);
                return Task.FromResult(entity);
            }
        }

        public class CountryAccessor : IMasterDetailsEntityAccessor<Country, City>
        {
            public ICollection<City> GetDetails(Country entity) => entity.Cities;
        }

        public class CountryManager : ManagerBase<Country, City>, IEntityManager<Country>, IMasterDetailsEntityManager<Country, City>
        {
            public CountryManager()
                : base(
                      store: new CountryStore(),
                      entityAccessor: new CountryAccessor(),
                      entityValidators: new List<CountryValidator>() { new CountryValidator() },
                      logger: new Mock<ILogger<CountryManager>>().Object)
            { }

            public IEntityStore<Country> EntityStore => Store as IEntityStore<Country>;

            public IEntityAccessor<Country> EntityAccessor => throw new NotImplementedException();

            public IMasterDetailsEntityAccessor<Country, City> MasterDetailsEntityAccessor => Accessor as IMasterDetailsEntityAccessor<Country, City>;
        }
    }
}
