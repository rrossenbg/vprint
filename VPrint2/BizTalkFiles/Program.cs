namespace BizTalkFiles
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;
    using System.Windows.Forms;

    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            bool newInstance;
            using (Mutex mutex = new Mutex(false, "BizTalkFiles", out newInstance))
            {
                if (newInstance)
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.ThreadException += new ThreadExceptionEventHandler(OnException);
                    Application.Run(new MainForm());
                }
                else
                {
                    MessageBox.Show("An instance is already started.\r\nPlease close it first.",
                        Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        public static void OnException(object sender, ThreadExceptionEventArgs e)
        {
            Exception ex = e.Exception;
            Trace.TraceError(ex.ToString());

            lock (typeof(File))
            {
                string fileName = "{0}{1:dd_MM_yyyy}.log".format(Application.ExecutablePath, DateTime.Now);
                try
                {
                    using (var file = File.AppendText(fileName))
                    {
                        file.WriteLine("ERR");
                        file.WriteLine(DateTime.Now);
                        file.WriteLine();
                        file.WriteLine(ex.ToString());
                        file.WriteLine();
                        file.WriteLine("====================================================");
                    }
                }
                catch { }
            }
        }
    }
}

