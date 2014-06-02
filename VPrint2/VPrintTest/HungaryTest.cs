using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VPrinting.Documents;

namespace VPrintTest
{
    /// <summary>
    /// Summary description for HungaryTest
    /// </summary>
    [TestClass]
    public class HungaryTest
    {
        static HungaryTest()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en");
            VoucherPrinter.Error += new ThreadExceptionEventHandler(VoucherPrinter_Error);
        }

        static void VoucherPrinter_Error(object sender, ThreadExceptionEventArgs e)
        {
            Debug.Fail(e.Exception.Message, e.Exception.ToString());
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
        public void hungary_print_format_TypeX_1()
        {
            VoucherPrinter printer = new VoucherPrinter();
            printer.UseLocalFormat = true;
            printer.UseLocalPrinter = true;
            printer.PrintOnce = true;
            printer.m_PrinterName = Printers.DELL;
            printer.m_ReportType2 = "VPrinting.Documents.VoucherPrintLayoutRazX";
            printer.m_PrinterXmlFilePath = @"C:\PROJECTS\VPrint\XmlConfigurations\print348_Type1_RazX.xml";
            printer.SimulatePrint = false;
            printer.PrintAllocation(440972, new List<int>() { 454247 });

            ////454247
            ////HO 100020
            ////r	100020
        }


        [TestMethod]
        public void hungary_print_format_Type_1()
        {
            VoucherPrinter printer = new VoucherPrinter();
            printer.UseLocalFormat = true;
            printer.m_PrinterName = Printers.DELL;
            printer.m_ReportType2 = "VPrinting.Documents.VoucherPrintLayoutRaz";
            printer.m_PrinterXmlFilePath = @"C:\PROJECTS\VPrint\XmlConfigurations\print348_Type1_Raz.xml";
            printer.PrintAllocation(401177, false);
        }

        [TestMethod]
        public void hungaryForFrance_print_format_Type_1()
        {
            VoucherPrinter printer = new VoucherPrinter();
            printer.UseLocalFormat = true;
            printer.UseLocalPrinter = true;
            printer.PrintOnce = true;
            printer.m_PrinterName = Printers.DELL;
            printer.m_ReportType2 = "VPrinting.Documents.VoucherPrintLayoutRazX";
            printer.m_PrinterXmlFilePath = @"C:\PROJECTS\VPrint\XmlConfigurations\print348_Type2_RazX.xml";
            printer.SimulatePrint = true;
            printer.PrintAllocation(454247, false);
        }

        [TestMethod]
        public void hungary_print_format_TypeX_1testing()
        {
            VoucherPrinter printer = new VoucherPrinter();
            printer.UseLocalFormat = true;
            printer.UseLocalPrinter = true;
            printer.PrintOnce = false;
            printer.m_PrinterName = Printers.DELL;
            printer.m_ReportType2 = "VPrinting.Documents.VoucherPrintLayoutRazX";
            printer.m_PrinterXmlFilePath = @"C:\PROJECTS\VPrint\XmlConfigurations\print348_Type1_RazX.xml";
            printer.SimulatePrint = true;
            printer.PrintAllocation(440972, new List<int>() { 110 });
        }
    }
}
