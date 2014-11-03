using System;
using System.Collections;
using System.Threading;
using VPrinting;

namespace VPrinting.Common
{
    public class ScheduledWorker<T>
    {
        public static event ThreadExceptionEventHandler Error;
        public event EventHandler<ValueEventArgs<T>> RunItem;

        private readonly ArrayList m_List = ArrayList.Synchronized(new ArrayList());
        private volatile Thread m_Thread;

        public void Add(T item, TimeSpan runAt, Action<T> procFunc = null)
        {
            if (procFunc != null)
                procFunc(item);

            TryStart(runAt);
            m_List.Add(item);
        }

        private void TryStart(TimeSpan runAt)
        {
            if (m_Thread == null)
            {
                m_Thread = new Thread(ThreadFunction);
                m_Thread.IsBackground = true;
                m_Thread.Name = "ScheduledWorker";
                m_Thread.Start(runAt);
            }
        }

        private void ThreadFunction(object runAt)
        {
            try
            {
                var date = DateTime.Now.At((TimeSpan)runAt);

                var tosleep = date.Subtract(DateTime.Now);

                Thread.Sleep(tosleep);

                foreach (T item in m_List)
                    if (RunItem != null)
                        RunItem(this, new ValueEventArgs<T>(item));
            }
            catch (Exception ex)
            {
                if (Error != null)
                    Error(this, new ThreadExceptionEventArgs(ex));
            }
            finally
            {
                m_Thread = null;
            }
        }
    }
}
