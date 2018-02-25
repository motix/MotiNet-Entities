using System.Threading.Tasks;

namespace MotiNet.Entities
{
    public delegate Task EntityValidatingAsync<TEntity>(IManager<TEntity> manager, ManagerTaskArgs<TEntity> taskArgs)
        where TEntity : class;

    public delegate Task EntityCreatingAsync<TEntity>(IManager<TEntity> manager, ManagerTaskArgs<TEntity> taskArgs)
        where TEntity : class;

    public delegate Task EntityUpdatingAsync<TEntity>(IManager<TEntity> manager, ManagerTaskArgs<TEntity> taskArgs)
        where TEntity : class;

    public delegate Task EntitySavingAsync<TEntity>(IManager<TEntity> manager, ManagerTaskArgs<TEntity> taskArgs)
        where TEntity : class;

    public delegate Task EntityValidateAsync<TEntity, TSubEntity>(IManager<TEntity, TSubEntity> manager, ValidateEntityTaskArgs<TEntity, TSubEntity> taskArgs)
        where TEntity : class
        where TSubEntity : class;

    public class ManagerTasks<TEntity, TSubEntity> : ManagerTasks<TEntity>
        where TEntity : class
        where TSubEntity : class
    {
        public EntityValidateAsync<TEntity, TSubEntity> EntityWithSubEntitiesValidateAsync { get; set; }
    }

    public class ManagerTasks<TEntity> where TEntity : class
    {
        public EntityValidatingAsync<TEntity> EntityValidatingAsync { get; set; }

        public EntityCreatingAsync<TEntity> EntityCreatingAsync { get; set; }

        public EntityUpdatingAsync<TEntity> EntityUpdatingAsync { get; set; }

        public EntitySavingAsync<TEntity> EntitySavingAsync { get; set; }
    }
}
