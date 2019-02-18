namespace MotiNet.Entities
{
    public interface IReadableIdEntityAccessor<TEntity>
        where TEntity : class
    {
        object GetIdSource(TEntity entity);

        void SetId(TEntity entity, string id);
    }
}
