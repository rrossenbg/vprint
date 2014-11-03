/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading;

namespace DEMATLib.Data
{
    public static class DEMARDataAccess
    {
        private static string m_ConnectionString;

        public static string ConnectionString
        {
            get
            {
                return m_ConnectionString;
            }
            set
            {
                using (var conn = new SqlConnection(value))
                    conn.Open();
                m_ConnectionString = value;
            }
        }

        #region DEMAT_INVOICES

        #region ZeroInvoiceNumbers

        public static void ZeroInvoiceNumbers(int countryId)
        {
            CheckConnectionStringThrow();

            #region SQL

            const string SQL = "update top (300) Branch set br_DEMAT_export_number = null where br_iso_id = @iso and br_DEMAT_export_number is not null;";

            int result = 100;

            while (result != 0)
            {
                using (var conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();

                    using (var comm = new SqlCommand(SQL, conn))
                    {
                        comm.Parameters.AddWithValue("@iso", countryId);
                        comm.CommandType = CommandType.Text;
                        comm.CommandTimeout = 10;
                        result = comm.ExecuteNonQuery();
                    }
                }

                Thread.Sleep(3000);
            }

            #endregion //SQL

        }

        #endregion //ZeroInvoiceNumbers

        #region SelectAllVouchersToExport

        public static List<DateTime> SelectAllDistinctDateVoucherDates(int countryId)
        {
            const string SQL = @"select distinct  v_date_voucher  from voucher
                                    where v_iso_id = @iso
                                    order by v_date_voucher desc";

            var list = new List<DateTime>();

            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                using (var comm = new SqlCommand(SQL, conn))
                {
                    comm.Parameters.AddWithValue("@iso", countryId);

                    using (var reader = comm.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while(reader.Read())
                        {
                            var v_date_voucher = reader.Get<DateTime>("v_date_voucher");
                            list.Add(v_date_voucher);
                        }
                    }
                }
            }

            return list;
        }

