namespace MotiNet.Entities
{
    public interface IScopedNameBasedEntityManager<TEntity, TEntityScope> : IManager<TEntity, TEntityScope>
        where TEntity : class
        where TEntityScope : class
    {
        IScopedNameBasedEntityStore<TEntity, TEntityScope> ScopedNameBasedEntityStore { get; }

        IScopedNameBasedEntityAccessor<TEntity, TEntityScope> ScopedNameBasedEntityAccessor { get; }

        ILookupNormalizer NameNormalizer { get; }
    }
}
