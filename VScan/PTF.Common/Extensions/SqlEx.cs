/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/

using System;
using System.Data.SqlClient;

namespace PremierTaxFree.PTFLib
{
    public static class SqlEx
    {
        /// <summary>
        /// Tests sql connection by opening it. Returns the error if any. Disposes the connection object.
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static string TestSf(this SqlConnection conn)
        {
            try
            {
                using (conn)
                    conn.Open();
                return null;
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Returns a string from a command object
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public static string ToSqlString(this SqlCommand cmd)
        {
            string query = cmd.CommandText;
            foreach (SqlParameter p in cmd.Parameters)
                query = query.Replace(p.ParameterName, p.Value.ToString());
            return query;
        }
    }
}
