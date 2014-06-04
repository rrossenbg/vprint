using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReceivingServiceLib;
using System.IO;
using System.Drawing;

namespace ReceivingServiceTest
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class PdfTest
    {
        public PdfTest()
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
        public void create_pdf_sign()
        {
            string[] files = Directory.GetFiles(@"C:\Users\Rosen.rusev\Pictures\Presenter");

            PdfManager manager = new PdfManager();

            string fileName = "C:\\test\\SES724-320377-0013491924-5.pdf";
            string signedFileName = "C:\\test\\SES724-320377-0013491924-5_Signed.pdf";

            using (var bmp = Image.FromFile("C:\\test\\SES724-320377-0013491924-5.jpg"))
            {
                manager.CreatePdf(fileName, new Image[] { bmp },
                    new PdfManager.CreationInfo()
                {
                    Title = "Voucher SES724-320377-0013491924-5",
                    Subject = "Retailer 320377",
                    Author = "PTF Spain",
                    Creator = "PTF Spain"
                });
            }

            manager.SignPdfFile(
                fileName,
                signedFileName,
            new PdfManager.SignInfo()
            {
                pfxFilePath = @"C:\PROJECTS\VPrint2\ReceivingServiceLib.Common\PTF.pfx",
                pfxKeyPass = "",
                docPass = null,
                signImagePath = @"C:\PROJECTS\VPrint2\ReceivingServiceLib.Common\Resources\PTFLogo.jpg",
                reasonForSigning = "Voucher SES724-320377-0013491924-5",
                location = "Madrid, Spain"
            });
        }

        [TestMethod]
        public void tiff_GetAllPages()
        {
            string fileName = @"C:\Users\Rosen.rusev\Pictures\Presenter\New folder (3)\826_152948_380202463.tif";
            using (Bitmap bitmap = (Bitmap)Image.FromFile(fileName))
            {
                var list = bitmap.GetAllPages(System.Drawing.Imaging.ImageFormat.Png);
            }
        }
    }
}
