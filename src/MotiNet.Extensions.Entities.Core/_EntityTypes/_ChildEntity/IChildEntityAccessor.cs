namespace MotiNet.Entities
{
    public interface IChildEntityAccessor<TEntity, TEntityParent>
        where TEntity : class
        where TEntityParent : class
    {
        object GetParentId(TEntity entity);

        void SetParentId(TEntity entity, object parentId);

        void SetParent(TEntity entity, TEntityParent parent);
    }
}
