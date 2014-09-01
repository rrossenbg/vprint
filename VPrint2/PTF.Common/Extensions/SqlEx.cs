using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;
using System.Runtime;

namespace VPrinting
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
        [TargetedPatchingOptOut("na")]
        public static Hashtable CreateSerializationData(this IDbCommand comm)
        {
            Debug.Assert(comm != null);

            Hashtable table = new Hashtable();
            table.Add("<sql>", comm.CommandText);
            table.Add("<type>", comm.CommandType);
            table.Add("<timeout>", comm.CommandTimeout);
            table.Add("<key>", DateTime.Now);

            foreach (IDbDataParameter p in comm.Parameters)
                table.Add(p.ParameterName, p.Value);

            return table;
        }
    }
}
