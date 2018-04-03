namespace MotiNet.Entities
{
    public interface INameBasedEntityAccessor<TEntity>
        where TEntity : class
    {
        object GetId(TEntity entity);

        string GetName(TEntity entity);

        void SetNormalizedName(TEntity entity, string normalizedName);
    }
}
