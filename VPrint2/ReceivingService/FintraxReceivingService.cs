/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Configuration;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceProcess;
using System.Threading;
using ReceivingServiceLib;
using ReceivingServiceLib.FileWorkers;

namespace ReceivingService
{
    public partial class FintraxReceivingService : ServiceBase
    {
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
            strings.Save();

            ImportFileWorker.Default.StartStop();
            ExportFileWorker.Default.StartStop();

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
    }
}
