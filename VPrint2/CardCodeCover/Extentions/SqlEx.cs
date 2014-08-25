/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Runtime;
using System.Reflection;

namespace CardCodeCover
{
    public static class SqlEx
    {
        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static Nullable<T> Get<T>(this SqlDataReader reader, int index) where T : struct
        {
            object value = reader.GetValue(index);
            if (value == DBNull.Value)
                return null;
            return (T)Convert.ChangeType(value, typeof(T));
        }

        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static Nullable<T> Get<T>(this SqlDataReader reader, string name) where T : struct
        {
            int index = reader.GetOrdinal(name);
            object value = reader.GetValue(index);
            if (value == DBNull.Value)
                return null;
            return (T)Convert.ChangeType(value, typeof(T));
        }

        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static string GetString(this SqlDataReader reader, string name)
        {
            int index = reader.GetOrdinal(name);
            object value = reader.GetValue(index);
            if (value == DBNull.Value)
                return null;
            return Convert.ToString(value);
        }

        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static object GetRaw(this SqlDataReader reader, string name)
        {
            int index = reader.GetOrdinal(name);
            object value = reader.GetValue(index);
            if (value == DBNull.Value)
                return null;
            return value;
        }

        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static object GetValue<T>(this Nullable<T> value) where T : struct
        {
            return value.HasValue ? (object)value.Value : DBNull.Value;
        }

        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static IEnumerable<T> ReadRange<T>(this SqlDataReader reader, Func<SqlDataReader, T> readFunct)
        {
            while (reader.Read())
                yield return readFunct(reader);
        }
    }
}
