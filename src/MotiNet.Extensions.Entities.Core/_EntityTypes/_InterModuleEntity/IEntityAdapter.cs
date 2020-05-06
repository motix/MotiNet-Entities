namespace MotiNet.Entities
{
    public interface IEntityAdapter<TEntity>
        where TEntity : class
    {
        EntityCreatedAsync<TEntity> CreatedAsync { get; set; }
    
        EntityUpdatedAsync<TEntity> UpdatedAsync { get; set; }
    
        EntityDeletedAsync<TEntity> DeletedAsync { get; set; }
    }
}
