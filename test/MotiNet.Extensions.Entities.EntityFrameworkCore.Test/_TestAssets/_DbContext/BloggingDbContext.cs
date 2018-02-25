using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;

namespace MotiNet.Entities.EntityFrameworkCore.Test
{
    public class BloggingDbContext : DbContext
    {
        public BloggingDbContext() { }

        public BloggingDbContext(DbContextOptions<BloggingDbContext> options) : base(options) { }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Author> Authors { get; set; }

        public DbSet<Article> Articles { get; set; }

        public DbSet<ArticleCategory> ArticleCategories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(ConfigureCategory);
            modelBuilder.Entity<Author>(ConfigureAuthor);
            modelBuilder.Entity<Article>(ConfigureArticle);

            modelBuilder.Entity<ArticleCategory>().HasKey(x => new { x.ArticleId, x.CategoryId });
        }

        private void ConfigureCategory(EntityTypeBuilder<Category> builder)
        {
            builder.Property(x => x.Name)
                   .IsRequired()
                   .HasMaxLength(StringLengths.TitleContent);

            builder.Property(x => x.NormalizedName)
                   .IsRequired()
                   .HasMaxLength(StringLengths.TitleContent);

            builder.Property(x => x.UrlFriendlyName)
                   .IsRequired()
                   .HasMaxLength(StringLengths.TitleContent);

            builder.Ignore(x => x.Articles);
        }

        private void ConfigureAuthor(EntityTypeBuilder<Author> builder)
        {
            builder.Property(x => x.FullName)
                   .IsRequired()
                   .HasMaxLength(StringLengths.FullName);

            builder.Property(x => x.UrlFriendlyFullName)
                   .IsRequired()
                   .HasMaxLength(StringLengths.FullName);
        }

        private void ConfigureArticle(EntityTypeBuilder<Article> builder)
        {
            builder.Property(x => x.Title)
                   .IsRequired()
                   .HasMaxLength(StringLengths.TitleContent);

            builder.Property(x => x.UrlFriendlyTitle)
                   .IsRequired()
                   .HasMaxLength(StringLengths.TitleContent);

            builder.Property(x => x.Content)
                   .HasMaxLength(StringLengths.MaxContent);

            builder.Property(x => x.Tags)
                   .HasMaxLength(StringLengths.MaxContent);

            builder.Ignore(x => x.Categories);
        }
    }
}
