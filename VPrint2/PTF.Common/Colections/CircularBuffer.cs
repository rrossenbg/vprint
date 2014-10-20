/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.Collections;
using System.Threading;

namespace VPrinting.Colections
{
    public class CircularBuffer<T>
    {
        private readonly int MaxLength;

        private readonly ArrayList m_List = ArrayList.Synchronized(new ArrayList());

        public CircularBuffer(int maxLength)
        {
            MaxLength = maxLength;
        }

        public void Add(T value)
        {
            if (Monitor.TryEnter(this, 150))
            {
                try
                {
                    m_List.Add(value);

                    if (m_List.Count > MaxLength)
                        m_List.RemoveAt(0);
                }
                finally
                {
                    Monitor.Exit(this);
                }
            }
        }

        public Array ToArray()
        {
            lock (m_List.SyncRoot)
                return m_List.ToArray(typeof(T));
        }

        public void Clear()
        {
            m_List.Clear();
        }
    }
}