using System;
using System.Text;
using log4net;
using Microsoft.Reporting.WebForms;
using PTF.Reports.Common;
using diag = System.Diagnostics;

namespace PTF.Reports
{
    public partial class ViewReport : System.Web.UI.Page
    {
        private static readonly ILog ms_logger = LogManager.GetLogger(typeof(ViewReport).Name);

        const string ERRMSG = "There was an error on the report server. Please excuse us.";

        protected override void OnInit(EventArgs e)
        {
            Server.ScriptTimeout = Convert.ToInt32(TimeSpan.FromMinutes(20).TotalSeconds);
            this.ReportViewer1.ReportError += new ReportErrorEventHandler(ReportViewer1_ReportError);
            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!this.Page.IsPostBack && Session[Strings.ReportParameterList] != null)
            {
                try
                {
                    ReportData table = Session[Strings.ReportParameterList].Cast<ReportData>();
                    this.Title = table.ReportName;

                    string serverUrl = Config.Get<string>(Strings.ReportServerUrl);
                    ServerReport server = this.ReportViewer1.ServerReport;
                    server.ReportServerUrl = new Uri(serverUrl);
                    server.ReportPath = table.ReportPath;
                    //A value of -1 means that there is no time-out period.
                    server.Timeout = -1;

                    bool anonimous = Config.Get<bool>(Strings.ReportServerAnonimousLoging);
                    if (!anonimous)
                    {
                        string userName = Config.Get<string>(Strings.ReportServerUserName);
                        string userPass = Config.Get<string>(Strings.ReportServerUserPass);
                        string domain = Config.Get<string>(Strings.ReportServerDomainName);
                        server.ReportServerCredentials = new ReportServerCredentials(userName, userPass, domain);
                    }

                    this.ReportViewer1.ProcessingMode = ProcessingMode.Remote;
                    this.ReportViewer1.ServerReport.SetParameters(table.List);
                    this.ReportViewer1.ServerReport.Refresh();

                    StringBuilder b = new StringBuilder();
                    foreach (ReportParameter p in table.List)
                    {
                        b.Append(p.toString());
                        b.AppendLine();
                    }
                    diag.Trace.WriteLine(b.ToString());
                }
                catch (Exception ex)
                {
                    diag.Trace.WriteLine(ex);
#if DEBUG
                    txtMessage.Text = ex.ToString();
#else
                    txtMessage.Text = ERRMSG;
#endif
                }
                finally
                {
                    Session.Remove(Strings.ReportParameterList);
                }
            }
            
            base.OnLoad(e);
        }

        private void ReportViewer1_ReportError(object sender, ReportErrorEventArgs e)
        {
            diag.Trace.WriteLine(e.Exception);
            e.Handled = true;
            txtMessage.Visible = true;
#if DEBUG
            txtMessage.Text = e.Exception.Message;
#else
            txtMessage.Text = ERRMSG;
#endif
            ServerReport server = this.ReportViewer1.ServerReport;
            string path = server.ReportPath;

            ms_logger.Info(path);
            ms_logger.Error(e.Exception.Message, e.Exception);
        }
    }
}