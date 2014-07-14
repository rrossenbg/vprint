/***************************************************
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

            ViewData["CountryList"] = HttpContext.Session.Get<List<SelectListItem>>("CountryList", Helper.CreateCountryDropDownLoadFunction());
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

            ViewData["CountryList"] = HttpContext.Session.Get<List<SelectListItem>>("CountryList",
                Helper.CreateCountryDropDownLoadFunction());

            ViewData["HeadOfficeList"] = HttpContext.Session.Get<List<SelectListItem>>("HeadOfficeList" + model.Country,
                Helper.CreateHeadOfficeDropDownLoadFunction(model.Country));

            ViewData["RetailerList"] = HttpContext.Session.Get<List<SelectListItem>>("RetailerList" + model.Country + ";" + model.HeadOffice,
                Helper.CreateRetailerDropDownLoadFunction(model.Country, model.HeadOffice));

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
            var headoffices = HttpContext.Session.Get<List<SelectListItem>>("HeadOfficeList" + isoId, 
                Helper.CreateHeadOfficeDropDownLoadFunction(isoId));
            return Json(new ArrayList(headoffices), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SelectRetailers(string value)
        {
            var strs = value.Split(';');
            var isoId = strs[0].cast<int>();
            var hoId = strs[1].cast<int>();

            var retailers = HttpContext.Session.Get<List<SelectListItem>>("RetailerList" + value, 
                Helper.CreateRetailerDropDownLoadFunction(isoId, hoId));
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
            proxy.ReadVoucherInfoCompleted += (sender, e) =>
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
                    AsyncManager.OutstandingOperations.Decrement();
                }
            };

            string webRootPath = Server.MapPath("~/WEBVOUCHERFOLDER");
            var keys = Security.CreateInstance().GenerateSecurityKeys();

            proxy.ReadVoucherInfoAsync(Id, webRootPath, keys.Item1, keys.Item2);
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
                    var model = new ShowModel(file.Name, this.Url.Content("~/WEBVOUCHERFOLDER/" + info.SessionId + "/" + file.Name));
                    files.Add(model);
                }
            }

            return View("Show", files.ToArray());
        }

        #endregion //Show
    }
}
