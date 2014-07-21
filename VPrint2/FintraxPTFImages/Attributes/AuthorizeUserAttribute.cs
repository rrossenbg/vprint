/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System.Threading;
using System.Web;
using System.Web.Mvc;
using FintraxPTFImages.Common;

namespace FintraxPTFImages.Attributes
{
    public class AuthorizeUserAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext.Session == null)
                return false;

            if (httpContext.User == null || httpContext.User.Identity == null)
                return false;

            Thread.CurrentPrincipal = httpContext.User;
            return httpContext.User.Identity.IsAuthenticated;
        }
    }
}