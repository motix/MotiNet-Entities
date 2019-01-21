using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MotiNet.Entities
{
    public interface IManager<TEntity, TSubEntity> : IManager<TEntity>
        where TEntity : class
        where TSubEntity : class
    {
        void InitExtensions(ManagerTasks<TEntity, TSubEntity> tasks);

        Task<GenericResult> ValidateSubEntityAsync(TSubEntity subEntity);

        Task ExecuteEntityWithSubEntityCreateValidatingAsync(TEntity entity);

        Task ExecuteEntityWithSubEntityUpdateValidatingAsync(TEntity entity, TEntity oldEntity);

        Task ExecuteEntityWithSubEntityValidateAsync(TEntity entity, List<GenericError> errors);

        Task ExecuteEntityWithSubEntitySavingAsync(TEntity entity);
    }

    public interface IManager<TEntity> : IDisposable
        where TEntity : class
    {
        IDisposable Store { get; }

        object Accessor { get; }

        ILogger Logger { get; }

        CancellationToken CancellationToken { get; }

        void InitExtensions(ManagerTasks<TEntity> tasks);

        Task<GenericResult> ValidateEntityAsync(TEntity entity);

        Task ExecuteEntityGetAsync(TEntity entity);

        Task ExecuteEntityCreateValidatingAsync(TEntity entity);

        Task ExecuteEntityCreatingAsync(TEntity entity);

        Task ExecuteEntityUpdateValidatingAsync(TEntity entity, TEntity oldEntity);

        Task ExecuteEntityUpdatingAsync(TEntity entity, TEntity oldEntity);

        Task ExecuteEntitySavingAsync(TEntity entity);

        void ThrowIfDisposed();
    }
}
