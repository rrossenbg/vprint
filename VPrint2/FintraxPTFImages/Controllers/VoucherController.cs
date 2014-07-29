/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FintraxPTFImages.Attributes;
using FintraxPTFImages.Common;
using FintraxPTFImages.Data;
using FintraxPTFImages.Models;
using FintraxPTFImages.PartyManagementRef;
using FintraxPTFImages.ScanServiceRef;
using VPrinting.Common;

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
                return (CurrentUser)HttpContext.Items["CurrentUser"];
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

            int value;
            if (Request.QueryString.Count != 0 && int.TryParse(Request.QueryString["grid-page"], out value))
            {
                var model = Session.Get<SearchModel>("SearchModel");
                return Search(model, (int?)value);
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

                var sdc = new ServiceAccess();
                var list = sdc.SelectVouchers(model.Country, model.Retailer);

                var lazy = new Lazy<Dictionary<int, CurrentUser>>(new Func<Dictionary<int, CurrentUser>>(() =>
                {
                    var sdc2 = new ServiceAccess();
                    var uslist = sdc2.RetrieveUsers();
                    return uslist;
                }), true);

                ViewBag.UserDictionary = DataTables.Default.Get<Dictionary<int, CurrentUser>>("USERS", lazy);
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
            var sessionIdFolder = webroot.Combine(info.SessionId);

            //var textFile = sessionIdFolder.CombineFileName("mark.txt");
            //System.IO.File.WriteAllText(textFile.FullName, "");

            foreach (var file in sessionIdFolder.GetFiles())
            {
                if (file.Extension.EqualsNoCase(".xml"))
                {
                    var model = new ShowXmlModel(file.Name, file.FullName);
                    model.Load();
                    files.Add(model);
                }
                else
                {
                    if (file.Extension.EqualsNoCase(".tif"))
                    {
                        int count = 0;
                        var fileImages = file.FullName.TiffGetAllImages2(ImageFormat.Jpeg);
                        foreach (var fileName in fileImages)
                        {
                            var model = new ShowModel(string.Format("{0} Page {1}", file.Name, count++), fileName);
                            model.Data = System.IO.File.ReadAllBytes(fileName);
                            var base64 = Convert.ToBase64String(model.Data, Base64FormattingOptions.None);
                            model.ImgSrc = string.Concat("data:", model.Type, ";base64,", base64);
                            files.Add(model);
                        }
                    }
                    else
                    {
                        //OTHERS
                        var model = new ShowModel(file.Name, string.Concat("/FintraxPTFImages/WEBVOUCHERFOLDER/", info.SessionId, "/", file.Name));
                        model.Data = System.IO.File.ReadAllBytes(file.FullName);
                        var base64 = Convert.ToBase64String(model.Data, Base64FormattingOptions.None);
                        model.ImgSrc = string.Concat("data:", model.Type, ";base64,", base64);
                        files.Add(model);
                    }
                }
            }

            return View("Show", files.ToArray());
        }

        #endregion //Show

        #region Barcodescan

        [HttpGet]
        public ActionResult Scanbarcode()
        {
            return View(new BarcodeModel());
        }

        [HttpPost]
        public ActionResult Scanbarcode(BarcodeModel b)
        {
            string barcode = this.Request.Params["Barcode"];
            if (!string.IsNullOrWhiteSpace(barcode))
            {
                BarcodeData data = new BarcodeDecoder().Match(barcode);
                if (data != null)
                {
                    if (PTFDataAccess.CheckP1Exists(data.CountryID, data.VoucherID))
                    {
                        PTFDataAccess.ExcudeFromDebitRun(data.CountryID, data.RetailerID, data.VoucherID, CurrentUser.UserID);
                        PTFDataAccess.LogVoucher(data.CountryID, data.VoucherID, CurrentUser.UserID);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ViewBag.Err = "P1 doesn't exist. Please process voucher by using voucher entry.";
                    }
                }
                else
                {
                    ViewBag.Err = "Wrong barcode. Please renter.";
                }
            }
            else
            {
                ViewBag.Err = "Barcode may not be empty.";
            }
            return View(b);
        }

        [HttpPost]
        public ActionResult CheckBarcode(string value)
        {
            var isoId = value.cast<int>();

            var headoffices = HttpContext.Session.Get<int, List<HeadOffice>>("HeadOfficeList" + isoId,
                Helper.CreateHeadOfficeDropDownLoadFunction(), isoId).CreateSelectList((h) => string.Format("{0} - {1}", h.Name, h.Id), (h) => h.Id.ToString());

            return Json(new ArrayList(headoffices), JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}
