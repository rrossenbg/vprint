/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime;
using System.Threading;
using FintraxServiceManager.Common;

namespace FintraxServiceManager
{
    public static class Ex
    {
        [TargetedPatchingOptOut("na")]
        public static void WriteEntrySf(this EventLog log, string message)
        {
            if (log != null)
            {
                if (Monitor.TryEnter(log, 1000))
                {
                    try
                    {
                        log.WriteEntry(message);
                    }
                    catch
                    {
                        // do nothing
                    }
                    finally
                    {
                        Monitor.Exit(log);
                    }
                }
            }
        }

        [TargetedPatchingOptOut("na")]
        public static void WriteEntrySf(this EventLog log, string message, EventLogEntryType type)
        {
            if (log != null)
            {
                if (Monitor.TryEnter(log, 1000))
                {
                    try
                    {
                        log.WriteEntry(message, type);
                    }
                    catch
                    {
                        // do nothing
                    }
                    finally
                    {
                        Monitor.Exit(log);
                    }
                }
            }
        }

        [TargetedPatchingOptOut("na")]
        public static CString ToCString<T>(this T obj)
        {
            CString cs = Convert.ToString(obj);
            return cs;
        }

        [TargetedPatchingOptOut("na")]
        public static string GetFolder(this string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("path");

            return Path.GetDirectoryName(path);
        }
    }
}