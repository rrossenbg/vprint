/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using System.Data;

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

        [TargetedPatchingOptOut("na")]
        public static List<object[]> ToTable(this IList list)
        {
            if (list == null && list.Count >=2)
                throw new ArgumentException("list");

            int rows = (int)list[0];
            int cols = (int)list[1];

            var resultList = new List<object[]>();

            for (int row = 0, index = 0; row < rows; row++)
            {
                var rowlist = new ArrayList();

                for (int col = 0; col < cols; col++, index++)
                    rowlist.Add(list[index]);

                resultList.Add(rowlist.ToArray());
            }
            return resultList;
        }

        [TargetedPatchingOptOut("na")]
        public static DataTable ToDataTable<T>(this IList<T> list, string name = "Table")
        {
            if (list == null)
                throw new ArgumentNullException("list");

            DataTable table = new DataTable(name);

            Type type = typeof(T);
            var props = type.GetProperties();

            foreach (var p in props)
                table.Columns.Add(p.Name, p.PropertyType);

            List<object> lst = new List<object>();

            foreach (T t in list)
            {
                foreach (var p in props)
                    lst.Add(p.GetValue(t, null));

                table.Rows.Add(lst.ToArray());

                lst.Clear();
            }
            return table;
        }

        [TargetedPatchingOptOut("na")]
        public static void AddRange<T>(this HashSet<T> set, IEnumerable<T> items)
        {
            foreach (T t in items)
                set.Add(t);
        }

        [TargetedPatchingOptOut("na")]
        public static IEnumerable<T> ForEach<T>(this HashSet<T> set)
        {
            foreach (T t in set)
                yield return t;
        }

        [TargetedPatchingOptOut("na")]
        public static bool Empty<T>(this HashSet<T> set)
        {
            return set.Count == 0;
        }

        [TargetedPatchingOptOut("na")]
        public static IEnumerable<T> ForEach<T>(this HashSet<T> set, Action<T> funct)
        {
            foreach (T t in set)
            {
                funct(t);
                yield return t;
            }
        }

        [TargetedPatchingOptOut("na")]
        public static T Random<T>(this IList<T> seq)
        {
            Random rnd = new Random(DateTime.Now.Millisecond);
            var index = rnd.Next(0, seq.Count - 1);
            return seq[index];
        }
    }
}