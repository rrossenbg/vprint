/***************************************************
//  Copyright (c) Premium Tax Free 2012
/***************************************************/

using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Runtime;
using System.Reflection;
using System.Collections.Generic;

namespace SiteCodeLib
{
    [Obfuscation(ApplyToMembers = true)]
    public static class ClassEx
    {
        [Obfuscation]
        [TargetedPatchingOptOut("na")]
        public static T Get<T>(this object data) where T : IConvertible
        {
            if (data == null || data == DBNull.Value)
                return default(T);

            return (T)Convert.ChangeType(data, typeof(T));
        }

        [Obfuscation]
        [TargetedPatchingOptOut("na")]
        public static T? GetNull<T>(this object data) where T : struct, IConvertible
        {
            if (data == null || data == DBNull.Value)
                return null;

            return (T)Convert.ChangeType(data, typeof(T));
        }

        [Obfuscation]
        [TargetedPatchingOptOut("na")]
        public static Hashtable CreateSerializationData(this IDbCommand comm)
        {
            Debug.Assert(comm != null);

            Hashtable table = new Hashtable();
            table.Add("<sql>", comm.CommandText);
            table.Add("<type>", comm.CommandType);
            table.Add("<timeout>", comm.CommandTimeout);

            foreach (IDbDataParameter p in comm.Parameters)
                table.Add(p.ParameterName, p.Value);

            return table;
        }

        [Obfuscation]
        [TargetedPatchingOptOut("na")]
        public static SqlCommand CreateCommand(this Hashtable table, string connString)
        {
            Debug.Assert(table != null);

            SqlConnection conn = new SqlConnection(connString);
            string sql = Convert.ToString(table["<sql>"]);
            CommandType type = (CommandType)table["<type>"];
            int timeout = Convert.ToInt32(table["<timeout>"]);
            SqlCommand comm = new SqlCommand(sql, conn);
            comm.CommandType = type;
            comm.CommandTimeout = timeout;
            foreach (DictionaryEntry en in table)
            {
                string name = Convert.ToString(en.Key);
                if (string.Equals(name, "<sql>") || string.Equals(name, "<type>") || string.Equals(name, "<timeout>"))
                    continue;
                comm.Parameters.AddWithValue(name, en.Value);
            }
            table.Clear();
            return comm;
        }

        [Obfuscation]
        [TargetedPatchingOptOut("na")]
        public static string RemoveStart(this string value, int count)
        {
            if (string.IsNullOrEmpty(value))
                return value;
            else if (value.Length > count)
                return value.Remove(0, count);
            return value;
        }
    }
}
