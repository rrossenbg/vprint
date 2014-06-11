/***************************************************
//  Copyright (c) Premium Tax Free 2012
/***************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Services.Protocols;
using PTF.Reports.Collections;
using PTF.Reports.PTFReportsDB;
using PTF.Reports.TRSReportingService;

namespace PTF.Reports.Common
{
    public static class Helper
    {
        public static IEnumerable<SelectListItem> Convert(ValidValue[] values)
        {
            foreach (var value in values)
                yield return new SelectListItem() { Text = value.Label, Value = value.Value };
        }

        public static void SaveSessionAsynch(string sessionId, UserDetail user)
        {
            new Action<string>((ssid) =>
            {
                var ctx = new PTFReportsContext();
                var session = ctx.Sessions.First(s => s.BrowserSessionID == ssid);
                if (session != null)
                {
                    session.UserID = user.Ud_id;
                    ctx.SaveChanges();
                }

            }).FireAndForgetSafe(sessionId);
        }

        public static void SaveLoginFailAsynch(string loginName, int maxLoginTries)
        {
            new Action<string, int>((name, count) =>
            {
                var ctx = new PTFReportsContext();
                var user = ctx.UserDetails.FirstOrDefault(s => s.Ud_loginName == name);
                if (user != null)
                {
                    user.LoginTries = user.LoginTries.GetValueOrDefault() + 1;
                    if (user.LoginTries > count)
                        user.BlockedAt = DateTime.Now;
                    ctx.SaveChanges();
                }

            }).FireAndForgetSafe(loginName, maxLoginTries);
        }

        public static void ResetLoginAttemptsAsynch(string loginName)
        {
            new Action<string>((name) =>
            {
                var ctx = new PTFReportsContext();
                var user = ctx.UserDetails.FirstOrDefault(s => s.Ud_loginName == name);
                if (user != null)
                {
                    user.LoginTries = null;
                    ctx.SaveChanges();
                }

            }).FireAndForgetSafe(loginName);
        }

        public static void LoadBlockedIPsAsynch()
        {
            new Action(() =>
            {
                PTFReportsContext.BlockedIPsTable.Clear();
                var ctx = new PTFReportsContext();
                foreach (var ip in ctx.IPs)
                {
                    PTFReportsContext.BlockedIPsTable[ip.IP1] = ip.BlockedAt;
                }
            }).FireAndForgetSafe();
        }

        public static void SaveIPBlockedAsynch(string ipAddr)
        {
            new Action<string>((ip) =>
            {
                PTFReportsContext.BlockedIPsTable[ip] = DateTime.Now;

                var ctx = new PTFReportsContext();
                var ipa = ctx.IPs.FirstOrDefault(i => i.IP1 == ip);
                if (ipa == null)
                    ctx.AddToIPs(new IP() { IP1 = ip, BlockedAt = DateTime.Now });
                else
                    ipa.BlockedAt = DateTime.Now;
                ctx.SaveChanges();
            }).FireAndForgetSafe(ipAddr);
        }

        static Helper()
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
        }

        public static DynamicDictionary GetReportParametersSafe(Guid id)
        {
            dynamic data = new DynamicDictionary();

            string url = Config.Get<string>(Strings.ReportServerServiceUrl);
            bool anonimous = Config.Get<bool>(Strings.ReportServerAnonimousLoging);
            string userName = Config.Get<string>(Strings.ReportServerUserName);
            string userPass = Config.Get<string>(Strings.ReportServerUserPass);
            string domain = Config.Get<string>(Strings.ReportServerDomainName);

            var ctx2 = PTFReportsContext.Current;
            var report = ctx2.Reports.First(r => r.ReportID == id);

            data.ReportID = report.ReportID;
            data.SqlParams = report.Parameters;
            data.Message = "";
            try
            {
                var client = new RSClient(url);
                if (!anonimous)
                    client.SetCredentials(userName, userPass, domain);

                data.Name = report.Name;
                data.Path = report.Page;
                data.Parameters = client.GetReportParameters(report.Page, null, false, null, null);
            }
            catch (SoapException ex)
            {
                data.Message = ex.Detail.InnerXml.ToString();
            }
            catch (Exception ex)
            {
                data.Message = ex.Message;
            }
            return data;
        }

        public static void SaveErrorAsynch(Exception ex)
        {
            new Action<Exception>((err) =>
            {
                var ctx = new PTFReportsContext();
                ctx.SaveError(err);
            }).FireAndForgetSafe(ex);
        }
    }
}