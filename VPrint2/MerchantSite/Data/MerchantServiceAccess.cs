/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System.Net;
using MerchantSite.Common;
using MerchantSite.DataServiceRef;

namespace MerchantSite.Data
{
    public class MerchantServiceAccess
    {
        static MerchantServiceAccess()
        {
            ServicePointManager.ServerCertificateValidationCallback = Helper.GetRemoteCertificateValidationCallback();
        }

        public static MerchantServiceAccess Instance
        {
            get
            {
                return new MerchantServiceAccess();
            }
        }

        public string test_GetData()
        {
            DataServiceClient client = new DataServiceClient();
            var result = client.GetData(123);
            client.Close();
            return result;
        }
    }
}