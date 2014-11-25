/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.Collections;
using System.Collections.Generic;

namespace VPrinting.Colections
{
    [Serializable]
    public class PriorityQueue<T> : IEnumerable<T>
    {
        private readonly Hashtable m_table = Hashtable.Synchronized(new Hashtable());
        private readonly ArrayList m_list = ArrayList.Synchronized(new ArrayList());

        [Serializable]
        private class Item : IEquatable<Item>, IComparable<Item>
        {
            public int Key { get; set; }
            public T Value { get; set; }

            public override bool Equals(object obj)
            {
                if (obj == null)
                    return false;

                Item objAsItem = obj as Item;
                if (objAsItem == null)
                    return false;
                else
                    return Equals(objAsItem);
            }

            public override int GetHashCode()
            {
                return Key;
            }

            public bool Equals(Item other)
            {
                if (other == null)
                    return false;
                return Key.Equals(other.Key);
            }

            public int CompareTo(Item other)
            {
                if (other == null)
                    return 1;
                return Key.CompareTo(other.Key);
            }
        }

        private class PriorityQueueComparer : IComparer, IComparer<Item>
        {
            public static readonly PriorityQueueComparer Default = new PriorityQueueComparer();

            public int Compare(object x, object y)
            {
                var xx = x as Item;
                var yy = y as Item;
                return Compare(xx, yy);
            }

            public int Compare(Item x, Item y)
            {
                if (x == null)
                    return -1;
                else if (y == null)
                    return 1;
                else
                    return x.CompareTo(y);
            }
        }

        public int Count
        {
            get
            {
                return m_list.Count;
            }
        }

        public PriorityQueue()
        {
        }

        public PriorityQueue(IEnumerable<T> values)
        {
            foreach (var value in values)
            {
                var item = new Item() { Key = 0, Value = value };
                m_table[value] = item;
                m_list.Add(item);
            }

            m_list.Sort(PriorityQueueComparer.Default);
        }

        public void Add(T value)
        {
            var item = new Item() { Key = 0, Value = value };
            m_table[value] = item;
            m_list.Add(item);

            m_list.Sort(PriorityQueueComparer.Default);
        }

        public void AddRange(IEnumerable<T> values)
        {
            foreach (var value in values)
            {
                var item = new Item() { Key = 0, Value = value };
                m_table[value] = item;
                m_list.Add(item);
            }

            m_list.Sort(PriorityQueueComparer.Default);
        }

        public void Set(T value)
        {
            if (m_table.ContainsKey(value))
            {
                ((Item)m_table[value]).Key -= 1;
                m_list.Sort(PriorityQueueComparer.Default);
            }
        }

        public void Clear()
        {
            m_table.Clear();
            m_list.Clear();
        }

        public IEnumerator<T> GetEnumerator()
        {
            lock (m_list.SyncRoot)
            {
                foreach (Item item in new ArrayList(m_list))
                    yield return item.Value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            lock (m_list.SyncRoot)
            {
                foreach (Item item in new ArrayList(m_list))
                    yield return item.Value;
            }
        }
    }
}