/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace ReceivingServiceLib.Common.Data
{
    [Obfuscation(StripAfterObfuscation = true)]
    public class PTFDataAccess
    {
        public class FindVoucher_VoucherInfo
        {
            public int v_iso_id { get; set; }
            public int v_number { get; set; }
            public int v_br_id { get; set; }
            public DateTime v_date_voucher { get; set; }
            public string sitecode { get; set; }
            public string v_final_country { get; set; }
        }

        public FindVoucher_VoucherInfo FindVoucher(int countryId, int voucherId)
        {
            #region SQL

            const string SQL = @"
select v_iso_id, v_number, vp_site_code, vp_location_number, v_date_voucher, v_final_country, v_br_id from voucher
inner join voucherpart on vp_iso_id = v_iso_id and vp_v_number = v_number and vp_type_id = 2
where v_iso_id = @iso and v_number = @number;";

            #endregion

            using (var conn = new SqlConnection(Global.Strings.PTFConnString))
            {
                conn.Open();

                using (var comm = new SqlCommand(SQL, conn))
                {
                    comm.CommandType = CommandType.Text;
                    comm.Parameters.AddWithValue("@iso", countryId);
                    comm.Parameters.AddWithValue("@number", voucherId);
                    using (var reader = comm.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        return
                            reader.ReadRange<FindVoucher_VoucherInfo>((r) =>
                            {
                                var result = new FindVoucher_VoucherInfo();
                                result.v_iso_id = r.Get<int>("v_iso_id").Value;
                                result.v_number = r.Get<int>("v_number").Value;
                                result.v_br_id = r.Get<int>("v_br_id").Value;
                                result.sitecode = r.GetString("vp_site_code") + r.GetString("vp_location_number");
                                result.v_date_voucher = r.Get<DateTime>("v_date_voucher").GetValueOrDefault();
                                result.v_final_country = r.GetString("v_final_country");
                                return result;
                            }).FirstOrDefault() ?? new PTFDataAccess.FindVoucher_VoucherInfo();
                    }
                }
            }
        }
    }
}
