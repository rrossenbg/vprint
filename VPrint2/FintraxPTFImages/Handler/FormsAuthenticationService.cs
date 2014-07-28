/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.Security.Principal;
using System.Web;
using System.Web.Security;
using FintraxPTFImages.Common;

namespace FintraxPTFImages.Handler
{
    /// <summary>
    /// http://stackoverflow.com/questions/14772034/asp-net-mvc-4-generic-principal-difficulties
    /// </summary>
    public class FormsAuthenticationService //: IFormsAuthenticationService
    {
        public void SignIn(string userName, bool createPersistentCookie, string UserData = "Admin|User")
        {
            if (string.IsNullOrEmpty(userName)) 
                throw new ArgumentException("Value cannot be null or empty.", "userName");

            // Create and tuck away the cookie
            FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(1, userName, DateTime.Now, DateTime.Now.AddDays(15), createPersistentCookie, UserData);
            string encTicket = FormsAuthentication.Encrypt(authTicket);
            HttpCookie faCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
            HttpContext.Current.Response.Cookies.Add(faCookie);

#if NUM2
            FormsAuthentication.SetAuthCookie(userName, createPersistentCookie);
#endif
        }

        /// <summary>
        /// call in Global.asax
        /// void Application_AuthenticateRequest(Object sender, EventArgs e)
        /// </summary>
        public void AuthenticateRequest()
        {
            HttpCookie authCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie == null) 
                return;

            FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            string[] UserData = authTicket.UserData.Split(new char[] { '|' }); //UserId, CountryId

            HttpContext.Current.Items.Add("CurrentUser", new CurrentUser(authTicket.Name, UserData));

            GenericIdentity userIdentity = new GenericIdentity(authTicket.Name);
            GenericPrincipal userPrincipal = new GenericPrincipal(userIdentity, UserData);
            HttpContext.Current.User = userPrincipal;
        }

        public void LogOut()
        {
            HttpContext.Current.Session.RemoveAll();
            HttpContext.Current.Items.Clear();
            FormsAuthentication.SignOut();
        }
    }
}