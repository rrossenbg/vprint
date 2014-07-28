using System;
using System.Data.SqlClient;
using System.Linq;
using FintraxPTFImages.Data.PTF;

namespace FintraxPTFImages.Data
{
    public class PTFDataAccess
    {
        public static string ConnectionString { get; set; }

        public IQueryable<Branch> GetBranchesByCountryId(int countryId)
        {
            PTFDataEntities2 db = new PTFDataEntities2();
            var branches = db.Branches.Where(br => br.br_iso_id == countryId);
            return branches;
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