using System.Collections.Generic;

namespace MotiNet.Entities.EntityFrameworkCore.Test
{
    public partial class Article
    {
        public int Id { get; set; }

        public int Priority { get; set; }

        public string Title { get; set; }

        public string UrlFriendlyTitle { get; set; }

        public string Content { get; set; }

        public int AuthorId { get; set; }
    }

    partial class Article
    {
        public Author Author { get; set; }

        public ICollection<Category> Categories { get; set; }
    }
}
