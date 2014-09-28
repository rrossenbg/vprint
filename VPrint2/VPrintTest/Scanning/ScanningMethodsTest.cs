using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VPrinting.Common;
using VPrinting.ScaningProcessors;
using System.Threading;
using VPrinting;
using System.Windows.Forms;
using System.IO;
using VPrint_372_addon;

namespace VPrintTest.Scanning
{
    [TestClass]
    public class ScanningMethodsTest
    {
        ManualResetEvent man = new ManualResetEvent(false);
        [TestMethod]
        public void VoucherWithSiteCodeAndNoDocumentProcessor_TEST()
        {
            StateManager stm = new StateManager();
            string fullPath = @"C:\IMAGES\IRELAND\20140910175441571.tif";
            StringTaskOrganizer proc = new StringTaskOrganizer(1);
            proc.Completed += new EventHandler<TaskProcessOrganizer<string>.CompletedEventArgs>(proc_Completed);
            proc.RunTask(new TaskProcessOrganizer<string>.TaskItem(fullPath, VoucherWithSiteCodeAndNoDocumentProcessor.Default.GetAction()));
            man.WaitOne(); 
        }

        void proc_Completed(object sender, TaskProcessOrganizer<string>.CompletedEventArgs e)
        {
            string str = e.Value;
        }

        [TestMethod]
        public void VoucherWithBarcodeAndTransferFileProcessor_TEST()
        {
            Class1 cl = new Class1();
            cl.Run();

            StateManager stm = new StateManager();
            string fullPath = @"C:\IMAGES\IRELAND\20140910175441571.tif";
            StringTaskOrganizer proc = new StringTaskOrganizer(1);
            proc.Completed += new EventHandler<TaskProcessOrganizer<string>.CompletedEventArgs>(proc_Completed);
            proc.RunTask(new TaskProcessOrganizer<string>.TaskItem(fullPath, VoucherWithBarcodeAndTransferFileProcessor.Default.GetAction()));
            man.WaitOne(); 
        }
    }
}
