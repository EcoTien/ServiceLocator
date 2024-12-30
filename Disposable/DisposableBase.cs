﻿using System;

namespace EcoMine.ServiceLocator.Disposable
{
    
    /// <summary>
    /// Disposable base class.
    /// Create a class that inherits from this class to implement IDisposable.
    /// </summary>
    internal abstract class DisposableBase
    {
        /// <summary>
        /// Dispose flag.
        /// </summary>
        bool _disposed;
        
        /// <summary>
        /// Dispose all resources by this.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Implement Disposable.
        /// </summary>
        /// <param name="disposing"></param>
        void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing) OnDispose();
                _disposed = true;
            }
        }

        protected abstract void OnDispose();
        
        ~DisposableBase()
        {
            Dispose(false);
        }
    }
}