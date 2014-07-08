using System.Linq;
using HobexCommonLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HeaderNP = HobexCommonLib.VoucherNP.AuthenticationHeader;

namespace HobexTest
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class ProcessTest
    {
        public ProcessTest()
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
        public void hobex_process_test()
        {
            var header = new HeaderNP();
            header.Init();

            FileAccessClass faccess = new FileAccessClass();
            faccess.Prepare("G:\\", Enumerable.Empty<int>());
            faccess.Process(header, @"C:\TEST1\POSHostRequest_.xml", @"C:\TEST2\", @"C:\TEST3\");
            faccess.Process(header, @"C:\TEST1\POSHostRequest_.xml", @"C:\TEST2\", @"C:\TEST3\");
        }
    }
}
