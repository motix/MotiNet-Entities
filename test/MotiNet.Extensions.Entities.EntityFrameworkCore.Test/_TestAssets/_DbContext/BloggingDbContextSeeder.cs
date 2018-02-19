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
                dbContext.Articles.AddRange(GetPreconfiguredArticles());
                dbContext.SaveChanges();
            }

            if (!dbContext.ArticleCategories.Any())
            {
                dbContext.ArticleCategories.AddRange(GetPreconfiguredArticleCategories());
                dbContext.SaveChanges();
            }
        }

        static IEnumerable<Category> GetPreconfiguredCategories()
        {
            return new List<Category>()
            {
                new Category() { Name = "Life"          , UrlFriendlyName = "life" },
                new Category() { Name = "Tech"          , UrlFriendlyName = "tech" },
                new Category() { Name = "Family"        , UrlFriendlyName = "family"},
                new Category() { Name = "Entertainment" , UrlFriendlyName = "entertainment" },
                new Category() { Name = "Politics"      , UrlFriendlyName = "politics" },
                new Category() { Name = "Universe"      , UrlFriendlyName = "universe" }
            };
        }

        static IEnumerable<Author> GetPreconfiguredAuthors()
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

        static IEnumerable<Article> GetPreconfiguredArticles()
        {
            return new List<Article>()
            {
                new Article() {
                    Title = "Di del gli la tale d'esse ingannati noi al ignoranza",
                    UrlFriendlyTitle = "di-del-gli-la-tale-d'esse-ingannati-noi-al-ignoranza",
                    Content = "Da impermutabile e per in d'angoscia lui alla di principio impermutabile di ma sí da essilio dio eterni maesta liberalita",
                    AuthorId = 1
                },
                new Article() {
                    Title = "Oppinione principio quel carissime la avvien di alcun senza quegli",
                    UrlFriendlyTitle = "oppinione-principio-quel-carissime-la-avvien-di-alcun-senza-quegli",
                    Content = "Discenda mortali durare delle di uomini come come i intendo se e non forza novellare ammirabile e impermutabile che ma",
                    AuthorId = 2
                },
                new Article() {
                    Title = "Informati essilio mortali che il io da facitore fu nostri",
                    UrlFriendlyTitle = "informati-essilio-mortali-che-il-io-da-facitore-fu-nostri",
                    Content = "Quel l'acume dalla per impetrata di una cospetto dare viviamo audaci beato di speranza sono sé che riguardando potendo la",
                    AuthorId = 3
                },
                new Article() {
                    Title = "Del procuratore 'l che piaceri durare come intendo mortali che",
                    UrlFriendlyTitle = "del-procuratore-l-che-piaceri-durare-come-intendo-mortali-che",
                    Content = "Fosse ciascheduna nostri dea cospetto divina di noia di uomini sua sé di impermutabile esse ma divina eterni quella cosa",
                    AuthorId = 4
                },
            };
        }

        static IEnumerable<ArticleCategory> GetPreconfiguredArticleCategories()
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
