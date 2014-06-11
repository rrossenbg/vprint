/***************************************************
//  Copyright (c) Premium Tax Free 2012
/***************************************************/

using System.Net;
using System.Security.Principal;
using System.Web;
using Microsoft.Reporting.WebForms;

namespace PTF.Reports
{
    public class ReportServerCredentials : IReportServerCredentials
    {
        private string _userName;
        private string _password;
        private string _domain;

        public ReportServerCredentials(string userName, string password, string domain)
        {
            _userName = userName;
            _password = password;
            _domain = domain;
        }

        public WindowsIdentity ImpersonationUser
        {
            get
            {
                // Use default identity.
                return null;
            }
        }

        public ICredentials NetworkCredentials
        {
            get
            {
                // Use default identity.
                return new NetworkCredential(_userName, _password, _domain);
            }
        }

#if GETFORMSCREDENTIALS
        public bool GetFormsCredentials(out Cookie authCookie, out string user, out string password, out string authority)
        {
            // Do not use forms credentials to authenticate.
            authCookie = null;
            user = null;
            password = null;
            authority = null;
            return false;
        }
#else
        /// <summary>
        /// 
        /// </summary>
        /// <param name="authCookie"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="authority"></param>
        /// <returns></returns>
        /// <see cref="http://www.devx.com/dotnet/Article/30610/1954"/>
        public bool GetFormsCredentials(out Cookie authCookie, out string user, out string password, out string authority)
        {
            user = password = authority = null;
            // The cookie name is specified in the <forms> element in Web.config (.ASPXAUTH by default)
            HttpCookie cookie = HttpContext.Current.Request.Cookies[".ASPXAUTH"];
            if (cookie == null)
                HttpContext.Current.Response.Redirect("/Account/LogOn");
            Cookie netCookie = new Cookie(cookie.Name, cookie.Value);
            if (cookie.Domain == null)
            {
                netCookie.Domain = HttpContext.Current.Request.
                ServerVariables["SERVER_NAME"].ToUpper();
            }
            netCookie.Expires = cookie.Expires;
            netCookie.Path = cookie.Path;
            netCookie.Secure = cookie.Secure;
            authCookie = netCookie;
            return true;
        }
#endif
    }
}