/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System.Web.Mvc;
using MerchantSite.Attributes;
using MerchantSite.Models;

namespace MerchantSite.Controllers
{
    [AuthorizeUser]
    [RequiresSSL]
    public class UserController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Setup()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Setup(SetupUser_Model model)
        {
            return View(model);
        }
    }
}
