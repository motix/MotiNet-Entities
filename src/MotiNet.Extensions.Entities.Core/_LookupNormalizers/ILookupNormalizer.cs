namespace MotiNet.Entities
{
    public interface ILookupNormalizer<TMarker> : ILookupNormalizer { }

    public interface ILookupNormalizer
    {
        string Normalize(string key);
    }
}
