using System.Collections.Generic;
using System;

namespace CPrint2
{
    public static class IListEx
    {
        public static void AddRange<K, V>(this SortedList<K, V> list, IEnumerable<V> items, Func<V, K> funct)
        {
            foreach (var i in items)
                list.Add(funct(i), i);
        }
    }
}