        public static List<int> SelectAllDistinctHOs(int countryId)
        {
            const string SQL = @"select ho_id  from HeadOffice
                                 where ho_iso_id = @iso";

            var list = new List<int>();

            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                using (var comm = new SqlCommand(SQL, conn))
                {
                    comm.Parameters.AddWithValue("@iso", countryId);

                    using (var reader = comm.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            var ho_id = reader.Get<int>("ho_id");
                            list.Add(ho_id);
                        }
                    }
                }
            }

            return list;
        }

        public class VoucherToExportInfo
        {
            public int v_br_id { get; set; }
            public int v_number { get; set; }
            /// <summary>
            /// Valid, Void
            /// </summary>
            public bool isValid { get; set; }
        }
        /// <summary>
        /// Returns List of RetailerId, VoucherId
        /// </summary>
        /// <param name="countryId"></param>
        /// <returns>RetailerId, VoucherId</returns>
        public static List<VoucherToExportInfo> SelectAllVouchersToExport(int countryId, int hoId)
        {
            CheckConnectionStringThrow();

            #region SQL

            //ho_contract_signature_date
            const string SQL =
            @"select v_number, v_br_id, 1 as [status] FROM Voucher v
            inner join Branch on br_iso_id = v_iso_id and br_id = v_br_id
            inner join VoucherPart vp on vp.vp_iso_id = v.v_iso_id and vp.vp_v_number = v.v_number
            where v_iso_id = @iso and br_enable_DEMAT_export = 1 and br_demat_contract_date <= v_date_voucher and
                (v_sent_to_demat is null or v_sent_to_demat = 0) and  
                (v_voided is null or v_voided = 0) and
            not v_status_id in (10, 12, 17, 18, 26, 27, 35) and 
                vp_type_id in (1, 4)
                and br_ho_id = @hoId  
            union all
            select v_number, v_br_id, 0 as [status] FROM Voucher v
            inner join Branch on br_iso_id = v_iso_id and br_id = v_br_id
            inner join VoucherPart vp on vp.vp_iso_id = v.v_iso_id and vp.vp_v_number = v.v_number
            where v_iso_id = @iso and br_enable_DEMAT_export = 1 and br_demat_contract_date <= v_date_voucher and
                v_sent_to_demat = 1 and  
                v_voided = 1 and
                vp_type_id = 1
               and br_ho_id = @hoId;";

            #endregion //SQL

            var result = new List<VoucherToExportInfo>();

            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                using (var comm = new SqlCommand(SQL, conn))
                {
                    comm.CommandTimeout = 0;
                    comm.Parameters.AddWithValue("@iso", countryId);
                    comm.Parameters.AddWithValue("@hoId", hoId);

                    using (var reader = comm.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            result.Add(new VoucherToExportInfo
                                {
                                    v_br_id = reader.Get<int>("v_br_id"),
                                    v_number = reader.Get<int>("v_number"),
                                    isValid = reader.Get<bool>("status"),
                                });
                        }
                    }
                }
            }

            return result;
        }

        #endregion //SelectAllVouchersToExport

        #region SelectVoucher

        public class VoucherInfo
        {
            public int v_number { get; set; }
            public DateTime v_date_voucher { get; set; }
            public int br_id { get; set; }
            public string br_name { get; set; }
            public string br_add_1 { get; set; }
            public string br_add_2 { get; set; }
            public string br_add_3 { get; set; }
            public string br_add_4 { get; set; }
            public string br_add_5 { get; set; }
            
            public long br_DEMAT_export_number { get; set; }
            public string ho_vat_number { get; set; }

            public string v_title { get; set; }
            public string v_firstname { get; set; }
            public string v_lastname { get; set; }
            public string v_final_country { get; set; }
            public string v_passport_no { get; set; }
            public string v_refund_str { get; set; }
            public decimal? v_refund_in_refund_currency { get; set; }
        }

        public static VoucherInfo SelectVoucher(int countryId, int voucherId)
        {
            CheckConnectionStringThrow();

            #region SQL

            const string SQL = @"select v_number, v_date_voucher, br_id, br_name, br_add_1, br_add_2, br_add_3, br_add_4, br_add_5, v_final_country, ho_vat_number, 
                                    br_DEMAT_export_number, v_title, v_firstname, v_lastname, v_passport_no, v_refund_str, v_refund_in_refund_currency                                 
                                 from Voucher
                                 inner join Branch on br_id = v_br_id and br_iso_id = v_iso_id
                                 inner join HeadOffice on ho_id = br_ho_id and ho_iso_id = br_iso_id
                                 where v_iso_id = @iso and v_number = @number";

            #endregion //SQL

            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                using (var comm = new SqlCommand(SQL, conn))
                {
                    comm.Parameters.AddWithValue("@iso", countryId);
                    comm.Parameters.AddWithValue("@number", voucherId);

                    using (var reader = comm.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        if (!reader.Read())
                            throw new ApplicationException(
                            string.Format("Cannot find voucher IsoId: {0} VoucherId: {1}", countryId, voucherId));

                        var result = new VoucherInfo
                        {
                            v_number = reader.Get<int>("v_number"),
                            v_date_voucher = reader.Get<DateTime>("v_date_voucher"),
                            br_id = reader.Get<int>("br_id"),
                            br_name = reader.GetString("br_name"),
                            br_add_1 = reader.GetString("br_add_1"),
                            br_add_2 = reader.GetString("br_add_2"),
                            br_add_3 = reader.GetString("br_add_3"),
                            br_add_4 = reader.GetString("br_add_4"),
                            br_add_5 = reader.GetString("br_add_5"),
                            br_DEMAT_export_number = reader.GetNull<long>("br_DEMAT_export_number").GetValueOrDefault(),
                            ho_vat_number = reader.GetString("ho_vat_number"),
                            v_final_country = reader.GetString("v_final_country"),
                            v_title = reader.GetString("v_title"),
                            v_firstname = reader.GetString("v_firstname"),
                            v_lastname = reader.GetString("v_lastname"),
                            v_passport_no = reader.GetString("v_passport_no"),
                            v_refund_str = reader.GetString("v_refund_str"),
                            v_refund_in_refund_currency = reader.GetNull<decimal>("v_refund_in_refund_currency"), 
                        };
                        return result;
                    }
                }
            }
        }

        public static void SetVoucherSentToDemat(int countryId, int voucherId)
        {
            CheckConnectionStringThrow();

            const string SQL = @"update Voucher 
                                set v_sent_to_demat = ISNULL(v_sent_to_demat, 0) + 1 
                                where v_iso_id = @iso and v_number = @v_number";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                using (SqlCommand comm = new SqlCommand(SQL, conn))
                {
                    comm.CommandType = CommandType.Text;
                    comm.Parameters.AddWithValue("@iso", countryId);
                    comm.Parameters.AddWithValue("@v_number", voucherId);
                    comm.ExecuteNonQuery();
                }
            }
        }

        #endregion //SelectVoucher

        #region SelectVoucherLines

        public enum VatCodes
        {
            N = 0,
            L = 1,
            H = 2,
            O = 3,
        }

        public class VoucherLineInfo
        {
            public int vl_line_number { get; set; }
            public decimal vl_unit_price { get; set; }
            public int vl_quantity { get; set; }
            public string vl_product { get; set; }
            public VatCodes vl_code_ttc { get; set; }
            public decimal vl_pp_excl_vat { get; set; }
            public decimal vl_pp_vat { get; set; }
            public decimal vl_pp_incl_vat { get; set; }
        }

        public static List<VoucherLineInfo> SelectVoucherLines(int countryId, int voucherId)
        {
            CheckConnectionStringThrow();

            #region SQL

            const string SQL = @"select vl_line_number, vl_unit_price, vl_quantity, vl_pp_excl_vat, vl_pp_vat, vl_pp_incl_vat, vl_product, vl_code_ttc 
                                from Voucher
                                inner join VoucherLine on vl_v_number = v_number and vl_iso_id = v_iso_id
                                where v_iso_id = @iso and v_number = @number";

            #endregion //SQL

            var result = new List<VoucherLineInfo>();

            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                using (var comm = new SqlCommand(SQL, conn))
                {
                    comm.Parameters.AddWithValue("@iso", countryId);
                    comm.Parameters.AddWithValue("@number", voucherId);
                    comm.CommandType = CommandType.Text;

                    using (var reader = comm.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            result.Add(new VoucherLineInfo
                            {
                                vl_line_number = reader.Get<int>("vl_line_number"),
                                vl_pp_excl_vat = reader.Get<decimal>("vl_pp_excl_vat"),
                                vl_pp_incl_vat = reader.Get<decimal>("vl_pp_incl_vat"),
                                vl_pp_vat = reader.Get<decimal>("vl_pp_vat"),
                                vl_quantity = reader.Get<int>("vl_quantity"),
                                vl_unit_price = reader.Get<decimal>("vl_unit_price"),
                                vl_product = reader.GetString("vl_product"),
                                vl_code_ttc = reader.GetString("vl_code_ttc").ToEnum<VatCodes>()
                            });
                        }
                        return result;
                    }
                }
            }
        }

        #endregion //SelectVoucherLines

        #region LOGGING

        public static void LogVoucherExported(string logProgram, int iso, int v_number, string message)
        {
            CheckConnectionStringThrow();

            string hostName = Environment.MachineName;

            #region SQL

            const string SQL = @"INSERT [Logging]( [log_date],[log_type_id],[log_level],[log_from],[log_description],
                                        [log_v_id],[log_iso_id],[log_us_id],[log_login],[log_v_payment_status_id],[log_hostname],[log_program])
                                VALUES (@date,3,2,'sa', @desc, @v_number, @v_iso, 0, 'sa', 1, @host, @prog)";

            #endregion //SQL

            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                using (var comm = new SqlCommand(SQL, conn))
                {
                    comm.Parameters.AddWithValue("@date", DateTime.Now);
                    comm.Parameters.AddWithValue("@desc", message);
                    comm.Parameters.AddWithValue("@v_number", v_number);
                    comm.Parameters.AddWithValue("@v_iso", iso);
                    comm.Parameters.AddWithValue("@host", hostName);
                    comm.Parameters.AddWithValue("@prog", logProgram);
                    comm.CommandType = CommandType.Text;
                    comm.ExecuteNonQuery();
                }
            }
        }

        public static void SaveDEMATExportID(int iso, int retailerId, long exportId)
        {
            #region SQL

            const string SQL = "UPDATE Branch Set br_DEMAT_export_number = @exportId WHERE br_iso_id = @isoId and br_id = @brId;";

            #endregion //SQL

            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                using (var comm = new SqlCommand(SQL, conn))
                {
                    comm.Parameters.AddWithValue("@isoId", iso);
                    comm.Parameters.AddWithValue("@brId", retailerId);
                    comm.Parameters.AddWithValue("@exportId", exportId);
                    comm.CommandType = CommandType.Text;
                    comm.ExecuteNonQuery();
                }
            }
        }

        #endregion

        #region 

        public static void CleanExportedVouchers(int countryId)
        {
            CheckConnectionStringThrow();

            #region SQL

            const string SQL = @"update v
                                set v_sent_to_demat = null
                                from voucher v
                                where v.v_iso_id = @iso";

            #endregion

            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                using (var comm = new SqlCommand(SQL, conn))
                {
                    comm.Parameters.AddWithValue("@iso", countryId);
                    comm.CommandType = CommandType.Text;
                    comm.ExecuteNonQuery();
                }
            }
        }

        #endregion

        #endregion //DEMAT_INVOICES

        #region DEMAT_RETAILERS_TO_REPORT

        public class SelectAllRetailersToReportInfo
        {
            public int dmh_br_iso_id { get; set; }
            public int dmh_br_id { get; set; }
            public string dmh_br_DEMAT_contact_email { get; set; }
            public DateTime? dmh_br_DEMAT_contract_date { get; set; }
            public bool? dmh_br_enable_DEMAT_export { get; set; }
        }

        public static List<SelectAllRetailersToReportInfo> SelectAllRetailersToReport(DateTime from, DateTime to)
        {
            CheckConnectionStringThrow();

            #region SQL

            const string SQL = @"select dmh_br_iso_id, dmh_br_id,
	                                   dmh_br_DEMAT_contact_email, dmh_br_DEMAT_contract_date, dmh_br_enable_DEMAT_export 
                                from DEMATHistory 
                                where dmh_createdat between @from and @to";

            #endregion

            var result = new List<SelectAllRetailersToReportInfo>();

            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                using (var comm = new SqlCommand(SQL, conn))
                {
                    comm.Parameters.AddWithValue("@from", from);
                    comm.Parameters.AddWithValue("@to", to);
                    comm.CommandType = CommandType.Text;

                    using (var reader = comm.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            result.Add(new SelectAllRetailersToReportInfo()
                            {
                                dmh_br_iso_id = reader.Get<int>("dmh_br_iso_id"),
                                dmh_br_id = reader.Get<int>("dmh_br_id"),
                                dmh_br_DEMAT_contact_email = reader.GetString("dmh_br_DEMAT_contact_email"),
                                dmh_br_DEMAT_contract_date = reader.GetNull<DateTime>("dmh_br_DEMAT_contract_date"),
                                dmh_br_enable_DEMAT_export = reader.GetNull<bool>("dmh_br_enable_DEMAT_export"),
                            });
                        }
                    }
                }
            }

            return result;
        }

        #endregion //DEMAT_RETAILERS

        public enum VatRates
        {
            VatRateNormal = 0,
            VATRateLower = 1,
            VATRateHigher = 2,
            VATRateOther = 3,
        }

        //declare @value varchar(255)
        //--= "VatRateNormal") || (ruleName == "VATRateHigher") || (ruleName == "VATRateLower") || 
        //---(ruleName == "VATRateOther") || (ruleName == "PurchasePrice")
        //exec dbo.GetCountryRules2  826, 'VatRateNormal', '2012-01-01', @value 
        //exec dbo.GetCountryRules2  826, 'VATRateHigher', '2012-01-01', @value 
        //exec dbo.GetCountryRules2  826, 'VATRateLower', '2012-01-01', @value 
        //exec dbo.GetCountryRules2  826, 'VATRateOther', '2012-01-01', @value 

        //lstVatRate.Items.Add("N");
        //lstVatRate.Items.Add("L");
        //lstVatRate.Items.Add("H");
        //lstVatRate.Items.Add("O");
        public static Dictionary<VatRates, decimal> GetVATRatesByCountryAndDate(int countryId, DateTime date)
        {
            CheckConnectionStringThrow();

            var dict = new Dictionary<VatRates, decimal>();

            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                foreach (var rate in new VatRates[] { VatRates.VatRateNormal, VatRates.VATRateLower, VatRates.VATRateHigher, VatRates.VATRateOther })
                {
                    using (var comm = new SqlCommand("GetCountryRules2", conn))
                    {
                        comm.CommandType = CommandType.StoredProcedure;
                        comm.Parameters.AddWithValue("@country_id", countryId);
                        comm.Parameters.AddWithValue("@rule_type", rate.ToString());
                        comm.Parameters.AddWithValue("@date", date);
                        SqlParameter pout;
                        comm.Parameters.Add(pout = new SqlParameter("@value", SqlDbType.VarChar, 255) { Direction = ParameterDirection.InputOutput });

                        using (var reader = comm.ExecuteReader())
                            if (reader.Read())
                                dict.Add(rate, reader.GetNull<decimal>("rv_rulevalue").GetValueOrDefault());
                    }
                }
            }
            return dict;
        }

        public static decimal? GetRefundCurrency(int countryId, int voucherId)
        {
            const string SQL = @"select abs(sum(ld_voucher_amount)) as value
                                from Ledger 
                                where ld_iso_id=@iso and ld_pt_id=2 and ld_tt_id in (19,25)
                                and ld_voucher_id=@v_number";

            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                using (var comm = new SqlCommand(SQL, conn))
                {
                    comm.CommandType = CommandType.Text;
                    comm.Parameters.AddWithValue("@iso", countryId);
                    comm.Parameters.AddWithValue("@v_number", voucherId);
                    return comm.ExecuteScalar().GetNull<decimal>();
                }
            }
        }

        private static void CheckConnectionStringThrow()
        {
            if (ConnectionString.IsNullOrWhiteSpace())
                throw new ApplicationException("Connection string is empty");
        }
    }
}
