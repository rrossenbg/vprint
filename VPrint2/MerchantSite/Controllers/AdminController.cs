using System.Web.Mvc;

namespace MerchantSite.Controllers
{
    public class AdminController : AsyncController// ApiController
    {
        [HttpGet]
        public ActionResult Home()
        {
            return View();
        }
    }
}
