/***************************************************
//  Copyright (c) Premium Tax Free 2012
/***************************************************/

using System;
using System.Diagnostics;
using System.IO;

namespace BackupRestore
{
    public class EventLogHelper
    {
        public static void WriteInfo(string message)
        {
            var elog = OpenOrCreateEventSource();
            elog.WriteEntry(message, EventLogEntryType.Information);
        }

        public static void WriteInfo(string format, params object[] values)
        {
            var elog = OpenOrCreateEventSource();
            elog.WriteEntry(string.Format(format, values), EventLogEntryType.Information);
        }

        public static void WriteWarning(string message)
        {
            var elog = OpenOrCreateEventSource();
            elog.WriteEntry(message, EventLogEntryType.Warning);
        }

        public static void WriteError(Exception ex)
        {
            var elog = OpenOrCreateEventSource();
            elog.WriteEntry(ex.ToString(), EventLogEntryType.Error);
        }

        private static EventLog OpenOrCreateEventSource()
        {
            string appName = Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]);

            EventLog elog = new EventLog();
            elog.Source = appName;
            elog.EnableRaisingEvents = true;

            if (!EventLog.SourceExists(appName))
                EventLog.CreateEventSource(appName, appName);
            return elog;
        }
    }
}
