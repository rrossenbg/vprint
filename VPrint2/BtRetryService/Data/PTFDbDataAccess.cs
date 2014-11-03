
using System.Collections;
using System.Data;
using System.Data.SqlClient;

namespace BtRetryService
{
    public class PTFDbDataAccess
    {
        public static volatile string ConnectionString;

        public static Hashtable GetAllRetailers(int coubntryId)
        {
            const string SQL = @"select br_id, br_name + ' - ' + CAST( br_id as varchar(10)) as name 
                                    from branch where br_iso_id = @iso";

            Hashtable result = new Hashtable();

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                using (SqlCommand comm = new SqlCommand(SQL, conn))
                {
                    comm.Parameters.AddWithValue("@iso", coubntryId);
                    using (var reader = comm.ExecuteReader(CommandBehavior.CloseConnection))
                        while (reader.Read())
                            result.Add(reader.Get<int>("br_id"), reader.GetString("name"));
                }
            }

            return result;
        }
    }
}
