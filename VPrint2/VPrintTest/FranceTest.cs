using System.Diagnostics;
using System.Globalization;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VPrinting.Documents;
using VPrinting;

namespace VPrintTest
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class FranceTest
    {
        static FranceTest()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en");
            VoucherPrinter.Error += new ThreadExceptionEventHandler(VoucherPrinter_Error);
        }

        static void VoucherPrinter_Error(object sender, ThreadExceptionEventArgs e)
        {
            Debug.Fail(e.Exception.Message, e.Exception.ToString());
        }

        public FranceTest()
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
        public void france_print_format_Type_1()
        {
            VoucherPrinter printer = new VoucherPrinter();
            printer.m_PrinterName = Printers.FRANCE;
            printer.m_ReportType2 = "VPrinting.Documents.VoucherPrintLayout250";
            printer.m_PrinterXmlFilePath = @"C:\PROJECTS\VPrint\XmlConfigurations\print250_Type1.xml";
            printer.PrintOnce = true;
            printer.UseLocalFormat = true;
            printer.UseLocalPrinter = true;
            printer.SimulatePrint = false;
            printer.PrintAllocation(358828, false);
        }

        [TestMethod]
        public void france_print_format_Type_2()
        {
            VoucherPrinter printer = new VoucherPrinter();
            printer.m_PrinterName = Printers.DELL;
            printer.m_ReportType2 = "VPrinting.Documents.VoucherPrintLayout250";
            printer.m_PrinterXmlFilePath = @"C:\PROJECTS\VPrint\XmlConfigurations\print250_Type2.xml";
            printer.PrintOnce = true;
            printer.UseLocalFormat = true;
            printer.UseLocalPrinter = true;
            printer.SimulatePrint = false;
            printer.PrintAllocation(358828, false);
        }

        [TestMethod]
        public void france_print_format_Type_3()
        {
            VoucherPrinter printer = new VoucherPrinter();
            printer.m_PrinterName = Printers.DELL;
            printer.m_ReportType2 = "VPrinting.Documents.VoucherPrintLayout250";
            printer.m_PrinterXmlFilePath = @"C:\PROJECTS\VPrint\XmlConfigurations\print250_Type3.xml";
            printer.PrintOnce = true;
            printer.UseLocalFormat = true;
            printer.UseLocalPrinter = true;
            printer.SimulatePrint = false;
            printer.PrintAllocation(358828, false);
        }

        [TestMethod]
        public void france_print_format_Type_4()
        {
            VoucherPrinter printer = new VoucherPrinter();
            printer.m_PrinterName = Printers.DELL;
            printer.m_ReportType2 = "VPrinting.Documents.VoucherPrintLayout250";
            printer.m_PrinterXmlFilePath = @"C:\PROJECTS\VPrint\XmlConfigurations\print250_Type4.xml";
            printer.PrintOnce = true;
            printer.UseLocalFormat = true;
            printer.UseLocalPrinter = true;
            printer.SimulatePrint = false;
            printer.PrintAllocation(358828, false);
        }

        [TestMethod]
        public void france_print_format_Type_5()
        {
            VoucherPrinter printer = new VoucherPrinter();
            printer.m_PrinterName = Printers.DELL;
            printer.m_ReportType2 = "VPrinting.Documents.VoucherPrintLayout250";
            printer.m_PrinterXmlFilePath = @"C:\PROJECTS\VPrint\XmlConfigurations\print250_Type5.xml";
            printer.PrintOnce = true;
            printer.UseLocalFormat = true;
            printer.UseLocalPrinter = true;
            printer.SimulatePrint = false;
            printer.PrintAllocation(358828, false);
        }

        [TestMethod]
        public void france_print_format_Type_6()
        {
            VoucherPrinter printer = new VoucherPrinter();
            printer.m_PrinterName = Printers.DELL;
            printer.m_ReportType2 = "VPrinting.Documents.VoucherPrintLayout250";
            printer.m_PrinterXmlFilePath = @"C:\PROJECTS\VPrint\XmlConfigurations\print250_Type6.xml";
            printer.PrintOnce = true;
            printer.UseLocalFormat = true;
            printer.UseLocalPrinter = true;
            printer.SimulatePrint = false;
            printer.PrintAllocation(358828, false);
        }

        [TestMethod]
        public void france_print_format_Type_7()
        {
            VoucherPrinter printer = new VoucherPrinter();
            printer.m_PrinterName = Printers.DELL;
            printer.m_ReportType2 = "VPrinting.Documents.VoucherPrintLayout250";
            printer.m_PrinterXmlFilePath = @"C:\PROJECTS\VPrint\XmlConfigurations\print250_Type7.xml";
            printer.PrintOnce = true;
            printer.UseLocalFormat = true;
            printer.UseLocalPrinter = true;
            printer.SimulatePrint = false;
            printer.PrintAllocation(358828, false);
        }

        [TestMethod]
        public void france_print_format_Type_1Unx()
        {
            VoucherPrinter printer = new VoucherPrinter();
            printer.m_PrinterName = Printers.DELL;
            printer.m_ReportType2 = "VPrinting.Documents.VoucherPrintLayoutUnitRazX";
            printer.m_PrinterXmlFilePath = @"C:\PROJECTS\VPrint\XmlConfigurations\print250_Type1Unx.xml";
            printer.PrintOnce = true;
            printer.UseLocalFormat = true;
            printer.UseLocalPrinter = true;
            printer.SimulatePrint = false;
            printer.PrintAllocation(358828, false);
        }

        private void TestCreateDocumentModelForType_1Unx()
        {
            IDataProvider Model = null;
            var voucherNumber = Model.VoucherNo + Model.Printing.CalculateCheckDigit(Model.VoucherNo);

            var barcodeNumber = Model.StrVoucherNo.Replace(" ", "").Substring(3);
            var barcodeText = Model.StrVoucherNo.Replace(" ", "");

            //Retailer
            var id = Model.Retailer.Id;
            var ShopName = Model.Retailer.TradingName.EscapeXml();
            var rLine1 = Model.Retailer.RetailAddress.Line1.EscapeXml();
            var rLine2 = Model.Retailer.RetailAddress.Line2.EscapeXml();
            var rLine3 = Model.Retailer.RetailAddress.Line3.EscapeXml();
            var rLine5 = Model.Retailer.RetailAddress.Line5.EscapeXml();

            var rPhone = Model.Retailer.Phone.EscapeXml();
            var rVAT = Model.Retailer.VatNumber.EscapeXml();

            var ShopName2 = string.Format("* {0} **************", ShopName);
            var Line1Line5Line3 = string.Format("* {0} {1} {2}  *", rLine1, rLine5, rLine3);
        }

        [TestMethod]
        public void france_print_format_Type_2Unx()
        {
            VoucherPrinter printer = new VoucherPrinter();
            printer.m_PrinterName = Printers.DELL;
            printer.m_ReportType2 = "VPrinting.Documents.VoucherPrintLayoutUnitRazX";
            printer.m_PrinterXmlFilePath = @"C:\PROJECTS\VPrint\XmlConfigurations\print250_Type2Unx.xml";
            printer.PrintOnce = true;
            printer.UseLocalFormat = true;
            printer.UseLocalPrinter = true;
            printer.SimulatePrint = false;
            printer.PrintAllocation(358828, false);
        }

        [TestMethod]
        public void france_print_format_Type_3Unx()
        {
            VoucherPrinter printer = new VoucherPrinter();
            printer.m_PrinterName = Printers.DELL;
            printer.m_ReportType2 = "VPrinting.Documents.VoucherPrintLayoutUnitRazX";
            printer.m_PrinterXmlFilePath = @"C:\PROJECTS\VPrint\XmlConfigurations\print250_Type3Unx.xml";
            printer.PrintOnce = true;
            printer.UseLocalFormat = true;
            printer.UseLocalPrinter = true;
            printer.SimulatePrint = false;
            printer.PrintAllocation(358828, false);
        }

        [TestMethod]
        public void france_print_format_Type_4Unx()
        {
            VoucherPrinter printer = new VoucherPrinter();
            printer.m_PrinterName = Printers.DELL;
            printer.m_ReportType2 = "VPrinting.Documents.VoucherPrintLayoutUnitRazX";
            printer.m_PrinterXmlFilePath = @"C:\PROJECTS\VPrint\XmlConfigurations\print250_Type4Unx.xml";
            printer.PrintOnce = true;
            printer.UseLocalFormat = true;
            printer.UseLocalPrinter = true;
            printer.SimulatePrint = false;
            printer.PrintAllocation(358828, false);
        }

        [TestMethod]
        public void france_print_format_Type_5Unx()
        {
            VoucherPrinter printer = new VoucherPrinter();
            printer.m_PrinterName = Printers.DELL;
            printer.m_ReportType2 = "VPrinting.Documents.VoucherPrintLayoutUnitRazX";
            printer.m_PrinterXmlFilePath = @"C:\PROJECTS\VPrint\XmlConfigurations\print250_Type5Unx.xml";
            printer.PrintOnce = true;
            printer.UseLocalFormat = true;
            printer.UseLocalPrinter = true;
            printer.SimulatePrint = false;
            printer.PrintAllocation(358828, false);
        }

        [TestMethod]
        public void france_print_format_Type_6Unx()
        {
            VoucherPrinter printer = new VoucherPrinter();
            printer.m_PrinterName = Printers.DELL;
            printer.m_ReportType2 = "VPrinting.Documents.VoucherPrintLayoutUnitRazX";
            printer.m_PrinterXmlFilePath = @"C:\PROJECTS\VPrint\XmlConfigurations\print250_Type6Unx.xml";
            printer.PrintOnce = true;
            printer.UseLocalFormat = true;
            printer.UseLocalPrinter = true;
            printer.SimulatePrint = false;
            printer.PrintAllocation(358828, false);
        }

        [TestMethod]
        public void france_print_format_Type_7Unx()
        {
            VoucherPrinter printer = new VoucherPrinter();
            printer.m_PrinterName = Printers.DELL;
            printer.m_ReportType2 = "VPrinting.Documents.VoucherPrintLayoutUnitRazX";
            printer.m_PrinterXmlFilePath = @"C:\PROJECTS\VPrint\XmlConfigurations\print250_Type7Unx.xml";
            printer.PrintOnce = true;
            printer.UseLocalFormat = true;
            printer.UseLocalPrinter = true;
            printer.SimulatePrint = false;
            printer.PrintAllocation(358828, false);
        }
    }
}
