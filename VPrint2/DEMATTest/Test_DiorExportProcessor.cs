using DEMATLib.Data;
using DEMATLib.Dior;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DEMATTest
{
    /// <summary>
    /// Summary description for Test_DiorExportProcessor
    /// </summary>
    [TestClass]
    public class Test_DiorExportProcessor
    {
        public Test_DiorExportProcessor()
        {
            DiorObjDataAccess.ReportsConnectionString =
            DiorDataAccess.ReportsConnectionString = "data source=192.168.58.27;initial catalog=PTF_Reports; Integrated Security = true; packet size=4096; Max Pool Size=75; Min Pool Size=5;";
            DiorExportProcessor.ExportDirectory = "C:\\TEST";
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
        public void test_diorexportprocessor()
        {
            var hos = HeadOffice.ParseList("250, 123171; 250, 131969; 250, 137261; 250, 139245;");
            DiorExportProcessor p = new DiorExportProcessor(hos);
            p.Run();
        }

        [TestMethod]
        public void test_nullable()
        {
            int? v = 1;
            bool result = Convert.ToBoolean(v);
        }
    }
}
