namespace MotiNet.Entities
{
    public interface IEntityPreprocessor<TEntity>
        where TEntity : class
    {
        bool Enabled { get; set; }

        void PreprocessEntityForGet(TEntity entity);

        void PreprocessEntityForUpdate(TEntity oldEntity, TEntity newEntity);
    }
}
