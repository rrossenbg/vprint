using System;
using System.Threading;
using System.Windows.Forms;
using PremierTaxFree.PTFLib.Data;

namespace WinDbLst
{
    static class Program
    {
        /// <summary>
        /// Data Source=.\SQL1;Initial Catalog=PTFVoucher;Integrated Security=SSPI
        /// </summary>
        public const string strCONNSTR = @"Data Source=.;Initial Catalog=PTFImage;Integrated Security=SSPI;";
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            SQLWorker.Default.Start(ThreadPriority.Normal, "SQLWorker");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
            SQLWorker.Default.Empty.WaitOne();
            SQLWorker.Default.Stop();
        }
    }
}
