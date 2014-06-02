using System;
using System.Collections.Generic;
using System.Text;

namespace BizTalkFiles
{
    public class UniqueList<T> : List<T>
    {
        private readonly HashSet<T> m_Set;

        public UniqueList()
        {
            m_Set = new HashSet<T>();
        }

        public UniqueList(IEqualityComparer<T> comparer)
        {
            if (comparer == null)
                throw new ArgumentNullException("comparer");

            m_Set = new HashSet<T>(comparer);
        }

        public new bool Add(T item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            if (m_Set.Add(item))
            {
                base.Add(item);
                return true;
            }
            return false;
        }

        public new void AddRange(IEnumerable<T> items)
        {
            if (items == null)
                throw new ArgumentNullException("items");

            foreach (T item in items)
                Add(item);
        }

        public new void Remove(T item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            if (m_Set.Remove(item))
                base.Remove(item);
        }

        public new void Clear()
        {
            m_Set.Clear();
            base.Clear();
        }

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            foreach (T item in this)
                b.AppendFormat("'{0}',", item);
            return b.ToString();
        }
    }
}
