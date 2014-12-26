/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Net;
using MerchantSite.Common;
using MerchantSite.DataServiceRef;
using VPrinting;

namespace MerchantSite.Data
{
    partial class PTFDataAccess
    {
        static PTFDataAccess()
        {
            ServicePointManager.ServerCertificateValidationCallback = Helper.GetRemoteCertificateValidationCallback();
        }

        public static List<DateTime> SelectAllNotPaidNotaDebitoInvoicesDistByDate(int countryId)
        {
            #region SQL

            const string SQL = @"SELECT DISTINCT CAST(in_date as DATE)as [date] FROM NotaDebitoInvoice
                                WHERE in_iso_id = @iso and in_type = 'N' and in_paid = 'N' and in_sepa_msgid is NULL
                                ORDER BY CAST(in_date as DATE) DESC;";

            #endregion

            var list = new List<DateTime>();

            using (var comm = new SqlCommand(SQL))
            {
                comm.Parameters.AddWithValue("@iso", countryId);
                var data = comm.CreateSerializationData().ToList().ToArray();
                DataServiceClient client = new DataServiceClient();
                var result = client.TRSExecuteReader(data);
                using (DataTableReader reader = new DataTableReader(result.Data.ToDataTable()))
                    while (reader.Read())
                    {
                        var date = reader.Get<DateTime>("date").GetValueOrDefault();
                        list.Add(date);
                    }
            }
            return list;
        }

        public static List<int> SelectAllNotPaidNotaDebitoInvoicesDistByNumberPerDate(int countryId, DateTime date)
        {
            #region SQL

            const string SQL = @"SELECT DISTINCT in_number FROM NotaDebitoInvoice 
                              WHERE in_iso_id = @iso and in_paid = 'N' and CAST(in_date as DATE) = @date and in_sepa_msgid is NULL and in_type in ('N', '0') 
                              ORDER BY in_number;";

            #endregion

            var list = new List<int>();

            using (var comm = new SqlCommand(SQL))
            {
                comm.Parameters.AddWithValue("@iso", countryId);
                comm.Parameters.AddWithValue("@date", date.Date);
                var data = comm.CreateSerializationData().ToList().ToArray();
                DataServiceClient client = new DataServiceClient();
                var result = client.TRSExecuteReader(data);
                using (DataTableReader reader = new DataTableReader(result.Data.ToDataTable()))
                    while (reader.Read())
                    {
                        var number = reader.Get<int>("in_number").GetValueOrDefault();
                        list.Add(number);
                    }
            }
            return list;
        }

        public class SelectForNotaDebitos_Data 
        {
            public int in_iso { get; set; }
            public int in_number { get; set; }
            public DateTime in_date { get; set; }
            public int in_ho_id { get; set; }
            public string ho_name { get; set; }
            public string in_period { get; set; }
            public string in_sepa_msgid { get; set; }
            public string in_type { get; set; }
            public Guid in_key { get; set; }
            public decimal total { get; set; }

            public SelectForNotaDebitos_Data()
            {
            }

            public SelectForNotaDebitos_Data(DbDataReader reader, int iso)
            {
                in_iso = iso;
                in_number = reader.Get<int>("in_number").GetValueOrDefault();
                in_date = reader.Get<DateTime>("in_date").GetValueOrDefault();
                in_ho_id = reader.Get<int>("in_ho_id").GetValueOrDefault();
                ho_name = reader.GetString("ho_name");
                in_period = reader.GetString("in_period");
                in_type = reader.GetString("in_type");
                total = reader.Get<decimal>("total").GetValueOrDefault();
            }
        }

        public static List<SelectForNotaDebitos_Data> SelectForNotaDebitosByNumber(int iso, int number, DateTime in_from, DateTime in_to)
        {
            #region SQL

            const string SQL = @"
            SELECT top 1000 in_number, in_date, in_ho_id, ho_name, in_period, in_sepa_msgid, in_type, in_key, SUM(inv_vat_amount) as [total] 
            FROM NotaDebitoInvoice 
            INNER JOIN HeadOffice on ho_id = in_ho_id and ho_iso_id = in_iso_id  
            INNER JOIN NotaDebitoInvoiceVouchers on inv_in_number = in_number and inv_iso_id = in_iso_id
            WHERE in_iso_id = @iso and in_date >= @in_from  and in_date <= @in_to and in_type in ('N', '0') and in_paid = 'Y' and in_number like @in_number
            GROUP BY in_number, in_date, in_ho_id, ho_name, in_period, in_sepa_msgid, in_type, in_key;";
            //in_paid = 'N' and in_sepa_msgid is NULL and 
            #endregion

            var list = new List<SelectForNotaDebitos_Data>();

            using (var comm = new SqlCommand(SQL))
            {
                comm.Parameters.AddWithValue("@iso", iso);
                comm.Parameters.AddWithValue("@in_number", number);
                comm.Parameters.AddWithValue("@in_from", in_from);
                comm.Parameters.AddWithValue("@in_to", in_to);
                var data = comm.CreateSerializationData().ToList().ToArray();
                DataServiceClient client = new DataServiceClient();
                var result = client.TRSExecuteReader(data);
                using (DataTableReader reader = new DataTableReader(result.Data.ToDataTable()))
                    while (reader.Read())
                        list.Add(new SelectForNotaDebitos_Data(reader, iso));
            }

            return list;
        }

