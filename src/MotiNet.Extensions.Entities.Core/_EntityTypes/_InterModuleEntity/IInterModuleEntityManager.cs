namespace MotiNet.Entities
{
    public interface IInterModuleEntityManager<TEntity> : IManager<TEntity>
        where TEntity : class
    {
        IEntityAdapter<TEntity> EntityAdapter { get; }
    }
}
