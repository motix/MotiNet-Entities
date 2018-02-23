using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace MotiNet.Entities.EntityFrameworkCore.Test
{
    public static class DbContextHelper
    {
        public static (BloggingDbContext dbContext, DbContextOptions<BloggingDbContext> options) InitBloggingDbContext(DbConnection connection)
        {
            var options = new DbContextOptionsBuilder<BloggingDbContext>()
                .UseSqlite(connection)
                .Options;

            // Create the schema in the database
            using (var dbContext = new BloggingDbContext(options))
            {
                dbContext.Database.EnsureCreated();
                BloggingDbContextSeeder.Seed(dbContext);
            }

            return (new BloggingDbContext(options), options);
        }
    }
}
