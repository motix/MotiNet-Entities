namespace MotiNet.Entities
{
    public interface INameBasedEntityManager<TEntity> : IManager<TEntity>
        where TEntity : class
    {
        INameBasedEntityStore<TEntity> NameBasedEntityStore { get; }

        INameBasedEntityAccessor<TEntity> NameBasedEntityAccessor { get; }

        ILookupNormalizer NameNormalizer { get; set; }
    }
}
