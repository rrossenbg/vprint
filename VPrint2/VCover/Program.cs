using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using VCover.Common;
using VCover.Data;
using VPrinting;
using VPrinting.Common;
using Emgu.CV.Structure;
using Emgu.CV;

namespace VCover
{
    static class Program
    {
        public static bool IsAdmin { get; set; }
        public static CurrentUser currentUser { get; set; }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool newInstance;
            using (Mutex mutex = new Mutex(false, "VCover", out newInstance))
            {
                if (newInstance)
                {
                    try
                    {
                        ThreadPool.SetMaxThreads(50, 100);
                        Thread.CurrentThread.CurrentCulture =
                        Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-us");

                        Application.EnableVisualStyles();
                        Application.SetCompatibleTextRenderingDefault(false);

                        AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
                        Application.ThreadException += new ThreadExceptionEventHandler(OnThreadException);

                        new DirectoryInfo(Config.IN_FOLDER).EnsureDirectory();
                        new DirectoryInfo(Config.SUCCESS_FOLDER).EnsureDirectory();
                        new DirectoryInfo(Config.FAILURE_FOLDER).EnsureDirectory();

                        var ctn = new AppContext();
                        ctn.InFolder = Config.IN_FOLDER;
                        ctn.InFilter = Config.ImageFileFilter;
                        ctn.NewInFileEvent += new EventHandler<ValueEventArgs<string>>(NewCommandFileEvent);
                        ctn.Started += new EventHandler(ctn_Started);

                        StateSaver.Error += new ThreadExceptionEventHandler(OnThreadException);
                        AppContext.Default.Error += new ThreadExceptionEventHandler(OnThreadException);

                        StateSaver.Default.Path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CPrint.dat");

                        StateSaver.Default.Load();
                        Application.Run(ctn);
                    }
                    finally
                    {
                        StateSaver.Default.Save();
                    }
                }
                else
                {
                    MessageBox.Show("An instance is already started.\r\nPlease close it first.",
                        Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        static void ctn_Started(object sender, EventArgs e)
        {
            new DirectoryInfo(Config.IN_FOLDER).ClearSafe();
        }

        private static void NewCommandFileEvent(object sender, ValueEventArgs<string> e)
        {
            Task.Factory.StartNew((o) =>
            {
                FileInfo file = new FileInfo((string)o);
                try
                {
                    using (var matcher = new TemplateMatcher(file.FullName))
                    {
                        if (matcher.MatchTemplate())
                        {
                            var dir = file.Directory.Combine(file.GetFileNameWithoutExtension());
                            dir.EnsureDirectory();

                            var imgcc = dir.CombineFileName(string.Concat(file.GetFileNameWithoutExtension(), "_cc", file.Extension));
                            matcher.PixellateHiddenAreas(imgcc.FullName);

                            var img = dir.CombineFileName(file.Name);
                            matcher.Save(img.FullName);
                            file.DeleteSafe();
                        }
                        else
                        {
                            //string imageFullFilePath = Path.Combine(Config.FAILURE_FOLDER, );
                            //file.MoveTo(imageFullFilePath);
                            MatchForm.Run(file.FullName);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                finally
                {
                    
                }
            }, e.Value, TaskCreationOptions.LongRunning);
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

                    //new Action<Exception>((ee) => ServiceDataAccess.Instance.LogOperation(OperationHistory.Error, Program.SessionId, Program.currentUser.CountryID, 0, 0, 0, 0, ee.ToString())).FireAndForgetSafe(ex);
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
    }

    static class ProgramTest
    {
        [STAThread]
        static void Main()
        {
            var form = new MatchForm();
            form.Image = new Image<Bgr, byte>(@"C:\IMAGES\PB\PB742001.jpg");
            Application.Run(form);
        }
    }
}