/***************************************************
//  Copyright (c) Premium Tax Free 2012
/***************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace BackupRestore
{
    public static class Extentions
    {
        public static bool Contains(this string[] array, params string[] values)
        {
            foreach (var value in values)
            {
                foreach (var str in array)
                {
                    if (string.Compare(str, value, true) == 0)
                        return true;
                }
            }
            return false;
        }

        public static T Cast<T>(this object value)
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }

        public static string TrimSafe(this string value, params char[] chars)
        {
            if (value == null)
                return value;
            return value.Trim(chars);
        }

        /// <summary>
        /// "ISO-iso_number;    Brach-br_iso_id,br_id,br_ho_id;     HeadOffice-ho_iso_id,ho_id"
        /// </summary>
        /// <param name="value"></param>
        /// <param name="splitter1">;</param>
        /// <param name="splitter2">-</param>
        /// <param name="splitter3">,</param>
        /// <returns></returns>
        public static IEnumerable<T> Parse<T>(this string value, char splitter1, char splitter2, char splitter3) where T : INamedList, new()
        {
            var strs1 = value.Split(new char[] { splitter1 }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in strs1)
            {
                var t = new T();
                var strs2 = s.Split(new char[] { splitter2 }, StringSplitOptions.RemoveEmptyEntries);
                Debug.Assert(strs2.Length == 2);

                t.Name = strs2[0].TrimSafe();
                t.Values = new List<string>();

                var strs3 = strs2[1].Split(new char[] { splitter3 }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string ss in strs3)
                    t.Values.Add(ss.TrimSafe());

                yield return t;
            }
        }
    }
}
