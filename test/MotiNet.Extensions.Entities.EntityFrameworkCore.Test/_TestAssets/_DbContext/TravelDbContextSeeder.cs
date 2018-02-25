using System.Collections.Generic;
using System.Linq;

namespace MotiNet.Entities.EntityFrameworkCore.Test
{
    public static class TravelDbContextSeeder
    {
        public static void Seed(TravelDbContext dbContext)
        {
            if (!dbContext.Countries.Any())
            {
                dbContext.Countries.AddRange(GetPreconfiguredCountries());
                dbContext.SaveChanges();
            }

            if (!dbContext.Cities.Any())
            {
                dbContext.Cities.AddRange(GetPreconfiguredCities());
                dbContext.SaveChanges();
            }
        }

        private static IEnumerable<Country> GetPreconfiguredCountries()
        {
            return new List<Country>()
            {
                new Country() { Id = 1, Name = "Country 1" },
                new Country() { Id = 2, Name = "Country 2" }
            };
        }

        private static IEnumerable<City> GetPreconfiguredCities()
        {
            return new List<City>()
            {
                new City() { Id = 1, Name = "City 1", NormalizedName = "CITY 1", CountryId = 1 },
                new City() { Id = 2, Name = "City 2", NormalizedName = "CITY 2", CountryId = 1 },
                new City() { Id = 3, Name = "City 3", NormalizedName = "CITY 3", CountryId = 1 },
                new City() { Id = 4, Name = "City 1", NormalizedName = "CITY 1", CountryId = 2 },
                new City() { Id = 5, Name = "City 2", NormalizedName = "CITY 2", CountryId = 2 },
            };
        }
    }
}
