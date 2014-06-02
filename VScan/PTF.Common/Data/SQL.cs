/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using PremierTaxFree.PTFLib.Sys;

namespace PremierTaxFree.PTFLib.Data
{
    public interface IReadable
    {
        void Load(IDataReader reader);
    }

    public interface IBinaryReadable
    {
        void Read(BinaryReader stream);
    }

    /// <summary>
    /// MSSQL class
    /// </summary>
    public static class MSSQL
    {
        /// <summary>
        /// Executes sql command as reader
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connString"></param>
        /// <param name="sql"></param>
        /// <param name="paramArray"></param>
        /// <returns></returns>
        public static List<T> ExecuteReader<T>(string connString, string sql, params SqlParameter[] paramArray)
                where T : IReadable, new()
        {
            SqlCommand comm = CreateCommand(connString, sql, CommandType.Text, paramArray);
            return SQL.ExecuteReader<T>(comm);
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
        public static List<T> ExecuteReader<T>(string connString, CommandType type, string sql, params SqlParameter[] paramArray)
        where T : IReadable, new()
        {
            SqlCommand comm = CreateCommand(connString, sql, type, paramArray);
            return SQL.ExecuteReader<T>(comm);
        }

        /// <summary>
        /// Executes sql command as non query
        /// </summary>
        /// <param name="connString"></param>
        /// <param name="sql"></param>
        /// <param name="paramArray"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string connString, string sql, params SqlParameter[] paramArray)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand comm = new SqlCommand(sql, conn);
            if (paramArray != null && paramArray.Length > 0)
                comm.Parameters.AddRange(paramArray);
            return SQL.ExecuteNonQuery(comm);
        }

        /// <summary>
        /// Executes sql command as scalar
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connString"></param>
        /// <param name="sql"></param>
        /// <param name="paramArray"></param>
        /// <returns></returns>
        public static T ExecuteScalar<T>(string connString, string sql, params SqlParameter[] paramArray) 
            where T : struct
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();

                using (SqlCommand comm = new SqlCommand(sql, conn))
                {
                    if (paramArray != null && paramArray.Length > 0)
                        comm.Parameters.AddRange(paramArray);

                    return (T)comm.ExecuteScalar();
                }
            }
        }

        /// <summary>
        /// Executes sql command as table
        /// </summary>
        /// <param name="connString"></param>
        /// <param name="tableName"></param>
        /// <param name="table"></param>
        public static void ExecuteTable(string connString, string tableName, DataTable table)
        {
            using (SqlBulkCopy copy = new SqlBulkCopy(connString, SqlBulkCopyOptions.CheckConstraints))
            {
                copy.DestinationTableName = tableName;
                copy.WriteToServer(table);
            }
        }

        public static SqlCommand CreateCommand(string connString, string sql, params SqlParameter[] paramArray)
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
        public static SqlCommand CreateCommand(string connString, string sql, CommandType type, params SqlParameter[] paramArray)
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
        public static SqlCommand CreateCommand(string connString, Hashtable table)
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

        /// <summary>
        /// Executes a file as osql command batch
        /// </summary>
        /// <param name="datasource"></param>
        /// <param name="path"></param>
        /// <param name="timeout">timeout miliseconds</param>
        public static void ExecuteSqlFile(string datasource, string path, int timeout)
        {
            string command = string.Format(@"osql -E -S {0} -i ""{1}""", datasource, path);
            if (OS.ExecuteCommand(command, timeout) != 0)
                throw new ApplicationException();
        }

        /// <summary>
        /// Executes a file as osql command batch
        /// </summary>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <param name="datasource"></param>
        /// <param name="path"></param>
        /// <param name="timeout">timeout miliseconds</param>
        public static void ExecuteSqlFile(string user, string pass, string datasource, string path, int timeout)
        {
            string command = string.Format(@"osql -U {0} -P {1} -S {2} -i ""{3}""", user, pass, datasource, path);
            if (OS.ExecuteCommand(command, timeout) != 0)
                throw new ApplicationException();
        }
    }

    /// <summary>
    /// Common SQL class
    /// </summary>
    public static class SQL
    {
        /// <summary>
        /// Executes SqlDbCommand
        /// </summary>
        /// <param name="comm"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(IDbCommand comm)
        {
            Debug.Assert(comm != null);
            Debug.Assert(comm.Connection != null);

            using (IDbConnection conn = comm.Connection)
            {
                conn.Open();

                using (comm)
                {
                    return comm.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Executes SqlDbCommand as scalar
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="comm"></param>
        /// <returns></returns>
        public static T ExecuteScalar<T>(IDbCommand comm)
        {
            Debug.Assert(comm != null);
            Debug.Assert(comm.Connection != null);

            using (IDbConnection conn = comm.Connection)
            {
                conn.Open();

                using (comm)
                {
                    object data = comm.ExecuteScalar();
                    if (data == DBNull.Value)
                        return default(T);
                    return (T)Convert.ChangeType(data, typeof(T));
                }
            }
        }

        /// <summary>
        /// Executes SqlDbCommand as reader
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="comm"></param>
        /// <returns></returns>
        public static List<T> ExecuteReader<T>(IDbCommand comm) where T : IReadable, new()
        {
            Debug.Assert(comm != null);
            Debug.Assert(comm.Connection != null);

            using (IDbConnection conn = comm.Connection)
            {
                conn.Open();

                using (comm)
                {
                    List<T> results = new List<T>();
                    IDataReader reader = comm.ExecuteReader();
                    while (reader.Read())
                    {
                        T t = new T();
                        t.Load(reader);
                        results.Add(t);
                    }
                    return results;
                }
            }
        }

        /// <summary>
        /// Executes SqlDbCommand as yield reader
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="comm"></param>
        /// <returns></returns>
        public static IEnumerable<T> ExecuteYieldReader<T>(IDbCommand comm) where T : IReadable, new()
        {
            Debug.Assert(comm != null);
            Debug.Assert(comm.Connection != null);

            using (IDbConnection conn = comm.Connection)
            {
                conn.Open();

                using (comm)
                {
                    IDataReader reader = comm.ExecuteReader();
                    while (reader.Read())
                    {
                        T t = new T();
                        t.Load(reader);
                        yield return t;
                    }
                }
            }
        }

        /// <summary>
        /// Saves IDbCommand in Hashtable
        /// </summary>
        /// <param name="comm">This parameter may not be null</param>
        public static Hashtable CreateSerializationData(IDbCommand comm)
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
    }
}