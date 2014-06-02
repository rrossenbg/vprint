/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/

using System;

namespace PremierTaxFree.PTFLib
{
    public static class CoreEx
    {
        /// <summary>
        /// Ckecks whether value is between two other values
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool IsBetween(this int value, int min, int max)
        {
            return min < value && value < max;
        }

        /// <summary>
        /// Ckecks whether value is between two other values
        /// </summary>
        /// <param name="value"></param>
        /// <param name="fromdate"></param>
        /// <param name="todate"></param>
        /// <returns></returns>
        public static bool IsBetween(this DateTime value, DateTime fromdate, DateTime todate)
        {
            return fromdate <= value && value <= todate;
        }

        /// <summary>
        /// Returns random number between min and max
        /// </summary>
        /// <param name="minimum"></param>
        /// <param name="maximum"></param>
        /// <returns></returns>
        public static double Between(this double minimum, double maximum)
        {
            Random random = new Random();
            return random.NextDouble() * (maximum - minimum) + minimum;
        }

        /// <summary>
        /// value? t1 : t2;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        public static T If<T>(this bool value, T t1, T t2)
        {
            return value ? t1 : t2;
        }

        /// <summary>
        /// Returns the default for type of T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T Default<T>(this T value)
        {
            return default(T);
        }

        /// <summary>
        /// Checks whether a time period has been expired 
        /// </summary>
        /// <param name="time"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        public static bool IsExpired(this DateTime time, TimeSpan interval)
        {
            return time.Add(interval) < DateTime.Now;
        }
    }
}
