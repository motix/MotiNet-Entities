using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
            IEnumerable<IValidator<TEntity, TSubEntity>> entityValidators,
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

            if (this is IChildEntityManager<TEntity, TSubEntity>)
            {
                InitExtensions(ChildEntityManagerExtensions.GetManagerTasks<TEntity, TSubEntity>());
            }
            else if (this is IScopedNameBasedEntityManager<TEntity, TSubEntity>)
            {
                InitExtensions(ScopedNameBasedEntityManagerExtensions.GetManagerTasks<TEntity, TSubEntity>());
            }
            else if (this is IMasterDetailsEntityManager<TEntity, TSubEntity>)
            {
                InitExtensions(MasterDetailsEntityManagerExtensions.GetManagerTasks<TEntity, TSubEntity>());
            }
        }

        #endregion

        #region Properties

        protected IList<EntityCreateValidatingAsync<TEntity, TSubEntity>> EntityWithSubEntityCreateValidatingTasks { get; } = new List<EntityCreateValidatingAsync<TEntity, TSubEntity>>();

        protected IList<EntityUpdateValidatingAsync<TEntity, TSubEntity>> EntityWithSubEntityUpdateValidatingTasks { get; } = new List<EntityUpdateValidatingAsync<TEntity, TSubEntity>>();

        protected IList<EntityValidateAsync<TEntity, TSubEntity>> EntityWithSubEntityValidateTasks { get; } = new List<EntityValidateAsync<TEntity, TSubEntity>>();

        protected IList<EntitySavingAsync<TEntity, TSubEntity>> EntityWithSubEntitySavingTasks { get; } = new List<EntitySavingAsync<TEntity, TSubEntity>>();

        private IList<IValidator<TEntity, TSubEntity>> EntityValidators { get; } = new List<IValidator<TEntity, TSubEntity>>();

        #endregion

        #region Public Operations

        public virtual void InitExtensions(ManagerTasks<TEntity, TSubEntity> tasks)
        {
            base.InitExtensions(tasks);

            if (tasks.EntityWithSubEntityCreateValidatingAsync != null)
            {
                EntityWithSubEntityCreateValidatingTasks.Add(tasks.EntityWithSubEntityCreateValidatingAsync);
            }
            if (tasks.EntityWithSubEntityUpdateValidatingAsync != null)
            {
                EntityWithSubEntityUpdateValidatingTasks.Add(tasks.EntityWithSubEntityUpdateValidatingAsync);
            }
            if (tasks.EntityWithSubEntityValidateAsync != null)
            {
                EntityWithSubEntityValidateTasks.Add(tasks.EntityWithSubEntityValidateAsync);
            }
            if (tasks.EntityWithSubEntitySavingAsync != null)
            {
                EntityWithSubEntitySavingTasks.Add(tasks.EntityWithSubEntitySavingAsync);
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
            await ExecuteEntityWithSubEntityValidateAsync(entity, errors);
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

        public override async Task ExecuteEntityCreateValidatingAsync(TEntity entity)
        {
            await base.ExecuteEntityCreateValidatingAsync(entity);
            await ExecuteEntityWithSubEntityCreateValidatingAsync(entity);
        }

        public virtual async Task ExecuteEntityWithSubEntityCreateValidatingAsync(TEntity entity)
        {
            foreach (var task in EntityWithSubEntityCreateValidatingTasks)
            {
                // Do one by one to ensure the order
                await task(this, new ManagerTaskArgs<TEntity>(entity));
            }
        }

        public override async Task ExecuteEntityUpdateValidatingAsync(TEntity entity, TEntity oldEntity)
        {
            await base.ExecuteEntityUpdateValidatingAsync(entity, oldEntity);
            await ExecuteEntityWithSubEntityUpdateValidatingAsync(entity, oldEntity);
        }

        public virtual async Task ExecuteEntityWithSubEntityUpdateValidatingAsync(TEntity entity, TEntity oldEntity)
        {
            foreach (var task in EntityWithSubEntityUpdateValidatingTasks)
            {
                // Do one by one to ensure the order
                await task(this, new ManagerUpdatingTaskArgs<TEntity>(entity, oldEntity));
            }
        }

        public virtual async Task ExecuteEntityWithSubEntityValidateAsync(TEntity entity, List<GenericError> errors)
        {
            foreach (var task in EntityWithSubEntityValidateTasks)
            {
                // Do one by one to ensure the order
                await task(this, new ValidateEntityTaskArgs<TEntity, TSubEntity>(entity, errors));
            }
        }

        public override async Task ExecuteEntitySavingAsync(TEntity entity)
        {
            await base.ExecuteEntitySavingAsync(entity);
            await ExecuteEntityWithSubEntitySavingAsync(entity);
        }

        public virtual async Task ExecuteEntityWithSubEntitySavingAsync(TEntity entity)
        {
            foreach (var task in EntityWithSubEntitySavingTasks)
            {
                // Do one by one to ensure the order
                await task(this, new ManagerTaskArgs<TEntity>(entity));
            }
        }

        #endregion
    }

    public abstract class ManagerBase<TEntity> : IManager<TEntity>
        where TEntity : class
    {
        #region Constructors

        protected ManagerBase(
            IDisposable store,
            object entityAccessor,
            IEnumerable<IValidator<TEntity>> entityValidators,
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
            if (this is ICodeBasedEntityManager<TEntity>)
            {
                InitExtensions(CodeBasedEntityManagerExtensions.GetManagerTasks<TEntity>());
            }
            if (this is INameBasedEntityManager<TEntity>)
            {
                InitExtensions(NameBasedEntityManagerExtensions.GetManagerTasks<TEntity>());
            }
            if (this is ITaggedEntityManager<TEntity>)
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

        protected IList<EntityGet<TEntity>> EntityGetTasks { get; } = new List<EntityGet<TEntity>>();

        protected IList<EntityGetAsync<TEntity>> EntityGetAsyncTasks { get; } = new List<EntityGetAsync<TEntity>>();

        protected IList<EntityCreateValidatingAsync<TEntity>> EntityCreateValidatingAsyncTasks { get; } = new List<EntityCreateValidatingAsync<TEntity>>();

        protected IList<EntityCreatingAsync<TEntity>> EntityCreatingAsyncTasks { get; } = new List<EntityCreatingAsync<TEntity>>();

        protected IList<EntityUpdateValidatingAsync<TEntity>> EntityUpdateValidatingAsyncTasks { get; } = new List<EntityUpdateValidatingAsync<TEntity>>();

        protected IList<EntityUpdatingAsync<TEntity>> EntityUpdatingAsyncTasks { get; } = new List<EntityUpdatingAsync<TEntity>>();

        protected IList<EntitySavingAsync<TEntity>> EntitySavingAsyncTasks { get; } = new List<EntitySavingAsync<TEntity>>();

        private IList<IValidator<TEntity>> EntityValidators { get; } = new List<IValidator<TEntity>>();

        #endregion

        #region Public Operations
        
        public virtual void InitExtensions(ManagerTasks<TEntity> tasks)
        {
            if (tasks.EntityGet != null)
            {
                EntityGetTasks.Add(tasks.EntityGet);
            }
            if (tasks.EntityGetAsync != null)
            {
                EntityGetAsyncTasks.Add(tasks.EntityGetAsync);
            }
            if (tasks.EntityCreateValidatingAsync != null)
            {
                EntityCreateValidatingAsyncTasks.Add(tasks.EntityCreateValidatingAsync);
            }
            if (tasks.EntityCreatingAsync != null)
            {
                EntityCreatingAsyncTasks.Add(tasks.EntityCreatingAsync);
            }
            if (tasks.EntityUpdateValidatingAsync != null)
            {
                EntityUpdateValidatingAsyncTasks.Add(tasks.EntityUpdateValidatingAsync);
            }
            if (tasks.EntityUpdatingAsync != null)
            {
                EntityUpdatingAsyncTasks.Add(tasks.EntityUpdatingAsync);
            }
            if (tasks.EntitySavingAsync != null)
            {
                EntitySavingAsyncTasks.Add(tasks.EntitySavingAsync);
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

        public virtual void ExecuteEntityGet(TEntity entity)
        {
            foreach (var task in EntityGetTasks)
            {
                task(this, new ManagerTaskArgs<TEntity>(entity));
            }
        }

        public virtual async Task ExecuteEntityGetAsync(TEntity entity)
        {
            foreach(var task in EntityGetAsyncTasks)
            {
                // Do one by one to ensure the order
                await task(this, new ManagerTaskArgs<TEntity>(entity));
            }
        }

        public virtual async Task ExecuteEntityCreateValidatingAsync(TEntity entity)
        {
            foreach(var task in EntityCreateValidatingAsyncTasks)
            {
                // Do one by one to ensure the order
                await task(this, new ManagerTaskArgs<TEntity>(entity));
            }
        }

        public virtual async Task ExecuteEntityCreatingAsync(TEntity entity)
        {
            await ExecuteEntitySavingAsync(entity);

            foreach (var task in EntityCreatingAsyncTasks)
            {
                // Do one by one to ensure the order
                await task(this, new ManagerTaskArgs<TEntity>(entity));
            }
        }

        public virtual async Task ExecuteEntityUpdateValidatingAsync(TEntity entity, TEntity oldEntity)
        {
            foreach (var task in EntityUpdateValidatingAsyncTasks)
            {
                // Do one by one to ensure the order
                await task(this, new ManagerUpdatingTaskArgs<TEntity>(entity, oldEntity));
            }
        }

        public virtual async Task ExecuteEntityUpdatingAsync(TEntity entity, TEntity oldEntity)
        {
            await ExecuteEntitySavingAsync(entity);

            foreach (var task in EntityUpdatingAsyncTasks)
            {
                // Do one by one to ensure the order
                await task(this, new ManagerUpdatingTaskArgs<TEntity>(entity, oldEntity));
            }
        }

        public virtual async Task ExecuteEntitySavingAsync(TEntity entity)
        {
            foreach (var task in EntitySavingAsyncTasks)
            {
                // Do one by one to ensure the order
                await task(this, new ManagerTaskArgs<TEntity>(entity));
            }
        }

        #endregion

        #region IDisposable Support

        private bool _disposed = false; // To detect redundant calls

        [ExcludeFromCodeCoverage]
        public void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        [ExcludeFromCodeCoverage]
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

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~EfRepository() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        [ExcludeFromCodeCoverage]
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        #endregion
    }
}
