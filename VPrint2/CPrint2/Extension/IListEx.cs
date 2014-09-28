/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.Collections.Generic;

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
