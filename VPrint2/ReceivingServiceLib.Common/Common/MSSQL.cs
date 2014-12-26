/***************************************************
//  Copyright (c) Premium Tax Free 2013
***************************************************/
/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace VPrinting
{
    /// <summary>
    /// MSSQL class
    /// </summary>
    [Obfuscation(StripAfterObfuscation = true)]
    public class MSSQL : SQL
    {
        public static MSSQL Instance { get { return new MSSQL(); } }

        /// <summary>
        /// Executes sql command as reader
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connString"></param>
        /// <param name="sql"></param>
        /// <param name="paramArray"></param>
        /// <returns></returns>
        [Obfuscation]
        public List<T> ExecuteReader<T>(string connString, string sql, params SqlParameter[] paramArray)
                where T : IReadable, new()
        {
            SqlCommand comm = CreateCommand(connString, sql, CommandType.Text, paramArray);
            return ExecuteReader<T>(comm);
        }

        /// <summary>
        /// Executes sql command as reader
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connString"></param>
        /// <param name="type"></param>
        /// <param name="sql"></param>
        /// <param name="paramArray"></param>
        /// <returns></returns>
        [Obfuscation]
        public List<T> ExecuteReader<T>(string connString, string sql, CommandType type, params SqlParameter[] paramArray)
        where T : IReadable, new()
        {
            SqlCommand comm = CreateCommand(connString, sql, type, paramArray);
            return ExecuteReader<T>(comm);
        }

        /// <summary>
        /// Executes sql command as non query
        /// </summary>
        /// <param name="connString"></param>
        /// <param name="sql"></param>
        /// <param name="paramArray"></param>
        /// <returns></returns>
        [Obfuscation]
        public int ExecuteNonQuery(string connString, string sql, CommandType type, params SqlParameter[] paramArray)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand comm = new SqlCommand(sql, conn);
            comm.CommandType = type;
            if (paramArray != null && paramArray.Length > 0)
                comm.Parameters.AddRange(paramArray);
            return ExecuteNonQuery(comm);
        }

        /// <summary>
        /// Executes sql command as scalar
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connString"></param>
        /// <param name="sql"></param>
        /// <param name="paramArray"></param>
        /// <returns></returns>
        [Obfuscation]
        public T ExecuteScalar<T>(string connString, string sql, CommandType type, params SqlParameter[] paramArray)
            where T : IConvertible
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();

                using (SqlCommand comm = new SqlCommand(sql, conn))
                {
                    comm.CommandType = type;
                    if (paramArray != null && paramArray.Length > 0)
                        comm.Parameters.AddRange(paramArray);

                    object result = comm.ExecuteScalar();
                    return (result != null && result != DBNull.Value) ?
                        (T)Convert.ChangeType(result, typeof(T)) : default(T);
                }
            }
        }

        /// <summary>
        /// Executes sql command as table
        /// </summary>
        /// <param name="connString"></param>
        /// <param name="tableName"></param>
        /// <param name="table"></param>
        [Obfuscation]
        public void ExecuteTable(string connString, string tableName, DataTable table)
        {
            using (SqlBulkCopy copy = new SqlBulkCopy(connString, SqlBulkCopyOptions.CheckConstraints))
            {
                copy.DestinationTableName = tableName;
                copy.WriteToServer(table);
            }
        }

        [Obfuscation]
        public SqlCommand CreateCommand(string connString, string sql, params SqlParameter[] paramArray)
        {
            return CreateCommand(connString, sql, CommandType.Text, paramArray);
        }

        /// <summary>
        /// Creates MSSQL Command object
        /// </summary>
        /// <param name="connString"></param>
        /// <param name="sql"></param>
        /// <param name="type"></param>
        /// <param name="paramArray"></param>
        /// <returns></returns>
        [Obfuscation]
        public SqlCommand CreateCommand(string connString, string sql, CommandType type, params SqlParameter[] paramArray)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand comm = new SqlCommand(sql, conn);
            comm.CommandType = type;
            if (paramArray.Length > 0)
                comm.Parameters.AddRange(paramArray);
            return comm;
        }

        /// <summary>
        /// Creates MSSQL Command object
        /// </summary>
        /// <param name="connString"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        [Obfuscation]
        public SqlCommand ReCreateCommand(string connString, Hashtable table)
        {
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
    }
}
