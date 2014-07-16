﻿/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FintraxPTFImages.Attributes;
using FintraxPTFImages.Common;
using FintraxPTFImages.Data;
using FintraxPTFImages.Models;
using FintraxPTFImages.ScanServiceRef;
using FintraxPTFImages.PartyManagementRef;

namespace FintraxPTFImages
{
    [AuthorizeUser]
    [RequiresSSL]
    public class VoucherController : AsyncController
    {
        public const string MESSAGE = "MESSAGE";

        public CurrentUser CurrentUser
        {
            get
            {
                return Session.Get<CurrentUser>("CurrentUser");
            }
        }

        public VoucherController()
        {
            ViewData[MESSAGE] = "";
        }

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <see cref="http://www.dotnetcurry.com/ShowArticle.aspx?ID=466"/>
        [HttpGet]
        public ActionResult Search()
        {
            #region BUILD COUNTRIES

            ViewData["CountryList"] =
                                HttpContext.Application.Get<List<CountryDetail>>("CountryList", Helper.CreateCountryDropDownLoadFunction()).CreateSelectList(
                (c) => string.Format("{0} - {1}", c.Country, c.Iso2), (c) => c.Number.ToString());

            ViewData["HeadOfficeList"] = HttpContext.Session.Get<List<SelectListItem>>("Empty", Helper.CreateEmptyLoadFunction());

            ViewData["RetailerList"] = HttpContext.Session.Get<List<SelectListItem>>("Empty", Helper.CreateEmptyLoadFunction());

            #endregion

            int? grid_page = null;
            int value;
            if (Request.QueryString.Count != 0 && int.TryParse(Request.QueryString["grid-page"], out value))
            {
                grid_page = value;
                var model = Session.Get<SearchModel>("SearchModel");
                return Search(model, grid_page);
            }
            else
            {
                ViewBag.VoucherList = Enumerable.Empty<VoucherInfo>();
                return View();
            }
        }

        [HttpPost]
        public ActionResult Search(SearchModel model, int? grid_page)
        {
            ViewData["VoucherList"] = Enumerable.Empty<VoucherInfo>();

            ViewData["CountryList"] = HttpContext.Application.Get<List<CountryDetail>>("CountryList", Helper.CreateCountryDropDownLoadFunction()).CreateSelectList(
                (c) => string.Format("{0} - {1}", c.Country, c.Iso2), (c) => c.Number.ToString());

            if (model.Country == 0)
                throw new ArgumentException("model.Country");

            ViewData["HeadOfficeList"] = HttpContext.Session.Get<int, List<HeadOffice>>("HeadOfficeList" + model.Country,
                Helper.CreateHeadOfficeDropDownLoadFunction(), model.Country).CreateSelectList((h) => string.Format("{0} - {1}", h.Name, h.Id), (h) => h.Id.ToString());

            if (model.HeadOffice == 0)
                throw new ArgumentException("model.HeadOffice");

            ViewData["RetailerList"] = HttpContext.Session.Get<int, int, List<Retailer>>("RetailerList" + model.Country + ";" + model.HeadOffice,
                Helper.CreateRetailerDropDownLoadFunction(), model.Country, model.HeadOffice).CreateSelectList((r) => string.Format("{0} - {1}", r.Name, r.Id), (r) => r.Id.ToString());

            model.Validate(this.ModelState);

            if (this.ModelState.IsValid)
            {
                Session.Set("SearchModel", model);

                ServiceAccess sdc = new ServiceAccess();
                var list = sdc.SelectVouchers(model.Country, model.Retailer);
                ViewData["VoucherList"] = list;
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult SelectHeadOffices(string value)
        {
            var isoId = value.cast<int>();

            var headoffices = HttpContext.Session.Get<int, List<HeadOffice>>("HeadOfficeList" + isoId,
                Helper.CreateHeadOfficeDropDownLoadFunction(), isoId).CreateSelectList((h) => string.Format("{0} - {1}", h.Name, h.Id), (h) => h.Id.ToString());

            return Json(new ArrayList(headoffices), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SelectRetailers(string value)
        {
            var strs = value.Split(';');
            var isoId = strs[0].cast<int>();
            var hoId = strs[1].cast<int>();

            var retailers = HttpContext.Session.Get<int, int, List<Retailer>>("RetailerList" + isoId + ";" + hoId,
                Helper.CreateRetailerDropDownLoadFunction(), isoId, hoId).CreateSelectList((r) => string.Format("{0} - {1}", r.Name, r.Id), (r) => r.Id.ToString());

            return Json(new ArrayList(retailers), JsonRequestBehavior.AllowGet);
        }

        #region Show

        [HttpGet]
        public void ShowAsync(int Id)
        {
            AsyncManager.OutstandingOperations.Increment();

            var tcpBinding = ScanServiceClient.GetBinding();
            var endpointAddress = ScanServiceClient.GetEnpoint();

            ScanServiceClient proxy = new ScanServiceClient(tcpBinding, endpointAddress);
            proxy.ReadVoucherInfoCompleted += OnReadVoucherInfoCompleted;

            string webRootPath = Server.MapPath("~/WEBVOUCHERFOLDER");
            var keys = Security.CreateInstance().GenerateSecurityKeys();

            proxy.ReadVoucherInfoAsync(Id, webRootPath, keys.Item1, keys.Item2);
        }

        private void OnReadVoucherInfoCompleted(object sender, ReadVoucherInfoCompletedEventArgs e)
        {
            try
            {
                AsyncManager.Parameters["info"] = e.Result;
            }
            catch (Exception ex)
            {
                ViewData[MESSAGE] = ex.InnerException.Message;
            }
            finally
            {
                ScanServiceClient proxy = (ScanServiceClient)sender;
                proxy.ReadVoucherInfoCompleted -= OnReadVoucherInfoCompleted;
                AsyncManager.OutstandingOperations.Decrement();
            }
        }

        // C:\VOUCHERS\[CountryID]\[RetailerId]\[VoucherId]
        public ActionResult ShowCompleted(VoucherInfo2 info = null)
        {
            var files = new List<ShowModel>();

            if (info == null)
                return View("Show", files);

            string webRootPath = Server.MapPath("~/WEBVOUCHERFOLDER");
            var webroot = new DirectoryInfo(webRootPath);
            var sessionId = webroot.Combine(info.SessionId);

            foreach (var file in sessionId.GetFiles())
            {
                if (file.Extension.EqualsNoCase(".xml"))
                {
                    var model = new ShowXmlModel(file.Name, file.FullName);
                    model.Load();
                    files.Add(model);
                }
                else
                {
                    var model = new ShowModel(file.Name,
                        string.Concat("/FintraxPTFImages/WEBVOUCHERFOLDER/", info.SessionId, "/", file.Name));
                    model.Data = System.IO.File.ReadAllBytes(file.FullName);
                    files.Add(model);
                }
            }

            return View("Show", files.ToArray());
        }

        #endregion //Show
    }
}