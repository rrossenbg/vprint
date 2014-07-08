using Microsoft.VisualStudio.TestTools.UnitTesting;
using DEMATLib.Data;
using System.Collections.Generic;

namespace DEMATTest
{
    /// <summary>
    /// Summary description for Test_NoSql
    /// </summary>
    [TestClass]
    public class Test_NoSql
    {
        public Test_NoSql()
        {
            DiorObjDataAccess.ReportsConnectionString = "data source=192.168.58.27;initial catalog=PTF_Reports; Integrated Security = true; packet size=4096; Max Pool Size=75; Min Pool Size=5;";
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
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
        public void nosql_insert_test()
        {
            var list = new List<Voucher>()
            {
                new Voucher() {IsoId = 826, BrId =1, VId = 1},
                new Voucher() {IsoId = 826, BrId =1, VId = 2},
                new Voucher() {IsoId = 826, BrId =1, VId = 3},

                new Voucher() {IsoId = 826, BrId =2, VId = 1},
                new Voucher() {IsoId = 826, BrId =2, VId = 2},

                new Voucher() {IsoId = 250, BrId =1, VId = 1},
                new Voucher() {IsoId = 250, BrId =1, VId = 2},
                new Voucher() {IsoId = 250, BrId =1, VId = 3},

                new Voucher() {IsoId = 250, BrId =2, VId = 1},
                new Voucher() {IsoId = 250, BrId =2, VId = 2},
            };

            DiorObjDataAccess.InsertVouchers(list);
        }

        [TestMethod]
        public void nosql_select_test()
        {
            var list1 = DiorObjDataAccess.SelectVouchersPerRetailer(250, 1);
            Assert.IsTrue(list1.Count == 3);

            var list2 = DiorObjDataAccess.SelectVouchersPerRetailer(250, 2);
            Assert.IsTrue(list2.Count == 2);

            var list3 = DiorObjDataAccess.SelectVouchersPerRetailer(826, 1);
            Assert.IsTrue(list3.Count == 3);

            var list4 = DiorObjDataAccess.SelectVouchersPerRetailer(826, 2);
            Assert.IsTrue(list4.Count == 2);
        }

        [TestMethod]
        public void nosql_delete_test()
        {
            var list1 = new List<Voucher>()
            {
                new Voucher() {IsoId = 826, BrId =1, VId = 1},
                new Voucher() {IsoId = 826, BrId =1, VId = 2},
            };
            DiorObjDataAccess.DeleteVouchers(826, list1);

            var list2 = DiorObjDataAccess.SelectVouchersPerRetailer(826, 1);
            Assert.IsTrue(list2.Count == 1);
            DiorObjDataAccess.DeleteVouchers(826, list2);

            var list3 = DiorObjDataAccess.SelectVouchersPerRetailer(826, 1);
            Assert.IsTrue(list3.Count == 0);
        }
    }
}
