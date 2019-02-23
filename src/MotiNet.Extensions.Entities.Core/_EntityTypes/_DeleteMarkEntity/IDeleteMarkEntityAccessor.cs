namespace MotiNet.Entities
{
    public interface IDeleteMarkEntityAccessor<TEntity>
        where TEntity : class
    {
        bool GetDeleteMarked(TEntity entity);

        void MarkDeleted(TEntity entity);

        void UnmarkDeleted(TEntity entity);
    }
}
