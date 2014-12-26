/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using MerchantSite.Attributes;
using MerchantSite.Common;
using MerchantSite.Data;
using MerchantSite.Models;
using MerchantSite.PartyManagementRef;
using MerchantSite.ScanServiceRef;
using VPrinting;
using MerchantSite.FileServiceRef;

namespace MerchantSite
{
    [AuthorizeUser]
    [RequiresSSL]
    public class VoucherController : AsyncController
    {
        public CurrentUser CurrentUser
        {
            get
            {
                return (CurrentUser)HttpContext.Items["CurrentUser"];
            }
        }

        public VoucherController()
        {
            ViewData[MerchantSiteStrings.MESSAGE] = "";
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
        public ActionResult Search(int? grid_page)
        {
            #region BUILD COUNTRIES

            ViewData["CountryList"] =
                                HttpContext.Application.Get<List<CountryDetail>>("CountryList", Helper.CreateCountryDropDownLoadFunction()).CreateSelectList(
                (c) => string.Format("{0} - {1}", c.Country, c.Iso2), (c) => c.Number.ToString());

            ViewData["HeadOfficeList"] = HttpContext.Session.Get<List<SelectListItem>>("Empty", Helper.CreateEmptyDropDownLoadFunction());

            ViewData["RetailerList"] = HttpContext.Session.Get<List<SelectListItem>>("Empty", Helper.CreateEmptyDropDownLoadFunction());

            #endregion

            int value;
            if (Request.QueryString.Count != 0 && int.TryParse(Request.QueryString["grid-page"], out value))
            {
                var model = Session.Get<SearchModel>("SearchModel");
                return Search(model);
            }
            else
            {
                ViewBag.VoucherList = Enumerable.Empty<VoucherInfo>();
                return View();
            }
        }

        [HttpPost]
        public ActionResult Search(SearchModel model)
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
                this.Session.Set(MerchantSiteStrings.SearchModel, model);

                var sdc = new ScanServiceAccess();
                var list = sdc.SelectVouchersByRetailer(model.Country, model.Retailer);

                var lazy = Helper.GetUserTableLazy();

                ViewBag.UserDictionary = DataTables.Default.Get<Dictionary<int, CurrentUser>>(MerchantSiteStrings.USERS, lazy);
                ViewData["VoucherList"] = list;
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult SearchByNumber()
        {
            #region BUILD COUNTRIES

            ViewData["VoucherList"] = Enumerable.Empty<fileInfo>();

            ViewData["CountryList"] = HttpContext.Application.Get<List<CountryDetail>>("CountryList",
                Helper.CreateCountryDropDownLoadFunction()).CreateSelectList(
                (c) => string.Format("{0} - {1}", c.Country, c.Iso2), (c) => c.Number.ToString());

            #endregion

            int value;
            if (Request.QueryString.Count != 0 && int.TryParse(Request.QueryString["grid-page"], out value))
            {
                var model = Session.Get<SearchModel2>("SearchModel2");
                return SearchByNumber(model, (int?)value);
            }
            else
            {
                return View("SearchByNumber");
            }
        }

        [HttpPost]
        public ActionResult SearchByNumber(SearchModel2 model, int? grid_page)
        {
            ViewData["VoucherList"] = Enumerable.Empty<fileInfo>();

            ViewData["CountryList"] = HttpContext.Application.Get<List<CountryDetail>>("CountryList",
                Helper.CreateCountryDropDownLoadFunction()).CreateSelectList(
                (c) => string.Format("{0} - {1}", c.Country, c.Iso2), (c) => c.Number.ToString());

            if (model.Country == 0)
                throw new ArgumentException("model.Country");

            model.Validate(this.ModelState);

            if (this.ModelState.IsValid)
            {
                Session.Set("SearchModel2", model);

                var sdc = new ScanServiceAccess();
                var infos = sdc.SelectVouchersByNumber(model.Country, model.Voucher);

                ViewData["VoucherList"] = infos.ToList();
            }

            return View(model);
        }

        #region SHOW VOUCHER IMAGE

        private const int ASINCH_TIMEOUT = 3000;

        [HttpGet]
        public ActionResult ShowBy(int iso_id, int v_number)
        {
            PTFImageDataAccess da = new PTFImageDataAccess();
            int id = da.SelectVoucherImageId(iso_id, v_number);
#warning TEST_CODE
            if (id == -1)
                id = id.Random(max: 1000);
            return RedirectToAction("Show", new { Id = id });
        }

        [HttpGet]
        public void ShowAsync(int Id)
        {
            string shareVoucherRootPath = Config.WEBVOUCHERFOLDER_SHARE;

            AsyncManager.OutstandingOperations.Increment();
            AsyncManager.Timeout = ASINCH_TIMEOUT;

            var tcpBinding = ScanServiceClient.GetBinding();
            var endpointAddress = ScanServiceClient.GetEnpoint();

            ScanServiceClient proxy = new ScanServiceClient(tcpBinding, endpointAddress);
            proxy.ReadVoucherInfoCompleted += OnReadVoucherInfoCompleted;

            var keys = Security.CreateInstance().GenerateSecurityKeys();
            proxy.ReadVoucherInfoAsync(Id, shareVoucherRootPath, keys.Item1, keys.Item2);
        }

        private void OnReadVoucherInfoCompleted(object sender, ReadVoucherInfoCompletedEventArgs e)
        {
            try
            {
                AsyncManager.Parameters["info"] = e.Result;
            }
            catch (Exception ex)
            {
                ViewData[MerchantSiteStrings.MESSAGE] = ex.InnerException.Message;
            }
            finally
            {
                ScanServiceClient proxy = (ScanServiceClient)sender;
                proxy.ReadVoucherInfoCompleted -= OnReadVoucherInfoCompleted;
                AsyncManager.OutstandingOperations.Decrement();
            }
        }

        private const int CICLES_COUNT = 10;

        private readonly TimeSpan DELETE_OLDER_THAN = TimeSpan.FromMinutes(5);

        // C:\VOUCHERS\[CountryID]\[RetailerId]\[VoucherId]
        public ActionResult ShowCompleted(VoucherInfo2 info = null)
        {
            var files = new List<ShowModel>();

            if (info == null)
                return View("Show", files);

            string webVoucherRootPath = Server.MapPath("~/WEBVOUCHERFOLDER");
            var webroot = new DirectoryInfo(webVoucherRootPath);
            var sessionIdFolder = webroot.Combine(info.SessionId);

            if (!sessionIdFolder.Exists() || sessionIdFolder.IsEmpty())
            {
                Thread.Sleep(Config.FILETRANSFER_TIMEOUT);
                Server.TransferRequest(Request.RawUrl);
                return null;
            }

            if (!sessionIdFolder.Exists())
                throw new Exception("It might take more to export sometimes. Please click either F5 to refresh or BackSpace to navigate back.");

            webroot.ClearSafe(DateTime.Now.Subtract(DELETE_OLDER_THAN));

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
                        var fileImages = file.FullName.TiffGetAllImages2(ImageFormat.Jpeg, true);
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
                        var model = new ShowModel(file.Name, string.Concat("/MerchantSite/WEBVOUCHERFOLDER/", info.SessionId, "/", file.Name));
                        model.Data = System.IO.File.ReadAllBytes(file.FullName);
                        var base64 = Convert.ToBase64String(model.Data, Base64FormattingOptions.None);
                        model.ImgSrc = string.Concat("data:", model.Type, ";base64,", base64);
                        files.Add(model);
                    }
                }
            }

