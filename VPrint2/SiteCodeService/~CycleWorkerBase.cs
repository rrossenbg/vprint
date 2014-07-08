/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/

using System;
using System.Threading;

namespace SiteCodeSrvc
{
    /// <summary>
    /// Cycle worker abstraction
    /// </summary>
    public abstract class CycleWorkerBase
    {
        protected Thread m_Worker = null;

        /// <summary>
        /// Error event
        /// </summary>
        public static event ThreadExceptionEventHandler Error;

        private bool m_Running;
        /// <summary>
        /// Running flag
        /// </summary>
        public bool Running
        {
            get
            {
                Thread.MemoryBarrier();
                return m_Running;
            }
            set
            {
                m_Running = value;
                Thread.MemoryBarrier();
            }
        }

        /// <summary>
        /// Sleep Time
        /// </summary>
        public TimeSpan SleepTime { get; set; }

        /// <summary>
        /// Starts the background worker
        /// </summary>
        /// <param name="priority"></param>
        /// <param name="name">For debug purposes only</param>
        public void Start(ThreadPriority priority, string name)
        {
            if (Running)
                throw new ApplicationException("Already running");

            Running = true;
            m_Worker = new Thread(RunThread);
            m_Worker.Name = name;
            m_Worker.IsBackground = true;
            m_Worker.Priority = priority;
            m_Worker.Start();
        }

        /// <summary>
        /// Stops the background worker
        /// </summary>
        public void Stop()
        {
            if (Running)
            {
                Running = false;
                m_Worker = null;
            }
        }

        protected virtual void RunThread()
        {
            while (Running)
            {
                try
                {
                    ThreadFunction();
                }
                catch (ThreadAbortException)
                {
                    //Do nothing
                }
                catch (ThreadInterruptedException)
                {
                    //Do nothing
                }
                catch (Exception ex)
                {
                    if (Error != null)
                        Error(this, new ThreadExceptionEventArgs(ex));
                }
                finally
                {
                    Thread.Sleep(SleepTime);
                }
            }
        }

        protected abstract void ThreadFunction();
    }
}
