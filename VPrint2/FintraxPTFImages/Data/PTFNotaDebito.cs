
/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;

namespace FintraxPTFImages.Data
{
    partial class PTFDataAccess
    {
        public class SelectForNotaDebitos_Data
        {
            public int in_number { get; set; }
            public DateTime in_date { get; set; }
            public int in_ho_id { get; set; }
            public string ho_name { get; set; }
            public string in_period { get; set; }
            public string in_sepa_msgid { get; set; }
            public int in_type { get; set; }
            public Guid in_key { get; set; }
            public decimal total { get; set; }

            public SelectForNotaDebitos_Data(SqlDataReader reader)
            {
            }
        }

        public static void SelectForNotaDebitos(int iso, int in_from, int in_to)
        {
            #region SQL

            const string SQL = @"
            SELECT in_number, in_date, in_ho_id, ho_name, in_period, in_sepa_msgid, in_type, in_key, SUM(inv_vat_amount) as [total] 
            FROM NotaDebitoInvoice 
            INNER JOIN HeadOffice on ho_id = in_ho_id and ho_iso_id = in_iso_id  
            INNER JOIN NotaDebitoInvoiceVouchers on inv_in_number = in_number and inv_iso_id = in_iso_id
            WHERE in_number >= @in_from  and in_number <= @in_to and in_iso_id = @iso and in_paid = 'N' and 
                    in_sepa_msgid is NULL and in_type in ('N', '0') 
            GROUP BY in_number, in_date, in_ho_id, ho_name, in_period, in_sepa_msgid, in_type, in_key;";

            #endregion

            using (var conn = new SqlConnection(ConnectionString))
            using (var comm = new SqlCommand(SQL, conn))
            {
                comm.Parameters.AddWithValue("@iso", iso);
                comm.Parameters.AddWithValue("@in_from", in_from);
                comm.Parameters.AddWithValue("@in_to", in_to);
                conn.Open();
                using (var reader = comm.ExecuteReader(CommandBehavior.CloseConnection))
                {
                }
            }
        }
    }
}