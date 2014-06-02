/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/

using System;
using System.Diagnostics;
using System.Text;

namespace PremierTaxFree.PTFLib
{
    public static class SystemEx
    {
        /// <summary>
        /// Converts an enum to string
        /// </summary>
        /// <param name="en"></param>
        /// <returns></returns>
        public static bool ToBool(this Enum en)
        {
            return Convert.ToBoolean(Convert.ToInt32(en));
        }

        /// <summary>
        /// Gets value from string by enum
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="en"></param>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static T ToValue<T>(this Enum en, params T[] arr)
        {
            int value = Convert.ToInt32(en);
            Debug.Assert(arr.Length > value);
            return arr[value];
        }

        /// <summary>
        /// Checks whether a value of T is in range
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool InRange<T>(this T value, T min, T max) where T : IComparable
        {
            return min.CompareTo(value) <= 0 && value.CompareTo(max) <= 0;
        }

        /// <summary>
        /// Returns a value which is not currently selected in a set of values
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static T Invert<T>(this T value, T v1, T v2) where T:IComparable
        {
            Debug.Assert(value != null);
            return value.CompareTo(v1) == 0 ? v2 : v1;
        }
        
        /// <summary>
        /// Returns a random string
        /// </summary>
        /// <param name="any"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string Random(this string @any, int length)
        {
            byte[] buffer = new byte[length];

            Random rnd = new Random(DateTime.Now.Millisecond);
            rnd.NextBytes(buffer);
            return UnicodeEncoding.Unicode.GetString(buffer);
        }

        /// <summary>
        /// Increase a value safely. Keeps the value in range of min, max values.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static int AddSf(this int value, int from, int to)
        {
            int newValue = value + 1;
            if (from <= newValue && newValue < to)
                return newValue;
            return value;
        }

        /// <summary>
        /// Decrease a value safely. Keeps the value in range of min, max values.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static int SubSf(this int value, int from, int to)
        {
            int newValue = value - 1;
            if (from <= newValue && newValue < to)
                return newValue;
            return value;
        }
    }
}
