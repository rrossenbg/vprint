using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using FintraxPTFImages.Common;
using FintraxPTFImages.Data;
using FintraxPTFImages.Handler;

namespace FintraxPTFImages
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    // http://www.techrepublic.com/article/working-with-the-aspnet-globalasax-file/
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
            BarcodeDecoder.Run();
            PTFDataAccess.ConnectionString = WebConfigurationManager.ConnectionStrings["PTF_ConnectionString"].ConnectionString;
        }

        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
            var info = new CultureInfo(Thread.CurrentThread.CurrentCulture.ToString());
            info.DateTimeFormat.ShortDatePattern = "dd-MM-yyyy";
            Thread.CurrentThread.CurrentCulture = info;

            Response.BufferOutput = true;
            //FileLogger.LogInfo(DateTime.Now.ToString(), "Application_BeginRequest");
        }

        protected void Application_End(Object sender, EventArgs e)
        {
            string webRootPath = HostingEnvironment.MapPath("~/WEBVOUCHERFOLDER");
            var dir = new DirectoryInfo(webRootPath);
            dir.DeleteSubFoldersSafe();
        }

        /// <summary>
        /// Fired before the ASP.NET page framework sends HTTP headers to a requesting client (browser).
        /// </summary>
        protected void Applcation_PreSendRequestHeaders(Object sender, EventArgs e)
        {
        }

        protected void Application_Error(Object sender, EventArgs e)
        {
            var ex = Server.GetLastError();
            FileLogger.LogError(ex.ToString(), "Application_Error");
        }

        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            new FormsAuthenticationService().AuthenticateRequest();
        }

        protected void Session_Start(Object sender, EventArgs e)
        {
            //FileLogger.LogInfo("Session_Start", "Session_Start");
            
        }

        protected void Session_End(Object sender, EventArgs e)
        {
            PTFImagesDataDataContext.Default.EndHistory(Session.SessionID, DateTime.Now);
            //FileLogger.LogInfo("Session_End", "Session_End");
        }
    }
}