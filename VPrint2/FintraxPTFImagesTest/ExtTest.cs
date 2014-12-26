using System.Diagnostics;
using System.Drawing.Imaging;
using FintraxPTFImages;
using FintraxPTFImages.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FintraxPTFImagesTest
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class ExtTest
    {
        public ExtTest()
        {
            //
            // TODO: Add constructor logic here
            //
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
        public void TestMethod1()
        {
            var s = "C:\\TEST\\UAA_022.jpg";
            string t = s.GetContentType();
        }

        [TestMethod]
        public void ConvertTiffToJpgTest()
        {
            "C:\\IMAGES\\UK\\test_result93b02365_9cbb_4597_9112_e0577e91be46.tif".TiffGetAllImages2(ImageFormat.Jpeg, true);
        }

        [TestMethod]
        public void TestHouseOfFrazer()
        {
            BarcodeData data = null;
            HouseOfFrazerBarcodeConfig cfg = new HouseOfFrazerBarcodeConfig();
            if (cfg.ParseBarcode("39236318582620123638", ref data))
            {
                Debug.Write("OK");
            }
        }

        [TestMethod]
        public void test_barcode()
        {
            BarcodeDecoder.Run();
            new BarcodeDecoder().Test();
        }
    }
}
