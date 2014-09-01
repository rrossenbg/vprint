/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime;
using System.Reflection;

namespace VPrinting
{
    [Obfuscation(StripAfterObfuscation = true, ApplyToMembers = true)]
    public static class Collection2Ex
    {
        [TargetedPatchingOptOut("na")]
        public static bool IsNullOrEmpty<T>(this ICollection<T> coll)
        {
            return coll == null || coll.Count == 0;
        }

        [TargetedPatchingOptOut("na")]
        public static U GetValueAndRemove<T, U>(this IDictionary dict, T key)
        {
            U u = (U)dict[key];
            dict.Remove(key);
            return u;
        }

        [TargetedPatchingOptOut("na")]
        public static U GetValueAndRemove2<T, U>(this IDictionary<T, U> dict, T key)
        {
            U u = dict[key];
            dict.Remove(key);
            return u;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        /// <exception cref="ApplicationException"/>
        public static T FindFirstOrDefault<T>(this SynchronizedCollection<T> coll, Func<T, bool> func) where T : class, IEquatable<T>
        {
            Debug.Assert(func != null);

            lock (coll.SyncRoot)
            {
                foreach (T t in coll)
                {
                    Debug.Assert(t != null);
                    if (func(t))
                        return t;
                }
            }
            return default(T);
        }
    }
}
