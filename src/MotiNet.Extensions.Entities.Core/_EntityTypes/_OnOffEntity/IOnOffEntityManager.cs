namespace MotiNet.Entities
{
    public interface IOnOffEntityManager<TEntity> : IEntityManager<TEntity>
        where TEntity : class
    {
        IOnOffEntityStore<TEntity> OnOffEntityStore { get; }
    }
}
