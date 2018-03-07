using System.Threading.Tasks;

namespace MotiNet.Entities
{
    public interface IValidator<in TEntity, in TSubEntity>
        : IValidator<TEntity>
        where TEntity : class
        where TSubEntity : class
    {
        Task<GenericResult> ValidateAsync(object manager, TSubEntity subEntity);
    }

    public interface IValidator<in TEntity>
        where TEntity : class
    {
        Task<GenericResult> ValidateAsync(object manager, TEntity entity);
    }
}
