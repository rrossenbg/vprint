using System;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PremierTaxFree.Data.Objects;
using PremierTaxFree.PTFLib;
using PremierTaxFree.PTFLib.Data;

namespace ScanTest
{
    [TestClass]
    public class ClientDataAccessTest
    {
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            ClientDataAccess.ConnectionString = ConfigurationManager.AppSettings[Strings.Scan_ConnectionString].ToStringSf();
            SQLWorker.Default.Start(ThreadPriority.Lowest, "SQLWorker");
        }

        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            SQLWorker.Default.Empty.WaitOne();
            SQLWorker.Default.Stop();
        }

        [TestMethod]
        public void OK_clientdataaccess_insertfile_test()
        {
            var file = new Bitmap("c:\\test.bmp");

            for (int i = 1; i < 10; i++)
            {
                Voucher voucher = new Voucher();
                voucher.CountryID = 826;
                voucher.RetailerID = 1234 + i;
                voucher.VoucherID = "test" + i;
                voucher.SiteCode = "AAH" + (500 + i);
                voucher.VoucherImage = file;
                voucher.BarCodeString = "test";
                voucher.BarCodeImage = file;
                ClientDataAccess.UpdateFile((DbClientVoucher)voucher);
#warning TODO
                //ClientDataAccess.UpdateFileAsync((DbClientVoucher)voucher, null, null);
            }
        }

        [TestMethod]
        public void clientdataaccess_selectfiles_test()
        {
        }

        public static void OnOK(object sender, EventArgs args)
        {
            Debug.WriteLine("OK");
        }

        private static void OnErr(object serder, ThreadExceptionEventArgs args)
        {
            Debug.Fail(args.Exception.Message);
        }
    }
}
