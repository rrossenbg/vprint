/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System.Collections.Generic;
using System.Threading;

namespace FintraxServiceManager.Common
{
    public class ConcurrentList<T> : List<T>
    {
        public new void Add(T t)
        {
            bool flag = false;
            try
            {
                Monitor.Enter(this, ref flag);
                base.Add(t);
            }
            finally
            {
                if (flag)
                {
                    Monitor.Exit(this);
                }
            }
        }
        public new bool Contains(T t)
        {
            bool flag = false;
            bool result;
            try
            {
                Monitor.Enter(this, ref flag);
                result = base.Contains(t);
            }
            finally
            {
                if (flag)
                {
                    Monitor.Exit(this);
                }
            }
            return result;
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
