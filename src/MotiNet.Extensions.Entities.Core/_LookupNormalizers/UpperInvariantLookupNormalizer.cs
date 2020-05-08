namespace MotiNet.Entities
{
    public class UpperInvariantLookupNormalizer : ILookupNormalizer
    {
        public virtual string Normalize(string key)
        {
            return key?.Normalize().ToUpperInvariant();
        }
    }
}
