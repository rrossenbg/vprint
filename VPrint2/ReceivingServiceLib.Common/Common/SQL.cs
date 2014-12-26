/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Reflection;

namespace VPrinting
{
    /// <summary>
    /// Common SQL class
    /// </summary>
    [Obfuscation(StripAfterObfuscation = true)]
    public class SQL
    {
        public SQL()
        {
        }
        /// <summary>
        /// Executes SqlDbCommand
        /// </summary>
        /// <param name="comm"></param>
        /// <returns></returns>
        [Obfuscation]
        public int ExecuteNonQuery(IDbCommand comm)
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
        [Obfuscation]
        public T ExecuteScalar<T>(IDbCommand comm)
        {
            Debug.Assert(comm != null);
            Debug.Assert(comm.Connection != null);

            using (IDbConnection conn = comm.Connection)
            {
                conn.Open();

                using (comm)
                {
                    object data = comm.ExecuteScalar();
                    if (data == null || data == DBNull.Value)
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
        [Obfuscation]
        public List<T> ExecuteReader<T>(IDbCommand comm) where T : IReadable, new()
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
        [Obfuscation]
        public IEnumerable<T> ExecuteYieldReader<T>(IDbCommand comm) where T : IReadable, new()
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
        [Obfuscation]
        public Hashtable CreateSerializationData(IDbCommand comm)
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