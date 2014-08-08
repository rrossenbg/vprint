/***************************************************
//  Copyright (c) Premium Tax Free 2012
/***************************************************/

using System;
using System.Configuration;
using System.Diagnostics;
using System.Reflection;
using System.ServiceModel;
using System.ServiceProcess;
using System.Threading;
using SiteCodeLib;

namespace SiteCodeSrvc
{
    [Obfuscation(ApplyToMembers = true)]
    public partial class FintraxSiteCodeService : ServiceBase
    {
        private static DateTime ms_date = DateTime.Parse("2014-12-27");
        private SaveThread m_SaveThread;
        private ServiceHost m_ServerHost;
        private SiteCodeObject m_Server = new SiteCodeObject();

        public FintraxSiteCodeService()
        {
            InitializeComponent();
            AutoLog = true;
            SaveThread.Save += new EventHandler(Server_SaveCommand);
            m_Server.Save += new EventHandler(Server_SaveCommand);
        }

        [Obfuscation]
        protected override void OnStart(string[] args)
        {
            //if (!Debugger.IsAttached)
            //    Debugger.Launch();

            EventLog.WriteEntry("Start", EventLogEntryType.Information);

            DataAccess.ConnectionString = ConfigurationManager.AppSettings["DBConn"];

            TimeSpan addtime;
            if (TimeSpan.TryParse(ConfigurationManager.AppSettings["AdditionalTime"], out addtime))
                base.RequestAdditionalTime(addtime.TotalMilliseconds.Get<int>());

            var lookupLocations = DataAccess.LoadLocationsFromLocations();
            //var voucherPartLocations = DataAccess.LoadLocationsFromVoucherPart();
            //var locations = DataAccess.JoinLocations(voucherPartLocations, lookupLocations);

            if (DateTime.Now > ms_date)
                throw new Exception("Out of service");

            m_Server.SetLocations(lookupLocations);
            m_Server.SetCountries(DataAccess.LoadCountries());

            m_ServerHost = new ServiceHost(m_Server);
            m_ServerHost.Open();

            SaveThreadStop();

            m_SaveThread = new SaveThread();
            m_SaveThread.SleepTime = TimeSpan.Parse(ConfigurationManager.AppSettings["SleepTime"]);
            m_SaveThread.Start(ThreadPriority.BelowNormal, "SaveThread");

            Trace.WriteLine("It's loaded successfully", Strings.SRVNAME);
            EventLog.WriteEntry("It's loaded successfully", EventLogEntryType.Information);
        }

        [Obfuscation]
        protected override void OnStop()
        {
            EventLog.WriteEntry("Stop", EventLogEntryType.Information);

            TimeSpan addtime;
            if (TimeSpan.TryParse(ConfigurationManager.AppSettings["AdditionalTime"], out addtime))
                base.RequestAdditionalTime(addtime.TotalMilliseconds.Get<int>());

            SaveThreadStop();
            SaveAndStopHost();
        }

        [Obfuscation]
        protected override void OnShutdown()
        {
            EventLog.WriteEntry("Shutdown", EventLogEntryType.Information);
            SaveThreadStop();
            SaveAndStopHost();
        }

        [Obfuscation]
        private void Server_SaveCommand(object sender, EventArgs e)
        {
            try
            {
                EventLog.WriteEntry("SaveCommand", EventLogEntryType.Information);
                Trace.WriteLine("Server_SaveCommand", Strings.SRVNAME);

                DataAccess.SaveLocations(m_Server.GetLocations());
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex, Strings.SRVNAME);
                EventLog.WriteEntry(ex.ToString(), EventLogEntryType.Error);
            }
        }

        [Obfuscation]
        private void SaveThreadStop()
        {
            if (m_SaveThread != null)
            {
                m_SaveThread.Stop();
                m_SaveThread = null;
            }
        }

        [Obfuscation]
        private void SaveAndStopHost()
        {
            try
            {
                EventLog.WriteEntry("Save", EventLogEntryType.Information);

                if (m_ServerHost != null)
                {
                    m_ServerHost.Close();
                    m_ServerHost = null;
                }

                DataAccess.SaveLocations(m_Server.GetLocations());
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex, Strings.SRVNAME);
                EventLog.WriteEntry(ex.ToString(), EventLogEntryType.Error);
            }
        }
    }
}