            return View("Show", files);
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
                    //data.CountryID = 826;
                    //data.VoucherID = 15715619; 
                    // Ravi: Don't test!
                    //if (PTFDataAccess.CheckP1Exists(data.CountryID, data.VoucherID))
                    {
                        PTFDataAccess.ExcudeFromDebitRun(data.CountryID, data.RetailerID, data.VoucherID, CurrentUser.UserID);
                        PTFDataAccess.LogVoucher(data.CountryID, data.VoucherID, CurrentUser.UserID);
                        ViewBag.Info = PTFDataAccess.SelectVoucherInfo(data.CountryID, data.VoucherID);
                        //return RedirectToAction("Index", "Home");
                    }
                    //else
                    //{
                    //    ViewBag.Err =
                    //        string.Format("P1 doesn't exist for Iso:'{0}' Branch:'{1}' Voucher:'{2}'. Please process voucher by using voucher entry in TRS.", data.CountryID, data.RetailerID, data.VoucherID);
                    //}
                }
                else
                {
                    ViewBag.Err = string.Format("Wrong barcode '{0}'. Please renter.", barcode);
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
            var isoId = value.Cast<int>();

            var headoffices = HttpContext.Session.Get<int, List<HeadOffice>>("HeadOfficeList" + isoId,
                Helper.CreateHeadOfficeDropDownLoadFunction(), isoId).CreateSelectList((h) => string.Format("{0} - {1}", h.Name, h.Id), (h) => h.Id.ToString());

            return Json(new ArrayList(headoffices), JsonRequestBehavior.AllowGet);
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
