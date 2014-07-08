/***************************************************
//  Copyright (c) Premium Tax Free 2012
***************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Runtime;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace BtRetryService
{
    public static class ClassEx
    {
        [TargetedPatchingOptOut("na")]
        public static byte[] toArray(this string value)
        {
            return Convert.FromBase64String(value);
        }

        [TargetedPatchingOptOut("na")]
        public static string toString(this byte[] value)
        {
            return Convert.ToBase64String(value);
        }

        [TargetedPatchingOptOut("na")]
        public static T cast<T>(this object value)
        {
            return (T)value;
        }

        [TargetedPatchingOptOut("na")]
        public static T Cast<T>(this string value) where T : IComparable
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value can not be null or empty", "value");

            return (T)Convert.ChangeType(value, typeof(T));
        }

        [TargetedPatchingOptOut("na")]
        public static Nullable<T> TryParse<T>(this string value) where T : struct
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value can not be null or empty", "value");

            switch (typeof(T).FullName)
            {
                case "System.TimeSpan":
                    {
                        TimeSpan t;
                        if (TimeSpan.TryParse(value, out t))
                            return (T?)(object)t;
                        return null;
                    }
                case "System.DateTime":
                    {
                        DateTime t;
                        if (DateTime.TryParse(value, out t))
                            return (T?)(object)t;
                        return null;
                    }
                default:
                    throw new NotImplementedException();
            }
        }

        [TargetedPatchingOptOut("na")]
        public static Nullable<T> Get<T>(this SqlDataReader reader, string name) where T : struct
        {
            int index = reader.GetOrdinal(name);
            object value = reader.GetValue(index);
            if (value == DBNull.Value)
                return null;
            return (T)Convert.ChangeType(value, typeof(T));
        }

        [TargetedPatchingOptOut("na")]
        public static string GetString(this SqlDataReader reader, string name)
        {
            int index = reader.GetOrdinal(name);
            object value = reader.GetValue(index);
            if (value == DBNull.Value)
                return null;
            return Convert.ToString(value);
        }

        [TargetedPatchingOptOut("na")]
        public static Nullable<T> Get<T>(this SqlDataReader reader, int index) where T : struct
        {
            object value = reader.GetValue(index);
            if (value == DBNull.Value)
                return null;
            return (T)Convert.ChangeType(value, typeof(T));
        }

        [TargetedPatchingOptOut("na")]
        public static IEnumerable<T> ReadRange<T>(this SqlDataReader reader, Func<SqlDataReader, T> readFunct)
        {
            while (reader.Read())
                yield return readFunct(reader);
        }

        [TargetedPatchingOptOut("na")]
        public static bool Match(this string value, string regex)
        {
            Regex re = new Regex(regex, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Singleline);
            return re.IsMatch(value);
        }

        [TargetedPatchingOptOut("na")]
        public static string Encode(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;
            StringBuilder b = new StringBuilder(value);
            b.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;");
            return b.ToString();
        }

        [TargetedPatchingOptOut("na")]
        public static string Unique(this string str)
        {
            var u = Guid.NewGuid().ToString().Replace('-', '_');
            return string.Concat(str, u);
        }

        [TargetedPatchingOptOut("na")]
        public static string TrimSafe(this string value, string @default = null)
        {
            if (value != null)
                return value.Trim();
            return @default;
        }

        [TargetedPatchingOptOut("na")]
        public static bool Exists<T, U>(this IEnumerable<T> rules, IEnumerable<U> items, Func<T, U, bool> evaluateFunc, out T activeRule)
        {
            activeRule = default(T);

            foreach (var rule in rules)
            {
                foreach (var item in items)
                {
                    if (evaluateFunc(rule, item))
                    {
                        activeRule = rule;
                        return true;
                    }
                }
            }
            return false;
        }

        [TargetedPatchingOptOut("na")]
        public static string toString(this CollectionBase coll)
        {
            StringBuilder b = new StringBuilder();
            foreach (var item in coll)
            {
                b.Append(item);
                b.AppendLine();
            }
            return b.ToString();
        }

        [TargetedPatchingOptOut("na")]
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        [TargetedPatchingOptOut("na")]
        public static string format(this string value, params object[] param)
        {
            return string.Format(value, param);
        }

        [TargetedPatchingOptOut("na")]
        public static string concat(this string str, params object[] values)
        {
            return string.Concat(str, string.Concat(values));
        }

        /// <summary>
        /// Returns yesterday 0am
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        [TargetedPatchingOptOut("na")]
        public static DateTime Yesterday(this DateTime date)
        {
            return DateTime.Today.Date.AddDays(-1);
        }

        [TargetedPatchingOptOut("na")]
        public static void WriteLine(this TextWriter writter, IList<string> items)
        {
            foreach (var item in items)
                writter.WriteLine(item);
        }

        [TargetedPatchingOptOut("na")]
        public static void Protect(this byte[] data)
        {
            ProtectedMemory.Protect(data, MemoryProtectionScope.CrossProcess);
        }

        [TargetedPatchingOptOut("na")]
        public static void Unprotect(this byte[] data)
        {
            ProtectedMemory.Unprotect(data, MemoryProtectionScope.CrossProcess);
        }

        [TargetedPatchingOptOut("na")]
        public static Nullable<T> ThrowIfNull<T>(this Nullable<T> t, string message) where T: struct
        {
            if (!t.HasValue)
                throw new Exception(message);
            return t;
        }

        [TargetedPatchingOptOut("na")]
        public static bool In(this int value, params int[] values)
        {
            foreach (var v in values)
                if (value == v)
                    return true;
            return false;
        }

        [TargetedPatchingOptOut("na")]
        public static bool In<T>(this T value, IEnumerable<T> values, Func<T, T, bool> compFunc, out T r)
        {
            foreach (var v in values)
            {
                if (compFunc(value, v))
                {
                    r = v;
                    return true;
                }
            }
            r = default(T);
            return false;
        }

        [TargetedPatchingOptOut("na")]
        public static string FirstOf(this string value, int count)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            if (value.Length < count)
                return value;

            return value.Substring(0, count);
        }
    }
}
