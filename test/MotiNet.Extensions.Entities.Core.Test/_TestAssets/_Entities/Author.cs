using System.Collections.Generic;

namespace MotiNet.Entities.Test
{
    public partial class Author : ICodeWiseEntity
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string FullName { get; set; }

        public string UrlFriendlyFullName { get; set; }
    }

    partial class Author
    {
        public ICollection<Article> Articles { get; set; }
    }
}
