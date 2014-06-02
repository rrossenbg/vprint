/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System.Collections.Generic;

namespace VPrinting.Common
{
    public class IgnoreList<T>
    {
        private readonly HashSet<T> m_State = new HashSet<T>();        

        public bool Add(T value)
        {
            lock (this)
            {
                return m_State.Add(value);
            }
        }

        public bool Contains(T value)
        {
            lock (this)
            {
                return m_State.Contains(value);
            }
        }

        public void Clear()
        {
            lock (this)
            {
                m_State.Clear();
            }
        }
    }
}
