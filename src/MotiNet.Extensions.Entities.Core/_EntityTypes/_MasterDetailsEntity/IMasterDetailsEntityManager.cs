namespace MotiNet.Entities
{
    public interface IMasterDetailsEntityManager<TEntity, TEntityDetail> : IManager<TEntity, TEntityDetail>
        where TEntity : class
        where TEntityDetail : class
    {
        IMasterDetailsEntityAccessor<TEntity, TEntityDetail> MasterDetailsEntityAccessor { get; }
    }
}
