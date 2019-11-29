using System;
using System.Collections.Generic;
using System.Linq;

namespace MotiNet.Entities.EntityFrameworkCore.Test
{
    public static class BloggingDbContextSeeder
    {
        public static void Seed(BloggingDbContext dbContext)
        {
            if (!dbContext.Categories.Any())
            {
                dbContext.Categories.AddRange(GetPreconfiguredCategories());
                dbContext.SaveChanges();
                dbContext.Categories.Find(1).ParentId = 6;
                dbContext.SaveChanges();
            }

            if (!dbContext.Authors.Any())
            {
                dbContext.Authors.AddRange(GetPreconfiguredAuthors());
                dbContext.SaveChanges();
            }

            if (!dbContext.Articles.Any())
            {
                var articles = GetPreconfiguredArticles();
                articles = articles.Reverse();
                dbContext.Articles.AddRange(articles);
                dbContext.SaveChanges();
            }

            if (!dbContext.ArticleCategories.Any())
            {
                dbContext.ArticleCategories.AddRange(GetPreconfiguredArticleCategories());
                dbContext.SaveChanges();
            }
        }

        private static IEnumerable<Category> GetPreconfiguredCategories()
        {
            return new List<Category>()
            {
                new Category() { Name = "Life"          , NormalizedName = "LIFE",          UrlFriendlyName = "life" },
                new Category() { Name = "Tech"          , NormalizedName = "TECH",          UrlFriendlyName = "tech" },
                new Category() { Name = "Family"        , NormalizedName = "FAMILY",        UrlFriendlyName = "family"},
                new Category() { Name = "Entertainment" , NormalizedName = "ENTERTAINMENT", UrlFriendlyName = "entertainment" },
                new Category() { Name = "Politics"      , NormalizedName = "POLITICS",      UrlFriendlyName = "politics" },
                new Category() { Name = "Universe"      , NormalizedName = "UNIVERSE",      UrlFriendlyName = "universe" }
            };
        }

        private static IEnumerable<Author> GetPreconfiguredAuthors()
        {
            return new List<Author>()
            {
                new Author() { FullName = "Mirt"            , UrlFriendlyFullName = "mirt" },
                new Author() { FullName = "Mood"            , UrlFriendlyFullName = "mood" },
                new Author() { FullName = "Wasase"          , UrlFriendlyFullName = "wasase" },
                new Author() { FullName = "Noi di maesta"   , UrlFriendlyFullName = "noi-di-maesta" },
                new Author() { FullName = "Nome mos"        , UrlFriendlyFullName = "nome-mos" }
            };
        }

        private static IEnumerable<Article> GetPreconfiguredArticles()
        {
            return new List<Article>()
            {
                new Article() {
                    DataCreateDate = DateTime.Now.AddDays(-1),
                    DataLastModifyDate = DateTime.Now,
                    Title = "Di del gli la tale d'esse ingannati noi al ignoranza",
                    UrlFriendlyTitle = "di-del-gli-la-tale-d'esse-ingannati-noi-al-ignoranza",
                    Content = "Da impermutabile e per in d'angoscia lui alla di principio impermutabile di ma sí da essilio dio eterni maesta liberalita",
                    AuthorId = 1
                },
                new Article() {
                    DataCreateDate = DateTime.Now,
                    DataLastModifyDate = DateTime.Now,
                    Title = "Oppinione principio quel carissime la avvien di alcun senza quegli",
                    UrlFriendlyTitle = "oppinione-principio-quel-carissime-la-avvien-di-alcun-senza-quegli",
                    Content = "Discenda mortali durare delle di uomini come come i intendo se e non forza novellare ammirabile e impermutabile che ma",
                    AuthorId = 2
                },
                new Article() {
                    DataCreateDate = DateTime.Now.AddDays(-1),
                    DataLastModifyDate = DateTime.Now,
                    Title = "Informati essilio mortali che il io da facitore fu nostri",
                    UrlFriendlyTitle = "informati-essilio-mortali-che-il-io-da-facitore-fu-nostri",
                    Content = "Quel l'acume dalla per impetrata di una cospetto dare viviamo audaci beato di speranza sono sé che riguardando potendo la",
                    AuthorId = 3
                },
                new Article() {
                    DataCreateDate = DateTime.Now.AddDays(-1),
                    DataLastModifyDate = DateTime.Now,
                    Title = "Del procuratore 'l che piaceri durare come intendo mortali che",
                    UrlFriendlyTitle = "del-procuratore-l-che-piaceri-durare-come-intendo-mortali-che",
                    Content = "Fosse ciascheduna nostri dea cospetto divina di noia di uomini sua sé di impermutabile esse ma divina eterni quella cosa",
                    AuthorId = 4
                },
            };
        }

        private static IEnumerable<ArticleCategory> GetPreconfiguredArticleCategories()
        {
            return new List<ArticleCategory>()
            {
                new ArticleCategory() { ArticleId = 1 , CategoryId = 1 },
                new ArticleCategory() { ArticleId = 2 , CategoryId = 1 },
                new ArticleCategory() { ArticleId = 3 , CategoryId = 1 },
                new ArticleCategory() { ArticleId = 2 , CategoryId = 2 },
                new ArticleCategory() { ArticleId = 3 , CategoryId = 3 },
                new ArticleCategory() { ArticleId = 4 , CategoryId = 4 },
                new ArticleCategory() { ArticleId = 1 , CategoryId = 5 }
            };
        }
    }
}
