namespace MotiNet.Entities
{
    public interface IGroupedEntityManager<TEntity, TEntityGroup> : IManager<TEntity, TEntityGroup>
        where TEntity : class
        where TEntityGroup : class
    {
        IGroupedEntityStore<TEntity, TEntityGroup> GroupedEntityStore { get; }
    }
}
