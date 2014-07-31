/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.ServiceModel;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using ReceivingServiceLib;
using ReceivingServiceLib.FileWorkers;

namespace ReceivingService
{
    public partial class FintraxReceivingService : ServiceBase
    {
        private const int HISTORY_LEN = 100;

        private readonly CircularBuffer<Tuple<string, string, DateTime>> m_HistiryBuffer = new CircularBuffer<Tuple<string, string, DateTime>>(HISTORY_LEN);

        private ServiceHost m_ServerHost;

        public FintraxReceivingService()
        {
            InitializeComponent();

            AutoLog = true;

            ImportFileWorker.Error += OnError;
            ExportFileWorker.Error += OnError;
            ErrorHandler.Error += OnError;
        }

        protected override void OnStart(string[] args)
        {
            Strings strings = new Strings();
            strings.ConnString = ConfigurationManager.ConnectionStrings["PTF_ImagesDB"].ConnectionString.IfNullOrEmptyThrow<ArgumentException>();
            strings.PTFConnString = ConfigurationManager.ConnectionStrings["PTF_DB"].ConnectionString.IfNullOrEmptyThrow<ArgumentException>();
            strings.UPLOADROOT = ConfigurationManager.AppSettings["UPLOADFOLDER"].IfNullOrEmptyThrow<ArgumentException>();
            strings.VOCUHERSFOLDER = ConfigurationManager.AppSettings["VOUCHERSFOLDER"].IfNullOrEmptyThrow<ArgumentException>();
            strings.VOCUHERSEXPORTFOLDER = ConfigurationManager.AppSettings["VOCUHERSEXPORTFOLDER"].IfNullOrEmptyThrow<ArgumentException>();
            strings.UPLOADERRORS = ConfigurationManager.AppSettings["UPLOADERRORS"].IfNullOrEmptyThrow<ArgumentException>();
            strings.VERSIONFOLDER = ConfigurationManager.AppSettings["VERSIONFOLDER"].IfNullOrEmptyThrow<ArgumentException>();
            strings.pfxFileFullPath = ConfigurationManager.AppSettings["pfxFileFullPath"].IfNullOrEmptyThrow<ArgumentException>();
            strings.PTFLogoFileFullPath = ConfigurationManager.AppSettings["PTFLogoFileFullPath"].IfNullOrEmptyThrow<ArgumentException>();
            strings.Save();

            ImportFileWorker.Default.StartStop();
            ExportFileWorker.Default.StartStop();

            ScanService.NewCall += new EventHandler<ValueEventArgs<Tuple<string, string, DateTime>>>(ScanService_NewCall);
            m_ServerHost = new ServiceHost(typeof(ScanService));
            m_ServerHost.Open();

            base.OnStart(args);
        }        

        protected override void OnStop()
        {
            ImportFileWorker.Default.StartStop();
            ExportFileWorker.Default.StartStop();

            if (m_ServerHost != null)
            {
                m_ServerHost.Close();
                m_ServerHost = null;
            }

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
            //Save call history
            if (command == 222)
            {
                new Action(() =>
                {
                    var path = Path.Combine("C:\\", base.ServiceName, ".log");
                    var arr = m_HistiryBuffer.ToArray();
                    var builder = new StringBuilder();

                    foreach (Tuple<string, string, DateTime> i in arr)
                        builder.AppendLine(string.Concat(i.Item3, "\t", i.Item1, "\t", i.Item2));

                    File.WriteAllText(path, builder.ToString());
                }).RunSafe();
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
    }
}
