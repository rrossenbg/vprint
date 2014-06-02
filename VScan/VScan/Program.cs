/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Configuration;
using System.Globalization;
using System.Reflection;
using System.Security;
using System.Threading;
using System.Windows.Forms;
using PremierTaxFree.PTFLib;
using PremierTaxFree.PTFLib.Collections;
using PremierTaxFree.PTFLib.Data;
using PremierTaxFree.PTFLib.Net;
using PremierTaxFree.PTFLib.Printing;
using PremierTaxFree.Utils;

namespace PremierTaxFree
{
    static class Program
    {
        public static bool InDebug { get; set; }
        /// <summary>
        /// 100
        /// </summary>
        public const int ITEMS_IN_CACHE = 100;
        //private static readonly PipeServer m_PipeServer = new PipeServer(Strings.VScan_PipeChannelName);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);
#if DEBUG
            InDebug = true;
#endif
            //Register application exception handler
            ThreadBase.Error += OnError;
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            Application.ThreadException += OnError;
            SQLWorker.Default.Error += OnError;
            SettingsTable.Default.Error += OnError;

            ClientDataAccess.ConnectionString = ConfigurationManager.AppSettings[Strings.Scan_ConnectionString].ToStringSf();

#if !INSTALL_SRV
            //Try start stransffering service. WARNING: Should be installed.
            UIUtils.TryStartTransferringServiceAsync();
#endif
            //Load settings
            SettingsTable.Default.Read();

            //Zero day counter
            if(SettingsTable.Get<DateTime>(Strings.VScan_LastStarted, DateTime.Now).Date < DateTime.Now.Date)
                SettingsTable.Set(Strings.VScan_ScanCount, 0);

            SettingsTable.Get<UniqueList<UserAuth>>(Strings.Transferring_AuthObjectList, 
                new UniqueList<UserAuth>(new UserAuthComparer()) { UserAuth.DefaultAdmin, UserAuth.Rosen, UserAuth.Default });

            Thread.CurrentThread.CurrentUICulture = SettingsTable.Get<CultureInfo>(Strings.Transferring_CurrentUICultureInfo, new CultureInfo("en"));

            ScanAppContext.QueryCountryesAsync();

            //Set last started. 
            SettingsTable.Set(Strings.VScan_LastStarted, DateTime.Now);
            //Start sql pool
            SQLWorker.Default.Start(ThreadPriority.Lowest, "VScan.SQLWorker");            

            //Show splash
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SplashScreen());

            //Start pipe server to the tranferring service
            ScanAppContext.Default = new ScanAppContext(new MainForm());

            //Load SiteIDs
            DelegateUtils.CreateAuditIdReloadDelegate().FireAndForget();

            //Set application printer
            //If no printer is set this will set default printer as application printer
            string defaultPrinterName = PrintManager.GetDefaultPrinterName();
            SettingsTable.Get(Strings.VScan_ApplicationPrinterName, defaultPrinterName);            

            //Start application
            Application.Run(ScanAppContext.Default);
            
            //Stop pipe server
            //Wait pool empty. Stop it.
            SQLWorker.Default.Empty.WaitOne();
            SQLWorker.Default.Stop();
            //Save settings
            SettingsTable.Default.Save();

            //Unregister application error handler
            ThreadBase.Error -= OnError;
            Application.ThreadException -= OnError;
            SQLWorker.Default.Error -= OnError;
            SettingsTable.Default.Error -= OnError;
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            OnError(sender, new ThreadExceptionEventArgs(e.ExceptionObject as Exception));
        }

        public static void OnError(object sender, ThreadExceptionEventArgs e)
        {
            //Every exception is recordered but only application predefined 
            //exceptions are shown in this scope
            Exception ex = e.Exception;

            //The exception comes from the ThreadPool dynamic Invoker.
            //The real exception is wrapped into it. Get it.
            if (ex is TargetInvocationException && ex.InnerException != null)
                ex = ex.InnerException;

            if (ex is ThreadAbortException || ex is ThreadInterruptedException)
                return;

            MessageBoxIcon icon = MessageBoxIcon.Error;
            if (e.Exception is AppInfoException)
                icon = MessageBoxIcon.Information;
            else if (ex is AppWarningException)
                icon = MessageBoxIcon.Warning;
            else if (ex is AppExclamationException)
                icon = MessageBoxIcon.Exclamation;
            else if (ex is AppStopException)
                icon = MessageBoxIcon.Stop;

            if (!(ex is ApplicationException))
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, icon);

            //TODO: eMessageTypes.Error should be described more precisely
            ClientDataAccess.InsertMessageAsync(ex.Message, eSources.VScan, 
                eMessageTypes.Error, ex.StackTrace);

            Delegate nextDelegate = ex.GetNext();
            if (nextDelegate != null && nextDelegate != default(Delegate))
            {
                nextDelegate.FireAndForget();
            }

            if (ex is AppStopException || ex is SecurityException)
                Environment.Exit(10);
        }
    }
}
