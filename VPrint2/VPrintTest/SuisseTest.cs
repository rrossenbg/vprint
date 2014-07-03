using System.Diagnostics;
using System.Globalization;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VPrinting.Documents;

namespace VPrintTest
{
    [TestClass]
    public class SuisseTest
    {
        static SuisseTest()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en");
            VoucherPrinter.Error += new ThreadExceptionEventHandler(VoucherPrinter_Error);
        }

        static void VoucherPrinter_Error(object sender, ThreadExceptionEventArgs e)
        {
            Debug.Fail(e.Exception.Message, e.Exception.ToString());
        }

        public SuisseTest()
        {

        }

        [TestMethod]
        public void suisse_print_format_Type_1()
        {
            VoucherPrinter printer = new VoucherPrinter();
            printer.m_PrinterName = Printers.Tally_T2365_2T;
            printer.m_ReportType2 = "VPrinting.Documents.VoucherPrintLayout250";
            printer.m_PrinterXmlFilePath = @"C:\PROJECTS\VPrint2\XmlConfigurations\print756_Type1.xml";
            printer.PrintAllocation(246244, false);
        }
    }
}
