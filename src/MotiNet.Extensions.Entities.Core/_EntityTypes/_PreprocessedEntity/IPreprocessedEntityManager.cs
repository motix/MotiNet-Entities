namespace MotiNet.Entities
{
    public interface IPreprocessedEntityManager<TEntity> : IManager<TEntity>
        where TEntity : class
    {
        IEntityPreprocessor<TEntity> EntityPreprocessor { get; }
    }
}
