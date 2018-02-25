using System.Collections.Generic;

namespace MotiNet.Entities.EntityFrameworkCore.Test
{
    public partial class Country : IIdWiseEntity<int>
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }

    partial class Country
    {
        public ICollection<City> Cities { get; set; }
    }
}
