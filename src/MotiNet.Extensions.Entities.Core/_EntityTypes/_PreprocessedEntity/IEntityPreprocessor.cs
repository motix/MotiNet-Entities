namespace MotiNet.Entities
{
    public interface IEntityPreprocessor<TEntity>
        where TEntity : class
    {
        void PreprocessEntityForGet(TEntity entity);

        void PreprocessEntityForUpdate(TEntity oldEntity, TEntity newEntity);
    }
}
