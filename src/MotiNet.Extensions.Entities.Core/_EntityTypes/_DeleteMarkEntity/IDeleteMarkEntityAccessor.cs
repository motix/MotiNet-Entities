namespace MotiNet.Entities
{
    public interface IDeleteMarkEntityAccessor<TEntity>
        where TEntity : class
    {
        void MarkDeleted(TEntity entity);

        void UnmarkDeleted(TEntity entity);
    }
}
