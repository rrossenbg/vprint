using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime;
using System.Xml.Linq;

namespace HobexCommonLib
{
    public static class ClassExtentions
    {
        [TargetedPatchingOptOut("na")]
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        [TargetedPatchingOptOut("na")]
        public static string format(this string format, params object[] data)
        {
            return string.Format(format, data);
        }

        [TargetedPatchingOptOut("na")]
        public static string join(this string format, params object[] data)
        {
            return string.Join("", data);
        }

        /// <summary>
        /// Returns the string itself or empty if it's null to lower case
        /// </summary>
        /// <param name="string"></param>
        /// <returns></returns>
        public static string ToLowerSafe(this string @string)
        {
            if (@string == null)
                return string.Empty;
            return @string.ToLower();
        }

        [TargetedPatchingOptOut("na")]
        public static bool IsNullOrEmpty(this ICollection coll)
        {
            return coll == null || coll.Count == 0;
        }

        [TargetedPatchingOptOut("na")]
        public static bool Test(this SqlConnection conn, ref string message)
        {
            try
            {
                using (conn)
                    conn.Open();
                return true;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
        }

        [TargetedPatchingOptOut("na")]
        public static string[] ParseList(this IList<string> items, char splitter, int column)
        {
            List<string> list = new List<string>();
            foreach (string line in items)
            {
                Debug.Assert(!line.IsNullOrEmpty());
                string[] strings = line.Split(new char[] { splitter }, StringSplitOptions.RemoveEmptyEntries);
                Debug.Assert(column < strings.Length);
                list.Add(strings[column]);
            }
            return list.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="value"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="Exception">If name != null && value == null</exception>
        [TargetedPatchingOptOut("na")]
        public static U ConvertTo<T, U>(this T value, string name = null) where T : IConvertible
        {
            if (name != null && value == null)
                throw new Exception(name + " can not be null");
            try
            {
                return (U)Convert.ChangeType(value, typeof(U), CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                throw new Exception(name.join(" can not be converted to ", typeof(U)), ex);
            }
        }

        [TargetedPatchingOptOut("na")]
        public static T Cast<T>(this object obj)
        {
            return (T)obj;
        }

        [TargetedPatchingOptOut("na")]
        public static bool AddIfNotExists<T>(this List<T> list, T value, IComparer<T> comp)
        {
            lock (((ICollection)list).SyncRoot)
            {
                if (list.BinarySearch(value, comp) == -1)
                {
                    list.Add(value);
                    return true;
                }
                return false;
            }
        }

        [TargetedPatchingOptOut("na")]
        public static string GetFileNameInErrDir(this string errDir, string fileName)
        {
            Debug.Assert(errDir != null);
            Debug.Assert(fileName != null);

            return Path.Combine(errDir,
                string.Concat(Path.GetFileNameWithoutExtension(fileName), Guid.NewGuid(), Path.GetExtension(fileName)));
        }

        [TargetedPatchingOptOut("na")]
        public static IEnumerable<int> ToEnumerable(this string str)
        {
            if (str == null)
                throw new ArgumentNullException("str");

            string[] values = str.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var value in values)
                yield return int.Parse(value.Trim(), CultureInfo.InvariantCulture);
        }

        [TargetedPatchingOptOut("na")]
        public static XElement ElementThrow(this XContainer container, string name)
        {
            var node = container.Element(name);
            if (node == null)
                throw new Exception("Can not find " + name);
            return node;
        }

        [TargetedPatchingOptOut("na")]
        public static void DeleteAll<T, U>(this IDictionary<T, U> dict, Func<T, bool> deleteFunct)
        {
            Debug.Assert(dict != null);
            Debug.Assert(deleteFunct != null);

            foreach (T key in new List<T>(dict.Keys))
                if (deleteFunct(key))
                    dict.Remove(key);
        }
    }
}
