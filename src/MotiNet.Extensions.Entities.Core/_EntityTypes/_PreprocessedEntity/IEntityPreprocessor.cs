namespace MotiNet.Entities
{
    public interface IEntityPreprocessor<TEntity>
        where TEntity : class
    {
        bool Disabled { get; set; }

        void PreprocessEntityForGet(IPreprocessedEntityManager<TEntity> manager, TEntity entity);

        void PreprocessEntityForUpdate(IPreprocessedEntityManager<TEntity> manager, TEntity oldEntity, TEntity newEntity);
    }
}
