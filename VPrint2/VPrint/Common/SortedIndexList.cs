/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System.Collections;

namespace VPrinting.Common
{
    public class SortedIndexList<K, V>
    {
        private readonly ArrayList m_List = ArrayList.Synchronized(new ArrayList());
        private readonly Hashtable m_Table = Hashtable.Synchronized(new Hashtable());

        public V this[K key]
        {
            get
            {
                return (V)m_Table[key];
            }
            set
            {
                Add(key, value);
            }
        }

        public int Add(K key, V value)
        {
            if (!m_Table.ContainsKey(key))
            {
                m_List.Add(key);
                m_List.Sort();
            }
            m_Table[key] = value;
            return m_List.IndexOf(key);
        }

        public void Remove(K key)
        {
            m_List.Remove(key);
            m_Table.Remove(key);
        }

        public int IndexOf(K key)
        {
            return m_List.IndexOf(key);
        }

        public void Clear()
        {
            m_List.Clear();
            m_Table.Clear();
        }
    }
}
