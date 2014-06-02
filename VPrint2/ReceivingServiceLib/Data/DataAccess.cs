/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

#define USE_INSERT

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace ReceivingServiceLib.Data
{
    /// <summary>
    /// WARNING: PREPARE SQL SERVER
    /// -----------------------------
    /// -- Reset the "allow updates" setting to the recommended 0
    /// sp_configure 'allow updates',0;
    /// reconfigure with override
    /// go
    /// sp_configure 'show advanced options',0;
    /// reconfigure
    /// go
    /// EXEC sp_configure filestream_access_level, 2
    /// RECONFIGURE
    /// go
    /// </summary>
    public class DataAccess : BaseDataAccess 
    {
        public static DataAccess Instance
        {
            get
            {
                return new DataAccess();
            }
        }

        #region HISTORY

        public void SaveHistory(int operCountryId, int operUserId, int operType, Guid oper_uniq_id, int br_iso_id, int br_id, int v_id, int v2_id, int count, string details)
        {
            CheckConnectionStringThrow();

            #region SQL

            const string SQL = @"INSERT INTO [History] ([h_iso_id],[h_user_id],[h_datetime],[h_operation],[h_br_iso_id],[h_br_id],[h_v_id],[h_v2_id],[h_count],[h_uniq_id],[h_details])
                                 VALUES (@iso_id, @user_id, getdate(), @operation, @br_iso_id, @br_id, @v_id, @v2_id, @count, @uniq_id, @details)";
            #endregion

            using (var conn = new SqlConnection(Global.Strings.ConnString))
            {
                conn.Open();

                using (var comm = new SqlCommand(SQL, conn))
                {
                    comm.CommandType = CommandType.Text;
                    comm.Parameters.AddWithValue("@iso_id", operCountryId);
                    comm.Parameters.AddWithValue("@user_id", operUserId);
                    comm.Parameters.AddWithValue("@operation", operType);
                    comm.Parameters.AddWithValue("@br_iso_id", br_iso_id);
                    comm.Parameters.AddWithValue("@br_id", br_id);
                    comm.Parameters.AddWithValue("@v_id", v_id);
                    comm.Parameters.AddWithValue("@v2_id", v2_id);
                    comm.Parameters.AddWithValue("@count", count);
                    comm.Parameters.AddWithValue("@uniq_id", oper_uniq_id);
                    comm.Parameters.AddWithValue("@details", details);

                    comm.ExecuteNonQuery();
                }
            }
        }

        public class HistoryByCountryData
        {
            public int Index { get; set; }
            public int h_iso_id { get; set; }
            public int h_user_id { get; set; }
            public DateTime h_datetime { get; set; }
            public OperationHistory h_operation { get; set; }
            public int h_br_iso_id { get; set; }
            public int h_br_id { get; set; }
            public int h_v_id { get; set; }
            public int h_v2_id { get; set; }
            public int h_count { get; set; }
            public Guid h_uniq_id { get; set; }
            public string h_details { get; set; }
        }

        public List<HistoryByCountryData> SelectHistoryByCountryAndOperator(int operCountryId, int? operUserId, int operation, DateTime from, DateTime to)
        {
            CheckConnectionStringThrow();

            #region SQL

            string SQL = operUserId.HasValue ?
                @"SELECT TOP 1000 ROW_NUMBER ( ) OVER(ORDER BY h_id ASC)  AS 'Index', [h_iso_id],[h_user_id],[h_datetime],[h_operation],[h_br_iso_id],[h_br_id],[h_v_id],[h_v2_id],[h_count],[h_uniq_id],[h_details] 
                  FROM History WHERE h_iso_id = @iso_id and h_user_id = @user_id and h_operation = @operation and h_datetime between @from and @to;" :

                @"SELECT TOP 1000 ROW_NUMBER ( ) OVER(ORDER BY h_id ASC)  AS 'Index', [h_iso_id],[h_user_id],[h_datetime],[h_operation],[h_br_iso_id],[h_br_id],[h_v_id],[h_v2_id],[h_count],[h_uniq_id],[h_details] 
                  FROM History WHERE h_iso_id = @iso_id and h_operation = @operation and h_datetime between @from and @to;";
            #endregion

            var list = new List<HistoryByCountryData>();

            using (var conn = new SqlConnection(Global.Strings.ConnString))
            {
                conn.Open();

                using (var comm = new SqlCommand(SQL, conn))
                {
                    comm.CommandType = CommandType.Text;
                    comm.Parameters.AddWithValue("@iso_id", operCountryId);
                    comm.Parameters.AddWithValue("@user_id", operUserId.GetValueOrDefault());
                    comm.Parameters.AddWithValue("@operation", operation);
                    comm.Parameters.AddWithValue("@from", from);
                    comm.Parameters.AddWithValue("@to", to);

                    using (var reader = comm.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            var data = new HistoryByCountryData()
                            {
                                Index = reader.Get<int>("Index").GetValueOrDefault(),
                                h_iso_id = reader.Get<int>("h_iso_id").GetValueOrDefault(),
                                h_user_id = reader.Get<int>("h_user_id").GetValueOrDefault(),
                                h_datetime = reader.Get<DateTime>("h_datetime").GetValueOrDefault(),
                                h_operation = (OperationHistory)reader.Get<int>("h_operation").GetValueOrDefault(),

                                h_br_iso_id = reader.Get<int>("h_br_iso_id").GetValueOrDefault(),
                                h_br_id = reader.Get<int>("h_br_id").GetValueOrDefault(),
                                h_v_id = reader.Get<int>("h_v_id").GetValueOrDefault(),
                                h_v2_id = reader.Get<int>("h_v2_id").GetValueOrDefault(),
                                h_count = reader.Get<int>("h_count").GetValueOrDefault(),
                                h_uniq_id = (Guid)reader.GetRaw("h_uniq_id"),
                                h_details = reader.GetString("h_details"),
                            };

                            list.Add(data);
                        }
                    }
                }
            }

            return list;
        }

        #endregion
    }
}
