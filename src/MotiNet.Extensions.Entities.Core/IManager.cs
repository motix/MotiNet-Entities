using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MotiNet.Entities
{
    public interface IManager<TEntity> where TEntity : class
    {
        CancellationToken CancellationToken { get; }

        ILogger Logger { get; }

        IDisposable Store { get; }

        object Accessor { get; }

        void InitExtensions(ManagerEventHandlers<TEntity> eventHandlers);

        Task<GenericResult> ValidateEntityAsync(TEntity entity);

        void RaiseEntityPreparingForValidation(TEntity entity);

        void RaiseEntityPreparingForCreating(TEntity entity);

        void RaiseEntityPreparingForUpdating(TEntity entity);

        void RaiseEntityPreparingForSaving(TEntity entity);

        void ThrowIfDisposed();
    }
}
