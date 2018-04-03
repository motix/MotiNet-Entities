namespace MotiNet.Entities
{
    public interface IEntityManager<TEntity> : IManager<TEntity>
        where TEntity : class
    {
        IEntityStore<TEntity> EntityStore { get; }

        IEntityAccessor<TEntity> EntityAccessor { get; }
    }
}
