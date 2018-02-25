using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MotiNet.Entities
{
    public abstract class ManagerBase<TEntity, TSubEntity> : ManagerBase<TEntity>, IManager<TEntity, TSubEntity>
        where TEntity : class
        where TSubEntity : class
    {
        #region Constructors

        protected ManagerBase(
            IDisposable store,
            object entityAccessor,
            IEnumerable<IEntityValidator<TEntity, TSubEntity>> entityValidators,
            ILogger<ManagerBase<TEntity>> logger)
            : base(store, entityAccessor, entityValidators, logger)
        {
            if (entityValidators != null)
            {
                foreach (var validator in entityValidators)
                {
                    EntityValidators.Add(validator);
                }
            }

            if (this is IMasterDetailsEntityManager<TEntity, TSubEntity>)
            {
                InitExtensions(MasterDetailsEntityManagerExtensions.GetManagerTasks<TEntity, TSubEntity>());
            }
        }

        #endregion

        #region Properties

        protected IList<EntityValidateAsync<TEntity, TSubEntity>> EntityWithSubEntitiesValidateTasks { get; } = new List<EntityValidateAsync<TEntity, TSubEntity>>();

        private IList<IEntityValidator<TEntity, TSubEntity>> EntityValidators { get; } = new List<IEntityValidator<TEntity, TSubEntity>>();

        #endregion

        #region Public Operations

        public virtual void InitExtensions(ManagerTasks<TEntity, TSubEntity> tasks)
        {
            base.InitExtensions(tasks);

            if (tasks.EntityWithSubEntitiesValidateAsync != null)
            {
                EntityWithSubEntitiesValidateTasks.Add(tasks.EntityWithSubEntitiesValidateAsync);
            }
        }

        public override async Task<GenericResult> ValidateEntityAsync(TEntity entity)
        {
            var errors = new List<GenericError>();
            foreach (var validator in EntityValidators)
            {
                var result = await validator.ValidateAsync(this, entity);
                if (!result.Succeeded)
                {
                    errors.AddRange(result.Errors);
                }
            }
            await ExecuteEntityWithSubEntitiesValidateAsync(entity, errors);
            if (errors.Count > 0)
            {
                // TODO:: Resource
                Logger.LogWarning(0, "Entity {name} validation failed: {errors}.", typeof(TEntity).Name, string.Join(", ", errors.Select(e => e.Code)));
                return GenericResult.Failed(errors.ToArray());
            }
            return GenericResult.Success;
        }

        public virtual async Task<GenericResult> ValidateSubEntityAsync(TSubEntity subEntity)
        {
            var errors = new List<GenericError>();
            foreach (var validator in EntityValidators)
            {
                var result = await validator.ValidateAsync(this, subEntity);
                if (!result.Succeeded)
                {
                    errors.AddRange(result.Errors);
                }
            }
            if (errors.Count > 0)
            {
                // TODO:: Resource
                Logger.LogWarning(0, "SubEntity {type} validation failed: {errors}.", typeof(TSubEntity).Name, string.Join(", ", errors.Select(e => e.Code)));
                return GenericResult.Failed(errors.ToArray());
            }
            return GenericResult.Success;
        }

        public async Task ExecuteEntityWithSubEntitiesValidateAsync(TEntity entity, List<GenericError> errors)
        {
            foreach (var task in EntityWithSubEntitiesValidateTasks)
            {
                // Do one by one to ensure the order
                await task(this, new ValidateEntityTaskArgs<TEntity, TSubEntity>(entity, EntityValidators, errors));
            }
        }

        #endregion
    }

    public abstract class ManagerBase<TEntity> : IManager<TEntity>, IDisposable
        where TEntity : class
    {
        #region Constructors

        protected ManagerBase(
            IDisposable store,
            object entityAccessor,
            IEnumerable<IEntityValidator<TEntity>> entityValidators,
            ILogger<ManagerBase<TEntity>> logger)
        {
            Store = store ?? throw new ArgumentNullException(nameof(store));
            Accessor = entityAccessor;
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));

            if (entityValidators != null)
            {
                foreach (var validator in entityValidators)
                {
                    EntityValidators.Add(validator);
                }
            }

            if (this is ITimeTrackedEntityManager<TEntity>)
            {
                InitExtensions(TimeTrackedEntityManagerExtensions.GetManagerTasks<TEntity>());
            }
            else if (this is ICodeBasedEntityManager<TEntity>)
            {
                InitExtensions(CodeBasedEntityManagerExtensions.GetManagerTasks<TEntity>());
            }
            else if (this is INameBasedEntityManager<TEntity>)
            {
                InitExtensions(NameBasedEntityManagerExtensions.GetManagerTasks<TEntity>());
            }
            else if (this is ITaggedEntityManager<TEntity>)
            {
                InitExtensions(TaggedEntityManagerExtensions.GetManagerTasks<TEntity>());
            }
        }

        #endregion

        #region Properties

        public IDisposable Store { get; }

        public object Accessor { get; }

        public ILogger Logger { get; }

        public virtual CancellationToken CancellationToken => CancellationToken.None;

        protected IList<EntityValidatingAsync<TEntity>> EntityValidatingTasks { get; } = new List<EntityValidatingAsync<TEntity>>();

        protected IList<EntityCreatingAsync<TEntity>> EntityCreatingTasks { get; } = new List<EntityCreatingAsync<TEntity>>();

        protected IList<EntityUpdatingAsync<TEntity>> EntityUpdatingTasks { get; } = new List<EntityUpdatingAsync<TEntity>>();

        protected IList<EntitySavingAsync<TEntity>> EntitySavingTasks { get; } = new List<EntitySavingAsync<TEntity>>();

        private IList<IEntityValidator<TEntity>> EntityValidators { get; } = new List<IEntityValidator<TEntity>>();

        #endregion

        #region Public Operations
        
        public virtual void InitExtensions(ManagerTasks<TEntity> tasks)
        {
            if (tasks.EntityValidatingAsync != null)
            {
                EntityValidatingTasks.Add(tasks.EntityValidatingAsync);
            }
            if (tasks.EntityCreatingAsync != null)
            {
                EntityCreatingTasks.Add(tasks.EntityCreatingAsync);
            }
            if (tasks.EntityUpdatingAsync != null)
            {
                EntityUpdatingTasks.Add(tasks.EntityUpdatingAsync);
            }
            if (tasks.EntitySavingAsync != null)
            {
                EntitySavingTasks.Add(tasks.EntitySavingAsync);
            }
        }

        public virtual async Task<GenericResult> ValidateEntityAsync(TEntity entity)
        {
            var errors = new List<GenericError>();
            foreach (var validator in EntityValidators)
            {
                var result = await validator.ValidateAsync(this, entity);
                if (!result.Succeeded)
                {
                    errors.AddRange(result.Errors);
                }
            }
            if (errors.Count > 0)
            {
                // TODO:: Resource
                Logger.LogWarning(0, "Entity {name} validation failed: {errors}.", typeof(TEntity).Name, string.Join(", ", errors.Select(e => e.Code)));
                return GenericResult.Failed(errors.ToArray());
            }
            return GenericResult.Success;
        }

        public async Task ExecuteEntityValidatingAsync(TEntity entity)
        {
            foreach(var task in EntityValidatingTasks)
            {
                // Do one by one to ensure the order
                await task(this, new ManagerTaskArgs<TEntity>(entity));
            }
        }

        public async Task ExecuteEntityCreatingAsync(TEntity entity)
        {
            await ExecuteEntitySavingAsync(entity);

            foreach (var task in EntityCreatingTasks)
            {
                // Do one by one to ensure the order
                await task(this, new ManagerTaskArgs<TEntity>(entity));
            }
        }

        public async Task ExecuteEntityUpdatingAsync(TEntity entity)
        {
            await ExecuteEntitySavingAsync(entity);

            foreach (var task in EntityUpdatingTasks)
            {
                // Do one by one to ensure the order
                await task(this, new ManagerTaskArgs<TEntity>(entity));
            }
        }

        public async Task ExecuteEntitySavingAsync(TEntity entity)
        {
            foreach (var task in EntitySavingTasks)
            {
                // Do one by one to ensure the order
                await task(this, new ManagerTaskArgs<TEntity>(entity));
            }
        }

        #endregion

        #region IDisposable Support

        private bool _disposed = false;

        public void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    Store.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}
