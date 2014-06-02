/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/

using System;
using System.Threading;

namespace PremierTaxFree.PTFLib.Threading
{
    /// <summary>
    /// Not cycle thread abstraction
    /// </summary>
    public abstract class ThreadWorkerBase
    {
        /// <summary>
        /// Error handler
        /// </summary>
        public event ThreadExceptionEventHandler Error;
        protected Thread m_thread;

        /// <summary>
        /// Start thread
        /// </summary>
        /// <param name="threadName"></param>
        public void Start(string threadName)
        {
            m_thread = new Thread(ThreadFunction);
            m_thread.IsBackground = true;
            m_thread.Name = threadName;
            m_thread.Start();
        }

        protected abstract void ThreadFunction();
        protected virtual void FireError(Exception ex)
        {
            if (Error != null)
                Error(this, new ThreadExceptionEventArgs(ex));
        }
    }
}
