namespace MotiNet.Entities
{
    public interface INameWiseEntity
    {
        string Name { get; set; }

        string NormalizedName { get; set; }
    }
}
