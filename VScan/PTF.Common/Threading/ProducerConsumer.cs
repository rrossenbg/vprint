/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/

using System;
using System.Collections.Generic;
using System.Threading;


namespace PremierTaxFree.PTFLib.Threading
{
    public class ProducerConsumer<T>
    {
        /// <summary>
        /// Time for thread spleep before check for new work
        /// </summary>
        public static TimeSpan DEFAULT_TIMEOUT = TimeSpan.FromSeconds(2);

        private Thread m_Thread;
        private readonly Queue<T> m_Queue = new Queue<T>();
        private readonly object m_Locker = new object();

        public event EventHandler<ItemEventArgs<T>> NewItem;
        public event ThreadExceptionEventHandler Error;

        public ProducerConsumer()
        {

        }

        /// <summary>
        /// Starts producer-consumer
        /// </summary>
        public void Start()
        {
            m_Thread = new Thread(ThreadFunction);
            m_Thread.Priority = ThreadPriority.Lowest;
            m_Thread.IsBackground = true;
            m_Thread.Start();
        }

        /// <summary>
        /// Stops producer-consumer
        /// </summary>
        public void Stop()
        {
            m_Thread.AbortSafe();
            m_Queue.Clear();
        }

        /// <summary>
        /// Adds value to producer-consumer
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            lock (m_Locker)
            {
                m_Queue.Enqueue(item);
                Monitor.Pulse(m_Locker);
            }
        }

        /// <summary>
        /// Adds range of values to producer-consumer
        /// </summary>
        /// <param name="items"></param>
        public void AddRange(IEnumerable<T> items)
        {
            lock (m_Locker)
            {
                foreach (T item in items)
                    m_Queue.Enqueue(item);
                Monitor.Pulse(m_Locker);
            }
        }

        private void ThreadFunction()
        {
            while (true)
            {
                T item = default(T);

                lock (m_Locker)
                {
                    while (m_Queue.Count == 0)
                        Monitor.Wait(m_Locker, DEFAULT_TIMEOUT);

                    item = m_Queue.Dequeue();
                }

                try
                {
                    if (NewItem != null)
                        NewItem(this, new ItemEventArgs<T>(item));
                }
                catch (ThreadAbortException)
                {
                    // Do nothing here
                }
                catch (Exception ex)
                {
                    if (Error != null)
                        Error(this, new ThreadExceptionEventArgs(ex));
                }
            }
        }
    }

    public class ItemEventArgs<T> : EventArgs
    {
        public T Item { get; private set; }

        public ItemEventArgs(T item)
        {
            Item = item;
        }
    }
}
