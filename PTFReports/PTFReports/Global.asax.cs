using System;
using System.Diagnostics;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using PTF.Reports.Common;
using PTF.Reports.Controllers;
using PTF.Reports.PTFReportsDB;

namespace PTF.Reports
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HttpBlockAttribute());
            filters.Add(new HandleErrorAttribute());
            filters.Add(new LogErrorsAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            
            routes.IgnoreRoute("favicon.ico");

            routes.MapRoute("Paging",
                "{controller}/{action}/Page/{page}",
                new { controller = "Home", action = "Index" }
            );

            routes.MapRoute("Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Reports", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

            routes.MapRoute("User",
                "User/{domain}/{username}",
                new { controller = "User", action = "Index", username = UrlParameter.Optional }
            );

            routes.MapRoute("ViewReport",
                 "Reports/{reportname}",
                 "~/Reports/{reportname}.aspx"
            );

            routes.MapRoute("NotFound", "{*catchall}", new
            {
                controller = "Common",
                action = "NotFound"
            });
        }

        protected void Application_Start()
        {
            Helper.LoadBlockedIPsAsynch();

            AreaRegistration.RegisterAllAreas();
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }

        protected void Session_Start()
        {
            string sessionId = Session.SessionID;
            string userIP = HttpContext.Current.Request.UserHostAddress;
            string userAgent = HttpContext.Current.Request.UserAgent;

            SessionManager.Instant.Add(Session, sessionId, userIP, userAgent);

            new Action<object>(o =>
            {
                var ctx = o.Cast(new { SessionID = "", UserHostAddress = "", UserAgent = "" });
                var ctx2 = new PTFReportsContext();
                var ip = ctx2.IPs.FirstOrCreate(
                    n => n.IP1 == ctx.UserHostAddress,
                    () => new IP() { IP1 = ctx.UserHostAddress });
                ctx2.AddToSessions(new Session()
                {
                    Begin = DateTime.Now,
                    BrowserSessionID = ctx.SessionID,
                    UserAgent = ctx.UserAgent,
                    IP = ip
                });
                ctx2.SaveChanges();

            }).FireAndForgetSafe(
            new
            {
                SessionID = sessionId,
                UserHostAddress = userIP,
                UserAgent = userAgent,
            });
        }

        protected void Session_End()
        {
            SessionManager.Instant.Remove(Session.SessionID);

            new Action<string>(id =>
            {
                var ctx2 = new PTFReportsContext();
                var session = ctx2.Sessions.First(s => s.BrowserSessionID == id);
                session.End = DateTime.Now;
                ctx2.SaveChanges();

            }).FireAndForgetSafe(Session.SessionID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <see cref="http://www.techrepublic.com/article/working-with-the-aspnet-globalasax-file/5771721"/>
        protected void Application_BeginRequest()
        {
            string userip = Request.UserHostAddress;

            try
            {
                if (Request.IsLocal || (Request.QueryString.Count == 1 && string.Equals(Request.QueryString.GetKey(0), "command")))
                    return;
                
                if (string.IsNullOrWhiteSpace(userip) ||
                    PTFReportsContext.BlockedIPsTable[userip] != null)
                {
                    Response.StatusCode = 401;
                    Response.StatusDescription = "Unauthorized";
                    Response.End();
                }
            }
            catch
            {
                Helper.SaveIPBlockedAsynch(userip);

                Response.StatusCode = 401;
                Response.StatusDescription = "Unauthorized";
                Response.End();
            }
        }

        protected void Application_AuthenticateRequest()
        {
        }

        protected void Application_AuthorizeRequest()
        {
        }

        protected void Application_EndRequest()
        {
            Uri u = Request.Url;
            // Deny frames for all Account pages
            if (!Request.IsLocal && (u.Segments.Length >1 && 
                (   u.Segments[1].StartsWith("Account", StringComparison.InvariantCultureIgnoreCase) ||
                    //TODO: There is an iframe
                    //u.Segments[1].StartsWith("Administration", StringComparison.InvariantCultureIgnoreCase) ||
                    u.Segments[1].StartsWith("Reports", StringComparison.InvariantCultureIgnoreCase))))
            {
#if !DEBUG
                //It should be hosted on IIS
                Response.Headers.Add("X-FRAME-OPTIONS", "DENY");
#endif
                Trace.WriteLine("X-FRAME-OPTIONS  -  DENY");
            }
        }

        protected void Application_Error()
        {
        }
    }
}