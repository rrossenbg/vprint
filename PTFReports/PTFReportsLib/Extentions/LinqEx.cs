/***************************************************
//  Copyright (c) Premium Tax Free 2012
/***************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Diagnostics;
using System.Linq;

namespace PTF.Reports
{
    public static class LinqEx
    {
        public static T FirstOrCreate<T>(this IEnumerable<T> coll, Func<T, bool> find, Func<T> create) where T : class
        {
            T obj = coll.FirstOrDefault(find);
            if (obj != null)
                return obj;
            return create();
        }

        public static T First<T>(this  ObjectSet<T> coll, Func<T, bool> success)
            where T : class
        {
            foreach (T t in coll)
                if (success(t))
                    return t;
            return null;
        }

        public static IEnumerable<U> ConvertAll<T, U>(this  ObjectSet<T> coll)
            where T : class
            where U : class
        {
            foreach (T t in coll)
                yield return t as U;
        }

        public static void RemoveFirst<T>(this ObjectSet<T> coll, Func<T, bool> funct)
            where T : class
        {
            foreach (T t in coll)
            {
                if (funct(t))
                {
                    coll.DeleteObject(t);
                    break;
                }
            }
        }

        /// <summary>
        /// Call SaveChanges to complete operation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="coll"></param>
        /// <param name="funct"></param>
        public static void RemoveAllWhere<T>(this ObjectSet<T> coll, Func<T, bool> funct)
            where T : class
        {
            foreach (T t in coll)
            {
                if (funct(t))
                {
                    coll.DeleteObject(t);
                }
            }
        }

        public static U GetValueSafe<T, U>(this EntityCollection<T> items, int index,
            Func<T, U> getValueFunc, U defaultValue = default(U))
            where T : class
        {
            var p = items.ElementAtOrDefault(index);
            if (p != null)
                return getValueFunc(p);
            return defaultValue;
        }

        public static void RemoveNotIncluded(this IList orgList, IList list2)
        {
        et1:
            foreach (var obj1 in orgList)
            {
                if (!list2.Contains(obj1))
                {
                    orgList.Remove(obj1);
                    goto et1;
                }
            }
        }

        public static void RemoveNotIncluded<T, U>(this IList<T> orgList,
            IList<U> list2, Func<T, U, bool> compare)
        {
        et1:
            foreach (var obj1 in orgList)
            {
                bool exist = false;

                foreach (var obj2 in list2)
                {
                    if (compare(obj1, obj2))
                    {
                        exist = true;
                        break;
                    }
                }

                if (!exist)
                {
                    orgList.Remove(obj1);
                    goto et1;
                }
            }
        }

        ///<param name="items">The enumerable to search.</param> 
        ///<param name="predicate">The expression to test the items against.</param> 
        ///<returns>The index of the first matching item, or -1 if no items match.</returns> 
        public static int FindIndex<T>(this IEnumerable<T> items, Func<T, bool> predicate)
        {
            Debug.Assert(items != null);
            Debug.Assert(predicate != null);

            int retVal = 0;
            foreach (var item in items)
            {
                if (predicate(item)) 
                    return retVal;
                retVal++;
            }
            return -1;
        }

        ///<summary>Finds the index of the first occurence of an item in an enumerable.</summary> 
        ///<param name="items">The enumerable to search.</param> 
        ///<param name="item">The item to find.</param> 
        ///<returns>The index of the first matching item, or -1 if the item was not found.</returns> 
        public static int IndexOf<T>(this IEnumerable<T> items, T item)
        {
            return items.FindIndex(i => EqualityComparer<T>.Default.Equals(item, i));
        }
    }
}
