/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/

using System;
using System.Threading;


namespace PremierTaxFree.PTFLib.Threading
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
        public event ThreadExceptionEventHandler Error;

        /// <summary>
        /// Running flag
        /// </summary>
        public volatile bool Running;

        /// <summary>
        /// Sleep Time
        /// </summary>
        public TimeSpan SleepTime { get; set; }

        /// <summary>
        /// Starts the background worker
        /// </summary>
        /// <param name="priority"></param>
        /// <param name="name">For debug purposes only</param>
        /// <example>
        /// SQLWorker.Default.Start(ThreadPriority.Lowest, "SQLWorker");
        /// </example>
        public void Start(ThreadPriority priority, string name)
        {
            if (m_Worker == null)
            {
                Running = true;
                m_Worker = new Thread(RunThread);
                m_Worker.Name = name;
                m_Worker.IsBackground = true;
                m_Worker.Priority = priority;
                m_Worker.Start();
            }
        }

        /// <summary>
        /// Stops the background worker
        /// </summary>
        /// <example>
        ///  SQLWorker.Default.Empty.WaitOne();
        ///  SQLWorker.Default.Stop();
        /// </example>
        public void Stop()
        {
            m_Worker.AbortSafe();
            m_Worker = null;
            Running = false;
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
