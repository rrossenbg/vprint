/***************************************************
//  Copyright (c) Premium Tax Free 2012
***************************************************/

//DON'T REMOVE REFERENCES
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using BtRetryService.Razor.RazorTemplating;
using thread = System.Threading.Thread;
//DON'T REMOVE REFERENCES

namespace BtRetryService
{
    public partial class FintraxRetryService : ServiceBase
    {
        private const string PATHX = "In";
        private const string COMMANDFILE = "C:\\FintraxRetryService.txt";
        private readonly FileSystemWatcher m_Watcher = new FileSystemWatcher();
        private readonly RetryWorker2 m_RetryWorker = new RetryWorker2();
        private readonly EmailWorker m_EmailWorker = new EmailWorker();

        public static FintraxRetryService Current { get; set; }

        public enum TransferStatus
        {
            Received = 1,
            Debatched = 2,
            Archived = 3,
            Prioritised = 4,
            Imported = 5,
            Rejected = 6
        }

        public FintraxRetryService()
        {
            Current = this;

            InitializeComponent();

            this.AutoLog = true;

            //m_RetryWorker.Started += new EventHandler(RetryWorker_StartedOrCompleted);
            //m_RetryWorker.Completed += new EventHandler(RetryWorker_StartedOrCompleted);
            m_Watcher.Created += new FileSystemEventHandler(Watcher_Event);
            m_Watcher.Changed += new FileSystemEventHandler(Watcher_Event);
        }

        protected override void OnStart(string[] args)
        {
            var retryTimeout = ConfigurationManager.AppSettings[Strings.RetryTimeout].TryParse<TimeSpan>();
            if (!retryTimeout.HasValue)
                throw new SystemException("'Timeout' cannot be null or empty.");

            var inParsed = ConfigurationManager.AppSettings[Strings.BT_InParsed].Cast<string>();
            if (inParsed.IsNullOrEmpty())
                throw new SystemException("'BT_InParsed' cannot be null or empty.");

            m_RetryWorker.BT_InParsed = inParsed;

            string connStringTransferDb = ConfigurationManager.ConnectionStrings[Strings.TransferDb].ConnectionString;
            if (connStringTransferDb.IsNullOrEmpty())
                throw new SystemException("'TransferDb' cannot be null or empty.");

            TransferDBDataAccess.ConnectionString = connStringTransferDb;

            string connStringPtf = ConfigurationManager.ConnectionStrings[Strings.PTFDb].ConnectionString;
            if (connStringPtf.IsNullOrEmpty())
                throw new SystemException("'PTFDb' cannot be null or empty.");

            PTFDbDataAccess.ConnectionString = connStringPtf;

            var inPath = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "In"));
            if (inPath.Exists)
            {
                m_Watcher.Path = inPath.FullName;
                m_Watcher.EnableRaisingEvents = true;
            }

            if (ConfigurationManager.AppSettings[Strings.START_SERVICES].Contains("RetryWorker"))
            {
                m_RetryWorker.Init();
                m_RetryWorker.SleepTime = retryTimeout.Value;
                m_RetryWorker.Start(ThreadPriority.AboveNormal, "RetryWorker");
            }

            if (ConfigurationManager.AppSettings[Strings.START_SERVICES].Contains("EmailWorker"))
            {
                if (EmailAccountIsCorrect())
                {
                    var emailAt = ConfigurationManager.AppSettings[Strings.SendEmailsAt].TryParse<TimeSpan>().ThrowIfNull("SendEmailsAt should exists").Value;
                    m_EmailWorker.StartAt = emailAt;
                    m_EmailWorker.SleepTime = retryTimeout.Value;
                    m_EmailWorker.Start(ThreadPriority.AboveNormal, "EmailWorker");
                }
            }
        }

        protected override void OnStop()
        {
            m_RetryWorker.Stop();
            m_EmailWorker.Stop();
        }

        protected override void OnCustomCommand(int command)
        {
            if (command == 222)
            {
                m_EmailWorker.EmailMe = true;
                m_EmailWorker.Force = true;
                m_EmailWorker.RunOnce();
                m_EmailWorker.Force = false;
                m_EmailWorker.EmailMe = false;
            }
            else if (command > 128 && command < 256)
            {
                if (command % 2 == 0)
                {
                    Trace.WriteLine(command, "RETRY");
                    //Even Email
                    m_RetryWorker.RunOnce();
                }
                else
                {
                    Trace.WriteLine(command, "EML");
                    //Odd Retry
                    m_EmailWorker.Force = true;
                    try
                    {
                        if (command == 131 && File.Exists(COMMANDFILE))
                        {
                            var lines = File.ReadAllLines(COMMANDFILE);
                            foreach (var line in lines)
                            {
                                DateTime date;
                                if (DateTime.TryParse(line, out date))
                                {
                                    m_EmailWorker.ForceDate = date;
                                    m_EmailWorker.RunOnce();
                                }
                            }
                        }
                        else
                        {
                            m_EmailWorker.RunOnce();
                        }
                    }
                    finally
                    {
                        m_EmailWorker.Force = false;
                    }
                }
            }

            base.OnCustomCommand(command);
        }

        private void RetryWorker_StartedOrCompleted(object sender, EventArgs e)
        {
            var daysback = ConfigurationManager.AppSettings[Strings.RUNFORDAYSBACK].Cast<int>();
            for (int i = 0; i < daysback; i++)
                m_RetryWorker.RunForDate(DateTime.Today.AddDays(-i));
        }

        private void Watcher_Event(object sender, FileSystemEventArgs e)
        {
            Thread.Sleep(500);
            var file = new FileInfo(e.FullPath);
            if (file.Exists)
            {
                var lines = File.ReadAllLines(file.FullName);
                foreach (var line in lines)
                {
                    DateTime date;
                    if (DateTime.TryParse(line.TrimSafe(""), out date))
                        m_RetryWorker.RunForDate(date);
                }
            }
        }

        private bool EmailAccountIsCorrect()
        {
            string EXCHANGESERVER = ConfigurationManager.AppSettings[Strings.EXCHANGESERVER];
            string EXCHANGESERVER_DOMAIN = ConfigurationManager.AppSettings[Strings.EXCHANGESERVER_DOMAIN];
            string EXCHANGESERVER_USER = ConfigurationManager.AppSettings[Strings.EXCHANGESERVER_USER];
            string EXCHANGESERVER_PASS = ConfigurationManager.AppSettings[Strings.EXCHANGESERVER_PASS];
            string EXCHANGESERVER_FROM = ConfigurationManager.AppSettings[Strings.EXCHANGESERVER_FROM];

            return  !EXCHANGESERVER.IsNullOrEmpty() &&
                    !EXCHANGESERVER_DOMAIN.IsNullOrEmpty() &&
                    !EXCHANGESERVER_USER.IsNullOrEmpty() &&
                    !EXCHANGESERVER_PASS.IsNullOrEmpty() &&
                    !EXCHANGESERVER_FROM.IsNullOrEmpty();
        }
    }
}
