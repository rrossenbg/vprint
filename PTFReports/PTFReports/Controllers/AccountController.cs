/***************************************************
//  Copyright (c) Premium Tax Free 2012
/***************************************************/

using System;
using System.Web.Mvc;
using System.Web.Routing;
using PTF.Reports.Common;
using PTF.Reports.Models;
using PTF.Reports.PTFReportsDB;
using PTF.Reports.Tools;
using io = System.IO;

namespace PTF.Reports.Controllers
{
    [HttpBlock]
    [HandleError]
    public class AccountController : Controller
    {
        public IFormsAuthenticationService FormsService { get; set; }
        public IMembershipService MembershipService { get; set; }

        protected override void Initialize(RequestContext requestContext)
        {
            if (FormsService == null)
                FormsService = new FormsAuthenticationService();

            if (MembershipService == null)
                MembershipService = new AccountMembershipService();

            base.Initialize(requestContext);
        }

        public ActionResult LogOn()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var ctx2 = PTFReportsContext.Current;

                UserDetail user = null;
                if (ctx2.IsUserBlocked(model.usr, out user) || user == null)
                    return RedirectToActionPermanent("NotFound", "Common");

                bool ok = ctx2.ValidateUser(model.pssw, user);
                if (ok)
                {
                    Session[Strings.USER] = user;
                    FormsService.SignIn(model.usr, model.rmbm);

                    Helper.ResetLoginAttemptsAsynch(model.usr);

                    Helper.SaveSessionAsynch(Session.SessionID, user);

                    if (user.Ud_firstLogin)
                        return RedirectToAction("ChangePassword", "Account");

                    Uri tmp;
                    if (!string.IsNullOrEmpty(returnUrl) && Uri.TryCreate(Request.Url, returnUrl, out tmp))
                        return Redirect(returnUrl);
                    else
                        return RedirectToAction("Index", "Reports");
                }
                else
                {
                    int MAX_LOGIN_TRIES = Config.Get<int>(Strings.MAX_LOGIN_TRIES);
                    Helper.SaveLoginFailAsynch(model.usr, MAX_LOGIN_TRIES);
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult LogOff()
        {
            FormsService.SignOut();
            Session.Remove(Strings.USER);
            return RedirectToAction("Index", "Reports");
        }

        public ActionResult Register()
        {
            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
            return View();
        }

        [HttpGet]
        public ActionResult ChangePassword(string id)
        {
            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;

            if (!string.IsNullOrWhiteSpace(id))
            {
                var str = id.Decrypt().Split(';');
                string userName = str[0].TrimSafe();
                string email = str[1].TrimSafe();
                DateTime date = DateTime.Parse(str[2].TrimSafe());
                if (date.AddHours(Config.Get<int>(Strings.PassExpireHours)) < DateTime.Now)
                    return View((ChangePasswordModel)null);

                UserDetail user = PTFReportsContext.Current.FindUserByEmail(userName, email);
                if (user != null)
                {
                    var model = (ChangePasswordModel)user;
                    Session[Strings.CachedMODELChangePasswordModel] = model;
                    return View(model);
                }
            }
            
            return View((ChangePasswordModel)null);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            // If we got this far, something failed, redisplay form
            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;

            if (model != null && (ModelState.IsValid || Session[Strings.CachedMODELChangePasswordModel] != null))
            {
                if (Session[Strings.CachedMODELChangePasswordModel] != null)
                {
                    model.opssw = ((ChangePasswordModel)Session[Strings.CachedMODELChangePasswordModel]).opssw;
                    Session.Remove(Strings.CachedMODELChangePasswordModel);
                }

                UserDetail user;
                var isOk = MembershipService.ChangePassword(model.usr, model.opssw, model.npssw, out user);
                if (isOk)
                {
                    var ctx2 = PTFReportsContext.Current;
                    user = ctx2.UserDetails.First(ud => ud.Ud_id == user.Ud_id);
                    user.Ud_firstLogin = false;
                    ctx2.SaveChanges();
                    Session[Strings.USER] = user;

                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                    return View((ChangePasswordModel)null);
                }
            }

            return View((ChangePasswordModel)null);
        }

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        public ActionResult ForgottenPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgottenPassword(ForgottenPassword model)
        {
            var ctx2 = PTFReportsContext.Current;
            var isOk = ctx2.SetPasswordForgotten(model.usr, model.eml);
            if (isOk)
            {
                var path = Server.MapPath("~/Views/Account/ForgottenPassword.htm");
                var text = io.File.ReadAllText(path);
                var str = Url.Encode(string.Concat(model.usr,";", model.eml, ";", DateTime.Now.ToString("g")).Encript());
                text = string.Format(text, str);
                EmailSender.Send(model.eml, "PTFReports password recovery service", text, true);
                return View("ForgottenPasswordSuccess");
            }
            return View("ForgottenPasswordFailure");
        }        
    }
}
