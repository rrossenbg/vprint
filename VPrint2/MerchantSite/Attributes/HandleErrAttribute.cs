using System.Reflection;
using System.Web.Mvc;
using MerchantSite.Common;

namespace MerchantSite.Attributes
{
    public class HandleErrAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            FileLogger.LogError(filterContext.Exception.ToString(), "HandleErrAttribute");

            //filterContext.Exception is TimeoutException
            if (!(filterContext.Controller is AsyncController))
            {
                filterContext.HttpContext.Response.StatusCode = 200;
                filterContext.Result = new RedirectResult("~/Error"); //filterContext.Exception.Message,
                filterContext.ExceptionHandled = true;
            }

            base.OnException(filterContext);
        }
    } 
}