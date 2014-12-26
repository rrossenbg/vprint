/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using MerchantSite.Attributes;
using MerchantSite.Common;
using MerchantSite.Data;
using MerchantSite.Models;
using MerchantSite.PartyManagementRef;
using MerchantSite.ScanServiceRef;
using VPrinting;

namespace MerchantSite
{
    [AuthorizeUser]
    [RequiresSSL]
    public class InvoiceController : AsyncController
    {
        public const string MESSAGE = "MESSAGE";

        public CurrentUser CurrentUser
        {
            get
            {
                return (CurrentUser)HttpContext.Items["CurrentUser"];
            }
        }

        [HttpGet]
        public ActionResult Search()
        {
            var model = new InvoiceSearch_Model();
            model.Country = CurrentUser.CountryID;

            ViewData["HeadOfficeList"] = HttpContext.Session.Get<int, List<HeadOffice>>("HeadOfficeList" + CurrentUser.CountryID,
                Helper.CreateHeadOfficeDropDownLoadFunction(), CurrentUser.CountryID).CreateSelectList((h) => string.Format("{0} - {1}", h.Name, h.Id), (h) => h.Id.ToString());

            ViewData["RetailerList"] =
                HttpContext.Session.Get<List<SelectListItem>>("Empty", Helper.CreateEmptyDropDownLoadFunction());

            return View(model);
        }

        [HttpPost]
        public ActionResult Search(InvoiceSearch_Model model)
        {

            model.Validate(this.ModelState);

            if (this.ModelState.IsValid)
            {
                if (model.UseNumber)
                    ViewData["InvoiceSearchList"] =
                        PTFDataAccess.SelectForNotaDebitosByNumber(model.Country, model.Number, model.FromDate, model.ToDate);
                else
                    ViewData["InvoiceSearchList"] =
                        PTFDataAccess.SelectForNotaDebitosPerHeadOffice(model.Country, model.HeadOffice, model.FromDate, model.ToDate);
            }
            ViewData["HeadOfficeList"] = HttpContext.Session.Get<int, List<HeadOffice>>("HeadOfficeList" + CurrentUser.CountryID,
                Helper.CreateHeadOfficeDropDownLoadFunction(), CurrentUser.CountryID).CreateSelectList((h) => string.Format("{0} - {1}", h.Name, h.Id), (h) => h.Id.ToString());

            ViewData["RetailerList"] =
                    HttpContext.Session.Get<List<SelectListItem>>("Empty", Helper.CreateEmptyDropDownLoadFunction());
            return View(model);
        }

        [HttpGet]
        public ActionResult ShowDetails(int country, int office, DateTime date, int invoice)
        {
            Guid id = CommTools.ToGuid(country, office, date.ToInt(), invoice);

            ObjectFileCache cache = new ObjectFileCache(Config.FILECACHEFOLDER);
            string bodyFileName = string.Concat("body_", id);
            string styleFileName = string.Concat("style_", id);
            string body = cache.Get(bodyFileName, () =>
            {
                var buffer1 = ScanServiceAccess.Instance.DownloadNotaDebitoReport(country, office, date, invoice, "XML");
                string xml = Encoding.UTF8.GetString(buffer1);

                var buffer2 = ScanServiceAccess.Instance.DownloadNotaDebitoReport(country, office, date, invoice, "HTML4.0", "&rc:Toolbar=False&rc:Section=0");
                var html = Encoding.UTF8.GetString(buffer2);
                string bodyTag = html.GetTags("body").FirstOrDefault();
                if (bodyTag == null)
                    throw new ArgumentException("Body tag empty");

                var builder = new StringBuilder(bodyTag);
                foreach (string v_number in xml.Search("inv_v_number2\\s*=\\s*\"(?<value>[0-9]+)\"", (Match m) => m.Groups["value"].Value))
                    builder.Replace(v_number, string.Concat("<a href=", Config.SITEROOT, "/Voucher/ShowBy?iso_id=", country, "&v_number=", v_number, " >", v_number, "</a>"));

                bodyTag = builder.ToString();
                bodyTag = bodyTag.ReplaceTags("img", new MatchEvaluator(ImgMatchEvaluator));
                var styleTag = html.GetTags("style").FirstOrDefault();
                cache.Set(styleFileName, styleTag);
                return bodyTag;
            });
            string style = cache.Get(styleFileName, () => string.Empty);


            return View(new string[] { body, style });
        }

        private string ImgMatchEvaluator(Match m)
        {
            string value = string.Format("<img width=\"77\" height=\"110\" src=\"{0}/Images/PTFLogo.png\"/>", Config.SITEROOT);
            return value;
        }

        [HttpPost]
        public ActionResult ShowDetails()
        {
            return View();
        }

        #region INTERNAL USE ONLY

        [HttpGet]
        public ActionResult NotaDebito()
        {
            ViewData["CountryList"] =
                                HttpContext.Application.Get<List<CountryDetail>>("CountryList", Helper.CreateCountryDropDownLoadFunction()).CreateSelectList(
                (c) => string.Format("{0} - {1}", c.Country, c.Iso2), (c) => c.Number.ToString());

            ViewData["HeadOfficeList"] = HttpContext.Session.Get<List<SelectListItem>>("Empty", Helper.CreateEmptyDropDownLoadFunction());

            ViewData["RetailerList"] = HttpContext.Session.Get<List<SelectListItem>>("Empty", Helper.CreateEmptyDropDownLoadFunction());
            return View();
        }

