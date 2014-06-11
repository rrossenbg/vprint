using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using PTF.Reports.Common;
using PTF.Reports.Models;
using PTF.Reports.PTFDB;
using PTF.Reports.PTFReportsDB;
using PTF.Reports.Tools;

namespace PTF.Reports.Controllers
{
    [HttpBlock]
    [LogErrors]
    public class AdministrationController : Controller
    {
        public static readonly int PAGESIZE = Config.Get<int>(Strings.PAGESIZE);
        public static readonly int DEFPASSLEN = Config.Get<int>(Strings.DefaultPassLength);

        [AdminAuthorize]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Test2()
        {
            return View();
        }

        #region COUNTRIES

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [AdminAuthorize]
        public ActionResult Countries(int? page)
        {
            var ctx1 = PTFContext.Current;
            List<CountryModel> list = new List<CountryModel>();
            ViewBag.Count = ctx1.ISO_ptf.Count();
            var countries = ctx1.ISO_ptf.OrderBy(i => i.iso_country).Page(page, PAGESIZE);
            foreach (var cnt in countries)
                list.Add((CountryModel)cnt);
            return View(list);
        }


        #endregion

        #region BRACHES/HEADOFFICES

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [AdminAuthorize]
        public ActionResult Branches(int? page)
        {
            var ctx1 = PTFContext.Current;
            List<BranchModel> list = new List<BranchModel>();

            ViewBag.Count = ctx1.Branches.Count();
            var braches = ctx1.Branches.OrderBy(c => c.br_name).Page(page, PAGESIZE);
            foreach (var br in braches)
                list.Add((BranchModel)br);
            return View(list);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [AdminAuthorize]
        public ActionResult ViewBranch(Guid id)
        {
            int iso_id, ho_id, br_id, fake;
            CommonTools.FromGuid(id, out iso_id, out ho_id, out br_id, out fake);
            var ctx1 = PTFContext.Current;
            var branch = ctx1.Branches.First(br => br.br_iso_id == iso_id && br.br_ho_id == ho_id && br.br_id == br_id);
            return View((BranchModel)branch);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [AdminAuthorize]
        public ActionResult HeadOffices(int? page)
        {
            var ctx1 = PTFContext.Current;
            List<HeadOfficeModel> list = new List<HeadOfficeModel>();
            var companies = new PaginatedList<HeadOffice>(ctx1.HeadOffices.OrderBy(c => c.ho_name), page.GetValueOrDefault(), PAGESIZE);
            ViewBag.Pager = companies;
            foreach (var cmp in companies)
                list.Add((HeadOfficeModel)cmp);
            return View(list);
        }

        #endregion

        #region IPS

        [AdminAuthorize]
        public ActionResult IPs(int? page)
        {
            var ctx2 = PTFReportsContext.Current;
            ViewBag.Count = ctx2.IPs.Count();
            var list = ctx2.IPs.OrderBy(c => c.IP1).Page(page, PAGESIZE);
            return View(list);
        }

        [AdminAuthorize]
        public ActionResult BlockIP(int? id)
        {
            return RunSafe(() =>
            {
                var ctx2 = PTFReportsContext.Current;
                IP ip = ctx2.IPs.FirstOrDefault(i => i.IP_id == id);
                Debug.Assert(ip != null, "IP should already exist");
                ip.BlockedAt = DateTime.Now;
                PTFReportsContext.BlockedIPsTable[ip.IP1] = DateTime.Now;
                ctx2.SaveChanges();
            });
        }

        [AdminAuthorize]
        public ActionResult UnBlockIP(int? id)
        {
            return RunSafe(() =>
            {
                var ctx2 = PTFReportsContext.Current;
                IP ip = ctx2.IPs.FirstOrDefault(i => i.IP_id == id);
                Debug.Assert(ip != null, "IP should already exist");
                ip.BlockedAt = null;
                PTFReportsContext.BlockedIPsTable.Remove(ip.IP1);
                ctx2.SaveChanges();
            });
        }

        [AdminAuthorize]
        public ActionResult UnblockIPScript()
        {
            var ctx2 = PTFReportsContext.Current;
            var str = Server.UrlEncode(DateTime.Now.ToString().Encript());
            string link = Config.Get<string>(Strings.SERVERUNBLOCKURL).Concat2(str);
            ViewData["LINK"] = link;
            ctx2.AddToUnblockCommands(new UnblockCommand() { Url = link, DateCreated = DateTime.Now });
            ctx2.SaveChanges();
            return View();
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult UnblockMe(string command)
        {
            return RunSafe(() =>
            {
                TimeSpan time24 = TimeSpan.FromHours(24);
                DateTime date = DateTime.Parse(command.Decrypt());
                if (date.Add(time24) > DateTime.Now)
                    throw new TimeoutException();

                string ipAddr = HttpContext.Request.UserHostAddress;
                var ctx2 = PTFReportsContext.Current;
                IP ip = ctx2.IPs.First(i => i.IP1 == ipAddr);
                ip.BlockedAt = null;
                PTFReportsContext.BlockedIPsTable.Remove(ipAddr);
                ctx2.SaveChanges();
            });
        }

        #endregion

        #region FOLDERS

        [AdminAuthorize]
        public ActionResult Folders(int? page)
        {
            var ctx2 = PTFReportsContext.Current;
            List<FolderModel> list = new List<FolderModel>();
            var folders = ctx2.Folders.OrderBy(f => f.Name).Page(page, PAGESIZE);
            foreach (var ac in folders)
                list.Add((FolderModel)ac);
            return View(list);
        }

        [AdminAuthorize]
        public ActionResult AddFolder()
        {
            var ctx2 = PTFReportsContext.Current;
            if (IsValidPostBack("Name"))
                return RunSafe(() =>
                {
                    var user = (UserDetail)Session[Strings.USER];
                    var model = new FolderModel();
                    TryUpdateModel(model);
                    var orderId = Request.Parse<int>("OrderList", -1);
                    ctx2.AddToFolders(new Folder()
                    {
                        FolderID = Guid.NewGuid(),
                        Name = model.Name,
                        Description = model.Description,
                        Order = orderId,
                        ParentID = null,
                    });
                    ctx2.SaveChanges();
                });
            ViewData["FolderList"] = new SelectList(ctx2.Folders.OrderBy(f => f.Name), "FolderID", "FullPath", null);
            ViewData["OrderList"] = new SelectList(Order.Eight.ToDict<Order>(), "Key", "Value", null);
            return View();
        }

        [AdminAuthorize]
        public ActionResult RemoveFolder(Guid id)
        {
            return RunSafe(() =>
            {
                var ctx2 = PTFReportsContext.Current;
                ctx2.Folders.RemoveFirst(r => r.FolderID == id);
                ctx2.SaveChanges();
            });
        }

        #endregion

        #region REPORTS

        [AdminAuthorize]
        public ActionResult Reports(int? page)
        {
            var ctx2 = PTFReportsContext.Current;
            List<ReportModel> list = new List<ReportModel>();
            ViewBag.Count = ctx2.Reports.Count();
            var reports = ctx2.Reports.OrderBy(c => c.Name).Page(page, PAGESIZE);
            foreach (var ac in reports)
                list.Add((ReportModel)ac);
            return View(list);
        }

        [AdminAuthorize]
        public ActionResult AddReport()
        {
            var ctx2 = PTFReportsContext.Current;
            if (IsValidPostBack("Name"))
                return RunSafe(() =>
                {
                    var user = Session[Strings.USER].Cast<UserDetail>();
                    var model = new ReportModel();
                    TryUpdateModel(model);
                    var folderId = new Guid(Request.Form["FolderList"]);
                    var report = new Report()
                    {
                        ReportID = Guid.NewGuid(),
                        Name = model.Name,
                        Description = model.Description,
                        Page = model.Page,
                        FolderID = folderId,
                    };

                    ctx2.AddToReports(report);
                    ctx2.SaveChanges();
                });

            ViewData["ParamMappingList"] = new SelectList(ParamMapping.IsoID.ToDict<ParamMapping>(), "Key", "Value", null);
            ViewData["FolderList"] = new SelectList(ctx2.Folders.OrderBy(f => f.Name), "FolderID", "FullPath", null);
            return View();
        }

        [AdminAuthorize]
        public ActionResult RemoveReport(Guid id)
        {
            return RunSafe(() =>
            {
                var ctx2 = PTFReportsContext.Current;
                ctx2.Reports.RemoveFirst(r => r.ReportID == id);
                ctx2.SaveChanges();
            });
        }

        [AdminAuthorize]
        public ActionResult Companies(int? page)
        {
            var ctx1 = PTFContext.Current;
            List<HeadOfficeModel> list = new List<HeadOfficeModel>();
            ViewBag.Count = ctx1.HeadOffices.Count();
            var offices = ctx1.HeadOffices.OrderBy(c => c.ho_name).Page(page, PAGESIZE);
            foreach (var of in offices)
                list.Add((HeadOfficeModel)of);
            return View(list);
        }

        [AdminAuthorize]
        [AcceptVerbs("get")]
        public ActionResult ShowReportParams(Guid id)
        {
            dynamic data = Helper.GetReportParametersSafe(id);
            //Returns string.Empty if ok or err message
            return View(data);
        }

        [AdminAuthorize]
        [AcceptVerbs("post")]
        public ActionResult SaveReportParameters()
        {
            return RunSafe(() =>
            {
                var id = Guid.Parse(Request.Form["ReportID"]);
                var ctx2 = PTFReportsContext.Current;
                var report = ctx2.Reports.First(r => r.ReportID == id);
                ctx2.Parameters.RemoveAllWhere(p => p.ReportID == id);

                for (int i = 0; i < 20; i++)
                {
                    string paramName = Request.Parse<string>("pName" + i);
                    string paramText = Request.Parse<string>("pText" + i);
                    int paramType = Request.Parse<int>("pType" + i, -1);

                    if (string.IsNullOrWhiteSpace(paramName))
                        continue;

                    if (paramType == -1)
                        continue;

                    var param = new Parameter()
                    {
                        ReportID = id,
                        Name = paramName,
                        Text = paramText,
                        ParamType = paramType,
                    };
                    ctx2.AddToParameters(param);
                }
                ctx2.SaveChanges();
            });
        }

        [AdminAuthorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult PingReport(ValueList details)
        {
            Guid id = Guid.Parse(details.Value);
            dynamic data = Helper.GetReportParametersSafe(id);
            //Returns string.Empty if ok or err message
            return Json(data.Message);
        }

        #endregion

        #region COMPANY PERMISSIONS

        [AdminAuthorize]
        [AcceptVerbs("get")]
        public ActionResult RetailerPermission(int? id, int? page = null)
        {
            var model = Session.Get<PermissionModel>(Strings.CachedMODELPermissions, new PermissionModel());
            model.UserID = id.GetValueOrDefault(model.UserID);

            var ctx1 = PTFContext.Current;
            var ctx2 = PTFReportsContext.Current;
            var user = ctx2.UserDetails.First(ud => ud.Ud_id == model.UserID);
            model.User = user;
            model.HeadOffice = user.GetCompany();
            model.Branch = user.GetBranch();
            model.ISOID = user.Ud_iso_id;

            var companies = new PaginatedList<HeadOffice>(
                ctx1.HeadOffices
                .Where(ho => ho.ho_iso_id == model.ISOID)
                .OrderBy(ho => ho.ho_name), 
                page.GetValueOrDefault(), PAGESIZE);

            ViewBag.Pager = companies;

            ViewData["AllCompanies"] = companies;

            ViewData["SelectedBranches"] = Branch.FindForPermissions(user);

            return View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [AdminAuthorize]
        public JsonResult SavePermission(ValueList details)
        {
            if (details.IsEmpty)
                return Json("ok");

            var model = Session.Get<PermissionModel>(Strings.CachedMODELPermissions);
            if (model == null)
                return Json("err");

            var ctx1 = PTFContext.Current;
            var ctx2 = PTFReportsContext.Current;

            var user = ctx2.UserDetails.First(ud => ud.Ud_id == model.UserID);
            //Add
            if (details.Value.Contains('A'))
            {
                foreach (string id in details.Split<string>('A'))
                {
                    var ids = id.Split('_');
                    int isoId = ids[0].Cast2<int>();
                    int compId = ids[1].Cast2<int>();

                    if (ids[2] == "all")
                    {
                        ctx2.Permissions.RemoveAllWhere(p => p.UserID == model.UserID && p.IsoID == isoId && p.HoID == compId);

                        foreach (var br in ctx1.Branches.Where(b => b.br_iso_id == model.ISOID && b.br_ho_id == compId))
                        {
                            ctx2.AddToPermissions(new Permission()
                            {
                                UserID = model.UserID,
                                IsoID = br.br_iso_id,
                                HoID = br.br_ho_id,
                                RetailerID = br.br_id,
                            });
                        }
                    }
                    else
                    {
                        int branchId = ids[2].Cast2<int>();
                        Permission.AddSafe(model.UserID, model.ISOID, compId, branchId);
                    }
                }
            }
            //Remove
            else
            {
                foreach (string id in details.Split<string>('R'))
                {
                    var ids = id.Split('_');
                    int isoId = ids[0].Cast2<int>();
                    int compId = ids[1].Cast2<int>();

                    if (ids[2] == "all")
                    {
                        ctx2.Permissions.RemoveAllWhere(
                            p => p.UserID == model.UserID &&
                                    p.IsoID == isoId &&
                                    p.HoID == compId);
                    }
                    else
                    {
                        int branchId = ids[2].Cast2<int>();
                        ctx2.Permissions.RemoveFirst(
                            p => p.UserID == model.UserID &&
                                    p.IsoID == isoId &&
                                    p.HoID == compId &&
                                    p.RetailerID == branchId);
                    }
                }
            }
            ctx2.SaveChanges();

            ViewData["SelectedBranches"] = Branch.FindForPermissions(user);

            var result = this.RenderPartialToString("SelectedBrachesPartial", model);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region COMPANY AUTOCOMPLETE

        private static readonly int AUTOCOMPL = Config.Get<int>(Strings.AUTOCOMPCOUNT);

        [AdminAuthorize]
        [HttpPost]
        public JsonResult GetTaggedParameters(string query)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(query), "Wrong query");

            var repository = Session.Get<TagRepository>(Strings.TagRepository, new TagRepository());

            if (repository.IsEmpty)
            {
                var model = Session.Get<PermissionModel>(Strings.CachedMODELPermissions, new PermissionModel());
                Debug.Assert(model != null, "RetailerPermission was called yet");

                var ctx1 = PTFContext.Current;
                int iso_id = model.User.Ud_iso_id;
                var q = ctx1.HeadOffices.Where(ho => ho.ho_iso_id == iso_id);
                repository.SetTags(q);
            }
            return Json(repository.GetTags(query, AUTOCOMPL));
        }

        [AdminAuthorize]
        [AcceptVerbs("post")]
        public ActionResult ShowRetailerPermissionPage(FormCollection forms)
        {
            Guid guid = new Guid(Request.Form["RetailerPermissionIndex"]);
            int iso_id, ho_id, br_id, fake;
            CommonTools.FromGuid(guid, out iso_id, out ho_id, out br_id, out fake);
            var ctx1 = PTFContext.Current;
            var countries = ctx1.HeadOffices.Where(ho => ho.ho_iso_id == iso_id).OrderBy(ho => ho.ho_name);
            var index = countries.FindIndex(ho => ho.ho_iso_id == iso_id && ho.ho_id == ho_id);

            return RedirectToActionPermanent("RetailerPermission", new { page = (index / PAGESIZE) });
        }

        #endregion

        #region FOLDER PERMISSIONS

        [AdminAuthorize]
        [AcceptVerbs("get")]
        public ActionResult ShowFolderUserPermission()
        {
            var model = Session.Get<FolderUserPermissionModel>(Strings.CachedMODELFolderUserPermissions, new FolderUserPermissionModel());
            if (!model.IsEmpty)
            {
                model.FolderID = Session.Get<Guid>(Strings.FOLDERID, Guid.Empty);

                if (model.FolderID != Guid.Empty)
                {
                    var ctx2 = PTFReportsContext.Current;
                    dynamic obj = ctx2.Folders.FirstOrDefault(f => f.FolderID == model.FolderID);
                    if (obj == null)
                        return View((FolderUserPermissionModel)null);

                    model.FolderName = obj.Name;

                    var user = UserDetail.GetByID(model.UserID);
                    var folder = user.Folders.FirstOrDefault(f => f.FolderID == model.FolderID);
                    model.Enabled = (folder != null);
                }
            }
            return View(model);
        }

        #endregion

        #region UserDetails

        [AdminAuthorize]
        public ActionResult UserDetails(int? page)
        {
            var ctx2 = PTFReportsContext.Current;
            ViewBag.Count = ctx2.UserDetails.Count();
            var list = ctx2.UserDetails.OrderBy(u => u.Ud_loginName).Page(page, PAGESIZE);
            return View(list);
        }

        [AdminAuthorize]
        public ActionResult UnBlockUser(int? id)
        {
            return RunSafe(() =>
            {
                var ctx2 = PTFReportsContext.Current;
                var user = ctx2.UserDetails.First(u => u.Ud_id == id);
                user.BlockedAt = null;
                user.LoginTries = 0;
                ctx2.SaveChanges();
            });
        }

        [AdminAuthorize]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult LogAs(int? id)
        {
            return RunSafe(() =>
                {
                    var ctx2 = PTFReportsContext.Current;
                    var user = ctx2.UserDetails.First(u => u.Ud_id == id);
                    Debug.Assert(user != null);                    
                    var admin = Session.Get<UserDetail>(Strings.USER);
                    user.LogAs(admin);
                    Session[Strings.USER] = user;
                });
        }

        [AdminAuthorize]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult UnLogAs()
        {
            return RunSafe(() =>
            {
                var user = Session.Get<UserDetail>(Strings.USER);
                Debug.Assert(user.Origin != null, "Not null is expected");
                Session[Strings.USER] = user.Origin;
            });
        }

        #endregion

        #region USERS

        [AdminAuthorize]
        public ActionResult Users(int? page)
        {
            Session.Remove(Strings.CachedMODELUserDetails, Strings.CachedMODELPermissions );

            var ctx2 = PTFReportsContext.Current;
            List<UserModel> list = new List<UserModel>();
            ViewBag.Count = ctx2.UserDetails.Count();
            var users = ctx2.UserDetails.OrderBy(f => f.Ud_loginName).Page(page, PAGESIZE);
            foreach (var u in users)
                list.Add((UserModel)u);
            return View(list);
        }

        [AdminAuthorize]
        public ActionResult ViewUser(int id)
        {
            var ctx2 = PTFReportsContext.Current;
            var user = ctx2.UserDetails.First(ud => ud.Ud_id == id);
            return View((UserModel)user);
        }

        [AdminAuthorize]
        public ActionResult DeleteUser(int id)
        {
            return RunSafe(() =>
            {
                var ctx2 = PTFReportsContext.Current;
                ctx2.UserDetails.RemoveFirst(ud => ud.Ud_id == id);
                ctx2.SaveChanges();
            });
        }

        [AdminAuthorize]
        [AcceptVerbs("post", "get")]
        public ActionResult AddUser()
        {
            var ctx1 = PTFContext.Current;
            var ctx2 = PTFReportsContext.Current;

            var model = Session.Get<UserModel>(Strings.CachedMODELUserDetails, new UserModel());
            TryUpdateModel(model);

            switch (model.CurrentStep)
            {
                case null:
                case "":
                case "FormStart":
                    ViewData["CountryList"] = new SelectList(ctx1.ISO_ptf
                        .OrderBy(iso => iso.iso_country), "iso_number", "iso_country", null);
                    break;
                case "CountryList":
                    model.CountryID = Request.Parse<int>("CountryList", model.CountryID);
                    ViewData["CompanyList"] = new SelectList(ctx1.HeadOffices
                        .Where(ho => ho.ho_iso_id == model.CountryID)
                        .OrderBy(ho => ho.ho_name), "ho_id", "ho_name", null);
                    break;
                case "CompanyList":
                    model.CompanyID = Request.Parse<int>("CompanyList", model.CompanyID);
                    ViewData["BranchList"] = new SelectList(ctx1.Branches
                        .Where(br => br.br_iso_id == model.CountryID && br.br_ho_id == model.CompanyID &&
                            (br.br_active == "Y" || !br.br_date_left.HasValue))
                        .OrderBy(br => br.br_name), "br_id", "FullName", null);
                    break;
                case "BranchList":
                    model.BranchID = Request.Parse<int>("BranchList", model.BranchID);
                    ViewData["UserTypeList"] = new SelectList(UserType.Normal.ToDict<UserType>(), "Key", "Value", model.UserTypeID);
                    ViewData["LanguageList"] = new SelectList(eLanguage.English.ToDict<eLanguage>(), "Key", "Value", model.LanguageID);
                    break;
                case "UserTypeList":
                    model.UserTypeID = Request.Parse<int>("UserTypeList", model.UserTypeID);
                    model.LanguageID = Request.Parse<int>("LanguageList", model.LanguageID);
                    model.Language = model.LanguageID.GetEnumName<eLanguage>();
                    model.DefaultPass = string.IsNullOrEmpty(model.DefaultPass) ? new Random().GenerateString(DEFPASSLEN) : model.DefaultPass;
                    model.CurrentStep = "FormSubmit";
                    break;
                case "FormSubmit":
                    {
                        if (IsValidPostBack("Name"))
                            return RunSafe(() =>
                            {
                                if (model.UserID == 0)
                                {
                                    //Add
                                    var user = (UserDetail)model;
                                    user.Ud_firstLogin = true;
                                    ctx2.AddToUserDetails(user);
                                    //TODO: Remove
                                    var root = ctx2.Folders.FirstOrDefault(f => !f.ParentID.HasValue);
                                    if (root != null)
                                        user.Folders.Add(root);
                                }
                                else
                                {
                                    //Update
                                    var pass = model.DefaultPass.Encript();
                                    var user = ctx2.UserDetails.First(u => u.Ud_id == model.UserID);
                                    user.Ud_loginName = model.Login;
                                    user.Ud_firstName = model.Name;
                                    user.Ud_lastName = model.Surname;
                                    user.Ud_email = model.Email;
                                    user.Ud_userType = model.UserTypeID;
                                    user.Ud_password = pass;
                                    user.Ud_salt = model.Salt ?? "";
                                    user.Ud_iso_id = model.CountryID;
                                    user.Ud_ho_id = model.CompanyID;
                                    user.Ud_br_id = model.BranchID;
                                    user.Ud_languageId = model.LanguageID;
                                }
                                ctx2.SaveChanges();
                                Session.Remove(Strings.CachedMODELUserDetails);
                            });
                    }
                    break;
            }

            model.Country = ISO_ptf.GetNameByID(model.CountryID);
            model.Company = HeadOffice.GetNameByID(model.CountryID, model.CompanyID);
            model.Branch = Branch.GetNameByID(model.CountryID, model.CompanyID, model.BranchID);
            model.UserType = ((UserType)model.UserTypeID).ToString();
            return View(model);
        }

        [AdminAuthorize]
        [AcceptVerbs("get")]
        public ActionResult EditUser(int id)
        {
            var ctx2 = PTFReportsContext.Current;
            var user = ctx2.UserDetails.First(ud => ud.Ud_id == id);
            var model = (UserModel)user;
            Session[Strings.CachedMODELUserDetails] = model;
            model.CurrentStep = "BranchList";
            return RedirectToAction("AddUser");
        }

        #endregion

        #region SESSIONS

        [AdminAuthorize]
        [AcceptVerbs("get")]
        public ActionResult Sessions(int? page)
        {
            var ctx2 = PTFReportsContext.Current;
            List<SessionModel> list = new List<SessionModel>();
            var sessions = new PaginatedList<Session>(ctx2.Sessions.OrderBy(f => f.UserID), page.GetValueOrDefault(), PAGESIZE);
            ViewBag.Pager = sessions;
            foreach (var u in sessions)
                list.Add((SessionModel)u);
            return View(list);
        }

        [AdminAuthorize]
        [AcceptVerbs("get")]
        public ActionResult Histories(int? page)
        {
            var ctx2 = PTFReportsContext.Current;
            List<HistoryModel> list = new List<HistoryModel>();
            var histories = new PaginatedList<History>(ctx2.Histories.OrderBy(f => f.Date), page.GetValueOrDefault(), PAGESIZE);
            ViewBag.Pager = histories;
            foreach (var u in histories)
                list.Add((HistoryModel)u);
            return View(list);
        }

        [AdminAuthorize]
        [AcceptVerbs("get")]
        public ActionResult OpenSessions()
        {
            var model = new List<Client>(SessionManager.Instant.Clients);
            return View(model);
        }

        [AdminAuthorize]
        [AcceptVerbs("get")]
        public ActionResult CloseSession(string id)
        {
            return RunSafe(() =>
                {
                    SessionManager.Instant.CloseAndBlockIP(id);
                });
        }

        #endregion

        #region Common

        public ActionResult RunSafe(System.Action act, System.Action<Exception> exAct = null)
        {
            try
            {
                act();
                return View("Success");
            }
            catch (Exception ex)
            {
                if (exAct == null)
                {
                    var sqlex = ex.InnerException as SqlException;
                    if (sqlex != null)
                    {
                        Debug.WriteLine(sqlex);
                        ViewData["ERR"] = sqlex.Message.Replace("\r\n", "<br/>");
                        PTFReportsContext.Current.SaveError(sqlex);
                    }
                    else
                    {
                        Debug.WriteLine(ex);
                        ViewData["ERR"] = ex.Message.Replace("\r\n", "<br/>");
                        PTFReportsContext.Current.SaveError(ex);
                    }
                    return View("Error");
                }
                else
                {
                    exAct(ex);
                    return View("Error");
                }
            }
        }

        public Exception RunSafeAsynch(System.Action act)
        {
            try
            {
                act();
                return null;
            }
            catch (Exception ex)
            {
                var sqlex = ex.InnerException as SqlException;
                return (sqlex != null) ? sqlex : ex;
            }
        }

        public bool IsValidPostBack(string tagName)
        {
            return Request.Form.Count != 0 &&
                !string.IsNullOrWhiteSpace(Request.Form[tagName]);
        }

        #endregion
    }
}
