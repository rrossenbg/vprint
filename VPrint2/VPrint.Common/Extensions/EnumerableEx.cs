/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime;

namespace VPrinting
{
    public static class EnumerableEx
    {
        [TargetedPatchingOptOut("na")]
        public static IEnumerable<U> Convert<T, U>(this IEnumerable<T> items, Func<T, U> convertFunc)
        {
            foreach (var t in items)
                yield return convertFunc(t);
        }

        [TargetedPatchingOptOut("na")]
        public static void ProcessPairs<T, U>(this IList<T> seq1, IList<U> seq2, Predicate<U> ignoreItemFunct, Action<T, U> processItemFunct)
        {
            Debug.Assert(seq1 != null);
            Debug.Assert(seq2 != null);
            Debug.Assert(ignoreItemFunct != null);

            var list = seq2.ToList();
            list.RemoveAll(ignoreItemFunct);

            for (int i = 0, j = 0; i < seq1.Count && j < list.Count; i++, j++)
                processItemFunct(seq1[i], list[j]);
        }

        [TargetedPatchingOptOut("na")]
        public static bool Empty(this IList list)
        {
            return list.Count == 0;
        }

        [TargetedPatchingOptOut("na")]
        public static T Get<T>(this T[] items, int index, T @default = default(T))
        {
            return items == null || items.Length <= index ? @default : items[index];
        }
    }
}
