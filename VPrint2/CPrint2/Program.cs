/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using CPrint2.Common;
using CPrint2.Data;
using CPrint2.ScanServiceRef;

namespace CPrint2
{
    static class Program
    {
        public static Guid SessionId = Guid.NewGuid();
        /// <summary>
        /// 192.168.53.117
        /// </summary>
        public static string LIVE_IP = ConfigurationManager.AppSettings["LiveServerIP"];

        /// <summary>
        /// 192.168.58.59
        /// </summary>
        public static string TEST_IP = ConfigurationManager.AppSettings["TestServerIP"];
        public static string SCAN_IP = ConfigurationManager.AppSettings["ScanServerIP"];

        public static CurrentUser currentUser;
        public static bool IsDebug;
        public static bool IsAdmin;
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
            bool newInstance;
            using (Mutex mutex = new Mutex(false, "CPrint2", out newInstance))
            {
                if (newInstance)
                {
                    try
                    {
                        Config.CAMERAS = Tools.GetNumberOfCameras(Config.CAMERA_CAPTION);

                        ThreadPool.SetMaxThreads(50, 100);
                        Thread.CurrentThread.CurrentCulture =
                        Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-us");

                        AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
                        Application.ThreadException += new ThreadExceptionEventHandler(OnThreadException);
                        StartUp.TryToAddAppSafe();

                        Application.EnableVisualStyles();
                        Application.SetCompatibleTextRenderingDefault(false);

                        Global.FolderID = Config.FolderID;

                        var comDir = new DirectoryInfo(Config.CommandInputPath);

                        if (comDir.Exists())
                        {
                            comDir.DeleteSafe(true);
                            Thread.Sleep(Config.CommondFolderDeleteWait);
                        }

                        comDir.Refresh();

                        if (!comDir.Exists())
                        {
                            comDir.Create();
                            Thread.Sleep(Config.CommondFolderDeleteWait);
                        }

                        ImageProcessor.EmptyCommandFolderSafe();

                        ImageProcessor.EmptyImageFolderSafe();

                        var ctn = new AppContext();
                        ctn.NewCommandFileEvent += new EventHandler<ValueEventArgs<string>>(NewCommandFileEvent);
                        ctn.Error += new ThreadExceptionEventHandler(OnThreadException);

                        StateSaver.Error += new ThreadExceptionEventHandler(OnThreadException);
                        ImageProcessor.Error += new ThreadExceptionEventHandler(OnThreadException);
                        AppContext.Default.Error += new ThreadExceptionEventHandler(OnThreadException);

                        StateSaver.Default.Path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CPrint.dat");
                        StateSaver.Default.Load();
                        Application.Run(ctn);
                    }
                    finally
                    {
                        StateSaver.Default.Save();
                        Global.Instance.DisposeSf();
                    }
                }
                else
                {
                    MessageBox.Show("An instance is already started.\r\nPlease close it first.",
                        Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        public static void OnThreadException(object sender, ThreadExceptionEventArgs e)
        {
            Debug.Assert(e != null);
            Debug.Assert(e.Exception != null);
            Debug.Assert(e.Exception.Message != null);

            if (Monitor.TryEnter(typeof(Program), 500))
            {
                try
                {
                    Exception ex = e.Exception;
                    if (ex is ObjectDisposedException)
                        return;

                    new Action<Exception>((ee) => ServiceDataAccess.Instance.LogOperation(OperationHistory.Error, Program.SessionId, Program.currentUser.CountryID, 0, 0, 0, 0, ee.ToString())).FireAndForgetSafe(ex);
                    MainForm.Default.InvokeSf(() => ex.ShowDialog(MainForm.Default));
                }
                finally
                {
                    Monitor.Exit(typeof(Program));
                }
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            MainForm.Default.InvokeSf(() => MainForm.Default.ShowError(ex.Message));
        }

        private static void NewCommandFileEvent(object sender, ValueEventArgs<string> e)
        {
            ImageProcessor.Default.ProcessCommandFile(e.Value);
        }

#warning NOT IN USE
        private static void ImageProcessor_NewVoucherStarted(object sender, EventArgs e)
        {
            AppContext.Default.Reset();
        }

        private static void ImageProcessor_VoucherProcessCompleted(object sender, ValueEventArgs<string> e)
        {
            MainForm.Default.ShowImageAsynch(e.Value);
        }
    }

    static class TestProgram
    {
        [STAThread]
        static void Main()
        {
            Application.Run(new MultyCamForm());
        }
    }
}
