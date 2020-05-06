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
            object accessor,
            IEnumerable<IValidator<TEntity, TSubEntity>> validators,
            ILogger<ManagerBase<TEntity>> logger)
            : base(store, accessor, validators, logger)
        {
            if (validators != null)
            {
                foreach (var validator in validators)
                {
                    EntityValidators.Add(validator);
                }
            }

            if (this is IScopedNameBasedEntityManager<TEntity, TSubEntity>)
            {
                InitExtensions(ScopedNameBasedEntityManagerExtensions<TEntity, TSubEntity>.GetManagerTasks());
            }
            if (this is IChildEntityManager<TEntity, TSubEntity>)
            {
                InitExtensions(ChildEntityManagerExtensions<TEntity, TSubEntity>.GetManagerTasks());
            }
            if (this is IMasterDetailsEntityManager<TEntity, TSubEntity>)
            {
                InitExtensions(MasterDetailsEntityManagerExtensions<TEntity, TSubEntity>.GetManagerTasks());
            }
        }

        #endregion

        #region Properties

        protected IList<EntityCreateValidatingAsync<TEntity, TSubEntity>> EntityWithSubEntityCreateValidatingAsyncTasks { get; } = new List<EntityCreateValidatingAsync<TEntity, TSubEntity>>();

        protected IList<EntityUpdateValidatingAsync<TEntity, TSubEntity>> EntityWithSubEntityUpdateValidatingAsyncTasks { get; } = new List<EntityUpdateValidatingAsync<TEntity, TSubEntity>>();

        protected IList<EntityValidateAsync<TEntity, TSubEntity>> EntityWithSubEntityValidateAsyncTasks { get; } = new List<EntityValidateAsync<TEntity, TSubEntity>>();

        protected IList<EntitySavingAsync<TEntity, TSubEntity>> EntityWithSubEntitySavingAsyncTasks { get; } = new List<EntitySavingAsync<TEntity, TSubEntity>>();

        private IList<IValidator<TEntity, TSubEntity>> EntityValidators { get; } = new List<IValidator<TEntity, TSubEntity>>();

        #endregion

        #region Public Operations

        public virtual void InitExtensions(ManagerTasks<TEntity, TSubEntity> tasks)
        {
            base.InitExtensions(tasks);

            if (tasks.EntityWithSubEntityCreateValidatingAsync != null)
            {
                EntityWithSubEntityCreateValidatingAsyncTasks.Add(tasks.EntityWithSubEntityCreateValidatingAsync);
            }
            if (tasks.EntityWithSubEntityUpdateValidatingAsync != null)
            {
                EntityWithSubEntityUpdateValidatingAsyncTasks.Add(tasks.EntityWithSubEntityUpdateValidatingAsync);
            }
            if (tasks.EntityWithSubEntityValidateAsync != null)
            {
                EntityWithSubEntityValidateAsyncTasks.Add(tasks.EntityWithSubEntityValidateAsync);
            }
            if (tasks.EntityWithSubEntitySavingAsync != null)
            {
                EntityWithSubEntitySavingAsyncTasks.Add(tasks.EntityWithSubEntitySavingAsync);
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
            foreach (var task in EntityWithSubEntityCreateValidatingAsyncTasks)
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
            foreach (var task in EntityWithSubEntityUpdateValidatingAsyncTasks)
            {
                // Do one by one to ensure the order
                await task(this, new ManagerUpdatingTaskArgs<TEntity>(entity, oldEntity));
            }
        }

        public virtual async Task ExecuteEntityWithSubEntityValidateAsync(TEntity entity, List<GenericError> errors)
        {
            foreach (var task in EntityWithSubEntityValidateAsyncTasks)
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
            foreach (var task in EntityWithSubEntitySavingAsyncTasks)
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
            object accessor,
            IEnumerable<IValidator<TEntity>> validators,
            ILogger<ManagerBase<TEntity>> logger)
        {
            Store = store ?? throw new ArgumentNullException(nameof(store));
            Accessor = accessor;
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));

            if (validators != null)
            {
                foreach (var validator in validators)
                {
                    EntityValidators.Add(validator);
                }
            }

            if (this is IReadableIdEntityManager<TEntity>)
            {
                InitExtensions(ReadableIdEntityManagerExtensions<TEntity>.GetManagerTasks());
            }
            if (this is ITimeTrackedEntityManager<TEntity>)
            {
                InitExtensions(TimeTrackedEntityManagerExtensions<TEntity>.GetManagerTasks());
            }
            if (this is ICodeBasedEntityManager<TEntity>)
            {
                InitExtensions(CodeBasedEntityManagerExtensions<TEntity>.GetManagerTasks());
            }
            if (this is INameBasedEntityManager<TEntity>)
            {
                InitExtensions(NameBasedEntityManagerExtensions<TEntity>.GetManagerTasks());
            }
            if (this is ITaggedEntityManager<TEntity>)
            {
                InitExtensions(TaggedEntityManagerExtensions<TEntity>.GetManagerTasks());
            }
            if (this is IPreprocessedEntityManager<TEntity>)
            {
                InitExtensions(PreprocessedEntityManagerExtensions<TEntity>.GetManagerTasks());
            }
            if (this is IInterModuleEntityManager<TEntity>)
            {
                InitExtensions(InterModuleEntityManagerExtensions<TEntity>.GetManagerTasks());
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

        protected IList<EntityCreatedAsync<TEntity>> EntityCreatedAsyncTasks { get; } = new List<EntityCreatedAsync<TEntity>>();

        protected IList<EntityUpdateValidatingAsync<TEntity>> EntityUpdateValidatingAsyncTasks { get; } = new List<EntityUpdateValidatingAsync<TEntity>>();

        protected IList<EntityUpdatingAsync<TEntity>> EntityUpdatingAsyncTasks { get; } = new List<EntityUpdatingAsync<TEntity>>();

        protected IList<EntityUpdatedAsync<TEntity>> EntityUpdatedAsyncTasks { get; } = new List<EntityUpdatedAsync<TEntity>>();

        protected IList<EntitySavingAsync<TEntity>> EntitySavingAsyncTasks { get; } = new List<EntitySavingAsync<TEntity>>();

        protected IList<EntityDeletedAsync<TEntity>> EntityDeletedAsyncTasks { get; } = new List<EntityDeletedAsync<TEntity>>();

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
            if (tasks.EntityCreatedAsync != null)
            {
                EntityCreatedAsyncTasks.Add(tasks.EntityCreatedAsync);
            }
            if (tasks.EntityUpdateValidatingAsync != null)
            {
                EntityUpdateValidatingAsyncTasks.Add(tasks.EntityUpdateValidatingAsync);
            }
            if (tasks.EntityUpdatingAsync != null)
            {
                EntityUpdatingAsyncTasks.Add(tasks.EntityUpdatingAsync);
            }
            if (tasks.EntityUpdatedAsync != null)
            {
                EntityUpdatedAsyncTasks.Add(tasks.EntityUpdatedAsync);
            }
            if (tasks.EntitySavingAsync != null)
            {
                EntitySavingAsyncTasks.Add(tasks.EntitySavingAsync);
            }
            if (tasks.EntityDeletedAsync != null)
            {
                EntityDeletedAsyncTasks.Add(tasks.EntityDeletedAsync);
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

        public virtual void ExecuteEntitiesGet(IEnumerable<TEntity> entities)
        {
            foreach (var task in EntityGetTasks)
            {
                foreach (var entity in entities)
                {
                    task(this, new ManagerTaskArgs<TEntity>(entity));
                }
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

        public virtual async Task ExecuteEntitiesGetAsync(IEnumerable<TEntity> entities)
        {
            foreach (var task in EntityGetAsyncTasks)
            {
                foreach (var entity in entities)
                {
                    // Do one by one to ensure the order
                    await task(this, new ManagerTaskArgs<TEntity>(entity));
                }
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

        public virtual async Task ExecuteEntityCreatedAsync(TEntity entity)
        {
            foreach (var task in EntityCreatedAsyncTasks)
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

        public virtual async Task ExecuteEntityUpdatedAsync(TEntity entity)
        {
            foreach (var task in EntityUpdatedAsyncTasks)
            {
                // Do one by one to ensure the order
                await task(this, new ManagerTaskArgs<TEntity>(entity));
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

        public virtual async Task ExecuteEntityDeletedAsync(TEntity entity)
        {
            foreach (var task in EntityDeletedAsyncTasks)
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
