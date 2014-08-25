using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using CardCodeCover;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;

namespace CardCodeCoverTest
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class CardCodeCover
    {
        public CardCodeCover()
        {
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
        public void test_creditCard_cover()
        {
            const string DIR = @"C:\IMAGES\PB";
            const string RESULT_DIR = @"C:\TEST6";
            const string TEMP = @"C:\IMAGES\PB\B\PB742007_cover2.jpg";

            using (var logger = new FileLogger(@"C:\TEST6\Log.txt", true))
            using (Bitmap tmp = (Bitmap)Image.FromFile(TEMP))
            using (Bitmap gtmp = tmp.ToGrayScale())
            foreach (string file in Directory.GetFiles(DIR, "*.jpg"))
            {
                logger.WriteLine("===============================");
                Stopwatch w = Stopwatch.StartNew();
                using (Bitmap bmp = (Bitmap)Image.FromFile(file))
                using (Bitmap gbmp = bmp.ToGrayScale())
                {
                    string resultName = Path.Combine(RESULT_DIR, Path.GetFileName(file));
                    var result = gbmp.Match(gtmp);
                    logger.WriteLine(resultName);
                    logger.WriteLine(result.Length.ToString());
                    logger.WriteLine(w.Elapsed.ToString());

                    bmp.DrawMatches(result);
                    bmp.Save(resultName, ImageFormat.Jpeg);
                }
            }
        }
    }
}
