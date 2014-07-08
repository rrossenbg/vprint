/***************************************************
//  Copyright (c) Premium Tax Free 2012
***************************************************/

using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;
using PremierTaxFree.PTFLib.Threading;

namespace BtRetryService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            CycleWorkerBase.Error += OnError;
            Threading.Error += OnError;

            ServiceBase[] ServicesToRun = new ServiceBase[] { new FintraxRetryService() };
            ServiceBase.Run(ServicesToRun);
        }

        public static void OnError(object sender, ThreadExceptionEventArgs args)
        {
            Exception ex = args.Exception;
            LogSafe(ex.ToString(), EventLogEntryType.Error);
        }

        public static void LogSafe(string text, EventLogEntryType type)
        {
            if (Monitor.TryEnter(typeof(FintraxRetryService), 300))
            {
                try
                {
                    FintraxRetryService.Current.EventLog.WriteEntry(text, type);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex);
                }
                finally
                {
                    Monitor.Exit(typeof(FintraxRetryService));
                }
            }
        }
    }
}
