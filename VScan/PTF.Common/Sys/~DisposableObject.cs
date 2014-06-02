/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/

using System;

namespace PremierTaxFree.PTFLib.Sys
{
    /// <summary>
    /// Base class for any disposable object. Can dispose itself.
    /// </summary>
    [Serializable]
    public abstract class DisposableObject : IDisposable
    {
        protected bool isDisposed = false;

        ~DisposableObject()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected abstract void DisposeManage();

        protected virtual void DisposeUnmanage()
        {
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                DisposeManage();
            }

            DisposeUnmanage();

            isDisposed = true;
        }
    }
}