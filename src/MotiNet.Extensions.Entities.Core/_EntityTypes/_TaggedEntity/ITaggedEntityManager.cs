namespace MotiNet.Entities
{
    public interface ITaggedEntityManager<TEntity> : IManager<TEntity>
        where TEntity : class
    {
        ITagProcessor TagProcessor { get; }

        ITaggedEntityAccessor<TEntity> TaggedEntityAccessor { get; }
    }
}
