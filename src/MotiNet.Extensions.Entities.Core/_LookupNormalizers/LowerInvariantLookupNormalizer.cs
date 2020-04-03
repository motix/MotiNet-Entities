namespace MotiNet.Entities
{
    public class LowerInvariantLookupNormalizer : ILookupNormalizer
    {
        public virtual string Normalize(string key)
        {
            return key?.Normalize().ToLowerInvariant();
        }
    }
}
