namespace MotiNet.Entities.EntityFrameworkCore.Test
{
    public class ArticleCategory
    {
        public int ArticleId { get; set; }
        public int CategoryId { get; set; }
        public Article Article { get; set; }
        public Category Category { get; set; }
    }
}
