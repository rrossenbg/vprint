﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FintraxPTFImages;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Threading;
using FintraxPTFImages.Common;

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
            "C:\\826_332562_3482739521092dce6_56db_454d_99e1_5250a6c3a8bc.tif".TiffGetAllImages2(ImageFormat.Jpeg);
        }

        [TestMethod]
        public void GuidTest()
        {

        }

        [TestMethod]
        public void test_barcode()
        {
            BarcodeDecoder.Run();
            new BarcodeDecoder().Test();
        }
    }
}
