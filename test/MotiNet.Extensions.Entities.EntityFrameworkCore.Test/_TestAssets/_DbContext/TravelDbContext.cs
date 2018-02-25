using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;

namespace MotiNet.Entities.EntityFrameworkCore.Test
{
    public class TravelDbContext : DbContext
    {
        public TravelDbContext() { }

        public TravelDbContext(DbContextOptions<TravelDbContext> options) : base(options) { }

        public DbSet<Country> Countries { get; set; }

        public DbSet<City> Cities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Country>(ConfigureCountry);
            modelBuilder.Entity<City>(ConfigureCity);
        }

        private void ConfigureCountry(EntityTypeBuilder<Country> builder)
        {
            builder.Property(x => x.Name)
                   .IsRequired()
                   .HasMaxLength(StringLengths.TitleContent);
        }

        private void ConfigureCity(EntityTypeBuilder<City> builder)
        {
            builder.Property(x => x.Name)
                   .IsRequired()
                   .HasMaxLength(StringLengths.TitleContent);

            builder.Property(x => x.NormalizedName)
                   .IsRequired()
                   .HasMaxLength(StringLengths.TitleContent);
        }
    }
}
