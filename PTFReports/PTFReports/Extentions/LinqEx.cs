/***************************************************
//  Copyright (c) Premium Tax Free 2012
/***************************************************/

using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Linq;

namespace PTF.Reports
{
    public static class LinqEx
    {
        public static IEnumerable<U> ToList<T, U>(this ObjectSet<T> set) where T : class, U
        {
            foreach (var t in set)
                yield return (U)t;
        }

        /// <summary>
        /// Remove First Where
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="coll"></param>
        /// <param name="funct"></param>
        public static void RemoveWhere<T>(this EntityCollection<T> coll, Func<T, bool> funct) 
            where T: class
        {
            var t = coll.FirstOrDefault(funct);
            if (t != null)
                coll.Remove(t);
        }

        public static List<T> Page<T>(this IQueryable<T> query, int? page, int size)
        {
            int skip = page.HasValue ? page.Value - 1 : 0;
            return query.Skip(skip * size).Take(size).ToList();
        }

        /// <summary>
        /// Remove all Where
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="funct"></param>
        public static void RemoveWhere<T>(this IList<T> list, Func<T, bool> funct)
        {
            var listToRemove = new List<T>(list.Where(funct));
            foreach (T t in listToRemove)
                list.Remove(t);
        }
    }
}