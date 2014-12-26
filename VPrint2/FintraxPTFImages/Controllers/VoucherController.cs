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
using FintraxPTFImages.Attributes;
using FintraxPTFImages.Common;
using FintraxPTFImages.Data;
using FintraxPTFImages.Models;
using FintraxPTFImages.PartyManagementRef;
using FintraxPTFImages.ScanServiceRef;
using VPrinting;

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
        public ActionResult Search(int? grid_page)
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
                this.Session.Set("SearchModel", model);

                var sdc = new ServiceAccess();
                var list = sdc.SelectVouchersByRetailer(model.Country, model.Retailer);

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

                var sdc = new ServiceAccess();
                var infos = sdc.SelectVouchersByNumber(model.Country, model.Voucher);

                ViewData["VoucherList"] = infos.ToList();
            }

            return View(model);
        }

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

        #region SHOW VOUCHER IMAGE

        private const int ASINCH_TIMEOUT = 3000;

        [HttpGet]
        public void ShowAsync(int Id)
        {
            string webRootPath = Server.MapPath("~/WEBVOUCHERFOLDER");

            AsyncManager.OutstandingOperations.Increment();
            AsyncManager.Timeout = ASINCH_TIMEOUT;

            var tcpBinding = ScanServiceClient.GetBinding();
            var endpointAddress = ScanServiceClient.GetEnpoint();

            ScanServiceClient proxy = new ScanServiceClient(tcpBinding, endpointAddress);
            proxy.ReadVoucherInfoCompleted += OnReadVoucherInfoCompleted;

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

        private const int CICLES_COUNT = 10;

        private readonly TimeSpan DELETE_OLDER_THAN = TimeSpan.FromMinutes(5);

        // C:\VOUCHERS\[CountryID]\[RetailerId]\[VoucherId]
        public ActionResult ShowCompleted(VoucherInfo2 info = null)
        {
            var files = new List<ShowModel>();

            if (info == null)
                return View("Show", files);

            string webRootPath = Server.MapPath("~/WEBVOUCHERFOLDER");
            var webroot = new DirectoryInfo(webRootPath);
            var sessionIdFolder = webroot.Combine(info.SessionId);

            if (!sessionIdFolder.Exists() || sessionIdFolder.IsEmpty())
            {
                Thread.Sleep(500);
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

        #region NOTA DEBITO

        [HttpGet]
        public ActionResult NotaDebito()
        {
            ViewData["CountryList"] =
                                HttpContext.Application.Get<List<CountryDetail>>("CountryList", Helper.CreateCountryDropDownLoadFunction()).CreateSelectList(
                (c) => string.Format("{0} - {1}", c.Country, c.Iso2), (c) => c.Number.ToString());

            ViewData["HeadOfficeList"] = HttpContext.Session.Get<List<SelectListItem>>("Empty", Helper.CreateEmptyLoadFunction());

            ViewData["RetailerList"] = HttpContext.Session.Get<List<SelectListItem>>("Empty", Helper.CreateEmptyLoadFunction());
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
                    HttpContext.Session.Get<List<SelectListItem>>("Empty", Helper.CreateEmptyLoadFunction());

            ViewData["RetailerList"] =
                    HttpContext.Session.Get<List<SelectListItem>>("Empty", Helper.CreateEmptyLoadFunction());

            return View(model);
        }

        [HttpGet]
        public ActionResult DownloadNotaDebito(int country, int office, DateTime date, int invoice)
        {
            string fileName = string.Format("{0}_{1}_{2:dd-MM-yyyy}_{3}.pdf", country, office, date, invoice);
            var buffer = ServiceAccess.Instance.DownloadNotaDebitoReport(country, office, date, invoice);
            return base.File(buffer, "application/pdf", fileName);
        }

        [HttpGet]
        public ActionResult NotaDebitoEmail()
        {
            ViewData["CountryList"] =
                    HttpContext.Application.Get<List<CountryDetail>>("CountryList", Helper.CreateCountryDropDownLoadFunction()).CreateSelectList(
    (c) => string.Format("{0} - {1}", c.Country, c.Iso2), (c) => c.Number.ToString());

            ViewData["HeadOfficeList"] = HttpContext.Session.Get<List<SelectListItem>>("Empty", Helper.CreateEmptyLoadFunction());

            ViewData["RetailerList"] = HttpContext.Session.Get<List<SelectListItem>>("Empty", Helper.CreateEmptyLoadFunction());
            return View();
        }

        [HttpPost]
        public ActionResult NotaDebitoEmail(NotaDebitoEmail_Model model)
        {
            if (this.ModelState.IsValid)
            {
                ViewData["NotaDebitoList"] =
                    PTFDataAccess.SelectForNotaDebitosPerHeadOffice(model.Country, model.FromDate, model.ToDate, model.HeadOffice);
            }

            ViewData["CountryList"] =
                    HttpContext.Application.Get<List<CountryDetail>>("CountryList", Helper.CreateCountryDropDownLoadFunction()).CreateSelectList(
                    (c) => string.Format("{0} - {1}", c.Country, c.Iso2), (c) => c.Number.ToString());

            ViewData["HeadOfficeList"] =
                    HttpContext.Session.Get<List<SelectListItem>>("Empty", Helper.CreateEmptyLoadFunction());

            ViewData["RetailerList"] =
                    HttpContext.Session.Get<List<SelectListItem>>("Empty", Helper.CreateEmptyLoadFunction());

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
                    list.Add(new EmailInfo() { IsoId = in_iso, HoId = ho_id, InNumber = in_number, InDate = in_date, Subject = model.Subject, Body = model.Body, CC = model.CC });
            }

            if (list.Count > 0)
            {
                var cc = model.CC;
                ServiceAccess.Instance.EmailNotaDebito(list);
            }

            return RedirectToAction("NotaDebitoEmail");
        }

        #endregion NOTA DEBITO
    }
}
