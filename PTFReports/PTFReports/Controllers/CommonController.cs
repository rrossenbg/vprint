/***************************************************
//  Copyright (c) Premium Tax Free 2012
/***************************************************/

using System.Web;
using System.Web.Mvc;
using PTF.Reports.Common;

namespace PTF.Reports.Controllers
{
    public class CommonController : Controller
    {
        public ActionResult NotFound()
        {
            var ip = HttpContext.Request.UserHostAddress;
            Helper.SaveIPBlockedAsynch(ip);
            ViewData[Strings.ERR] = "Please contact system admin and report error 2707";
            return View("Error");
        }

        public ActionResult ContactUs()
        {
            return View();
        }
    }
}
