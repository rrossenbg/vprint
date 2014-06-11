/***************************************************
//  Copyright (c) Premium Tax Free 2012
/***************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace PTF.Reports
{
    public static class IListEx
    {
        /// <summary>
        /// Returns any or all or collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="funct"></param>
        /// <returns></returns>
        public static IEnumerable<T> Where<T>(this IList list, Func<T, bool> funct = null)
        {
            Debug.Assert(list != null);

            lock (((ICollection)list).SyncRoot)
            {
                foreach (T value in list)
                    if (funct == null || funct(value))
                        yield return value;
            }
        }
    }
}
