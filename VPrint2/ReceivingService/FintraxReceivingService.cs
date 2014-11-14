/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.ServiceModel;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ReceivingServiceLib;
using ReceivingServiceLib.Common.Data;
using ReceivingServiceLib.Data;
using ReceivingServiceLib.FileWorkers;
using VPrinting;
using VPrinting.Colections;

namespace ReceivingService
{
    public partial class FintraxReceivingService : ServiceBase
    {
        private const int HISTORY_LEN = 500;

        private readonly CircularBuffer<Tuple<string, string, DateTime>> m_HistiryBuffer = new CircularBuffer<Tuple<string, string, DateTime>>(HISTORY_LEN);

        private ServiceHost m_ServerHost;

        public FintraxReceivingService()
        {
            InitializeComponent();

            AutoLog = true;

            ImportFileWorker.Error += OnError;
            ExportFileWorker.Error += OnError;
            CoverWorker.Error += OnError;
            ErrorHandler.Error += OnError;
        }

        protected override void OnStart(string[] args)
        {
            Strings strings = new Strings();
            strings.ConnString = ConfigurationManager.ConnectionStrings["PTF_ImagesDB"].ConnectionString.IfNullOrEmptyThrow<ArgumentException>();
            strings.PTFConnString = ConfigurationManager.ConnectionStrings["PTF_DB"].ConnectionString.IfNullOrEmptyThrow<ArgumentException>();
            strings.UPLOADROOT = ConfigurationManager.AppSettings["UPLOADFOLDER"].IfNullOrEmptyThrow<ArgumentException>();
            strings.DOWNLOADROOT = ConfigurationManager.AppSettings["DOWNLOADFOLDER"].IfNullOrEmptyThrow<ArgumentException>();
            strings.VOCUHERSFOLDER = ConfigurationManager.AppSettings["VOUCHERSFOLDER"].IfNullOrEmptyThrow<ArgumentException>();
            strings.VOCUHERSEXPORTFOLDER = ConfigurationManager.AppSettings["VOCUHERSEXPORTFOLDER"].IfNullOrEmptyThrow<ArgumentException>();
            strings.COVERWORKFOLDER = ConfigurationManager.AppSettings["COVERWORKFOLDER"].IfNullOrEmptyThrow<ArgumentException>();
            strings.CONTENTWORKFOLDER = ConfigurationManager.AppSettings["CONTENTWORKFOLDER"].IfNullOrEmptyThrow<ArgumentException>();
            strings.UPLOADERRORS = ConfigurationManager.AppSettings["UPLOADERRORS"].IfNullOrEmptyThrow<ArgumentException>();
            strings.VERSIONFOLDER = ConfigurationManager.AppSettings["VERSIONFOLDER"].IfNullOrEmptyThrow<ArgumentException>();
            strings.pfxFileFullPath = ConfigurationManager.AppSettings["pfxFileFullPath"].IfNullOrEmptyThrow<ArgumentException>();
            strings.PTFLogoFileFullPath = ConfigurationManager.AppSettings["PTFLogoFileFullPath"].IfNullOrEmptyThrow<ArgumentException>();
            strings.Save();

            ImportFileWorker.Default.StartStop();
            ExportFileWorker.Default.StartStop();
            // CoverWorker.Default.StartStop();

            ScanService.NewCall += new EventHandler<ValueEventArgs<Tuple<string, string, DateTime>>>(ScanService_NewCall);
            ScanService.ExtractVoucher += new EventHandler<ValueEventArgs<Tuple<VoucherDataAccess.SelectVoucherInfoData, DirectoryInfo>>>(ScanService_ExtractVoucher);
            ScanService.EmailNotaDebitoEvent += new EventHandler<ValueEventArgs<EmailInfo>>(ScanService_EmailNotaDebitoEvent);
            string user = ConfigurationManager.AppSettings["REPORTINGSERVER_USER"].IfNullOrEmptyThrow<ArgumentException>();
            string pass = ConfigurationManager.AppSettings["REPORTINGSERVER_PASS"].IfNullOrEmptyThrow<ArgumentException>();
            string domain = ConfigurationManager.AppSettings["REPORTINGSERVER_DOMAIN"].IfNullOrEmptyThrow<ArgumentException>();
            ScanService.ReportServerCredentials = new NetworkCredential(user, pass, domain);
            m_ServerHost = new ServiceHost(typeof(ScanService));
            m_ServerHost.Open();

            base.OnStart(args);
        }

        protected override void OnStop()
        {
            ImportFileWorker.Default.StartStop();
            ExportFileWorker.Default.StartStop();
            CoverWorker.Default.StartStop();

            if (m_ServerHost != null)
            {
                m_ServerHost.Close();
                m_ServerHost = null;
            }

            m_HistiryBuffer.Clear();

            base.OnStop();
        }

