/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Text;
using System.Runtime;
using System.Diagnostics;
using System.Reflection;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

namespace DEMATLib
{
    public static class CoreEx
    {
        [TargetedPatchingOptOut("na")]
        public static T Cast<T>(this IConvertible value) 
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks>10:10 is equal to 10:10:00!!</remarks>
        [TargetedPatchingOptOut("na")]
        public static TimeSpan ToTimeSpan(this string value)
        {
            return TimeSpan.Parse(value);
        }

        [TargetedPatchingOptOut("na")]
        public static TimeSpan Minus(this DateTime date, TimeSpan value)
        {
            var t1 = date.Date.Add(value);
            if (t1 > date)
                return t1.Subtract(date);
            else
                return date.Subtract(t1);
        }

        [TargetedPatchingOptOut("na")]
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        [TargetedPatchingOptOut("na")]
        public static bool IsNullOrWhiteSpace(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="stringLength"></param>
        /// <returns></returns>
        /// <example>
        /// 1.ToString(10) == "0000000001"
        /// 1.ToString(1) == "1"
        /// 1.ToString(0) == "1"
        /// 1.ToString(-3) == "1"
        /// </example>
        [TargetedPatchingOptOut("na")]
        public static string ToString(this int value, int stringLength = 0, char fillChar = '0')
        {
            string str = value.ToString();
            var b = new StringBuilder();
            for (int i = 0; i < stringLength - str.Length; i++)
                b.Append(fillChar);
            b.Append(str);
            return b.ToString();
        }

        [TargetedPatchingOptOut("na")]
        public static string ToString(this long value, int stringLength = 0, char fillChar = '0')
        {
            string str = value.ToString();
            var b = new StringBuilder();
            for (int i = 0; i < stringLength - str.Length; i++)
                b.Append(fillChar);
            b.Append(str);
            return b.ToString();
        }

        [TargetedPatchingOptOut("na")]
        public static string ToString(this string value, int stringLength = 0, char fillChar = '0')
        {
            Debug.Assert(value != null);
            var b = new StringBuilder();
            for (int i = 0; i < stringLength - value.Length; i++)
                b.Append(fillChar);
            b.Append(value);
            return b.ToString();
        }

        [TargetedPatchingOptOut("na")]
        public static T ToEnum<T>(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("value may not be null or empty");
            return (T)Enum.Parse(typeof(T), value, true);
        }

        [TargetedPatchingOptOut("na")]
        public static string format(this string formatStr, params object[] values)
        {
            return string.Format(formatStr, values);
        }

        [TargetedPatchingOptOut("na")]
        public static string RemoveLast(this string str, int number = 1)
        {
            if (string.IsNullOrEmpty(str))
                throw new NotImplementedException();
            if (number > str.Length - 1)
                throw new ArgumentOutOfRangeException("number");
            return str.Substring(0, str.Length - number);
        }

        private static readonly int[] WEIGHTS = new int[] { 2, 3, 4, 5, 6, 7, 8, 9,
										             2, 3, 4, 5, 6, 7, 8, 9,
										             2, 3, 4, 5, 6, 7, 8, 9,
										             2, 3, 4, 5, 6, 7, 8, 9};

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Value + CheckDigit</returns>
        [TargetedPatchingOptOut("na")]
        public static int CheckDigit(this int value)
        {
            char[] base_val = value.ToString().ToCharArray();

            Array.Reverse(base_val);

            int i, sum;

            for (i = 0, sum = 0; i < base_val.Length; i++)
                sum += int.Parse(base_val[i].ToString()) * WEIGHTS[i];

            // Determine check digit.
            return (value * 10) + ((sum % 11) % 10);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns>CheckDigit</returns>
        [TargetedPatchingOptOut("na")]
        public static int CheckDigit(this string value)
        {
            Debug.Assert(!value.IsNullOrEmpty());

            char[] base_val = value.ToCharArray();

            Array.Reverse(base_val);

            long i, sum;

            for (i = 0, sum = 0; i < base_val.Length; i++)
            {
                int v = 0;

                bool result = int.TryParse(base_val[i].ToString(), out v);

                Debug.Assert(result, "Cannot parse value");

                sum += v * WEIGHTS[i];
            }

            // Determine check digit.
            return (int)((sum % 11) % 10);
        }

        [TargetedPatchingOptOut("na")]
        public static string FromObject<T>(this XmlSerializer serializer, T value)
        {
            var builder = new StringBuilder();
            using (var str = XmlWriter.Create(new StringWriter(builder)))
            {
                Debug.Assert(str != null);
                serializer.Serialize(str, value);
                return builder.ToString();
            }
        }

        [TargetedPatchingOptOut("na")]
        public static T ToObject<T>(this string text)
        {
            Debug.Assert(!string.IsNullOrEmpty(text));
            XmlSerializer formatter = new XmlSerializer(typeof(T));
            using (var str = XmlReader.Create(new StringReader(text)))
            {
                Debug.Assert(str != null);
                return (T)formatter.Deserialize(str);
            }
        }

        [TargetedPatchingOptOut("na")]
        public static byte[] FromObject<T>(this T value)
        {
            using(var mem = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(mem, value);
                return mem.ToArray();
            }
        }

        [TargetedPatchingOptOut("na")]
        public static T ToObject<T>(this byte[] buffer) 
        {
            if (buffer == null || buffer.Length == 0)
                return default(T);

            using (var mem = new MemoryStream(buffer))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return (T)formatter.Deserialize(mem);
            }
        }

        [TargetedPatchingOptOut("na")]
        public static bool IsNullOrEmpty<T>(this IList<T> list)
        {
            if (list == null)
                return true;
            return list.Count == 0;
        }
    }
}
