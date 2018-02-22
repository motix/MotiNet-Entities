using System.Collections.Generic;

namespace MotiNet.Extensions.Entities.Core.Test
{
    public partial class Author
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public string UrlFriendlyFullName { get; set; }
    }

    partial class Author
    {
        public ICollection<Article> Articles { get; set; }
    }
}
