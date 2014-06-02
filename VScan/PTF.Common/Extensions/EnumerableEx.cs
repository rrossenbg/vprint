/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace PremierTaxFree.PTFLib
{
    public static class EnumerableEx
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence1"></param>
        /// <param name="sequence2"></param>
        /// <param name="comparer"></param>
        /// <param name="ignoreFunct"></param>
        /// <returns></returns>
        /// <example>
        /// string[] arr1 = { "A", "B", ".", "$", "C", "D", "E" };
        /// string[] arr2 = { "B", "!", "C",  };
        /// Func<string, bool> funct = (s) => !Char.IsLetter(s[0]) || Char.IsLower(s[0]);
        /// arr1.IntersectIgnore(arr2, EqualityComparer<string>.Default, funct).ForEach((v) => Console.WriteLine(v));
        /// ======================
        /// Output: B, C
        /// </example>
        public static IEnumerable<T> IntersectIgnore<T>(this IList<T> sequence1, IList<T> sequence2, IEqualityComparer<T> comparer, Func<T, bool> ignoreFunct)
        {
            Debug.Assert(sequence1 != null);
            Debug.Assert(sequence2 != null);
            Debug.Assert(comparer != null);

            int start = 0, end = 0;

            for (int i = 0, j = 0; i < sequence1.Count; i++, j++)
            {
                //Search for ignored items sequence1
                if (ignoreFunct != null && ignoreFunct(sequence1[i]))
                {
                    j--;
                    continue;
                }

                //Search for end
                if (j == sequence2.Count)
                {
                    end = i;
                    break;
                }

                //Search for ignored items sequence2
                if (ignoreFunct != null && ignoreFunct(sequence2[j]))
                {
                    i--;
                    continue;
                }

                if (!comparer.Equals(sequence1[i], sequence2[j]))
                {
                    start = i + 1;
                    j = -1;
                }
            }

            for (int i = start; i < end; i++)
                if (ignoreFunct == null || !ignoreFunct(sequence1[i]))
                    yield return sequence1[i];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <param name="del"></param>
        /// <example>
        /// var clients = new Hashtable();
        /// ServerDataAccess.SelectClients(clients, false);
        /// var list = new ArrayList();
        /// clients.ForEach<DictionaryEntry>((e) => { list.Add(e.Key); list.Add(e.Value); });
        /// </example>
        public static void ForEach<T>(this IEnumerable values, Action<T> del)
        {
            Debug.Assert(values != null);
            Debug.Assert(del != null);
            foreach (T o in values)
                del(o);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="values"></param>
        /// <param name="convert"></param>
        /// <returns></returns>
        /// <example>
        /// var list = table.ConvertAll<DictionaryEntry, DataPair<int, string>>((di) => new DataPair<int, string>(di.Key.Cast<int>(), di.Value.Cast<string>()));
        /// </example>
        public static IEnumerable<U> ConvertAll<T, U>(this IEnumerable values, Func<T, U> convert)
        {
            Debug.Assert(values != null);
            Debug.Assert(convert != null);
            foreach (T o in values)
                yield return convert(o);
        }
    }
}
