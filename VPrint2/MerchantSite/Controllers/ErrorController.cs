/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System.Web.Mvc;

namespace MerchantSite.Controllers
{
    public class ErrorController : AsyncController
    {
        public ActionResult Index()
        {
            string message = (HttpContext.Error != null) ? HttpContext.Error.Message : "Error occurred. Please excuse us";
            return View((object)message);
        }

        public ActionResult Show(string message)
        {
            return View((object)message);
        }
    }
}
