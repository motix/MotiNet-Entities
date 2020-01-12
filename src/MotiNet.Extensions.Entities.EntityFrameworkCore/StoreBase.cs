using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics.CodeAnalysis;

namespace MotiNet.Entities.EntityFrameworkCore
{
    public abstract class StoreBase<TEntity, TDbContext> : IStore<TEntity, TDbContext>, IDisposable
        where TEntity : class
        where TDbContext : DbContext
    {
        #region Properties

        public TDbContext DbContext { get; }

        #endregion

        #region Constructors

        public StoreBase(TDbContext dbContext)
            => DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

        protected StoreBase() { }

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
                    DbContext.Dispose();
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
