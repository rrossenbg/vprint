/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.Collections.Generic;
using System.Runtime;
using System.Collections;

namespace VPrinting
{
    public static class CollectionsEx
    {
        [TargetedPatchingOptOut("na")]
        public static T Max<T>(this IEnumerable<T> items, Func<T, T, bool> func)
            where T : class
        {
            T tt = default(T);

            foreach (var t in items)
                if (tt == null || func(t, tt))
                    tt = t;

            return tt;
        }

        [TargetedPatchingOptOut("na")]
        public static ArrayList ToList(this Hashtable table)
        {
            var list = new ArrayList();
            foreach (DictionaryEntry kv in table)
            {
                list.Add(kv.Key);
                list.Add(kv.Value);
            }
            return list;
        }

        [TargetedPatchingOptOut("na")]
        public static Hashtable ToHashtable<K, V>(this ArrayList list)
        {
            if (list == null || list.Count % 2 != 0)
                throw new ArgumentException("list");

            var table = new Hashtable();
            for (var i = 0; i < list.Count; i += 2)
                table.Add(Convert.ChangeType(list[i], typeof(K)), Convert.ChangeType(list[i + 1], typeof(V)));
            return table;
        }
    }
}
