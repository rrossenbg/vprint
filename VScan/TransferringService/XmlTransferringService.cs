/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Configuration;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;
using PremierTaxFree.PTFLib;
using PremierTaxFree.PTFLib.Data;

namespace TransferringService
{
    public partial class XmlTransferringService : ServiceBase
    {
        public static readonly TimeSpan TRANSFERRINGWORKER_PERIOD = TimeSpan.FromMinutes(1);
        private readonly TransferringWorker m_TransferringWorker = new TransferringWorker();

        public XmlTransferringService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            ClientDataAccess.ConnectionString = ConfigurationManager.AppSettings[Strings.WinService_ConnectionString].ToStringSf();

            SQLWorker.Default.Error += OnError;
            m_TransferringWorker.Error += OnError;
            m_TransferringWorker.Error += TransferringWorker_OnError;

            m_TransferringWorker.SleepTime = TRANSFERRINGWORKER_PERIOD;

            SQLWorker.Default.Start(ThreadPriority.Lowest, "TransferringService.SQLWorker");
            m_TransferringWorker.Start(ThreadPriority.Lowest, "TransferringService.TransferringWorker");
        }

        protected override void OnStop()
        {
            //Wait SQL to finish
            SQLWorker.Default.Empty.WaitOne();
            SQLWorker.Default.Stop();

            SQLWorker.Default.Error -= OnError;
            m_TransferringWorker.Error -= OnError;
            m_TransferringWorker.Error -= TransferringWorker_OnError;
        }

        private static void OnError(object sender, ThreadExceptionEventArgs e)
        {
            Trace.WriteLine(e.Exception, "TransferringService: ERR");

            ClientDataAccess.InsertMessageAsync(e.Exception.Message, eSources.TransferringService,
                eMessageTypes.Error, e.Exception.StackTrace);
        }

        private void TransferringWorker_OnError(object sender, ThreadExceptionEventArgs e)
        {
            Trace.WriteLine("TransferringWorker_Error", "TransferringService: ERR");
        }
    }
}
