using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using PremierTaxFree.PTFLib;
using PremierTaxFree.PTFLib.Data;
using PremierTaxFree.PTFLib.Serialization;
using System.Threading;

namespace TestWinForm2
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            fileSystemWatcher1.Path = "M:\\Scans";
            fileSystemWatcher1.IncludeSubdirectories = true;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var g = Guid.NewGuid();
            var buffer = File.ReadAllBytes(@"C:\Users\Rosen.rusev\Desktop\Voucher1.png");
            ClientDataAccess.InsertBarcodeInfo(g, buffer);

            var p = Process.Start(@"C:\PROJECTS\VScan\BarReaderProc\bin\Debug\ReaderProc.exe", g.ToString());
            p.WaitForExit();
            var buffer2 = ClientDataAccess.SelectBarcodeInfoData(g);
            ClientDataAccess.DeleteBarcodeInfo(g);
            ObjectSerializer ser = new ObjectSerializer(true);
            var barArray = ser.Deserialize<BarcodeInfoArray>(buffer2);
        }

        private void button2_Click(object sender, EventArgs e)
        {
        }

        void watcher_Created(object sender, FileSystemEventArgs e)
        {
            
        }

        private void fileSystemWatcher1_Created(object sender, FileSystemEventArgs e)
        {
            var file = new FileInfo(e.FullPath);

            while (file.Exists && file.IsFileLocked())
                Thread.Sleep(1000);

            file.MoveTo("C:\\TEST\\".Unique().concat(".jpg"));
        }
    }
}
