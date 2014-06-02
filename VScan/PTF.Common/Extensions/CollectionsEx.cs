/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace PremierTaxFree.PTFLib
{
    public static class CollectionsEx
    {
        /// <summary>
        /// Shows whether collection is empty
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool IsEmpty<T>(this ICollection<T> list)
        {
            return list.Count == 0;
        }

        /// <summary>
        /// Creates string from list
        /// </summary>
        /// <param name="list"></param>
        /// <param name="devider"></param>
        /// <param name="funct"></param>
        /// <returns></returns>
        public static string ToString(this IList list, string devider, Func<object, string> funct)
        {
            if (list == null || list.Count == 0)
                return string.Empty;

            StringBuilder b = new StringBuilder();
            for (int i = 0; i < list.Count - 1; i++)
            {
                b.Append(funct != null ? funct(list[i]) : list[i]);
                b.Append(devider);
            }

            return b.ToString();
        }

        /// <summary>
        /// Processes an enumerrable by delegate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="coll"></param>
        /// <param name="del"></param>
        /// <param name="params"></param>
        public static void ForEach<T>(this IEnumerable<T> coll, Delegate del, params object[] @params)
        {
            foreach (T t in coll)
                del.DynamicInvoke(@params);
        }

        /// <summary>
        /// Processes an enumerrable by action
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="coll"></param>
        /// <param name="act"></param>
        public static void ForEach<T>(this IEnumerable<T> coll, Action<T> act)
        {
            Debug.Assert(act != null);
            foreach (var t in coll)
                act(t);
        }

        /// <summary>
        /// Checks whether item of type T exists in collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="coll"></param>
        /// <returns></returns>
        public static bool ExistItemOf<T>(this ICollection coll)
        {
            foreach (var i in coll)
                if (i is T)
                    return true;
            return false;
        }
    }
}
