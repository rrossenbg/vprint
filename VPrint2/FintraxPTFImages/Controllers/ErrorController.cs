/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System.Web.Mvc;

namespace FintraxPTFImages.Controllers
{
    public class ErrorController : AsyncController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Show(string message)
        {
            return View(message);
        }
    }
}
