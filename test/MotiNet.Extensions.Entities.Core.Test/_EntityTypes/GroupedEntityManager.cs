using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MotiNet.Entities.Test
{
    public class GroupedEntityManager
    {
        private CityManager Manager { get; } = new CityManager();

        private CityStore Store => Manager.Store as CityStore;

        [Fact(DisplayName = "GroupedEntityManager.GetsAllEntities")]
        public async Task GetsAllGroups()
        {
            var groups = await Manager.AllGroupsAsync();
            var expected = Store.Data.Count;

            Assert.Equal(expected, groups.Count());
        }

        [Fact(DisplayName = "GroupedEntityManager.GetsAllNonEmptyGroups")]
        public async Task GetsAllNonEmptyGroups()
        {
            var groups = await Manager.AllNonEmptyGroupsAsync();
            var expected = Store.Data.Where(x => x.Cities != null && x.Cities.Count > 0).Count();

            Assert.Equal(expected, groups.Count());
        }

        public class CityStore : EntityStoreBase<City>, IGroupedEntityStore<City, Country>
        {
            internal List<Country> Data { get; } = new List<Country>()
            {
                new Country() { Id = 1, Name = "Country 1", Cities = new List<City>() { new City() { Id = 1, CountryId = 1 }, new City() { Id = 2, CountryId = 1 } } },
                new Country() { Id = 2, Name = "Country 2", Cities = new List<City>() { new City() { Id = 3, CountryId = 2 }, new City() { Id = 4, CountryId = 2 } } },
                new Country() { Id = 3, Name = "Country 3" }
            };

            public Task<IEnumerable<Country>> AllGroupsAsync(CancellationToken cancellationToken)
            {
                var result = Data.AsEnumerable();
                return Task.FromResult(result);
            }

            public Task<IEnumerable<Country>> AllNonEmptyGroupsAsync(CancellationToken cancellationToken)
            {
                var result = Data.Where(x => x.Cities != null && x.Cities.Count > 0).AsEnumerable();
                return Task.FromResult(result);
            }
        }

        public class CityManager : ManagerBase<City, Country>, IGroupedEntityManager<City, Country>
        {
            public CityManager()
                : base(
                      store: new CityStore(),
                      accessor: null,
                      validators: null,
                      logger: new Mock<ILogger<CityManager>>().Object)
            { }

            public IGroupedEntityStore<City, Country> GroupedEntityStore => Store as IGroupedEntityStore<City, Country>;
        }
    }
}
