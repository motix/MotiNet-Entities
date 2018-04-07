using Microsoft.Data.Sqlite;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MotiNet.Entities.EntityFrameworkCore.Test
{
    public class ScopedNameBasedEntityStore
    {
        [Fact(DisplayName = "ScopedNameBasedEntityStore.FindsEntityByName")]
        public async Task FindsEntityByName()
        {
            var testName = "City 2";
            var testCountryId = 1;

            // In-memory database only exists while the connection is open
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            try
            {
                // Run the test against one instance of the context
                using (var dbContext = DbContextHelper.InitTravelDbContext(connection).dbContext)
                {
                    var store = new CityStore(dbContext);
                    var entity = await store.FindByNameAsync(testName.ToUpper(), new Country { Id = testCountryId }, CancellationToken.None);
                    var expected = dbContext.Cities.Single(x => x.CountryId == testCountryId && x.NormalizedName == testName.ToUpper()).Id;

                    Assert.Equal(expected, entity.Id);
                }
            }
            finally
            {
                connection.Close();
            }
        }

        [Fact(DisplayName = "ScopedNameBasedEntityStore.FindsScopeById")]
        public async Task FindsScopeById()
        {
            var testId = 1;

            // In-memory database only exists while the connection is open
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            try
            {
                // Run the test against one instance of the context
                using (var dbContext = DbContextHelper.InitTravelDbContext(connection).dbContext)
                {
                    var store = new CityStore(dbContext);
                    var entity = await store.FindScopeByIdAsync(testId, CancellationToken.None);

                    Assert.Equal(testId, entity.Id);
                }
            }
            finally
            {
                connection.Close();
            }
        }

        public class CityStore : StoreBase<City, TravelDbContext>, IScopedNameBasedEntityStoreMarker<City, Country, TravelDbContext>
        {
            public CityStore(TravelDbContext dbContext) : base(dbContext) { }

            public City FindByName(string normalizedName, Country scope) => throw new NotImplementedException();

            public Task<City> FindByNameAsync(string normalizedName, Country scope, CancellationToken cancellationToken)
                => ScopedNameBasedEntityStoreHelper.FindEntityByNameAsync(this, normalizedName, scope, x => x.CountryId, cancellationToken);

            public Country FindScopeById(object id) => throw new NotImplementedException();

            public Task<Country> FindScopeByIdAsync(object id, CancellationToken cancellationToken)
                => ScopedNameBasedEntityStoreHelper.FindScopeByIdAsync(this, id, cancellationToken);
        }
    }
}
