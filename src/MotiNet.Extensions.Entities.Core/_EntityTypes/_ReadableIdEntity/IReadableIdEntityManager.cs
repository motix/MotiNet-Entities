namespace MotiNet.Entities
{
    public interface IReadableIdEntityManager<TEntity> : IManager<TEntity>
        where TEntity : class
    {
        IReadableIdEntityAccessor<TEntity> ReadableIdEntityAccessor { get; }
    }
}
