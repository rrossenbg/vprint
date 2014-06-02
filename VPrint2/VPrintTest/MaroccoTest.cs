using System.Diagnostics;
using System.Globalization;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VPrinting.Documents;

namespace VPrintTest
{
    [TestClass]
    public class MaroccoTest
    {
        static MaroccoTest()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en");
            VoucherPrinter.Error += new ThreadExceptionEventHandler(VoucherPrinter_Error);
        }

        static void VoucherPrinter_Error(object sender, ThreadExceptionEventArgs e)
        {
            Debug.Fail(e.Exception.Message, e.Exception.ToString());
        }

        [TestMethod]
        public void marocco_print_format_Type_1()
        {
            VoucherPrinter printer = new VoucherPrinter();
            printer.m_PrinterName = @"\\192.168.44.158\della52fe3-p";
            printer.m_ReportType2 = "VPrinting.Documents.VoucherPrintLayout276";
            printer.m_PrinterXmlFilePath = @"C:\PROJECTS\VPrint\XmlConfigurations\print504_Type1.xml";
            printer.PrintAllocation(0, true);
        }
    }
}
