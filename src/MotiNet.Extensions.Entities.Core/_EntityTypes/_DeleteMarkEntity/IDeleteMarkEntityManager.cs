namespace MotiNet.Entities
{
    public interface IDeleteMarkEntityManager<TEntity> : IEntityManager<TEntity>
        where TEntity : class
    {
        IDeleteMarkEntityAccessor<TEntity> DeleteMarkEntityAccessor { get; }
    }
}
