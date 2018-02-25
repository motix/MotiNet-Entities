namespace MotiNet.Entities
{
    public interface IScopedNameBasedEntityAccessor<TEntity, TEntityScope>
        where TEntity : class
        where TEntityScope : class
    {
        string GetName(TEntity entity);

        void SetNormalizedName(TEntity entity, string normalizedName);

        object GetScopeId(TEntity entity);

        void SetScope(TEntity entity, TEntityScope scope);
    }
}
