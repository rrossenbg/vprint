/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FintraxPTFImages.Data;
using FintraxPTFImages.PartyManagementRef;

namespace FintraxPTFImages.Common
{
    public static class Helper
    {
                    ////        var items = new List<SelectListItem>();
                    ////items.Add(new SelectListItem { Text = "Please Select", Value = "0", Selected = true });

                    ////foreach (var cnt in results2)
                    ////    items.Add(new SelectListItem { Text = string.Format("{0} - {1}", cnt.Country, cnt.Iso2), Value = cnt.Number.ToString() });
                    ////return items;
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

                    ////        var items = new List<SelectListItem>();
                    ////items.Add(new SelectListItem { Text = "Please Select", Value = "0", Selected = true });

                    ////foreach (var cnt in results2)
                    ////    items.Add(new SelectListItem { Text = string.Format("{0} - {1}", cnt.Name, cnt.Id), Value = cnt.Id.ToString() });
                    ////return items;

        public static Func<int, List<HeadOffice>> CreateHeadOfficeDropDownLoadFunction()
        {
            var funct = new Func<int, List<HeadOffice>>((countryId) =>
                {
                    var sac = new ServiceAccess();
                    var results = sac.RetrieveHeadOfficeList(new AuthenticationHeader(), countryId);
                    var results2 = results.ToList();
                    results2.Sort((c1, c2) => string.Compare(c1.Name, c2.Name));
                    return results2;
                });
            return funct;
        }

                   ////        var items = new List<SelectListItem>();
                   ////items.Add(new SelectListItem { Text = "Please Select", Value = "0", Selected = true });

                   ////foreach (var cnt in results2)
                   ////    items.Add(new SelectListItem { Text = string.Format("{0} - {1}", cnt.Name, cnt.Id), Value = cnt.Id.ToString() });
                   ////return items;

        public static Func<int, int, List<Retailer>> CreateRetailerDropDownLoadFunction()
        {
            var funct = new Func<int, int, List<Retailer>>((countryId, headOfficeId) =>
               {
                   var sac = new ServiceAccess();
                   var results = sac.RetrieveRetailerList(new AuthenticationHeader(), countryId, headOfficeId);
                   var results2 = results.ToList();
                   results2.Sort((c1, c2) => string.Compare(c1.Name, c2.Name));
                   return results2;
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