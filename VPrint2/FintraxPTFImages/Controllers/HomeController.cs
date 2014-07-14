/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System.Web.Mvc;

namespace FintraxPTFImages.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Voucher Images database. Search and show all scanned images by Country and Retailer.";
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "This application shows all scanned Vouchers by Retails.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Mr. Rosen Rusev";
            return View();
        }
    }
}
