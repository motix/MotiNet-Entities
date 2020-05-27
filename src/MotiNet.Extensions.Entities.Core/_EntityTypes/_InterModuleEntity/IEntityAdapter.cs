using System.Threading.Tasks;

namespace MotiNet.Entities
{
    public interface IEntityAdapter<TEntity>
        where TEntity : class
    {
        Task OnCreatedAsync(IInterModuleEntityManager<TEntity> manager, TEntity entity);

        Task OnUpdatedAsync(IInterModuleEntityManager<TEntity> manager, TEntity entity);

        Task OnDeletedAsync(IInterModuleEntityManager<TEntity> manager, TEntity entity);
    }
}
