namespace MotiNet.Entities
{
    public interface ITimeTrackedEntityManager<TEntity> : IManager<TEntity>
        where TEntity : class
    {
        ITimeTrackedEntityStore<TEntity> TimeTrackedEntityStore { get; }

        ITimeTrackedEntityAccessor<TEntity> TimeTrackedEntityAccessor { get; }
    }
}
