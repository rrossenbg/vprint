/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Remoting.Messaging;

namespace VPrinting.Common
{
    /// <summary>
    /// Runs maxActiveTasks simultaneously.
    /// Once a task is executed it runs the next one.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class TaskProcessOrganizer<T>
    {
        public readonly Hashtable Data = Hashtable.Synchronized(new Hashtable());

        public class TaskItem
        {
            public T Item { get; set; }
            public Action<TaskItem> Method { get; set; }
            public TaskItem(T t, Action<TaskItem> m)
            {
                Item = t;
                Method = m;
            }
        }

        public class CompletedEventArgs : EventArgs
        {
            public T Value { get; private set; }

            public CompletedEventArgs(T t)
            {
                Value = t;
            }
        }

        public event EventHandler<CompletedEventArgs> Completed;

        private readonly SynchronizedCollection<TaskItem> m_ToDoList = new SynchronizedCollection<TaskItem>();
        private readonly SynchronizedCollection<TaskItem> m_ActiveList = new SynchronizedCollection<TaskItem>();

        private volatile int m_ProcessedItems = 0;
        private volatile int m_MaxActiveTasks = 0;

        public int ProcessedItems
        {
            get
            {
                return m_ProcessedItems;
            }
        }

        public TaskProcessOrganizer(int maxActiveTasks)
        {
            Debug.Assert(maxActiveTasks > 0);
            m_MaxActiveTasks = maxActiveTasks;
        }

        public void RunTask(TaskItem task)
        {
            Debug.Assert(task != null);
            Debug.Assert(task.Method != null);

            if (m_ActiveList.Count < m_MaxActiveTasks)
            {
                m_ActiveList.Add(task);
                task.Method.BeginInvoke(task, new AsyncCallback(TaskCompleted), task);
            }
            else
            {
                m_ToDoList.Add(task);
            }
        }

        private void TaskCompleted(IAsyncResult ares)
        {
            if (ares != null)
            {
                AsyncResult aresult = (AsyncResult)ares;
                TaskItem task = (TaskItem)ares.AsyncState;
                Debug.Assert(task != null);
                Action<TaskItem> del = (Action<TaskItem>)aresult.AsyncDelegate;

                try
                {
                    del.EndInvoke(ares);
                }
                finally
                {
                    m_ActiveList.Remove(task);
                    FireCompleted(task);

                    m_ProcessedItems++;

                    if (m_ActiveList.Count < m_MaxActiveTasks && m_ToDoList.Count > 0)
                    {
                        TaskItem task2 = (TaskItem)m_ToDoList[0];
                        m_ToDoList.RemoveAt(0);
                        m_ActiveList.Add(task2);
                        task2.Method.BeginInvoke(task2, new AsyncCallback(TaskCompleted), task2);
                    }
                }
            }
        }

        public void FireCompleted(TaskProcessOrganizer<T>.TaskItem task)
        {
            if (Completed != null)
                Completed(this, new CompletedEventArgs(task.Item));
        }

        public void Clear()
        {
            m_ProcessedItems = 0;
            m_ToDoList.Clear();
            m_ActiveList.Clear();
        }

        public bool HasItems()
        {
            return (m_ToDoList.Count > 0 || m_ActiveList.Count > 0);
        }
    }

    //Sends files
    public class StringTaskOrganizer : TaskProcessOrganizer<string>
    {
        public StringTaskOrganizer(int maxActiveTasks)
            : base(maxActiveTasks)
        {
        }
    }

    //Scan/Download files
    public class StateManagerItemOrganizer : TaskProcessOrganizer<StateManager.Item>
    {
        public StateManagerItemOrganizer(int maxActiveTasks)
            : base(maxActiveTasks)
        {
        }
    }
}
