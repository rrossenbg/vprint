using System.Diagnostics;
using System.Globalization;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VPrinting.Documents;

namespace VPrintTest
{
    [TestClass]
    public class BelgiumTest
    {
        static BelgiumTest()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en");
            VoucherPrinter.Error += new ThreadExceptionEventHandler(VoucherPrinter_Error);
        }

        static void VoucherPrinter_Error(object sender, ThreadExceptionEventArgs e)
        {
            Debug.Fail(e.Exception.Message, e.Exception.ToString());
        }

        public BelgiumTest()
        {

        }

        [TestMethod]
        public void belgium_print_format_Type_1()
        {
            VoucherPrinter printer = new VoucherPrinter();
            printer.m_PrinterName = Printers.AJ_RAVI_ROOM;
            printer.m_ReportType2 = "VPrinting.Documents.VoucherPrintLayout250";
            printer.m_PrinterXmlFilePath = @"C:\PROJECTS\VPrint2\XmlConfigurations\print56_Type1.xml";
            printer.PrintOnce = true;
            printer.UseLocalPrinter = true;
            printer.UseLocalFormat = false;
            printer.SimulatePrint = true;
            printer.PrintAllocation(246244, false);
        }
    }
}