        protected override void OnShutdown()
        {
            if (m_ServerHost != null)
            {
                m_ServerHost.Close();
                m_ServerHost = null;
            }

            base.OnShutdown();
        }

        protected override void OnCustomCommand(int command)
        {
            switch (command)
            {
                //Save call history
                case 222:
                    new Action(() =>
                    {
                        const string path = "C:\\ReceivingService.log";
                        var arr = m_HistiryBuffer.ToArray();
                        var builder = new StringBuilder();

                        foreach (Tuple<string, string, DateTime> i in arr)
                            builder.AppendLine(string.Concat(i.Item3, "\t\t", i.Item1, "\t\t", i.Item2));

                        File.WriteAllText(path, builder.ToString());
                    }).RunSafe();
                    break;
                case 223:
                    ImportFileWorker.Default.StartStop();
                    break;
                case 224:
                    ExportFileWorker.Default.StartStop();
                    break;
                case 225:
                    CoverWorker.Default.StartStop();
                    break;
            }

            base.OnCustomCommand(command);
        }

        private void OnError(object sender, ThreadExceptionEventArgs e)
        {
            if (Monitor.TryEnter(this, 200))
            {
                try
                {
                    var msg = e.Exception.ToString();
                    Trace.WriteLine(msg, Strings.APPNAME);
                    this.EventLog.WriteEntry(msg, EventLogEntryType.Error);
                }
                finally
                {
                    Monitor.Exit(this);
                }
            }
        }

        private void ScanService_NewCall(object sender, ValueEventArgs<Tuple<string, string, DateTime>> e)
        {
            m_HistiryBuffer.Add(e.Value);
        }

        private void ScanService_ExtractVoucher(object sender, ValueEventArgs<Tuple<VoucherDataAccess.SelectVoucherInfoData, DirectoryInfo>> e)
        {
            var t = Task.Factory.StartNew((o) =>
            {
                FileInfo binFile = null;
                FileInfo zipFile = null;

                try
                {
                    var result2 = (Tuple<VoucherDataAccess.SelectVoucherInfoData, DirectoryInfo>)o;
                    byte[] data = VoucherDataAccess.Instance.SelectImageById(result2.Item1.vid, true);

                    result2.Item2.EnsureDirectory();
                    result2.Item1.session_Id = result2.Item1.session_Id ?? Guid.NewGuid().ToString();

                    binFile = result2.Item2.CombineFileName(result2.Item1.session_Id + ".bin");

                    zipFile = result2.Item2.CombineFileName(result2.Item1.session_Id + ".zip");

                    if (result2.Item1.v_protected)
                    {
                        binFile.WriteAllBytes(data);
                        binFile.DecriptFile(zipFile);
                    }
                    else
                    {
                        zipFile.WriteAllBytes(data);
                    }

                    var fac2 = new ZipFileAccess();
                    fac2.RestoreZip(zipFile.FullName, result2.Item2.FullName);
                }
                catch (Exception ex)
                {
                    OnError(this, new ThreadExceptionEventArgs(ex));
                }
                finally
                {
                    binFile.DeleteSafe();
                    zipFile.DeleteSafe();
                }
            }, e.Value, TaskCreationOptions.LongRunning);
        }

        private void ScanService_EmailNotaDebitoEvent(object sender, ValueEventArgs<EmailInfo> e)
        {
            var t = Task.Factory.StartNew((o) =>
            {
                try
                {
                    EmailInfo val = (EmailInfo)o;
                    int countryId = val.IsoId;
                    int officeId = val.HoId;
                    int invoiceNumber = val.InNumber;
                    DateTime invoiceDate = val.InDate;

                    string serverUrl = string.Format("http://192.168.53.144/Reportserver/Pages/ReportViewer.aspx?%2fNota+Debito%2fNota+Debito+0032&rs:Command=Render&rs:format=PDF&iso_id={0}&Office={1}&in_date={2:dd/MM/yyyy}&invoicenumber={3}",
                    countryId, officeId, invoiceDate, invoiceNumber);

                    WebDataAccess access = new WebDataAccess();
                    var buffer = access.DownloadReport(serverUrl, ScanService.ReportServerCredentials);
#warning TEST_ONLY
                    string email = "rosen.rusev@fintrax.com";// new PTFDataAccess().FindHeadOfficeEmail(countryId, officeId);//"rosen.rusev@fintrax.com";// 
                    string ccEmail = val.CC;
                    string subject = val.Subject;
                    string message = val.Body;

                    var att = new Attachment(new MemoryStream(buffer), new ContentType("application/pdf"));
                    EmailSender.SendSafe(email, ccEmail, subject, message, false, att);
                }
                catch (Exception ex)
                {
                    OnError(this, new ThreadExceptionEventArgs(ex));
                }

            }, e.Value, TaskCreationOptions.LongRunning);
        }
    }
}
