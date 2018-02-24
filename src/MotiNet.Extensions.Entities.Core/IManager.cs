using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MotiNet.Entities
{
    public interface IManager<TEntity> where TEntity : class
    {
        IDisposable Store { get; }

        object Accessor { get; }

        ILogger Logger { get; }

        CancellationToken CancellationToken { get; }

        void InitExtensions(ManagerTasks<TEntity> tasks);

        Task<GenericResult> ValidateEntityAsync(TEntity entity);

        Task ExecuteEntityValidatingAsync(TEntity entity);

        Task ExecuteEntityValidateAsync(TEntity entity, List<GenericError> errors);

        Task ExecuteEntityCreatingAsync(TEntity entity);

        Task ExecuteEntityUpdatingAsync(TEntity entity);

        Task ExecuteEntitySavingAsync(TEntity entity);

        void ThrowIfDisposed();
    }
}
