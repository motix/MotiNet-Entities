using System;
using System.Threading.Tasks;

namespace MotiNet.Entities
{
    public delegate void EntityPreparingForValidationEventHandler<TEntity>(object sender, ManagerEventArgs<TEntity> e) where TEntity : class;

    public delegate void EntityPreparingForCreatingEventHandler<TEntity>(object sender, ManagerEventArgs<TEntity> e) where TEntity : class;

    public delegate void EntityPreparingForUpdatingEventHandler<TEntity>(object sender, ManagerEventArgs<TEntity> e) where TEntity : class;

    public delegate void EntityPreparingForSavingEventHandler<TEntity>(object sender, ManagerEventArgs<TEntity> e) where TEntity : class;

    public class ManagerEventHandlers<TEntity> where TEntity : class
    {
        public EntityPreparingForValidationEventHandler<TEntity> EntityPreparingForValidation { get; set; }

        public EntityPreparingForCreatingEventHandler<TEntity> EntityPreparingForCreating { get; set; }

        public EntityPreparingForUpdatingEventHandler<TEntity> EntityPreparingForUpdating { get; set; }

        public EntityPreparingForSavingEventHandler<TEntity> EntityPreparingForSaving { get; set; }
        public Func<IManager<object>, ManagerTaskArgs<object>, Task> EntitySavingAsync { get; internal set; }
    }
}
