using System.Collections.Generic;

namespace MotiNet.Entities.Test
{
    public partial class Category : INameWiseEntity
    {
        public int Id { get; set; }

        public int Priority { get; set; }

        public string Name { get; set; }

        public string NormalizedName { get; set; }

        public string UrlFriendlyName { get; set; }

        public int? ParentId { get; set; }
    }

    partial class Category
    {
        public Category Parent { get; set; }

        public ICollection<Article> Articles { get; set; }
    }
}
