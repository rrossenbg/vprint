/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using FintraxPTFImages.AuthenticationRef;
using FintraxPTFImages.Common;
using FintraxPTFImages.PartyManagementRef;
using FintraxPTFImages.ScanServiceRef;
using AAuthenticationHeader = FintraxPTFImages.AuthenticationRef.AuthenticationHeader;
using PAuthenticationHeader = FintraxPTFImages.PartyManagementRef.AuthenticationHeader;

namespace FintraxPTFImages.Data
{
    public class ServiceAccess
    {
        public List<VoucherInfo> SelectVouchers(int countryId, int retailerId)
        {
            IScanService client = null;
            try
            {
                client = ScanServiceClient.CreateProxy();
                var keys = Security.CreateInstance().GenerateSecurityKeys();
                return new List<VoucherInfo>(client.ReadData(countryId, retailerId, keys.Item1, keys.Item2));
            }
            finally
            {
                ((IDisposable)client).DisposeSf();
            }
        }

        public CurrentUser TryLogin(int countryId, string userName, string password)
        {
            var proxy = new AuthenticationSoapClient();
            string result = proxy.AuthenticateUser(countryId, userName, password);
            if (string.IsNullOrWhiteSpace(result))
                return CurrentUser.Empty;
            var userId = proxy.RetrieveUser(new AAuthenticationHeader(), countryId, userName);
            return new CurrentUser(userId, userName, countryId);
        }

        public HeadOffice[] RetrieveHeadOfficeList(PAuthenticationHeader header, int countryId)
        {
            var client = new PartyManagementSoapClient();
            var results = client.RetrieveHeadOfficeList(header, countryId);
            return results;
        }

        public Retailer[] RetrieveRetailerList(PAuthenticationHeader header, int countryId, int headOfficeId)
        {
            var client = new PartyManagementSoapClient();
            var results = client.RetrieveRetailerList(header, countryId, headOfficeId);
            return results;
        }
    }
}