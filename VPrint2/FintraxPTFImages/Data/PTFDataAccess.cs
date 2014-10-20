using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using FintraxPTFImages.Data.PTF;
using FintraxPTFImages.Models;

namespace FintraxPTFImages.Data
{
    public partial class PTFDataAccess
    {
        public static string ConnectionString { get; set; }

        public IQueryable<Branch> GetBranchesByCountryId(int countryId)
        {
            PTFDataEntities2 db = new PTFDataEntities2();
            var branches = db.Branches.Where(br => br.br_iso_id == countryId);
            return branches;
        }

        public static bool CheckP1Exists(int iso, int voucherId)
        {
            #region SQL

            const string SQL = @"select 1 from VoucherPart where vp_iso_id = @iso and vp_v_number = @voucher and vp_type_id = 1";

            #endregion

            using (var conn = new SqlConnection(ConnectionString))
            using (var comm = new SqlCommand(SQL, conn))
            {
                comm.Parameters.AddWithValue("@iso", iso);
                comm.Parameters.AddWithValue("@voucher", voucherId);
                conn.Open();
                return comm.ExecuteScalar().Cast<bool>();
            }
        }

        public static void LogVoucher(int iso, int voucherId, int userId)
        {
            #region SQL

            const string SQL = @"INSERT INTO Logging (log_date, log_type_id, log_level, log_from, log_v_id, log_iso_id, 
                                    log_us_id, log_login, log_hostname, log_program, log_nt_user) 
                                 VALUES ( GETDATE() , 3, 1, 'ExcludeFromDR', @voucher, @iso, 
                                    @usId, 'sa', '192.168.53.143', 'FintraxPTFImages', '\')";

            #endregion

            using (var conn = new SqlConnection(ConnectionString))
            using (var comm = new SqlCommand(SQL, conn))
            {
                comm.Parameters.AddWithValue("@iso", iso);
                comm.Parameters.AddWithValue("@voucher", voucherId);
                comm.Parameters.AddWithValue("@usId", userId);
                conn.Open();
                comm.ExecuteNonQuery();
            }
        }

        public static BarcodeInfo SelectVoucherInfo(int iso, int voucherId)
        {
            #region SQL

            const string SQL = @"select * from Voucher where v_iso_id = @iso and v_number = @voucher";

            #endregion

            using (var conn = new SqlConnection(ConnectionString))
            using (var comm = new SqlCommand(SQL, conn))
            {
                comm.Parameters.AddWithValue("@iso", iso);
                comm.Parameters.AddWithValue("@voucher", voucherId);
                conn.Open();
                using (var reader = comm.ExecuteReader(CommandBehavior.CloseConnection))
                    return BarcodeInfo.ReadFromReader(reader);
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

            using (var conn = new SqlConnection(ConnectionString))
            using (var comm = new SqlCommand(SQL, conn))
            {
                comm.Parameters.AddWithValue("@iso", isoId);
                comm.Parameters.AddWithValue("@br", retailerId);
                comm.Parameters.AddWithValue("@voucher", voucherId);
                comm.Parameters.AddWithValue("@dte", DateTime.Now);
                comm.Parameters.AddWithValue("@usr", accountId);
                conn.Open();
                comm.ExecuteNonQuery();
            }
        }
    }
}