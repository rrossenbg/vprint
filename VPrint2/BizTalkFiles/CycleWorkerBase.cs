using System;
using System.Threading;

namespace BizTalkFiles
{
    public abstract class CycleWorkerBase
    {
        protected Thread m_Worker;
        public volatile bool Running;

        public event EventHandler Step;
        public event ThreadExceptionEventHandler Error;

        protected CycleWorkerBase()
        {
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
                    //
                }
                catch (ThreadInterruptedException)
                {
                    //
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

        public void Start(ThreadPriority priority, string name)
        {
            if (this.m_Worker == null)
            {
                this.Running = true;
                this.m_Worker = new Thread(new ThreadStart(RunThread));
                this.m_Worker.Name = name;
                this.m_Worker.IsBackground = false;
                this.m_Worker.Priority = priority;
                this.m_Worker.Start();
            }
        }

        public void Stop()
        {
            this.Running = false;
            this.m_Worker.JoinSafe();
            this.m_Worker = null;
        }

        protected abstract void ThreadFunction();

        public TimeSpan SleepTime { get; set; }

        protected void FireStep()
        {
            if (Step != null)
                Step(this, EventArgs.Empty);
        }
    }
}

