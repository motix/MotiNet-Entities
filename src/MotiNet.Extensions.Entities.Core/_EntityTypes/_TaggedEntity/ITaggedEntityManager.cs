namespace MotiNet.Entities
{
    public interface ITaggedEntityManager<TEntity> : IManager<TEntity>
        where TEntity : class
    {
        ITagProcessor TagProcessor { get; set; }

        ITaggedEntityAccessor<TEntity> TaggedEntityAccessor { get; }
    }
}
