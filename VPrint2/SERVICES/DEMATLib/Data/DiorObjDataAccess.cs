/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace DEMATLib.Data
{
    /// <summary>
    /// 
    /// </summary>
    public class DiorObjDataAccess
    {
        public static string ReportsConnectionString { get; set; }

        public static List<Voucher> SelectVouchersPerRetailer(int countryId, int retailerId)
        {
            #region SQL

            const string SQL = "SELECT data FROM ObjCache WHERE key1 = @iso and key2 = @brid;";

            #endregion

            var lst = new List<Voucher>();

            using (var conn = new SqlConnection(ReportsConnectionString))
            {
                conn.Open();

                using (var comm = new SqlCommand(SQL, conn))
                {
                    comm.Parameters.AddWithValue("@iso", countryId);
                    comm.Parameters.AddWithValue("@brid", retailerId);

                    using (var reader = comm.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            byte[] buffer = (byte[])reader[0];
                            var voucher = buffer.ToObject<Voucher>();
                            lst.Add(voucher);
                        }
                    }
                }
            }

            return lst;
        }

        public static void DeleteVoucher(Voucher v)
        {
            #region SQL

            const string SQL = "DELETE ObjCache WHERE key1 = @iso and key2 = @brid and key3 = @vid;";

            #endregion

            using (var conn = new SqlConnection(ReportsConnectionString))
            {
                conn.Open();

                using (var comm = new SqlCommand(SQL, conn))
                {
                    comm.Parameters.AddWithValue("@iso", v.IsoId);
                    comm.Parameters.AddWithValue("@brid", v.BrId);
                    comm.Parameters.AddWithValue("@vid", v.VId);
                    comm.ExecuteNonQuery();
                }
            }
        }

        public static void DeleteVouchers(int countryId, List<Voucher> vouchers)
        {
            if (vouchers.IsNullOrEmpty())
                return;

            StringBuilder b = new StringBuilder();
            foreach (var v in vouchers)
            {
                b.Append(v.VId);
                b.Append(",");
            }

            b.Remove(b.Length - 1, 1);

            string SQL = string.Format( "DELETE ObjCache WHERE key1 = @iso and key3 in ({0});", b.ToString());

            using (var conn = new SqlConnection(ReportsConnectionString))
            {
                conn.Open();

                using (var comm = new SqlCommand(SQL, conn))
                {
                    comm.Parameters.AddWithValue("@iso", countryId);
                    comm.ExecuteNonQuery();
                }
            }
        }

        public static void InsertVoucher(Voucher v)
        {
            #region SQL

            const string SQL = "INSERT ObjCache (key1, key2, key3, key4, data) VALUES (@key1, @key2, @key3, @key4, @data);";

            #endregion

            using (var conn = new SqlConnection(ReportsConnectionString))
            {
                conn.Open();

                using (var comm = new SqlCommand(SQL, conn))
                {
                    comm.Parameters.AddWithValue("@key1", v.IsoId);
                    comm.Parameters.AddWithValue("@key2", v.BrId);
                    comm.Parameters.AddWithValue("@key3", v.VId);
                    comm.Parameters.AddWithValue("@key4", 0);

                    byte[] data = v.FromObject();
                    comm.Parameters.AddWithValue("@data", data);
                    comm.ExecuteNonQuery();
                }
            }
        }

        public static void InsertVouchers(List<Voucher> vouchers)
        {
            #region SQL

            const string SQL = "INSERT ObjCache (key1, key2, key3, key4, data) VALUES (@key1, @key2, @key3, @key4, @data);";

            #endregion

            foreach (var v in vouchers)
            {
                using (var conn = new SqlConnection(ReportsConnectionString))
                {
                    conn.Open();

                    using (var comm = new SqlCommand(SQL, conn))
                    {
                        comm.Parameters.AddWithValue("@key1", v.IsoId);
                        comm.Parameters.AddWithValue("@key2", v.BrId);
                        comm.Parameters.AddWithValue("@key3", v.VId);
                        comm.Parameters.AddWithValue("@key4", 0);

                        byte[] data = v.FromObject();
                        comm.Parameters.AddWithValue("@data", data);
                        comm.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
