/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Diagnostics;
using System.Threading;
using Thr = System.Threading;

namespace ReceivingServiceLib.FileWorkers
{
    public abstract class WorkerBase
    {
        protected volatile bool m_Exit;
        public static event ThreadExceptionEventHandler Error;

        private Thread m_WorkerThread = null;

        /// <summary>
        /// 2 sec.
        /// </summary>
        protected readonly TimeSpan TIMEOUT =
#if DEBUG
 TimeSpan.FromSeconds(2);
#else
            TimeSpan.FromSeconds(30);
#endif

        /// <summary>
        /// 15 sec.
        /// </summary>
        protected readonly TimeSpan EMPTYTIMEOUT =
#if DEBUG
 TimeSpan.FromSeconds(15);
#else
            TimeSpan.FromMinutes(5);
#endif

        /// <summary>
        /// Starts/stops printer queue
        /// </summary>
        /// <returns>True- working, False- not working, Null- not started</returns>
        public bool? StartStop()
        {
            if (!TryStart())
            {
                if ((m_WorkerThread.ThreadState & Thr.ThreadState.Suspended) == Thr.ThreadState.Suspended)
                {
                    m_WorkerThread.Resume();
                    return true;
                }
                else
                {
                    m_WorkerThread.Suspend();
                    return false;
                }
            }
            return true;
        }

        private bool TryStart()
        {
            if (m_WorkerThread == null)
            {
                m_WorkerThread = new Thread(WorkerThreadFunction);
                m_WorkerThread.IsBackground = true;
                m_WorkerThread.Priority = ThreadPriority.Lowest;
                m_WorkerThread.Name = this.GetType().FullName + "_Thread";
                m_WorkerThread.SetApartmentState(ApartmentState.MTA);
                m_WorkerThread.Start();
                return true;
            }
            return false;
        }

        protected abstract void WorkerThreadFunction();

        protected void FireError(Exception ex)
        {
            if (Error != null)
                Error(this, new ThreadExceptionEventArgs(ex));
        }
    }
}
