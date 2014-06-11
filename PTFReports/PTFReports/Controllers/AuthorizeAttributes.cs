/***************************************************
//  Copyright (c) Premium Tax Free 2012
/***************************************************/

using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PTF.Reports.PTFReportsDB;

namespace PTF.Reports
{
    [AttributeUsage(AttributeTargets.Class,
        AllowMultiple = false, Inherited = true)]
    public class HttpBlockAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext httpContext)
        {
            if (!httpContext.HttpContext.Request.IsLocal)
            {
                if (string.IsNullOrWhiteSpace(httpContext.HttpContext.Request.UserHostAddress))
                {
                    //Request carries hidden somehow client ip
                    httpContext.Result = new HttpUnauthorizedResult();
                }
                //else if (httpContext.HttpContext.Request.IsViaProxy())
                //{
                //    //Request is via proxy
                //    httpContext.Result = new HttpUnauthorizedResult();
                //}
                else
                {
                    //IP is blocked
                    var ctx = PTFReportsContext.Current;
                    string userip = httpContext.HttpContext.Request.UserHostAddress;
                    var ipaddr = ctx.IPs.FirstOrDefault(ip => ip.IP1 == userip && ip.BlockedAt.HasValue);
                    if (ipaddr != null)
                        httpContext.Result = new HttpUnauthorizedResult();
                }
            }
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AdminAuthorize : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            UserDetail user = httpContext.Session[Strings.USER].Cast<UserDetail>();
            //No user & localhost
            if (user == null && httpContext.Request.IsLocal)
                return true;
            return
                user != null && (user.IsAdmin || user.IsLoggedAs);
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ReportAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var user = httpContext.Session[Strings.USER].Cast<UserDetail>();
            if (user == null)
                return false;

            if (base.AuthorizeCore(httpContext))
                return true;

            return false;
        }
    }
}