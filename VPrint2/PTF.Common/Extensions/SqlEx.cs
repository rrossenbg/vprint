/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;
using System.Runtime;
using System.Data.Common;

namespace VPrinting
{
    public static class SqlEx
    {
        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static Nullable<T> Get<T>(this DbDataReader reader, int index) where T : struct
        {
            object value = reader.GetValue(index);
            if (value == DBNull.Value)
                return null;
            return (T)Convert.ChangeType(value, typeof(T));
        }

        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static Nullable<T> Get<T>(this DbDataReader reader, string name) where T : struct
        {
            int index = reader.GetOrdinal(name);
            object value = reader.GetValue(index);
            if (value == DBNull.Value)
                return null;
            return (T)Convert.ChangeType(value, typeof(T));
        }

        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static object GetRaw(this DbDataReader reader, string name)
        {
            int index = reader.GetOrdinal(name);
            object value = reader.GetValue(index);
            if (value == DBNull.Value)
                return null;
            return value;
        }

        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static string GetString(this DbDataReader reader, string name)
        {
            int index = reader.GetOrdinal(name);
            object value = reader.GetValue(index);
            if (value == DBNull.Value)
                return null;
            return Convert.ToString(value);
        }

        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static object GetValue<T>(this Nullable<T> value) where T : struct
        {
            return value.HasValue ? (object)value.Value : DBNull.Value;
        }

        [TargetedPatchingOptOut("na")]
        [Obfuscation]
        public static IEnumerable<T> ReadRange<T>(this DbDataReader reader, Func<DbDataReader, T> readFunct)
        {
            while (reader.Read())
                yield return readFunct(reader);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="comm"></param>
        /// <returns></returns>
        /// using (SqlCommand cmd = new SqlCommand("update voucher set v_vsr_number=@a where v_iso_id=@iso and v_number=@n"))
        /// {
        ///     cmd.Parameters.AddWithValue("@a", "123");
        ///     cmd.Parameters.AddWithValue("@iso", 752);
        ///     cmd.Parameters.AddWithValue("@n", 24);
        ///     var cl = new ServiceReference1.PartyManagementSoapClient();
        ///     cl.UpdateTableData(new ServiceReference1.AuthenticationHeader(), cmd.CreateSerializationData().ToList().ToArray());
        /// }
        /// 
        /// [WebMethod]
        /// [SoapHeader("Authentication")]
        /// public int UpdateTableData(ArrayList table)
        /// {
        ///     if (table == null || table.Count == 0)
        ///         throw new ArgumentException("table");
            
        ///     var htable = table.ToHashtable<string, object>();
        ///     return DiData.Ptf.Business.PartyManagement.UpdateTableData(htable);
        /// }
        ///
        /// public static int UpdateTableData(Hashtable table)
        /// {
        ///     using (var conn = Database.CreateConnection("WSDBConn"))
        ///     using (var comm = CreateCommand(conn, table))
        ///     {
        ///         conn.Open();
        ///         return comm.ExecuteNonQuery();
        ///     }
        /// }
        [TargetedPatchingOptOut("na")]
        public static Hashtable CreateSerializationData(this IDbCommand comm)
        {
            Debug.Assert(comm != null);

            Hashtable table = new Hashtable();
            table.Add("<sql>", comm.CommandText);
            table.Add("<type>", (int)comm.CommandType);
            table.Add("<timeout>", comm.CommandTimeout);
            table.Add("<key>", DateTime.Now);

            foreach (IDbDataParameter p in comm.Parameters)
                table.Add(p.ParameterName, p.Value);

            return table;
        }

        /// <summary>
        /// Creates MSSQL Command object
        /// </summary>
        /// <param name="connString"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        public static SqlCommand CreateCommand(this Hashtable table)
        {
            if (!table.ContainsKey("<key>"))
                throw new Exception("Not authorized");

            DateTime date = DateTime.MinValue;
            if (!DateTime.TryParse(Convert.ToString(table["<key>"]), out date) || date.Date != DateTime.Now.Date)
                throw new Exception("Not authorized");

            string sql = Convert.ToString(table["<sql>"]);
            CommandType type = (CommandType)(int)table["<type>"];
            int timeout = Convert.ToInt32(table["<timeout>"]);
            SqlCommand comm = new SqlCommand(sql);
            comm.CommandType = type;
            comm.CommandTimeout = timeout;
            foreach (DictionaryEntry en in table)
            {
                string name = Convert.ToString(en.Key);
                if (string.Equals(name, "<sql>") || string.Equals(name, "<type>") || string.Equals(name, "<timeout>") || string.Equals(name, "<key>"))
                    continue;
                comm.Parameters.AddWithValue(name, en.Value);
            }
            table.Clear();
            return comm;
        }
    }
}
