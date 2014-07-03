using System.Diagnostics;
using System.Globalization;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VPrinting.Documents;
using System;

namespace VPrintTest
{
    [TestClass]
    public class PrintUnitsTest
    {
        static PrintUnitsTest()
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

        [TestMethod]
        public void printUnits_format_Type_1()
        {
            VoucherPrinter printer = new VoucherPrinter();
            printer.UseLocalFormat = true;
            printer.UseLocalPrinter = true;
            printer.PrintOnce = true;
            printer.SimulatePrint = true;
            printer.m_PrinterName = Printers.DELL;
            printer.m_ReportType2 = "VPrinting.Documents.VoucherPrintLayoutUnitRazX";
            printer.m_PrinterXmlFilePath = @"C:\PROJECTS\VPrint2\XmlConfigurations\printUnits.xml";
            printer.PrintAllocation(77493, false);
        }

        [TestMethod]
        public void datetime_Type_1()
        {
            var date = DateTime.Now.ToBinary();
        }
    }
}