        [HttpPost]
        public ActionResult NotaDebito(NotaDebito_Model model)
        {
            if (this.ModelState.IsValid)
            {
                ViewData["NotaDebitoList"] =
                    PTFDataAccess.SelectForNotaDebitosPerCountry(model.Country, model.FromDate, model.ToDate);
            }

            ViewData["CountryList"] =
                    HttpContext.Application.Get<List<CountryDetail>>("CountryList", Helper.CreateCountryDropDownLoadFunction()).CreateSelectList(
                    (c) => string.Format("{0} - {1}", c.Country, c.Iso2), (c) => c.Number.ToString());

            ViewData["HeadOfficeList"] =
                    HttpContext.Session.Get<List<SelectListItem>>("Empty", Helper.CreateEmptyDropDownLoadFunction());

            ViewData["RetailerList"] =
                    HttpContext.Session.Get<List<SelectListItem>>("Empty", Helper.CreateEmptyDropDownLoadFunction());

            return View(model);
        }

        [HttpGet]
        public ActionResult DownloadNotaDebito(int country, int office, DateTime date, int invoice)
        {
            Guid id = CommTools.ToGuid(country, office, date.ToInt(), invoice);
            ObjectFileCache cache = new ObjectFileCache(Config.FILECACHEFOLDER);
            var buffer = cache.Get("pdf_" + id, () => ScanServiceAccess.Instance.DownloadNotaDebitoReport(country, office, date, invoice));
            string fileName = string.Format("{0}_{1}_{2:dd-MM-yyyy}_{3}.pdf", country, office, date, invoice);
            return base.File(buffer, "application/pdf", fileName);
        }

        [HttpGet]
        public ActionResult NotaDebitoEmail()
        {
            ViewData["CountryList"] =
                    HttpContext.Application.Get<List<CountryDetail>>("CountryList", Helper.CreateCountryDropDownLoadFunction()).CreateSelectList(
    (c) => string.Format("{0} - {1}", c.Country, c.Iso2), (c) => c.Number.ToString());

            ViewData["HeadOfficeList"] = HttpContext.Session.Get<List<SelectListItem>>("Empty", Helper.CreateEmptyDropDownLoadFunction());

            ViewData["RetailerList"] = HttpContext.Session.Get<List<SelectListItem>>("Empty", Helper.CreateEmptyDropDownLoadFunction());
            return View();
        }

        [HttpPost]
        public ActionResult NotaDebitoEmail(NotaDebitoEmail_Model model)
        {
            if (this.ModelState.IsValid)
            {
                ViewData["NotaDebitoList"] =
                    PTFDataAccess.SelectForNotaDebitosPerHeadOffice(model.Country, model.HeadOffice, model.FromDate, model.ToDate);
            }

            ViewData["CountryList"] =
                    HttpContext.Application.Get<List<CountryDetail>>("CountryList", Helper.CreateCountryDropDownLoadFunction()).CreateSelectList(
                    (c) => string.Format("{0} - {1}", c.Country, c.Iso2), (c) => c.Number.ToString());

            ViewData["HeadOfficeList"] =
                    HttpContext.Session.Get<List<SelectListItem>>("Empty", Helper.CreateEmptyDropDownLoadFunction());

            ViewData["RetailerList"] =
                    HttpContext.Session.Get<List<SelectListItem>>("Empty", Helper.CreateEmptyDropDownLoadFunction());

            return View(model);
        }

        [HttpPost]
        public ActionResult NotaDebitoEmail2(NotaDebitoEmail_Model model)
        {
            var emails = new List<string>(this.Request["Email2"].Split(',')).ConvertAll(s => Convert.ToBoolean(s));
            var in_isos = new List<string>(this.Request["in_iso"].Split(',')).ConvertAll(s => Convert.ToInt32(s));
            var in_ho_ids = new List<string>(this.Request["in_ho_id"].Split(',')).ConvertAll(s => Convert.ToInt32(s));
            var in_numbers = new List<string>(this.Request["in_number"].Split(',')).ConvertAll(s => Convert.ToInt32(s));
            var in_dates = new List<string>(this.Request["in_date"].Split(',')).ConvertAll(s => DateTime.Parse(s));

            var list = new List<EmailInfo>();

            for (int i = 0; i < in_isos.Count; i++)
            {
                int in_iso = in_isos[i];
                int ho_id = in_ho_ids[i];
                int in_number = in_numbers[i];
                DateTime in_date = in_dates[i];
                bool email = emails[i];
                if (email)
                    list.Add(new EmailInfo() { IsoId = in_iso, HoId = ho_id, InNumber = in_number, 
                        InDate = in_date, Subject = model.Subject, Body = model.Body, CC = model.CC });
            }

            if (list.Count > 0)
            {
                var cc = model.CC;
                ScanServiceAccess.Instance.EmailNotaDebito(list);
            }

            return RedirectToAction("NotaDebitoEmail");
        }

        #endregion

        #region AJAX CALLS

        [HttpPost]
        public ActionResult SelectHeadOffices(string value)
        {
            var isoId = value.ConvertTo<string, int>();

            var headoffices = HttpContext.Session.Get<int, List<HeadOffice>>("HeadOfficeList" + isoId,
                Helper.CreateHeadOfficeDropDownLoadFunction(), isoId).CreateSelectList((h) => string.Format("{0} - {1}", h.Name, h.Id), (h) => h.Id.ToString());

            return Json(new ArrayList(headoffices), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SelectRetailers(string value)
        {
            var strs = value.Split(';');
            var isoId = strs[0].Cast<int>();
            var hoId = strs[1].Cast<int>();

            var retailers = HttpContext.Session.Get<int, int, List<Retailer>>("RetailerList" + isoId + ";" + hoId,
                Helper.CreateRetailerDropDownLoadFunction(), isoId, hoId).CreateSelectList((r) => string.Format("{0} - {1}", r.Name, r.Id), (r) => r.Id.ToString());

            return Json(new ArrayList(retailers), JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}
