namespace MotiNet.Entities
{
    public interface ICodeBasedEntityManager<TEntity> : IManager<TEntity>
        where TEntity : class
    {
        ICodeBasedEntityStore<TEntity> CodeBasedEntityStore { get; }

        ICodeBasedEntityAccessor<TEntity> CodeBasedEntityAccessor { get; }

        ILookupNormalizer CodeNormalizer { get; set; }

        IEntityCodeGenerator<TEntity> CodeGenerator { get; set; }
    }
}
