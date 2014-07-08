/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace BtRetryService
{
    public class TransferDBDataAccess
    {
        public static volatile string ConnectionString;

        public TransferDBDataAccess()
        { 
        }

        public List<int> GetAllCountries()
        {
            const string SQL = @"select vt_v_country_iso_id from VoucherTransfer
                                group by vt_v_country_iso_id
                                order by vt_v_country_iso_id";

            List<int> result = new List<int>();

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                using (SqlCommand comm = new SqlCommand(SQL, conn))
                {
                    using (var reader = comm.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        result.AddRange(
                        reader.ReadRange((r) =>
                              {
                                  var id = r.Get<int>("vt_v_country_iso_id");
                                  return id.HasValue ? id.Value : 0;
                              }));
                    }
                }
            }

            return result;
        }

        public List<string> GetExportRules()
        {
            const string SQL = @"select * from Config 
                                where cfg_name IS NOT NULL and  CHARINDEX ('ExportRule', cfg_name )>0";

            var result = new List<string>();

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                using (SqlCommand comm = new SqlCommand(SQL, conn))
                {
                    using (var reader = comm.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        result.AddRange(reader.ReadRange((r) =>
                        {
                            var rule = r.GetString("cfg_value");
                            return rule;
                        }));
                    }
                }
            }

            return result;
        }

        public List<Tuple<int, int, int>> GetListOfVouchers(int countryId, DateTime date)
        {
            const string SQL = 
@"select vt_v_country_iso_id, vt_v_number, vt_v_type_id from VoucherTransfer v (NOLOCK)
where vt_v_country_iso_id = @countryId and CAST( vt_creation_date as date) = CAST( @date as date) 
group by vt_v_country_iso_id, vt_v_number, vt_v_type_id
order by vt_v_country_iso_id, vt_v_number, vt_v_type_id";

            var result = new List<Tuple<int, int, int>>();

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                using (SqlCommand comm = new SqlCommand(SQL, conn))
                {
                    comm.Parameters.AddWithValue("@countryId", countryId);
                    comm.Parameters.AddWithValue("@date", date);

                    using (var reader = comm.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        result.AddRange(reader.ReadRange((r) =>
                        {
                            var iso = r.Get<int>("vt_v_country_iso_id").Value;
                            var v_number = r.Get<int>("vt_v_number").Value;
                            var type_id = r.Get<int>("vt_v_type_id").Value;
                            return new Tuple<int, int, int>(iso, v_number, type_id);
                        }));
                    }
                }
            }
            return result;
        }

        public class DbVoucherInfo
        {
            public int vt_id { get; set; }
            public int vt_v_number { get; set; }
            public int vt_v_country_iso_id { get; set; }
            public int vt_v_type_id { get; set; }
            public int vt_status_id { get; set; }
            public string vt_status_description { get; set; }
        }

        public List<DbVoucherInfo> GetListOfVouchersForDate(int iso, DateTime date, int typeId)
        {
            const string SQL = @"select vt_id, vt_v_number, vt_v_country_iso_id, 
                                    vt_v_type_id, vt_status_id, vt_status_description
                                 from VoucherTransfer (NOLOCK)
                                 where vt_v_country_iso_id = @iso and vt_v_type_id = @type and cast( vt_creation_date as date) = @date";

            const string SPNAME = "getAllVouchersByIdAndDate";

            var result = new List<DbVoucherInfo>();

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                using (SqlCommand comm = new SqlCommand(SQL, conn))
                {
                    comm.CommandType = CommandType.Text;
                    comm.Parameters.AddWithValue("@iso", iso);
                    comm.Parameters.AddWithValue("@date", date);
                    comm.Parameters.AddWithValue("@type", typeId);

                    using (var reader = comm.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        result.AddRange(reader.ReadRange((r) =>
                        {
                            var vtid = r.Get<int>("vt_id").GetValueOrDefault();
                            var vid = r.Get<int>("vt_v_number").GetValueOrDefault();
                            var isoid = r.Get<int>("vt_v_country_iso_id").GetValueOrDefault();
                            var typeid = r.Get<int>("vt_v_type_id").GetValueOrDefault();
                            var status_id = r.Get<int>("vt_status_id").GetValueOrDefault();
                            var status_description = r.GetString("vt_status_description");
                            return new DbVoucherInfo()
                            {
                                vt_id = vtid,
                                vt_v_number = vid,
                                vt_v_country_iso_id = isoid,
                                vt_v_type_id = typeid,
                                vt_status_id = status_id,
                                vt_status_description = status_description
                            };
                        }));
                    }
                }
            }
            return result;
        }

        public List<DbVoucherInfo> GetListOfVouchersByVid(int voucherId, int iso, int typeId)
        {
            const string SQL = @"select vt_id, vt_v_number, vt_v_country_iso_id, 
                                    vt_v_type_id, vt_status_id, vt_status_description
                                 from VoucherTransfer (NOLOCK)
                                 where vt_v_number = @vId and vt_v_country_iso_id = @iso and vt_v_type_id = @type";

            const string SPNAME = "getAllVouchersByIdAndDate";

            var result = new List<DbVoucherInfo>();

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                using (SqlCommand comm = new SqlCommand(SQL, conn))
                {
                    comm.CommandType = CommandType.Text;
                    comm.Parameters.AddWithValue("@vId", voucherId);
                    comm.Parameters.AddWithValue("@iso", iso);
                    comm.Parameters.AddWithValue("@type", typeId);

                    using (var reader = comm.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        result.AddRange(reader.ReadRange((r) =>
                        {
                            var vtid = r.Get<int>("vt_id").GetValueOrDefault();
                            var vid = r.Get<int>("vt_v_number").GetValueOrDefault();
                            var isoid = r.Get<int>("vt_v_country_iso_id").GetValueOrDefault();
                            var typeid = r.Get<int>("vt_v_type_id").GetValueOrDefault();
                            var status_id = r.Get<int>("vt_status_id").GetValueOrDefault();
                            var status_description = r.GetString("vt_status_description");
                            return new DbVoucherInfo()
                            {
                                vt_id = vtid,
                                vt_v_number = vid,
                                vt_v_country_iso_id = isoid,
                                vt_v_type_id = typeid,
                                vt_status_id = status_id,
                                vt_status_description = status_description
                            };
                        }));
                    }
                }
            }
            return result;
        }

        public bool CheckVoucherForStatusOK(int countryId, int voucherId, int voucherType)
        {
            const string SQL = @"select COUNT(*) from VoucherTransfer (NOLOCK)
                                where vt_v_country_iso_id = @countryId and vt_v_number = @v_number and vt_status_id = 5 and vt_v_type_id = @type_id";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                using (SqlCommand comm = new SqlCommand(SQL, conn))
                {
                    comm.Parameters.AddWithValue("@countryId", countryId);
                    comm.Parameters.AddWithValue("@v_number", voucherId);
                    comm.Parameters.AddWithValue("@type_id", voucherType);

                    var obj = comm.ExecuteScalar();
                    return Convert.ToInt32(obj) > 0;
                }
            }
        }

        public List<string> GetVoucherTransDescriptiont(int countryId, int number, int typeId)
        {
            const string SQL = @"select vt_status_description from VoucherTransfer v
                                where vt_v_country_iso_id = @countryId and vt_v_number = @v_number and vt_v_type_id = @v_type_id;";

            var result = new List<string>();

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                using (SqlCommand comm = new SqlCommand(SQL, conn))
                {
                    comm.Parameters.AddWithValue("@countryId", countryId);
                    comm.Parameters.AddWithValue("@v_number", number);
                    comm.Parameters.AddWithValue("@v_type_id", typeId);

                    using (var reader = comm.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        result.AddRange(reader.ReadRange((r) =>
                        {
                            var xml = r.GetString("vt_status_description");
                            return xml;
                        }));
                    }
                }
            }

            return result;
        }

        public string GetVoucher(int vt_id)
        {
            const string SQL = @"select top 1 vt_file_xml from VoucherTransfer v
                               where vt_id = @vt_id;";

            const string SPNAME = "getVoucherById";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                using (SqlCommand comm = new SqlCommand(SPNAME, conn))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    comm.Parameters.AddWithValue("@vt_id", vt_id);

                    using (var reader = comm.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        return reader.ReadRange((r) =>
                        {
                            var xml = r.GetString("vt_file_xml");
                            return xml;
                        }).FirstOrDefault();
                    }
                }
            }
        }

        public List<string> GetVoucher(int countryId, int number, int typeId)
        {
            const string SQL = @"select top 1 vt_file_xml from VoucherTransfer v
                               where vt_v_country_iso_id = @countryId and vt_v_number = @v_number and vt_v_type_id = @v_type_id;";
            var result = new List<string>();

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                using (SqlCommand comm = new SqlCommand(SQL, conn))
                {
                    comm.Parameters.AddWithValue("@countryId", countryId);
                    comm.Parameters.AddWithValue("@v_number", number);
                    comm.Parameters.AddWithValue("@v_type_id", typeId);

                    using (var reader = comm.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        result.AddRange(reader.ReadRange((r) =>
                        {
                            var xml = r.GetString("vt_file_xml");
                            return xml;
                        }));
                    }
                }
            }

            return result;
        }

        public string GetTagByName(long vt_id, string name)
        {
            const string SQL = "SELECT [vt_file_xml].value('(/root//{0}/node())[1]', 'nvarchar(max)') as F1 FROM VoucherTransfer WHERE vt_id = @vt_id";

            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                var sql = SQL.format(name);

                using (var comm = new SqlCommand(sql, conn))
                {
                    comm.Parameters.AddWithValue("@vt_id", vt_id);

                    var obj = comm.ExecuteScalar();
                    if (obj == DBNull.Value)
                        return null;
                    return Convert.ToString(obj);
                }
            }
        }
    }
}