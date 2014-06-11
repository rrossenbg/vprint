/***************************************************
//  Copyright (c) Premium Tax Free 2012
/***************************************************/

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace PTF.Reports
{
    public static class CoreEx
    {
        public static T GetValue<T>(this Nullable<T> t, T @defaultForNull)
            where T : struct
        {
            if (!t.HasValue)
                return @defaultForNull;
            return t.Value;
        }

        /// <summary>
        /// return (T)Convert.ChangeType(obj, typeof(T));
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T Cast2<T>(this object obj)
        {
            return (T)Convert.ChangeType(obj, typeof(T));
        }

        /// <summary>
        /// return (T)obj;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T Cast<T>(this object obj)
        {
            return (T)obj;
        }

        /// <summary>
        /// return (T)obj;
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

        public static string ToXml(this object value)
        {
            Debug.Assert(value != null);
            Type t = value.GetType();
            XmlSerializer serializer = new XmlSerializer(t);
            using (var mem = new MemoryStream())
            {
                serializer.Serialize(mem, value);
                return Encoding.UTF8.GetString(mem.ToArray());
            }
        }

        public static string GetEnumName<T>(this int value)
        {
            return Enum.GetName(typeof(T), value);
        }
    }
}
