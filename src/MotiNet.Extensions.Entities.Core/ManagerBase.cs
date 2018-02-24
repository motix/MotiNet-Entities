using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MotiNet.Entities
{
    public abstract class ManagerBase<TEntity, TSubEntity> : ManagerBase<TEntity>
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
        }

        #endregion

        #region Properties

        private IList<IEntityValidator<TEntity, TSubEntity>> EntityValidators { get; }
            = new List<IEntityValidator<TEntity, TSubEntity>>();

        #endregion

        #region Public Operations

        public override async Task<GenericResult> ValidateEntityAsync(TEntity entity)
        {
            var result = await base.ValidateEntityAsync(entity);

            return result;
        }

        #endregion
    }

    public abstract class ManagerBase<TEntity> : IManager<TEntity>, IDisposable
        where TEntity : class
    {
        #region Events

        protected event EntityPreparingForValidationEventHandler<TEntity> EntityPreparingForValidation;

        protected event EntityPreparingForCreatingEventHandler<TEntity> EntityPreparingForCreating;

        protected event EntityPreparingForUpdatingEventHandler<TEntity> EntityPreparingForUpdating;

        protected event EntityPreparingForSavingEventHandler<TEntity> EntityPreparingForSaving;

        #endregion

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
                InitExtensions(TimeTrackedEntityManagerExtensions.GetManagerEventHandlers<TEntity>());
            }
            else if (this is ICodeBasedEntityManager<TEntity>)
            {
                InitExtensions(CodeBasedEntityManagerExtensions.GetManagerEventHandlers<TEntity>());
            }
            else if (this is INameBasedEntityManager<TEntity>)
            {
                InitExtensions(NameBasedEntityManagerExtensions.GetManagerEventHandlers<TEntity>());
            }
            else if (this is ITaggedEntityManager<TEntity>)
            {
                InitExtensions(TaggedEntityManagerExtensions.GetManagerEventHandlers<TEntity>());
            }
        }

        #endregion

        #region Properties

        public IDisposable Store { get; }

        public object Accessor { get; }

        public ILogger Logger { get; }

        public virtual CancellationToken CancellationToken => CancellationToken.None;

        private IList<IEntityValidator<TEntity>> EntityValidators { get; }
            = new List<IEntityValidator<TEntity>>();

        #endregion

        #region Public Operations

        public void InitExtensions(ManagerEventHandlers<TEntity> eventHandlers)
        {
            if (eventHandlers.EntityPreparingForValidation != null)
            {
                EntityPreparingForValidation += eventHandlers.EntityPreparingForValidation;
            }
            if (eventHandlers.EntityPreparingForCreating != null)
            {
                EntityPreparingForCreating += eventHandlers.EntityPreparingForCreating;
            }
            if (eventHandlers.EntityPreparingForUpdating != null)
            {
                EntityPreparingForUpdating += eventHandlers.EntityPreparingForUpdating;
            }
            if (eventHandlers.EntityPreparingForSaving != null)
            {
                EntityPreparingForSaving += eventHandlers.EntityPreparingForSaving;
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
                Logger.LogWarning(0, "Entity validation failed: {errors}.", string.Join(", ", errors.Select(e => e.Code)));
                return GenericResult.Failed(errors.ToArray());
            }
            return GenericResult.Success;

        }

        public void RaiseEntityPreparingForValidation(TEntity entity)
        {
            EntityPreparingForValidation?.Invoke(this, new ManagerEventArgs<TEntity>() { Entity = entity });
        }

        public void RaiseEntityPreparingForCreating(TEntity entity)
        {
            RaiseEntityPreparingForSaving(entity);
            EntityPreparingForCreating?.Invoke(this, new ManagerEventArgs<TEntity>() { Entity = entity });
        }

        public void RaiseEntityPreparingForUpdating(TEntity entity)
        {
            RaiseEntityPreparingForSaving(entity);
            EntityPreparingForUpdating?.Invoke(this, new ManagerEventArgs<TEntity>() { Entity = entity });
        }

        public void RaiseEntityPreparingForSaving(TEntity entity)
        {
            EntityPreparingForSaving?.Invoke(this, new ManagerEventArgs<TEntity>() { Entity = entity });
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
