/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MerchantSite.Data;
using MerchantSite.PartyManagementRef;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace MerchantSite.Common
{
    public static class Helper
    {
        public static Func<List<CountryDetail>> CreateCountryDropDownLoadFunction()
        {
            var funct = new Func<List<CountryDetail>>(() =>
                {
                    var client = new PartyManagementSoapClient();
                    var results = client.GetPtfCountryList(new AuthenticationHeader());
                    var results2 = results.ToList();
                    results2.Sort((c1, c2) => string.Compare(c1.Country, c2.Country));
                    return results2;
                });
            return funct;
        }

        public static Func<int, List<HeadOffice>> CreateHeadOfficeDropDownLoadFunction()
        {
            var funct = new Func<int, List<HeadOffice>>((countryId) =>
                {
                    var sac = new ScanServiceAccess();
                    var results = sac.RetrieveHeadOfficeList(new AuthenticationHeader(), countryId);
                    var results2 = results.ToList();
                    results2.Sort((c1, c2) => string.Compare(c1.Name, c2.Name));
                    return results2;
                });
            return funct;
        }

        public static Func<int, int, List<Retailer>> CreateRetailerDropDownLoadFunction()
        {
            var funct = new Func<int, int, List<Retailer>>((countryId, headOfficeId) =>
               {
                   var sac = new ScanServiceAccess();
                   var results = sac.RetrieveRetailerList(new AuthenticationHeader(), countryId, headOfficeId);
                   var results2 = results.ToList();
                   results2.Sort((c1, c2) => string.Compare(c1.Name, c2.Name));
                   return results2;
               });
            return funct;
        }

        public static Func<List<SelectListItem>> CreateEmptyDropDownLoadFunction()
        {
            var funct = new Func<List<SelectListItem>>(() =>
            {
                var items = new List<SelectListItem>();
                items.Add(new SelectListItem { Text = "Please Select", Value = "0", Selected = true });
                return items;
            });
            return funct;
        }

        public static Lazy<Dictionary<int, CurrentUser>> GetUserTableLazy()
        {
            var lazy = new Lazy<Dictionary<int, CurrentUser>>(new Func<Dictionary<int, CurrentUser>>(() =>
            {
                var sdc2 = new ScanServiceAccess();
                var uslist = sdc2.RetrieveUsers();
                return uslist;
            }), true);
            return lazy;
        }

        public static RemoteCertificateValidationCallback GetRemoteCertificateValidationCallback()
        {
            return new RemoteCertificateValidationCallback(ValidateRemoteCertificate);
        }

        private static bool ValidateRemoteCertificate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors policyErrors)
        {
#if DEBUG
#warning TEST_CODE
            return true;
#endif

            string serial = cert.GetSerialNumberString();
            if (string.Equals(serial, "33C093D4D173FEB60D138AAE81336E17"))
                return true;
            return false;
        }
    }
}