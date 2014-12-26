/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System.Web.Mvc;
using MerchantSite.Common;

namespace MerchantSite
{
    public class HandleErrAsyncAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            FileLogger.LogError(filterContext.Exception.ToString(), "HandleErrAsyncAttribute");

            //filterContext.Exception is TimeoutException
            if (filterContext.Controller is AsyncController)
            {
                filterContext.HttpContext.Response.StatusCode = 200;
                filterContext.Result = new JsonResult
                 {
                     Data = filterContext.Exception.Message,
                     JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                 };

                filterContext.ExceptionHandled = true;
            }

            base.OnException(filterContext);
        }
    }
}