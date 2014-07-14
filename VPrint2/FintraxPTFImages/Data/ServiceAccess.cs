/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

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
            ScanServiceClient proxy = new ScanServiceClient();
            return proxy.ReadData(countryId, retailerId).ToList();
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

        public HeadOffice[] RetrieveHeadOfficeList(int countryId)
        {
            var client = new PartyManagementSoapClient();
            var results = client.RetrieveHeadOfficeList(new PAuthenticationHeader(), countryId);
            return results;
        }

        public Retailer[] RetrieveRetailerList(int countryId, int headOfficeId)
        {
            var client = new PartyManagementSoapClient();
            var results = client.RetrieveRetailerList(new PAuthenticationHeader(), countryId, headOfficeId);
            return results;
        }
    }
}