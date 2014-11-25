using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using VCover.Common;
using VCover.Data;
using VPrinting;
using VPrinting.Common;
using System.ServiceModel;
using VPrinting.Communication;

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

                        StartUp.TryToAddAppSafe();

                        Application.EnableVisualStyles();
                        Application.SetCompatibleTextRenderingDefault(false);

                        AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
                        Application.ThreadException += new ThreadExceptionEventHandler(OnThreadException);

                        var ctn = new AppContext();
                        ctn.NewInFileEvent += new EventHandler<ValueEventArgs<Guid, string>>(NewCommandFileEvent);
                        ctn.Started += new EventHandler(ctn_Started);

                        StateSaver.Error += new ThreadExceptionEventHandler(OnThreadException);
                        AppContext.Default.Error += new ThreadExceptionEventHandler(OnThreadException);

                        StateSaver.Default.Path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "VCover.dat");
                        StateSaver.Default.Load();

                        TemplateMatcher.Path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TemplateMatcher.dat");
                        TemplateMatcher.Load();
                        
                        NamedPipes.ReceivedData += new ReceivedDataDelegate(NamedPipes_ReceivedData);
                        NamedPipes.Error += new ThreadExceptionEventHandler(OnThreadException);
                        NamedPipes.StartServer("VCOVER");
                        Application.Run(ctn);
                    }
                    finally
                    {
                        StateSaver.Default.Save();
                        TemplateMatcher.Save();
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
            new DirectoryInfo(Config.OUT_FOLDER).ClearSafe();
        }

        private static void NewCommandFileEvent(object sender, ValueEventArgs<Guid, string> e)
        {
            Task.Factory.StartNew((o) =>
            {
                var r = (ValueEventArgs<Guid, string>)o;
                FileInfo file = new FileInfo(r.Value2);
                try
                {
                    using (var matcher = new TemplateMatcher(file.FullName))
                    {
                        if (!matcher.MatchTemplate())
                        {
                            if (!MatchForm.Run(file.FullName))
                            {
                                AppContext.Default.SaveResult(r.Value1, "");
                                return;
                            }
                        }

                        if (matcher.MatchTemplate())
                        {
                            var @out = new DirectoryInfo(Config.OUT_FOLDER);
                            @out.EnsureDirectory();

                            var imgcc = @out.CombineFileName(string.Concat(file.GetFileNameWithoutExtension(), "_ccimg", file.Extension));
                            matcher.PixellateHiddenAreasAndSaveUnderArea(imgcc.FullName);

                            var img = @out.CombineFileName(string.Concat(file.GetFileNameWithoutExtension(), "_cc", file.Extension));
                            matcher.SaveResult(img.FullName);

                            AppContext.Default.SaveResult(r.Value1, imgcc.FullName, img.FullName);
                        }
                        else
                        {
                            throw new ApplicationException("Cannot match template");
                        }
                    }
                }
                catch (Exception ex)
                {
                    AppContext.Default.FireError(ex);
                }
            }, e, TaskCreationOptions.LongRunning);
        }

        private static string NamedPipes_ReceivedData(string data)
        {
            if (string.IsNullOrWhiteSpace(data))
                return string.Empty;

            //Start
            var strings = data.FromStr();
            var id = Guid.Parse(strings[0]);
            var filePath = strings[1];

            AppContext.Default.NewImage(id, filePath);
            return string.Empty;
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