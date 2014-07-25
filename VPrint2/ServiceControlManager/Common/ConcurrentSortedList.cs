/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System.Collections.Generic;
using System.Threading;

namespace FintraxServiceManager.Common
{
    public class ConcurrentSortedList<T1, T2> : SortedList<T1, T2>
    {
        public ConcurrentSortedList()
        {
        }
        public ConcurrentSortedList(IDictionary<T1, T2> list)
        {
            lock (list)
            {
                foreach (KeyValuePair<T1, T2> current in list)
                {
                    this.Add(current.Key, current.Value);
                }
            }
        }
        public new void Add(T1 key, T2 value)
        {
            bool flag = false;
            try
            {
                Monitor.Enter(this, ref flag);
                base.Add(key, value);
            }
            finally
            {
                if (flag)
                {
                    Monitor.Exit(this);
                }
            }
        }
        public void AddRange(IDictionary<T1, T2> list)
        {
            lock (list)
            {
                foreach (KeyValuePair<T1, T2> current in list)
                {
                    this.Add(current.Key, current.Value);
                }
            }
        }
        public new void Clear()
        {
            bool flag = false;
            try
            {
                Monitor.Enter(this, ref flag);
                base.Clear();
            }
            finally
            {
                if (flag)
                {
                    Monitor.Exit(this);
                }
            }
        }
    }
}