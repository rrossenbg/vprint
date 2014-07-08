/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Runtime;

namespace DEMATLib
{
    public static class SqlEx
    {
        [TargetedPatchingOptOut("na")]
        public static Nullable<T> GetNull<T>(this SqlDataReader reader, int index) where T : struct
        {
            object value = reader.GetValue(index);
            if (value == DBNull.Value)
                return null;
            return (T)Convert.ChangeType(value, typeof(T));
        }

        [TargetedPatchingOptOut("na")]
        public static Nullable<T> GetNull<T>(this object value) where T : struct
        {
            if (value == DBNull.Value)
                return null;
            return (T)Convert.ChangeType(value, typeof(T));
        }

        [TargetedPatchingOptOut("na")]
        public static Nullable<T> GetNull<T>(this SqlDataReader reader, string name) where T : struct
        {
            int index = reader.GetOrdinal(name);
            object value = reader.GetValue(index);
            if (value == DBNull.Value)
                return null;
            return (T)Convert.ChangeType(value, typeof(T));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException if value is null"></exception>
        [TargetedPatchingOptOut("na")]
        public static T Get<T>(this SqlDataReader reader, int index) where T : struct
        {
            object value = reader.GetValue(index);
            if (value == DBNull.Value)
                throw new ArgumentNullException(string.Format("field number {0} is null", index));
            return (T)Convert.ChangeType(value, typeof(T));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException if value is null"></exception>
        [TargetedPatchingOptOut("na")]
        public static T Get<T>(this SqlDataReader reader, string name) where T : struct
        {
            int index = reader.GetOrdinal(name);
            object value = reader.GetValue(index);
            if (value == DBNull.Value)
                throw new ArgumentNullException(string.Format("field '{0}' is null", name));
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
        public static IEnumerable<T> ReadRange<T>(this SqlDataReader reader, Func<SqlDataReader, T> readFunct)
        {
            while (reader.Read())
                yield return readFunct(reader);
        }
    }
}
