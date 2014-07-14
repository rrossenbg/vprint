/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FintraxPTFImages.PartyManagementRef;
using FintraxPTFImages.Data;

namespace FintraxPTFImages.Common
{
    public static class Helper
    {
        public static Func<List<SelectListItem>> CreateCountryDropDownLoadFunction()
        {
            var funct = new Func<List<SelectListItem>>(() =>
            {
                var client = new PartyManagementSoapClient();
                var results = client.GetPtfCountryList(new AuthenticationHeader());
                var results2 = results.ToList();
                results2.Sort((c1, c2) => string.Compare(c1.Country, c2.Country));

                var items = new List<SelectListItem>();
                items.Add(new SelectListItem { Text = "Please Select", Value = "0", Selected = true });

                foreach (var cnt in results2)
                    items.Add(new SelectListItem { Text = string.Format("{0} - {1}", cnt.Country, cnt.Iso2), Value = cnt.Number.ToString() });
                return items;
            });
            return funct;
        }

        public static Func<List<SelectListItem>> CreateHeadOfficeDropDownLoadFunction(int countryId)
        {
            var funct = new Func<List<SelectListItem>>(() =>
            {
                var sac = new ServiceAccess();
                var results = sac.RetrieveHeadOfficeList(countryId);
                var results2 = results.ToList();
                results2.Sort((c1, c2) => string.Compare(c1.Name, c2.Name));

                var items = new List<SelectListItem>();
                items.Add(new SelectListItem { Text = "Please Select", Value = "0", Selected = true });

                foreach (var cnt in results2)
                    items.Add(new SelectListItem { Text = string.Format("{0} - {1}", cnt.Name, cnt.Id), Value = cnt.Id.ToString() });
                return items;
            });
            return funct;
        }

        public static Func<List<SelectListItem>> CreateRetailerDropDownLoadFunction(int countryId, int headOfficeId)
        {
            var funct = new Func<List<SelectListItem>>(() =>
            {
                var sac = new ServiceAccess();
                var results = sac.RetrieveRetailerList(countryId, headOfficeId);
                var results2 = results.ToList();
                results2.Sort((c1, c2) => string.Compare(c1.Name, c2.Name));

                var items = new List<SelectListItem>();
                items.Add(new SelectListItem { Text = "Please Select", Value = "0", Selected = true });

                foreach (var cnt in results2)
                    items.Add(new SelectListItem { Text = string.Format("{0} - {1}", cnt.Name, cnt.Id), Value = cnt.Id.ToString() });
                return items;
            });
            return funct;
        }

        public static Func<List<SelectListItem>> CreateEmptyLoadFunction()
        {
            var funct = new Func<List<SelectListItem>>(() =>
            {
                var items = new List<SelectListItem>();
                items.Add(new SelectListItem { Text = "Please Select", Value = "0", Selected = true });
                return items;
            });
            return funct;
        }
    }
}