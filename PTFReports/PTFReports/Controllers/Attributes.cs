/***************************************************
//  Copyright (c) Premium Tax Free 2012
/***************************************************/

using System;
using System.Diagnostics;
using System.IO.Compression;
using System.Web;
using System.Web.Mvc;
using log4net;
using PTF.Reports.Common;

namespace PTF.Reports.Controllers
{
    public class LogErrorsAttribute : FilterAttribute, IExceptionFilter
    {
        private static readonly ILog ms_logger = LogManager.GetLogger(typeof(LogErrorsAttribute).Name);

        void IExceptionFilter.OnException(ExceptionContext filterContext)
        {
            if (filterContext != null && filterContext.Exception != null)
            {
                if (filterContext.Exception is HttpAntiForgeryException)
                {
                    string ip = filterContext.RequestContext.HttpContext.Request.UserHostAddress;
                    Helper.SaveIPBlockedAsynch(ip);
                }

                string controller = Convert.ToString(filterContext.RouteData.Values["controller"]);
                string action = Convert.ToString(filterContext.RouteData.Values["action"]);
                string error = string.Format("Controller: {0}\r\nAction: {1}\r\n{2}", controller, action, filterContext.Exception);

                Trace.WriteLine(error, "ReportPortal");

                ms_logger.Error(filterContext.Exception.Message, filterContext.Exception);

                Helper.SaveErrorAsynch(filterContext.Exception);
            }
        }
    }

    public class CacheFilterAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Gets or sets the cache duration in seconds. The default is 10 seconds.
        /// </summary>
        /// <value>The cache duration in seconds.</value>
        public int Duration
        {
            get;
            set;
        }

        public CacheFilterAttribute()
        {
            Duration = 10;
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (Duration <= 0)
                return;

            HttpCachePolicyBase cache = filterContext.HttpContext.Response.Cache;
            TimeSpan cacheDuration = TimeSpan.FromSeconds(Duration);

            cache.SetCacheability(HttpCacheability.Public);
            cache.SetExpires(DateTime.Now.Add(cacheDuration));
            cache.SetMaxAge(cacheDuration);
            cache.AppendCacheExtension("must-revalidate, proxy-revalidate");
        }
    }

    public class CompressFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpRequestBase request = filterContext.HttpContext.Request;

            string acceptEncoding = request.Headers["Accept-Encoding"];

            if (string.IsNullOrEmpty(acceptEncoding)) return;

            acceptEncoding = acceptEncoding.ToUpperInvariant();

            HttpResponseBase response = filterContext.HttpContext.Response;

            if (acceptEncoding.Contains("GZIP"))
            {
                response.AppendHeader("Content-encoding", "gzip");
                response.Filter = new GZipStream(response.Filter, CompressionMode.Compress);
            }
            else if (acceptEncoding.Contains("DEFLATE"))
            {
                response.AppendHeader("Content-encoding", "deflate");
                response.Filter = new DeflateStream(response.Filter, CompressionMode.Compress);
            }
        }
    }
}