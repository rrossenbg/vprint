/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/

using System;
using System.Collections.Generic;

namespace PremierTaxFree.PTFLib
{
    public static class ObjectEx
    {
        /// <summary>
        /// Converts to string safely
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToStringSf(this object value)
        {
            return Convert.ToString(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// <example>this.BackgroundImage = this.BackgroundImage.DisposeSf();</example>
        public static T DisposeSf<T>(this T obj) where T : class, IDisposable
        {
            using (obj) ;
            return default(T);
        }

        /// <summary>
        /// (T)obj;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <example>
        /// object o = ReturnAnonymous();
        /// var typed = o.Cast(new { City = "", Name = "" });
        /// Console.WriteLine("Name={0}, City={1}", typed.Name, typed.City);
        /// </example>
        public static T Cast<T>(this object obj, T type)
        {
            return (T)obj;
        }

        /// <summary>
        /// (T)Convert.ChangeType(obj, typeof(T));
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T Cast<T>(this object obj) where T : IConvertible
        {
            if (obj == null || obj == DBNull.Value)
                return default(T);
            return (T)Convert.ChangeType(obj, typeof(T));
        }

        /// <summary>
        /// Checks whether the object is null.
        /// If it is, it throws an exception.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T ThrowIfNull<T, E>(this T obj)
            where T : class
            where E : Exception, new()
        {
            if (obj == null)
                throw new E();
            return obj;
        }

        /// <summary>
        /// Checks whether the object is null.
        /// If it is, it throws an exception.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T ThrowIfDefault<T, E>(this T obj)
            where T : IComparable<T>
            where E : Exception, new()
        {
            if (obj.CompareTo(default(T)) == 0)
                throw new E();
            return obj;
        }

        /// <summary>
        /// Throws exception with type of E if value is false
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool ThrowIfFalse<E>(this bool obj)
            where E : Exception, new()
        {
            if (!obj) 
                throw new E();
            return obj;
        }
    }
}