        public static List<SelectForNotaDebitos_Data> SelectForNotaDebitosPerCountry(int iso, DateTime in_from, DateTime in_to)
        {
            #region SQL

            const string SQL = @"
            SELECT top 1000 in_number, in_date, in_ho_id, ho_name, in_period, in_sepa_msgid, in_type, in_key, SUM(inv_vat_amount) as [total] 
            FROM NotaDebitoInvoice 
            INNER JOIN HeadOffice on ho_id = in_ho_id and ho_iso_id = in_iso_id  
            INNER JOIN NotaDebitoInvoiceVouchers on inv_in_number = in_number and inv_iso_id = in_iso_id
            WHERE in_iso_id = @iso and in_date >= @in_from  and in_date <= @in_to and in_type in ('N', '0') and in_paid = 'Y' 
            GROUP BY in_number, in_date, in_ho_id, ho_name, in_period, in_sepa_msgid, in_type, in_key;";
            //in_paid = 'N' and in_sepa_msgid is NULL and 
            #endregion

            var list = new List<SelectForNotaDebitos_Data>();

            using (var comm = new SqlCommand(SQL))
            {
                comm.Parameters.AddWithValue("@iso", iso);
                comm.Parameters.AddWithValue("@in_from", in_from);
                comm.Parameters.AddWithValue("@in_to", in_to);
                var data = comm.CreateSerializationData().ToList().ToArray();
                DataServiceClient client = new DataServiceClient();
                var result = client.TRSExecuteReader(data);
                using (DataTableReader reader = new DataTableReader(result.Data.ToDataTable()))
                    while (reader.Read())
                        list.Add(new SelectForNotaDebitos_Data(reader, iso));
            }

            return list;
        }

        public static List<SelectForNotaDebitos_Data> SelectForNotaDebitosPerHeadOffice(int iso, int in_ho_id, DateTime in_from, DateTime in_to)
        {
            #region SQL

            const string SQL = @"
            SELECT top 1000 in_number, in_date, in_ho_id, ho_name, in_period, in_sepa_msgid, in_type, in_key, SUM(inv_vat_amount) as [total] 
            FROM NotaDebitoInvoice 
            INNER JOIN HeadOffice on ho_id = in_ho_id and ho_iso_id = in_iso_id  
            INNER JOIN NotaDebitoInvoiceVouchers on inv_in_number = in_number and inv_iso_id = in_iso_id
            WHERE in_iso_id = @iso and in_ho_id = @in_ho_id and in_date >= @in_from  and in_date <= @in_to and in_type in ('N', '0') and in_paid = 'Y' 
            GROUP BY in_number, in_date, in_ho_id, ho_name, in_period, in_sepa_msgid, in_type, in_key;";
            //in_paid = 'N' and in_sepa_msgid is NULL and 
            #endregion

            var list = new List<SelectForNotaDebitos_Data>();

            using (var comm = new SqlCommand(SQL))
            {
                comm.Parameters.AddWithValue("@iso", iso);
                comm.Parameters.AddWithValue("@in_from", in_from);
                comm.Parameters.AddWithValue("@in_to", in_to);
                comm.Parameters.AddWithValue("@in_ho_id", in_ho_id);
                var data = comm.CreateSerializationData().ToList().ToArray();
                DataServiceClient client = new DataServiceClient();
                var result = client.TRSExecuteReader(data);
                using (DataTableReader reader = new DataTableReader(result.Data.ToDataTable()))
                    while (reader.Read())
                        list.Add(new SelectForNotaDebitos_Data(reader, iso));
                client.Close();
            }

            return list;
        }
    }
}