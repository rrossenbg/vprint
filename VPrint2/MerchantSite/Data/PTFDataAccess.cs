/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.Data;
using System.Data.SqlClient;
using MerchantSite.DataServiceRef;
using MerchantSite.Models;
using VPrinting;

namespace MerchantSite.Data
{
    public partial class PTFDataAccess
    {
        public static void LogVoucher(int iso, int voucherId, int userId)
        {
            #region SQL

            const string SQL = @"INSERT INTO Logging (log_date, log_type_id, log_level, log_from, log_v_id, log_iso_id, 
                                    log_us_id, log_login, log_hostname, log_program, log_nt_user) 
                                 VALUES ( GETDATE() , 3, 1, 'ExcludeFromDR', @voucher, @iso, 
                                    @usId, 'sa', '192.168.53.143', 'MerchantSite', '\')";

            #endregion

            using (var comm = new SqlCommand(SQL))
            {
                comm.Parameters.AddWithValue("@iso", iso);
                comm.Parameters.AddWithValue("@voucher", voucherId);
                comm.Parameters.AddWithValue("@usId", userId);
                var data = comm.CreateSerializationData().ToList().ToArray();
                DataServiceClient client = new DataServiceClient();
                client.TRSExecuteNonQuery(data);
            }
        }

        public static BarcodeInfo SelectVoucherInfo(int iso, int voucherId)
        {
            #region SQL

            const string SQL = @"select * from Voucher where v_iso_id = @iso and v_number = @voucher";

            #endregion

            using (var comm = new SqlCommand(SQL))
            {
                comm.Parameters.AddWithValue("@iso", iso);
                comm.Parameters.AddWithValue("@voucher", voucherId);
                var data = comm.CreateSerializationData().ToList().ToArray();
                DataServiceClient client = new DataServiceClient();
                var result = client.TRSExecuteReader(data);
                using (DataTableReader reader = new DataTableReader(result.Data.ToDataTable()))
                    if (reader.Read())
                        return BarcodeInfo.ReadFromReader(reader);
            }
            return new BarcodeInfo();
        }

        public static string SelectHoName(int iso, int ho_id)
        {
            #region SQL

            const string SQL = @"select ho_id, ho_iso_id, ho_name from headoffice
                            where ho_iso_id = @iso_id and ho_id = @id;";

            #endregion

            using (var comm = new SqlCommand(SQL))
            {
                comm.Parameters.AddWithValue("@iso_id", iso);
                comm.Parameters.AddWithValue("@id", ho_id);
                var data = comm.CreateSerializationData().ToList().ToArray();
                DataServiceClient client = new DataServiceClient();
                var result = client.TRSExecuteScalar(data);
                return result.Cast<string>("na");
            }
        }

        public static void ExcudeFromDebitRun(int isoId, int retailerId, int voucherId, int accountId)
        {
            #region SQL
            const string SQL = @"   
MERGE ExcludeVouchersDebitRun AS t
    USING (SELECT @iso, @br, @voucher, @dte, @usr) AS s (iso, br, voucher, dte, usr)
    ON (s.iso = t.evdr_iso_id and s.br = t.evdr_br_id and s.voucher = t.evdr_v_number)
    WHEN MATCHED THEN 
        UPDATE SET t.evdr_date = s.dte, t.evdr_us_id = s.usr
WHEN NOT MATCHED THEN
    INSERT (evdr_iso_id, evdr_br_id, evdr_v_number, evdr_date, evdr_us_id)
    VALUES (s.iso, s.br, s.voucher, s.dte, s.usr);";
            #endregion

            using (var comm = new SqlCommand(SQL))
            {
                comm.Parameters.AddWithValue("@iso", isoId);
                comm.Parameters.AddWithValue("@br", retailerId);
                comm.Parameters.AddWithValue("@voucher", voucherId);
                comm.Parameters.AddWithValue("@dte", DateTime.Now);
                comm.Parameters.AddWithValue("@usr", accountId);
                var data = comm.CreateSerializationData().ToList().ToArray();
                DataServiceClient client = new DataServiceClient();
                client.TRSExecuteNonQuery(data);
            }
        }
    }
}