namespace MotiNet.Entities.Test
{
    public partial class City
    {
        public int Id { get; set; }

        public int CountryId { get; set; }

        public string Name { get; set; }

        public string NormalizedName { get; set; }
    }

    partial class City
    {
        public Country Country { get; set; }
    }
}
