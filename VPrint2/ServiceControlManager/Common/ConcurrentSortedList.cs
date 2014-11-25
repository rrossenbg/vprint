/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System.Collections.Generic;

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
                foreach (var i in list)
                    Add(i.Key, i.Value);
            }
        }

        public new void Add(T1 key, T2 value)
        {
            lock (this)
            {
                base.Add(key, value);
            }
        }

        public void AddRange(IDictionary<T1, T2> list)
        {
            lock (list)
            {
                foreach (var i in list)
                    Add(i.Key, i.Value);
            }
        }

        public new void Clear()
        {
            lock (this)
            {
                base.Clear();
            }
        }
    }
}
