/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/
using System;
using System.Threading;

namespace VPrinting.Threading
{
    /// <summary>
    /// Cycle worker abstraction
    /// </summary>
    public abstract class CycleWorkerBase
    {
        protected volatile Thread m_Worker = null;

        /// <summary>
        /// Error event
        /// </summary>
        public static event ThreadExceptionEventHandler Error;

        /// <summary>
        /// Running flag
        /// </summary>
        public volatile bool Running;
        public volatile bool FirstRun;

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
            if (m_Worker == null)
            {
                Running = true;
                m_Worker = new Thread(RunThreadFunction);
                m_Worker.Name = name;
                m_Worker.IsBackground = true;
                m_Worker.Priority = priority;
                m_Worker.Start();

                FireStarted();
            }
        }

        /// <summary>
        /// Stops the background worker
        /// </summary>
        public void Stop()
        {
            Running = false;
        }

        protected virtual void RunThreadFunction()
        {
            FirstRun = true;

            while (Running)
            {
                try
                {
                    RunOnce();
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
                    FireError(ex);
                }
                finally
                {
                    Thread.Sleep(SleepTime);
                    FirstRun = false;
                }
            }

            m_Worker = null;
        }

        public abstract void RunOnce();

        protected abstract void FireStarted();

        protected void FireError(Exception ex)
        {
            if (Error != null)
                Error(this, new ThreadExceptionEventArgs(ex));
        }
    }
}
