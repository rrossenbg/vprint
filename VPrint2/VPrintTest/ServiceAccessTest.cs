using Microsoft.VisualStudio.TestTools.UnitTesting;
using VPrinting.Data;
using VPrinting;

namespace VPrintTest
{
    /// <summary>
    /// Summary description for ServiceAccessTest
    /// </summary>
    [TestClass]
    public class ServiceAccessTest
    {
        public ServiceAccessTest()
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
        public void receive_file()
        {
            var downloadDir = MainForm.GetAppSubFolder("DOWNLOAD");
            string fileName = ServiceDataAccess.Instance.ReceiveFile(130, true, false, downloadDir.FullName);
        }
    }
}
