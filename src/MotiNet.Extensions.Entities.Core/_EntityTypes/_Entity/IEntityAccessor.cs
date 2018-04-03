namespace MotiNet.Entities
{
    public interface IEntityAccessor<TEntity>
        where TEntity : class
    {
        object GetId(TEntity entity);
    }
}
