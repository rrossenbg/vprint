using System;
using System.Configuration;
using System.Globalization;
using HobexCommonLib;

namespace HobexServer
{
    public class Config
    {
        public static string InDirName { get { return GetValue("INDIRNAME"); } }
        public static string OutDirName { get { return GetValue("OUTDIRNAME"); } }
        public static string ErrDirName { get { return GetValue("ERRDIRNAME"); } }
        public static string ExDirName { get { return GetValue("EXDIRNAME"); } }
        public static string FileEx { get { return GetValue("FILEEX"); } }
        public static string IgnoreTerminalIds { get { return GetValue("IGNORETERM"); } }

        public static string EmailList { get { return GetValue("EMAILLIST"); } }
        public static TimeSpan EmailAt { get { return TimeSpan.Parse(GetValue("EMAILAT")); } }
        public static string FROM { get { return GetValue("FROM"); } }
        public static string SUBJECT { get { return GetValue("SUBJECT"); } }
        public static string SMTP { get { return GetValue("SMTP"); } }
        public static int SMTP_PORT { get { return GetValue("SMTP_PORT").ConvertTo<string, int>("SMTP_PORT"); } }
        public static string EMAIL_PASS { get { return GetValue("EMAIL_PASS"); } }

        public static DateTime LastRunDate
        {
            get
            {
                return GetValue("LASTRUN").ConvertTo<string, DateTime>("LASTRUN");
            }
            set
            {
                SetValue("LASTRUN", value.ToString(CultureInfo.InvariantCulture));
            }
        }

        public static int AllocationsCount
        {
            get
            {
                return GetValue("COUNT").ConvertTo<string, int>("COUNT");
            }
            set
            {
                SetValue("COUNT", value.ToString(CultureInfo.InvariantCulture));
            }
        }

        private static string GetValue(string name)
        {
            lock (typeof(Config))
            {
                return ConfigurationManager.AppSettings[name];
            }
        }

        private static void SetValue(string name, string value)
        {
            lock (typeof(Config))
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                AppSettingsSection app = config.AppSettings;
                app.Settings.Remove(name);
                app.Settings.Add(name, value);
                config.Save(ConfigurationSaveMode.Modified);
            }
        }
    }
}