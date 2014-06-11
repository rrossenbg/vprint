/***************************************************
//  Copyright (c) Premium Tax Free 2012
/***************************************************/

using System;
using System.Diagnostics;

namespace PTF.Reports.Common
{
    public class EventLogLogger
    {
        public void WriteInfo(string message)
        {
            var elog = OpenOrCreateEventSource();
            elog.WriteEntry(message, EventLogEntryType.Information);
        }

        public void WriteInfo(string format, params object[] values)
        {
            var elog = OpenOrCreateEventSource();
            elog.WriteEntry(string.Format(format, values), EventLogEntryType.Information);
        }

        public void WriteWarning(string message)
        {
            var elog = OpenOrCreateEventSource();
            elog.WriteEntry(message, EventLogEntryType.Warning);
        }

        public void WriteError(Exception ex)
        {
            var elog = OpenOrCreateEventSource();
            elog.WriteEntry(ex.ToString(), EventLogEntryType.Error);
        }

        private static EventLog OpenOrCreateEventSource()
        {
            EventLog elog = new EventLog();
            elog.Source = Strings.APPNAME;
            elog.EnableRaisingEvents = true;

            if (!EventLog.SourceExists(Strings.APPNAME))
                EventLog.CreateEventSource(Strings.APPNAME, Strings.APPNAME);
            return elog;
        }
    }
}
