using System.Web.Mvc;

namespace FintraxPTFImages.Controllers
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
