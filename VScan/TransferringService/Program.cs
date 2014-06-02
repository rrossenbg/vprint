/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/

using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;

namespace TransferringService
{
    static class Program
    {
        public static bool IsDebug;

        /// <summary>
        /// 100
        /// </summary>
        public const int MINIMUM_SITECODE_IDS_COUNT = 100;
        /// <summary>
        /// 1
        /// </summary>
        public const int PURGE_RECORDS_OLDER_THAN_DAYS = 1;
        /// <summary>
        /// 10
        /// </summary>
        public const int MAXIMUM_FILES_FOR_EXPORT = 10;
        /// <summary>
        /// 1000
        /// </summary>
        public const int MAXIMUM_MESSAGES_FOR_EXPORT = MAXIMUM_FILES_FOR_EXPORT * 5;

        /// <summary>
        /// 1
        /// </summary>
        public const int START_EXPORTMESSAGES_HOUR = 1;

        /// <summary>
        /// 7
        /// </summary>
        public const int END_EXPORTMESSAGES_HOUR = 7;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
#if DEBUG
            IsDebug = true;
#endif
            ServiceBase[] servicesToRun = new ServiceBase[] { new XmlTransferringService() };
            ServiceBase.Run(servicesToRun);                   
        }

        public static void OnError(object sender, ThreadExceptionEventArgs args)
        {
            Debug.Fail(args.Exception.Message);
        }
    }
}
