/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace DEMATLib.Data
{
    public static class DiorDataAccess
    {
        private static string m_PTFConnectionString;
        private static string m_ReportConnectionString;

        public static string PTFConnectionString
        {
            get
            {
                return m_PTFConnectionString;
            }
            set
            {
                using (var conn = new SqlConnection(value))
                    conn.Open();
                m_PTFConnectionString = value;
            }
        }

        public static string ReportsConnectionString
        {
            get
            {
                return m_ReportConnectionString;
            }
            set
            {
                using (var conn = new SqlConnection(value))
                    conn.Open();
                m_ReportConnectionString = value;
            }
        }

        public static string SelectTradingName(int countryId, int hoId)
        {
            #region SQL

            const string SQL = @"select ho_trading_name from HeadOffice where ho_iso_id = @iso and ho_id = @hoId;";

            #endregion

            using (var conn = new SqlConnection(PTFConnectionString))
            {
                conn.Open();

                using (var comm = new SqlCommand(SQL, conn))
                {
                    comm.Parameters.AddWithValue("@iso", countryId);
                    comm.Parameters.AddWithValue("@hoId", hoId);

                    return Convert.ToString(comm.ExecuteScalar());
                }
            }
        }

        public static List<Retailer> SelectAllDiorRetailes(int countryId, int hoId)
        {
            #region SQL

            const string SQL = "SELECT br_iso_id, br_id, br_ho_id FROM Branch (nolock) WHERE br_iso_id = @iso and br_ho_id = @hoId;";

            #endregion

            var list = new List<Retailer>();

            using (var conn = new SqlConnection(ReportsConnectionString))
            {
                conn.Open();

                using (var comm = new SqlCommand(SQL, conn))
                {
                    comm.Parameters.AddWithValue("@iso", countryId);
                    comm.Parameters.AddWithValue("@hoId", hoId);

                    using (var reader = comm.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            var br = new Retailer();
                            br.IsoId = reader.Get<int>("br_iso_id");
                            br.BrId = reader.Get<int>("br_id");
                            br.HoId = reader.Get<int>("br_ho_id");
                            list.Add(br);
                        }
                    }
                }
            }

            return list;
        }

        public static List<Voucher> SelectVouchersPerRetailer(int countryId, int retailerId)
        {
            #region SQL

            const string SQL = @"SELECT v_iso_id, v_br_id, v_number, v_voucher_void, v_date_purchase, v_date_qualified, 
                                        v_date_stamp, v_date_refund, v_date_rebate, v_date_debit, v_date_P0, v_date_P1, v_date_P15, v_date_P2, 
                                        P2_site_code, P2_location_number, v_ic_id
                                FROM Voucher (nolock) WHERE v_iso_id = @iso and v_br_id = @brId and v_date_purchase > '2014-07-01';";

            #endregion

            var list = new List<Voucher>();

            using (var conn = new SqlConnection(ReportsConnectionString))
            {
                conn.Open();

                using (var comm = new SqlCommand(SQL, conn))
                {
                    comm.Parameters.AddWithValue("@iso", countryId);
                    comm.Parameters.AddWithValue("@brId", retailerId);

                    using (var reader = comm.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            var v = new Voucher();
                            v.IsoId = reader.Get<int>("v_iso_id");
                            v.BrId = reader.Get<int>("v_br_id");
                            v.VId = reader.Get<int>("v_number");
                            v.v_date_purchase = reader.GetNull<DateTime>("v_date_purchase");
                            v.v_date_debited = reader.GetNull<DateTime>("v_date_debit");
                            v.v_date_stamp = reader.GetNull<DateTime>("v_date_stamp");

                            v.SiteCodeRose = string.Concat(
                                reader.GetString("P2_site_code"),
                                reader.GetString("P2_location_number"));

                            v.v_ic_id = reader.GetString("v_ic_id");
                            v.v_date_p0 = reader.GetNull<DateTime>("v_date_p0");
                            v.v_date_p1 = reader.GetNull<DateTime>("v_date_p1");
                            v.v_date_p2 = reader.GetNull<DateTime>("v_date_p2");

                            v.v_voucher_void = Convert.ToBoolean(reader.GetNull<int>("v_voucher_void"));

                            list.Add(v);
                        }
                    }
                }
            }

            return list;
        }
    }    
}