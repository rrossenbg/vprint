using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Lib.Web.Mvc.JQuery.TreeView;
using PTF.Reports.Collections;
using PTF.Reports.Common;
using PTF.Reports.Models;
using PTF.Reports.PTFDB;
using PTF.Reports.PTFReportsDB;
using ms = Microsoft.Reporting.WebForms;

namespace PTF.Reports.Controllers
{
    [HttpBlock]
    [LogErrors]
    public class ReportsController : Controller
    {
        [ReportAuthorize]
        public ActionResult Index()
        {
            var ctx2 = PTFReportsContext.Current;
            return View(ctx2.Folders);
        }

        [ReportAuthorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Reports(FormCollection form)
        {
            string node = Request.Form["root"];

            Debug.WriteLine(node, "Load request");

            List<TreeViewNode> nodes = new List<TreeViewNode>();

            var ctx2 = PTFReportsContext.Current;

            foreach (Report child in ctx2.Reports)
            {
                nodes.Add(new TreeViewNode()
                {
                    //Report
                    id = "R" + child.ReportID,
                    text = child.Name,
                    classes = "file",
                    hasChildren = true,
                });
            }

            return Json(nodes);
        }

        [ReportAuthorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SelectReport(ValueList details)
        {
            if (string.IsNullOrWhiteSpace(details.Value))
                return Json("ok");
            //Check for report hit
            if (details.Value.Contains('R'))
            {
                var nodeId = details.Split<Guid>('R').FirstOrDefault();
                Session[Strings.REPORTID] = nodeId;
            }
            else if (details.Value.Contains('K'))
            {
                Session[Strings.FOLDERID] = details.Split<Guid>('K').FirstOrDefault();
                Session.Remove(Strings.REPORTID);
            }
            Session.Remove(Strings.CachedMODELParameters);
            return Json("ok");
        }

        public class ParamObj
        {
            public string Step { get; set; }
            public string IsoID { get; set; }
            public string CompanyID { get; set; }
        }

        [ReportAuthorize]
        [AcceptVerbs("get", "post")]
        [CompressFilter(Order = 1)]
        [CacheFilter(Duration = 5, Order = 2)]
        public JsonResult ReportParameters(ParamObj data)
        {
            ViewData[Strings.ERR] = "No report selected";

            ReportParametersModel model = null;

            if (Session[Strings.REPORTID] == null || data == null)
                return Json("err", JsonRequestBehavior.AllowGet);

            var currenReportId = Session[Strings.REPORTID].Cast<Guid>();
            var user = Session[Strings.USER].Cast<UserDetail>();

            model = Session.Get<ReportParametersModel>(Strings.CachedMODELParameters, new ReportParametersModel());

            model.CurrentStep = data.Step;

            if (model.CurrentStep == "Reset")
                model = new ReportParametersModel();

            var ctx1 = PTFContext.Current;
            var ctx2 = PTFReportsContext.Current;
            var report = ctx2.Reports.First(r => r.ReportID == currenReportId);

            if (report == null)
            {
                Session.Remove(Strings.REPORTID);
                Session.Remove(Strings.CachedMODELParameters);
                var result1 = this.RenderPartialToString("ReportParameters", model);
                return Json(result1, JsonRequestBehavior.AllowGet);
            }
            else
            {
                model.Name = report.Name;
                model.Description = report.Description;
                model.Page = report.Page;
                model.ReportID = report.ReportID;
                model.CountryID = user.Ud_iso_id;
                model.CompanyID = user.Ud_ho_id;
                model.LanguageID = user.Ud_languageId.GetValueOrDefault((int)eLanguage.English);
            }

            var permissions = CommonTools.Cached<IQueryable<Permission>>(string.Concat(user.Ud_loginName, "_Permissions"),
                () => ctx2.Permissions.Where(p => p.UserID == user.Ud_id && p.IsoID == user.Ud_iso_id));
            var companies = permissions.GroupBy(c => c.HoID);

            string result2;
            if (companies.Count() <= 1)
            {
                ShowShortParameterForm(model, report, data, permissions);
                result2 = this.RenderPartialToString("ReportParametersShort", model);
            }
            else
            {
                ShowLongParameterForm(model, report, data);
                result2 = this.RenderPartialToString("ReportParameters", model);
            }

            return Json(result2, JsonRequestBehavior.AllowGet);
        }

        private void ShowShortParameterForm(ReportParametersModel model, Report report, ParamObj data, IQueryable<Permission> permissions)
        {
            var user = Session[Strings.USER].Cast<UserDetail>();
            var repository = Session.Get<TagRepository>(Strings.TagRepository, new TagRepository());
            var ctx1 = PTFContext.Current;
            var ctx2 = PTFReportsContext.Current;

            // retailers
            var list = CommonTools.Cached<List<Branch>>(string.Concat(user.Ud_loginName, "_List<Branch>"), () => new List<Branch>());

            if (list.Count == 0)
            {
                repository.Clear();

                foreach (Permission p in permissions)
                {
                    var branch = ctx1.Branches.FirstOrDefault(
                                br => br.br_iso_id == p.IsoID &&
                                        br.br_ho_id == p.HoID &&
                                        br.br_id == p.RetailerID);
                    list.Add(branch);

                    repository.SetTag(branch);
                }
            }
            //get user's retailer
            if (list.Count == 0)
            {
                var branch = Branch.GetUserDefault(user);
                list.Add(branch);
                repository.SetTag(branch);
            }

            ViewData[ParamMapping.RetailerID.ToString()] =
                new MultiSelectList(list.OrderBy(br => br.FullName2), "br_id", "FullName2", null);
        }

        private void ShowLongParameterForm(ReportParametersModel model, Report report, ParamObj data)
        {
            var user = Session[Strings.USER].Cast<UserDetail>();
            var repository = Session.Get<TagRepository>(Strings.TagRepository, new TagRepository());

            var ctx1 = PTFContext.Current;
            var ctx2 = PTFReportsContext.Current;

            if (model.CurrentStep == "IsoID")
                model.CurrentStep = "CompanyID";
            else if (model.CurrentStep == "CompanyID")
                model.CurrentStep = "RetailerID";
            else if (model.CurrentStep == "RetailerID")
                model.CurrentStep = "FormSubmit";
            else if (model.CurrentStep == "FormSubmit")
                model.CurrentStep = null;

            switch (model.CurrentStep)
            {
                case "":
                case null:
                    {
                        goto case "IsoID";
                    }
                case "IsoID":
                    {
                        model.CurrentStep = "IsoID";
                        model.SkipCountry = true;
                        model.CountryID = user.Ud_iso_id;
                        goto case "CompanyID";

                        //always get user country
                        var list = new List<ISO_ptf>();
                        list.Add(ISO_ptf.GetUserDefault(user));

                        ViewData[ParamMapping.IsoID.ToString()] = new SelectList(list.OrderBy(c => c.iso_country), "iso_number", "iso_country", null);
                    }
                    break;
                case "CompanyID":
                    {
                        model.CurrentStep = "CompanyID";
                        model.CompanyID = user.Ud_ho_id;
                        model.Country = ISO_ptf.GetNameByID(model.CountryID);

                        var list = new List<HeadOffice>();

                        //user permissions
                        var permissions = ctx2.Permissions.Where(p => p.UserID == user.Ud_id && p.IsoID == model.CountryID);
                        foreach (var gp in permissions.GroupBy(c => c.HoID))
                        {
                            var p = gp.First();
                            list.Add(ctx1.HeadOffices.FirstOrDefault(
                                ho => ho.ho_iso_id == p.IsoID && ho.ho_id == p.HoID));
                        }

                        model.SkipCompany = list.Count < 2;

                        //get user's company
                        if (list.Count == 0)
                        {
                            model.CompanyID = user.Ud_ho_id;
                            goto case "RetailerID";
                        }

                        ViewData[ParamMapping.CompanyID.ToString()] = new SelectList(list.OrderBy(ho => ho.ho_name), "ho_id", "ho_name", null);
                    }
                    break;
                case "RetailerID":
                    {
                        model.CurrentStep = "RetailerID";
                        model.CompanyID = data.CompanyID.Parse(user.Ud_ho_id);
                        model.Company = HeadOffice.GetNameByID(user.Ud_iso_id, model.CompanyID);

                        // retailers
                        var list = new List<Branch>();

                        //user permissions
                        var permissions = ctx2.Permissions.Where(p => p.UserID == user.Ud_id && p.IsoID == user.Ud_iso_id && p.HoID == model.CompanyID);

                        repository.Clear();

                        foreach (Permission p in permissions)
                        {
                            var branch = ctx1.Branches.FirstOrDefault(
                                br =>   br.br_iso_id == p.IsoID &&
                                        br.br_ho_id == p.HoID &&
                                        br.br_id == p.RetailerID);

                            list.Add(branch);
                            repository.SetTag(branch);
                        }

                        //get user's retailer
                        if (list.Count == 0)
                        {
                            var branch = Branch.GetUserDefault(user);
                            list.Add(branch);
                            repository.SetTag(branch);
                        }

                        ViewData[ParamMapping.RetailerID.ToString()] =
                            new MultiSelectList(list.OrderBy(br => br.FullName2), "br_id", "FullName2", null);
                    }
                    break;
            }
        }

        private static readonly int AUTOCOMPL = Config.Get<int>(Strings.AUTOCOMPCOUNT);

        [ReportAuthorize]
        [HttpPost]
        public JsonResult GetTaggedParameters(string query)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(query), "Wrong query");
            var repository = Session.Get<TagRepository>(Strings.TagRepository, new TagRepository());
            var data = repository.GetTags(query, AUTOCOMPL, (x, y) => string.CompareOrdinal(x.Name, y.Name));
            return Json(data);
        }

