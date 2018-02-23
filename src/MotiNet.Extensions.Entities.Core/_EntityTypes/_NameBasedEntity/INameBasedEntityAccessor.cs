namespace MotiNet.Entities
{
    public interface INameBasedEntityAccessor<TEntity>
        where TEntity : class
    {
        string GetName(TEntity entity);

        void SetNormalizedName(TEntity entity, string normalizedName);
    }
}
