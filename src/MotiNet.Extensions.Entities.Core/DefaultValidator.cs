using System.Threading.Tasks;

namespace MotiNet.Entities
{
    public class DefaultValidator<TEntity, TEntityManager> : IValidator<TEntity>
        where TEntity : class
        where TEntityManager : class, IManager<TEntity>
    {
        public Task<GenericResult> ValidateAsync(object manager, TEntity entity)
        {
            _ = this.GetManager<TEntity, TEntityManager>(manager);

            return Task.FromResult(GenericResult.Success);
        }
    }
}