        [ReportAuthorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ShowReport(FormCollection form)
        {
            var model = Session.Get<ReportParametersModel>(Strings.CachedMODELParameters);
            if (model == null)
                return RedirectToAction("Index", "Administration");

            var currenReportId = Session[Strings.REPORTID].Cast<Guid>();

            var ctx2 = PTFReportsContext.Current;
            var report = ctx2.Reports.First(r => r.ReportID == currenReportId);

            var param = report.Parameters.FirstOrDefault(p => p.ParamType == (int)ParamMapping.IsoID);
            model.CountryParamID = (param != null) ? param.Name : null;
            param = report.Parameters.FirstOrDefault(p => p.ParamType == (int)ParamMapping.CompanyID);
            model.CompanyParamID = (param != null) ? param.Name : null;
            param = report.Parameters.FirstOrDefault(p => p.ParamType == (int)ParamMapping.RetailerID);
            model.RetailerParamID = (param != null) ? param.Name : null;
            param = report.Parameters.FirstOrDefault(p => p.ParamType == (int)ParamMapping.Language);
            model.LanguageParam = (param != null) ? param.Name : null;

            if (model.CountryParamID.IsNullEmptyOrWhite() &&
                model.CompanyParamID.IsNullEmptyOrWhite() &&
                model.RetailerParamID.IsNullEmptyOrWhite())
            {
                return Redirect("ViewReport.aspx");
            }

            var retailerIds = Request.Form["RetailerID"];
            model.RetailerID = retailerIds.Split<int, Guid>(',',
                (s) => int.Parse(s), 
                (s) => Guid.Parse(s),
                (g) =>
                {
                    int iso_id, ho_id, br_id, fake;
                    CommonTools.FromGuid(g, out iso_id, out ho_id, out br_id, out fake);
                    return br_id;
                });

            var names = new string[] { model.CountryParamID, model.CompanyParamID, model.LanguageParam };
            var values = new int[] { model.CountryID, model.CompanyID, model.LanguageID };

            ReportData table = new ReportData();
            table.ReportPath = model.Page;
            table.ReportName = model.Name;

            string url = string.Format("{0};{1};{2};{3};{4};{5};{6};{7}",
                                    table.ReportPath, 
                                    model.CountryParamID,
                                    model.CountryID,
                                    model.CompanyParamID, 
                                    model.CompanyID, 
                                    model.RetailerParamID, 
                                    model.LanguageParam,
                                    retailerIds);

            var session = PTFReportsDB.Session.GetByID(Session.SessionID);
            ctx2.AddToHistories(new History()
            {
                Session = session,
                Page = url,
                Date = DateTime.Now,
            });
            ctx2.SaveChanges();

            ms.ReportParameter rparam = null;

            //CountryID, HeadOfficeID
            for (int i = 0; i < names.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(names[i]))
                {
                    rparam = new ms.ReportParameter();
                    rparam.Name = names[i];
                    rparam.Values.Add(Convert.ToString(values[i]));
                    rparam.Visible = false;
                    table.List.Add(rparam);
                }
            }

            //RetailerID
            if (!model.RetailerParamID.IsNullEmptyOrWhite())
            {
                rparam = new ms.ReportParameter();
                rparam.Name = model.RetailerParamID;
                foreach (int id in model.RetailerID)
                    rparam.Values.Add(id.ToString());

                rparam.Visible = false;
                table.List.Add(rparam);
            }

            Session[Strings.ReportParameterList] = table;
            return Redirect("ViewReport.aspx");
        }

        public ActionResult TestReportRendering()
        {
            ms.ReportParameter p1 = new ms.ReportParameter();
            p1.Name = "ISO_ID";
            p1.Values.Add("826");

            ms.ReportParameter p2 = new ms.ReportParameter();
            p2.Name = "LedgerPeriod";
            p2.Values.Add("201202");

            ms.ReportParameter p3 = new ms.ReportParameter();
            p3.Name = "ho_id";
            p3.Values.Add("0");

            ms.ReportParameter p4 = new ms.ReportParameter();
            p4.Name = "br_id";
            p4.Values.Add("0");

            HttpContext.Items[Strings.ReportPath] = "/Accounting/Agent_Chargeback_Details";
            HttpContext.Items[Strings.ReportParameterList] = new List<ms.ReportParameter>(new ms.ReportParameter[] { p1, p2, p3, p4 });

            return PartialView("ViewReport");
        }
    }
}
