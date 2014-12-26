/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System.Data.SqlClient;
using System.Net;
using MerchantSite.Common;
using MerchantSite.DataServiceRef;
using VPrinting;

namespace MerchantSite.Data
{
    public partial class PTFImageDataAccess
    {
        static PTFImageDataAccess()
        {
            ServicePointManager.ServerCertificateValidationCallback = Helper.GetRemoteCertificateValidationCallback();
        }

        public int SelectVoucherImageId(int iso, int v_number)
        {
            #region SQL

            const string SQL = "SELECT id FROM VOUCHER WHERE iso_id = @iso_id and v_number = @v_number;";

            #endregion

            using (var comm = new SqlCommand(SQL))
            {
                comm.Parameters.AddWithValue("@iso_id", iso);
                comm.Parameters.AddWithValue("@v_number", v_number);
                var data = comm.CreateSerializationData().ToList().ToArray();
                DataServiceClient client = new DataServiceClient();
                var result = client.ImagesExecuteScalar(data);
                return result.Cast<int>(-1);
            }
        }
    }
}