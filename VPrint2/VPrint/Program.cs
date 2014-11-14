/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using VPrinting.Common;
using VPrinting.Data;
using VPrinting.Documents;
using VPrinting.Extentions;
using VPrinting.ScanServiceRef;
using VPrinting.Tools;
using VPrinting.Loggers;

namespace VPrinting
{    
    static class Program
    {
        /// <summary>
        /// 1000
        /// </summary>
        public static readonly int ITEMS_SHOWN = ConfigurationManager.AppSettings["ITEMSSHOWN"].Cast<int>(500);
        public static Guid SessionId = Guid.NewGuid();
        /// <summary>
        /// 192.168.53.117
        /// </summary>
        public static readonly string LIVE_IP = ConfigurationManager.AppSettings["LiveServerIP"];

        /// <summary>
        /// 192.168.58.59
        /// </summary>
        public static readonly string TEST_IP = ConfigurationManager.AppSettings["TestServerIP"];
        public static readonly string SCAN_URL = string.Concat("net.tcp://", ConfigurationManager.AppSettings["ScanServerIP"], ":8080/ReceivingServiceLib.ScanService");
        //"192.168.53.143";"127.0.0.1";

        public static CurrentUser currentUser = new CurrentUser(1, "NA", 286);
        public static bool IsDebug;
        public static bool IsAdmin;

        private static PluginLoader loader = new PluginLoader();

        [STAThread]
        static void Main()
        {
#if DEBUG
            //#warning COMMENTED_CODE_HERE!!!
            //            SetTestServiceUrls();
            //          IsAdmin = true;

            IsDebug = true;
#else
            IsAdmin = false;
            IsDebug = false;
#endif
            ThreadPool.SetMaxThreads(50, 100);

            Thread.CurrentThread.CurrentCulture =
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-us");

            Application.ThreadException += new ThreadExceptionEventHandler(OnThreadException);
            DelegateBase.Error += new ThreadExceptionEventHandler(OnThreadException);
            VoucherPrinter.Error += new ThreadExceptionEventHandler(OnThreadException);
            PrinterQueue.Error += new ThreadExceptionEventHandler(OnThreadException);
            DelegateHelper.Error += new ThreadExceptionEventHandler(OnThreadException);
            Global.Error += new ThreadExceptionEventHandler(OnThreadException);

            StateSaver.Default.Path = Path.ChangeExtension(Application.ExecutablePath, "dat");
            StateSaver.Default.Load();

            PluginLoader.Error += new ThreadExceptionEventHandler(OnThreadException);
            string path = Path.GetDirectoryName(Application.ExecutablePath);
            
            loader.Process(path, PluginLoader.Operation.Start);
            StateSaver.Default.Set(Strings.ClearScanDirectory, ConfigurationManager.AppSettings["ClearScanDirectory"].Cast<bool>());

            Speeker.Enabled = ConfigurationManager.AppSettings["SPEAK"].Cast<bool>();

            StateSaver.Default.Set(Strings.VERSION, Assembly.GetEntryAssembly().GetName());

#if ! DEBUG
            if (ConfigurationManager.AppSettings["USE_SCAN_SERVER"].Cast<bool>())
            {
                if (!File.Exists(ConfigurationManager.AppSettings["ScanServerPath"]))
                    MessageBox.Show("Cannot find scan server.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                else
                    ConfigurationManager.AppSettings["ScanServerPath"].StartProcessSafe();
            }
#endif
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            FormLogin frmLogin = new FormLogin();
            if (frmLogin.ShowDialog() == DialogResult.OK)
                Application.Run(new MainForm());

            StateSaver.Default.Save();
            DelegateHelper.Close();

            new Action(() =>
            {
                var downloadDir = MainForm.GetAppSubFolder(Strings.DOWNLOAD);
                downloadDir.Delete(true);
            }).RunSafe();

            Global.Instance.DisposeSf();
            loader.Process(path, PluginLoader.Operation.Stop);
        }

        public static void OnThreadException(object sender, ThreadExceptionEventArgs e)
        {
            Debug.Assert(e != null);
            Debug.Assert(e.Exception != null);
            Debug.Assert(e.Exception.Message != null);

            Exception ex = e.Exception;
            if (Global.Instance.ExitSignal && ex is ObjectDisposedException)
                return;

#if DEBUGGER
            Trace.WriteLine(ex, Strings.VRPINT);
#endif
            new Action<Exception>((ee) => ServiceDataAccess.Instance.LogOperation(OperationHistory.Error, Program.SessionId, Program.currentUser.CountryID, 0, 0, 0, 0, ee.ToString())).FireAndForgetSafe(ex);
            Speeker.SpeakAsynchSf(ex.Message);
#if !USE_LOGGER
            FileLogger.LogError(ex, Strings.VRPINT);
#endif

            FileInfoApplicationException ex2 = e.Exception as FileInfoApplicationException;
            if (ex2 != null)
                MainForm.Default.InvokeSf(() => { FileMsgForm.show(MainForm.Default, "Error", ex2.Message, ex2.Info); });
            else
                MainForm.Default.InvokeSf(() => ex.ShowDialog(MainForm.Default));
        }

#if DEBUG
        private static void SetTestServiceUrls()
        {
        // UseTEST Server
            string url1 = global::VPrinting.Properties.Settings.Default.VoucherAllocationPrinting_Authentication_Authentication;
            global::VPrinting.Properties.Settings.Default.VoucherAllocationPrinting_Authentication_Authentication =
                url1.Replace(LIVE_IP, TEST_IP);

            string url2 = global::VPrinting.Properties.Settings.Default.VoucherAllocationPrinting_PartyManagement_PartyManagement;
            global::VPrinting.Properties.Settings.Default.VoucherAllocationPrinting_PartyManagement_PartyManagement =
                url2.Replace(LIVE_IP, TEST_IP);

            string url3 = global::VPrinting.Properties.Settings.Default.VoucherAllocationPrinting_VoucherNumberingAllocationPrinting_VoucherNumberingAllocationPrinting;
            global::VPrinting.Properties.Settings.Default.VoucherAllocationPrinting_VoucherNumberingAllocationPrinting_VoucherNumberingAllocationPrinting =
                url3.Replace(LIVE_IP, TEST_IP);
        }
#endif
    }
}
