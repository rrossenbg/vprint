/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

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

            var user = httpContext.Session.Get<CurrentUser>("CurrentUser", false);
            return user != null && user.IsValid;
        }
    }
}