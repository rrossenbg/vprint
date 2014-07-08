using System;
using System.IO;
using DEMATLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DEMATTest
{
    [TestClass]
    public class Test_BDV_InvoiceBuilder
    {
        public Test_BDV_InvoiceBuilder()
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
        public void test_BDV_InvoiceBuilder_serialization()
        {
            var b = new BDV_InvoiceBuilder(false);

            b.SetBuyer("test1", "test2", "add1", "add2", "add3", "city", "code", "123456");
            b.SetError("Test error message");

            b.SetRetailer(123, "rname1", "rname2", "addr1", "addr2", "addr3", "city", "rpcode", "rvar");
            b.SetInvoiceTotal(100);
            b.SetTotalPerVAT(1.1m, 2.2m, 3.3m, 4.4m);
            b.SetVoucherDetails(12345672, DateTime.Now);

            var name = b.CreateFileName(0);

            for (int i = 0; i < 3; i++)
                b.SetVoucherLine(i.ToString(), string.Format("{0} line {0}", i), 2.2m, "2", 1.1m, 2.2m, 3.3m, 4.4m);

            var xml = b.CreateXML();
            File.WriteAllText("C:\\text.xml", xml);
        }
    }
}
