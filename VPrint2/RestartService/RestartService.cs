using System;
using System.Configuration;
using System.Diagnostics;
using System.ServiceProcess;
using System.Timers;

namespace RestartService
{
    /// <summary>
    /// C:\Windows\Microsoft.NET\Framework\v4.0.30319\installutil "C:\PROJECTS\VPrint\RestartService\bin\Debug\RestartService.exe" -i
    /// </summary>
    public partial class RestartService : ServiceBase
    {
        private readonly Timer m_Timer = new Timer();

        public RestartService()
        {
            InitializeComponent();
            m_Timer.Enabled = false;
            m_Timer.Elapsed += new ElapsedEventHandler(Timer_Elapsed);
        }

        protected override void OnStart(string[] args)
        {
            Configuration cfg = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var interval = TimeSpan.Parse(cfg.AppSettings.Settings["Interval"].Value);
            var value = cfg.AppSettings.Settings["StartedAt"].Value;
            var date = string.IsNullOrWhiteSpace(value) ? DateTime.MinValue : Convert.ToDateTime(value);
            cfg.AppSettings.Settings["StartedAt"].Value = Convert.ToString(DateTime.Now.Date);
            cfg.Save(ConfigurationSaveMode.Modified);

            if (date == DateTime.Now.Date)
                StopMachine();

            m_Timer.Interval = interval.TotalMilliseconds;
            m_Timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            StopMachine();
        }

        private void StopMachine()
        {
            Process.Start("shutdown", "/s /t 0");
        }
    }
}
