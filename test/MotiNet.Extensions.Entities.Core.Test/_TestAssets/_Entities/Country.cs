using System.Collections.Generic;

namespace MotiNet.Entities.Test
{
    public partial class Country
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string NormalizedName { get; set; }
    }

    partial class Country
    {
        public ICollection<City> Cities { get; set; }
    }
}
