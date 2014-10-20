using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VPrinting;
using VPrinting.Common;
using VPrinting.Tools;

namespace VPrintTest.Scanning
{
    [TestClass]
    public class BarcodeParserTest
    {
        [TestMethod]
        public void scan_file_for_barcode_test_not_runnable()
        {
            MainForm.Default = new MainForm();
            string path = @"C:\Users\Rosen.rusev\Desktop\SCANNING\Spainish_Vouchers\bbva sample 2.jpg";
            var act = DelegateHelper.CreateScanAction();
            var item = new TaskProcessOrganizer<string>.TaskItem(path, act);
            item.Method.DynamicInvoke(item);
        }

        [TestMethod]
        public void test_a()
        {
            var v = string.Format("{0:000}{2:000000}{3:000000000}", 826, 1, 12345, 4567890);
        }

        [TestMethod]
        public void test_sitecode_parser()
        {
            var info = new FileInfo(@"C:\IMAGES\PD\PD149002.jpg");
            string site;
            int location;
            if (!VPrinting.Common.CommonTools.ParseSiteCode(info.Name, out site, out location))
                throw new Exception("Wrong sitecode");
        }
    }
}
