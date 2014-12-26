/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using VPrinting;

namespace MerchantService
{
    public class DataAccess
    {
        public static string TRSConnectionString { get; set; }
        public static string ImagesConnectionString { get; set; }

        static DataAccess()
        {
            TRSConnectionString = ConfigurationManager.ConnectionStrings["TRSConnectionString"].ConnectionString;
            ImagesConnectionString = ConfigurationManager.ConnectionStrings["ImagesConnectionString"].ConnectionString;
        }

        #region TRS

        public int TRSExecuteNonQuery(ArrayList sqlCommand)
        {
            return ExecuteNonQuery(TRSConnectionString, CreateCommand(sqlCommand));
        }

        public object TRSExecuteScalar(ArrayList sqlCommand)
        {
            return ExecuteScalar(TRSConnectionString, CreateCommand(sqlCommand));
        }

        public DataTable TRSExecuteReader(ArrayList sqlCommand)
        {
            return ExecuteReader(TRSConnectionString, CreateCommand(sqlCommand));
        }

        #endregion

        #region Images

        public int ImagesExecuteNonQuery(ArrayList sqlCommand)
        {
            return ExecuteNonQuery(ImagesConnectionString, CreateCommand(sqlCommand));
        }

        public object ImagesExecuteScalar(ArrayList sqlCommand)
        {
            return ExecuteScalar(ImagesConnectionString, CreateCommand(sqlCommand));
        }

        public DataTable ImagesExecuteReader(ArrayList sqlCommand)
        {
            return ExecuteReader(ImagesConnectionString, CreateCommand(sqlCommand));
        }

        public class SelectVoucherInfo_Data
        {
            public int iso_id { get; set; }
            public int branch_id { get; set; }
            public int v_number { get; set; }
            public string sitecode { get; set; }
            public int location { get; set; }
            public string session_Id { get; set; }

            public bool IsValid
            {
                get
                {
                    return iso_id > 0 && v_number > 0;
                }
            }

            public SelectVoucherInfo_Data()
            {
            }

            public SelectVoucherInfo_Data(DbDataReader reader)
            {
                iso_id = reader.Get<int>("iso_id").GetValueOrDefault();
                branch_id = reader.Get<int>("branch_id").GetValueOrDefault();
                v_number = reader.Get<int>("v_number").GetValueOrDefault();
                sitecode = reader.GetString("sitecode");
                location = reader.Get<int>("location").GetValueOrDefault();
                session_Id = reader.GetString("session_Id");
            }
        }

        public SelectVoucherInfo_Data SelectVoucherInfo(int Id)
        {
            const string SQL = "SELECT iso_id, branch_id, v_number, sitecode, location, session_Id  FROM Voucher WHERE Id = @Id";

            var list = new List<SelectVoucherInfo_Data>();

            using (var conn = new SqlConnection(ImagesConnectionString))
            {
                conn.Open();

                using (var comm = new SqlCommand(SQL, conn))
                {
                    comm.Parameters.AddWithValue("@Id", Id);
                    using (var reader = comm.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        if (reader.Read())
                            return new SelectVoucherInfo_Data(reader);
                    }
                }
            }

            return new SelectVoucherInfo_Data();
        }

        public SelectVoucherInfo_Data SelectVoucherInfo(int iso_id, int v_number)
        {
            const string SQL = "SELECT iso_id, branch_id, v_number, sitecode, location, session_Id  FROM Voucher WHERE iso_id = @iso_id and v_number=@v_number";

            using (var conn = new SqlConnection(ImagesConnectionString))
            {
                conn.Open();

                using (var comm = new SqlCommand(SQL, conn))
                {
                    comm.Parameters.AddWithValue("@iso_id", iso_id);
                    comm.Parameters.AddWithValue("@v_number", v_number);

                    using (var reader = comm.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        if (reader.Read())
                            return new SelectVoucherInfo_Data(reader);
                    }
                }
            }

            return new SelectVoucherInfo_Data();
        }

        #endregion

        private SqlCommand CreateCommand(ArrayList sqlCommand)
        {
            if (sqlCommand == null || sqlCommand.Count == 0)
                throw new ArgumentException("sqlCommand");

            var htable = sqlCommand.ToHashtable<string, object>();
            var command = htable.CreateCommand();
            return command;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connString"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        private int ExecuteNonQuery(string connString, SqlCommand command)
        {
            using (var conn = new SqlConnection(connString))
            using (command)
            {
                command.Connection = conn;
                command.CommandType = CommandType.Text;
                conn.Open();

                return command.ExecuteNonQuery();
            }
        }

        private object ExecuteScalar(string connString, SqlCommand command)
        {
            using (var conn = new SqlConnection(connString))
            using (command)
            {
                command.Connection = conn;
                command.CommandType = CommandType.Text;
                conn.Open();

                return command.ExecuteScalar();
            }
        }

        private DataTable ExecuteReader(string connString, SqlCommand command)
        {
            DataTable table = new DataTable();
            using (var conn = new SqlConnection(connString))
            using (command)
            {
                command.Connection = conn;
                conn.Open();

                using (var reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    table.Load(reader);
            }

            return table;
        }
    }
}