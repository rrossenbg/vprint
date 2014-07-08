using System;
using System.Data;
using System.Data.SqlClient;

namespace BtRetryConfigurator
{
    static class DataAccess
    {
        public static string ConnectionString {get;set;}

        public static void UpdateEmailList(int id, string emails)
        {
            if (string.IsNullOrWhiteSpace(ConnectionString))
                throw new Exception("ConnectionString is not set");

            const string SQL = @"UPDATE EmailList
                                SET el_list = @el_list
                                WHERE el_iso_id = @el_iso_id;";
            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                using (SqlCommand comm = new SqlCommand(SQL, conn))
                {
                    comm.CommandType = CommandType.Text;
                    comm.Parameters.AddWithValue("@el_iso_id", id);
                    comm.Parameters.AddWithValue("@el_list", emails);
                    comm.ExecuteNonQuery();
                }
            }
        }
    }
}
