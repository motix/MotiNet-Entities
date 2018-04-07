namespace MotiNet.Entities
{
    public interface IScopedNameBasedEntityAccessor<TEntity, TEntityScope>
        where TEntity : class
        where TEntityScope : class
    {
        object GetId(TEntity entity);

        string GetName(TEntity entity);

        void SetNormalizedName(TEntity entity, string normalizedName);

        object GetScopeId(TEntity entity);

        void SetScopeId(TEntity entity, object scopeId);

        TEntityScope GetScope(TEntity entity);

        void SetScope(TEntity entity, TEntityScope scope);
    }
}
