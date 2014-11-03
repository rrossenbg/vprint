/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Collections;

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

            using (var conn = new SqlConnection(PTFConnectionString))
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

        public static Hashtable SelectInhibitCodes()
        {
            #region SQL

            const string SQL = "select * from InhibitCode";

            #endregion

            var dict = new Hashtable();

            using (var conn = new SqlConnection(PTFConnectionString))
            {
                conn.Open();

                using (var comm = new SqlCommand(SQL, conn))
                using (var reader = comm.ExecuteReader(CommandBehavior.CloseConnection))
                    while (reader.Read())
                        dict.Add(reader.GetString("ic_id"), reader.GetString("ic_code"));
            }
            return dict;
        }

        public static List<Voucher> SelectVouchersPerRetailer(int countryId, int retailerId)
        {
            #region SQL

#if USE_REPORTS_DB

            const string SQL = @"SELECT v_iso_id, v_br_id, v_number, v_voucher_void, v_date_purchase, v_date_qualified, 
                                        v_date_stamp, v_date_refund, v_date_rebate, v_date_debit, v_date_P0, v_date_P1, v_date_P15, v_date_P2, 
                                        P2_site_code, P2_location_number, v_ic_id
                                FROM Voucher (nolock) WHERE v_iso_id = @iso and v_br_id = @brId and v_date_purchase > '2014-07-01';";

#else

            const string SQL = 
@"SELECT v_iso_id, v_br_id, v_number, v_voucher_void, v_date_voucher as v_date_purchase, v_date_qualified, v_date_stamp 
		,case when v_rm_id in (3,5) then isnull (v_refund_date,v_date_stamp) else  rh.rh_date end as v_date_refund
		,rb.in_date as v_date_rebate 
		,ld.ld_date  as v_date_debit
		,p0.vp0p_date as v_date_P0
		,p1.vp_date as   v_date_P1
		,p2.vp_date as   v_date_P2
		,p15.vp_date as   v_date_P15
        , p2.vp_site_code as  P2_site_code
		, p2.vp_location_number as   P2_location_number
		, v_ic_id
FROM Voucher (nolock) 
left join (select vp_iso_id, vp_v_number, vp_date from voucherpart (nolock) where vp_type_id in (1,4) and vp_iso_id=@iso) p1
on v_number=p1.vp_v_number and v_iso_id=p1.vp_iso_id
left join (select vp_iso_id, vp_v_number, vp_date, vp_site_code, vp_location_number  from voucherpart (nolock) where vp_type_id=3 and vp_iso_id=@iso) p2
on v_number=p2.vp_v_number and v_iso_id=p2.vp_iso_id
left join (select vp_iso_id, vp_v_number, vp_date from voucherpart (nolock) where vp_type_id in (2,5) and vp_iso_id=@iso) p15
on v_number=p15.vp_v_number and v_iso_id=p15.vp_iso_id
left join (select vp0p_iso_id , vp0p_v_number , vp0p_date  from VoucherP0Part (nolock) where vp0p_type_id =1 and vp0p_iso_id=@iso) p0
on v_number=p0.vp0p_v_number  and v_iso_id=p0.vp0p_iso_id 
left join (select iv_v_number, iv_iso_id, min(in_date) in_date from invoices (nolock) inner join invoicevoucher (nolock) on in_number=iv_in_number and in_iso_id=iv_iso_id where iv_iso_id =@iso group by iv_v_number, iv_iso_id ) rb
on v_number=iv_v_number and v_iso_id=iv_iso_id
left join (select rh_v_number,min(rh_date) rh_date, rh_iso_id  from refundhistory (nolock) where rh_iso_id=@iso group by rh_v_number, rh_iso_id) rh
on v_number=rh_v_number and v_iso_id=rh_iso_id 
left join (select ld_iso_id, ld_voucher_id, min(ld_date) ld_date from ledger where ld_iso_id=@iso and ld_tt_id=39 and ld_pt_id=6 group by ld_iso_id, ld_voucher_id) ld
on v_number=ld.ld_voucher_id and v_iso_id=ld.ld_iso_id
where v_iso_id=@iso and v_br_id=@brid and v_datE_voucher> '2014-07-01'";

#endif

            #endregion

            var list = new List<Voucher>();

            using (var conn = new SqlConnection(PTFConnectionString))
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
                            v.v_date_refund = reader.GetNull<DateTime>("v_date_refund");

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