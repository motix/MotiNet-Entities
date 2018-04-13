namespace MotiNet.Entities
{
    public interface IChildEntityManager<TEntity, TEntityParent> : IManager<TEntity, TEntityParent>
        where TEntity : class
        where TEntityParent : class
    {
        IChildEntityStore<TEntity, TEntityParent> ChildEntityStore { get; }

        IChildEntityAccessor<TEntity, TEntityParent> ChildEntityAccessor { get; }
    }
}
