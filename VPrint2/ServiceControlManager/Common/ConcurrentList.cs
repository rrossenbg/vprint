/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System.Collections.Generic;

namespace FintraxServiceManager.Common
{
    public class ConcurrentList<T> : List<T>
    {
        public new void Add(T t)
        {
            lock (this)
                base.Add(t);
        }

        public new bool Contains(T t)
        {
            lock (this)
            {
                return base.Contains(t);
            }
        }

        public new void Clear()
        {
            lock (this)
                base.Clear();
        }
    }
}
