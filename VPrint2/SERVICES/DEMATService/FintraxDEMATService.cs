/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using DEMATLib;
using DEMATLib.Data;
using DEMATLib.Dior;
using TTimer = System.Timers.Timer;

namespace DEMATService
{
    public partial class FintraxDEMATService : ServiceBase
    {
        private TTimer m_DimatExportTimer;
        private readonly DIMATExportManager m_DimatExportManager = new DIMATExportManager();

        public FintraxDEMATService()
        {
            InitializeComponent();
            this.AutoLog = true;
            DIMATExportManager.Error += new ThreadExceptionEventHandler(OnError);
            DiorExportProcessor.Error += new ThreadExceptionEventHandler(OnError);
            diorFileSystemWatcher.Created += new System.IO.FileSystemEventHandler(DiorFileSystemWatcher_Created);
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                DEMARDataAccess.ConnectionString = config.AppSettings.Settings["ConnectionString"].Value;
                DIMATExportManager.Iso = config.AppSettings.Settings["Iso"].Value.Cast<int>();
                DIMATExportManager.ExportDirectory = config.AppSettings.Settings["ExportDirectory"].Value;

                DiorExportProcessor.ExportDirectory = ConfigurationManager.AppSettings["DiorExportDirectory"];

                DiorObjDataAccess.ReportsConnectionString =
                    DiorDataAccess.ReportsConnectionString =
                        ConfigurationManager.AppSettings["DiorPTF_ReportsConnectionString"];

                DiorDataAccess.PTFConnectionString =
                    ConfigurationManager.AppSettings["DiorPTF_PTFConnectionString"];

                var sdate = config.AppSettings.Settings["LastRun"].Value;
                var date = sdate.IsNullOrWhiteSpace() ? DateTime.MinValue : sdate.Cast<DateTime>();
                m_DimatExportManager.FirstRun = (date != DateTime.Now.Date);

                var time = config.AppSettings.Settings["ExportEveryInMunutes"].Value.Cast<int>();
                var dtime = TimeSpan.FromMinutes(time).TotalMilliseconds;  // 30'000 milliseconds = 30 seconds

                m_DimatExportTimer = new TTimer(dtime);
                m_DimatExportTimer.Elapsed += new ElapsedEventHandler(DimatTimer_Elapsed);
                m_DimatExportTimer.AutoReset = true;
                m_DimatExportTimer.Start();

                config.AppSettings.Settings["LastRun"].Value = DateTime.Now.Date.ToString();
                config.Save(ConfigurationSaveMode.Modified);

                diorFileSystemWatcher.Path = ConfigurationManager.AppSettings["DiorTriggerWatchPath"];
                diorFileSystemWatcher.EnableRaisingEvents = true;
            }
            catch (Exception ex)
            {
                OnError(this, new ThreadExceptionEventArgs(ex));
                throw;
            }
        }

        protected override void OnStop()
        {
            m_DimatExportTimer.Stop();
            m_DimatExportTimer.Elapsed -= new ElapsedEventHandler(DimatTimer_Elapsed);
            diorFileSystemWatcher.EnableRaisingEvents = false;
        }

        protected override void OnCustomCommand(int command)
        {
            if (command == 222)
            {
                DIMATExportManager.FirceRetailerExport();
            }
            else if(command > 128 && command < 256)
            {
                Trace.WriteLine(command, Strings.DEMAT);
                m_DimatExportManager.Start();
            }

            base.OnCustomCommand(command);
        }

        private void DimatTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            m_DimatExportTimer.Stop();
            try
            {
                m_DimatExportManager.Start();
            }
            finally
            {
                m_DimatExportTimer.Start();
            }
        }

        private void DiorFileSystemWatcher_Created(object sender, FileSystemEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                var hostr = ConfigurationManager.AppSettings["DiorHOs"];
                var hos = HeadOffice.ParseList(hostr);
                var proc = new DiorExportProcessor(hos);
                proc.Run();

            }, TaskCreationOptions.LongRunning);

            Thread.Sleep(1000);

            if (File.Exists(e.FullPath))
                File.Delete(e.FullPath);
        }

        private void OnError(object sender, ThreadExceptionEventArgs e)
        {
            if (Monitor.TryEnter(this, 200))
            {
                var str = e.Exception.ToString();
                try
                {
                    File.AppendAllText("F:\\LOGGER\\Log.txt", str);
                    this.EventLog.WriteEntry(str, EventLogEntryType.Error);
                }
                catch(Exception ex)
                {
                    string source = (sender != null && sender.GetType() == typeof(DiorExportProcessor)) ? Strings.DIOR : Strings.DEMAT;
                    Trace.WriteLine(str, source);
                }
                finally
                {
                    Monitor.Exit(this);
                }
            }
        }
    }
}
