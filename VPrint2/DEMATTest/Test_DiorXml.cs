using System;
using System.Diagnostics;
using DEMATLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DEMATTest
{
    [TestClass]
    public class Test_DiorXml
    {
        public Test_DiorXml()
        {
        }

        private TestContext testContextInstance;

        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void test_diorxmlbuilder()
        {
            var b = new DiorXmlBuilder();

            var h = new DiorXmlBuilder.VoucherHeader();
            h.DocumentDate = DateTime.Now;
            h.PremierStoreId = 123;
            h.StoreCountryCode = 826;
            h.StoreId = 123;
            b.AddHeader(h);

            var s = new DiorXmlBuilder.VoucherStatus();
            s.VoucherNumber = 1234567;
            s.PremierStoreId = 123;
            s.BdvDate = DateTime.Now.AddDays(2);
            s.ClaimedDate = DateTime.Now.AddDays(1);
            s.CountryCode = 826;
            s.DebitDate = DateTime.Now.AddDays(2);
            s.DebitRejectedDate = DateTime.Now.AddDays(3);
            s.ErrorCode = "Error 1";
            s.FactureP1Date = DateTime.Now;
            s.FactureP2Date = DateTime.Now;
            s.RefundedDate = DateTime.Now;
            s.StampedDate = DateTime.Now;
            s.TimeStamp = DateTime.Now;
            s.VoidedDate = DateTime.Now; 

            for (int i = 0; i < 3; i++)
                b.AddStatus(s);

            b.Close();
            Debug.WriteLine(b.ToString());
        }

        const string DBNAME = "diordb4";

        //[TestMethod]
        //public void test_dior_save_nosql()
        //{
        //    DiorNonSqlDataAccess.Open("C:\\TEST", DBNAME);

        //    var lst = new List<Voucher>();
        //    lst.Add(new Voucher() { BrId = 1, IsoId = 1, VId = 1 });
        //    lst.Add(new Voucher() { BrId = 1, IsoId = 1, VId = 2 });
        //    lst.Add(new Voucher() { BrId = 1, IsoId = 1, VId = 3 });
        //    DiorNonSqlDataAccess.InsertVouchers(lst);

        //    DiorNonSqlDataAccess.Close();
        //}

        //[TestMethod]
        //public void test_dior_read_nosql()
        //{
        //    DiorNonSqlDataAccess.Open("C:\\TEST", DBNAME);

        //    var list = DiorNonSqlDataAccess.SelectVouchersPerRetailer(1, 1);
        //    Assert.IsTrue(list.Count == 3);

        //    var list2 = DiorNonSqlDataAccess.SelectVouchersPerRetailer(1, 2);
        //    Assert.IsTrue(list2.Count == 0);

        //    DiorNonSqlDataAccess.Close();
        //}

        //[TestMethod]
        //public void test_dior_delete_nosql()
        //{
        //    DiorNonSqlDataAccess.Open("C:\\TEST", DBNAME);

        //    var lst = new List<Voucher>();
        //    lst.Add(new Voucher() { BrId = 1, IsoId = 1, VId = 1 });
        //    lst.Add(new Voucher() { BrId = 1, IsoId = 1, VId = 2 });

        //    DiorNonSqlDataAccess.DeleteVouchers(lst);

        //    var list2 = DiorNonSqlDataAccess.SelectVouchersPerRetailer(1, 1);
        //    Assert.IsTrue(list2.Count == 1);

        //    DiorNonSqlDataAccess.DeleteVouchers(list2);
        //    var list3 = DiorNonSqlDataAccess.SelectVouchersPerRetailer(1, 1);
        //    Assert.IsTrue(list3.Count == 1);

        //    DiorNonSqlDataAccess.Close();
        //}
    }
}
