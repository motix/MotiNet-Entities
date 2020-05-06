using System.Threading.Tasks;

namespace MotiNet.Entities
{
    public interface IEntityAdapter<TEntity>
        where TEntity : class
    {
        Task OnCreatedAsync(IManager<TEntity> manager, ManagerTaskArgs<TEntity> taskArgs);

        Task OnUpdatedAsync(IManager<TEntity> manager, ManagerTaskArgs<TEntity> taskArgs);

        Task OnDeletedAsync(IManager<TEntity> manager, ManagerTaskArgs<TEntity> taskArgs);
    }
}
