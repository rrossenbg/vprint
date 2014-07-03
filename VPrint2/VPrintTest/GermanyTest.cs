using System.Diagnostics;
using System.Globalization;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VPrinting.Documents;

namespace VPrintTest
{
    [TestClass]
    public class GermanyTest
    {
        static GermanyTest()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en");
            VoucherPrinter.Error += new ThreadExceptionEventHandler(VoucherPrinter_Error);
        }

        static void VoucherPrinter_Error(object sender, ThreadExceptionEventArgs e)
        {
            Debug.Fail(e.Exception.Message, e.Exception.ToString());
        }

        [TestMethod]
        public void germany_print_format_Type_1()
        {
            VoucherPrinter printer = new VoucherPrinter();
            printer.m_PrinterName = Printers.DELL;
            printer.m_ReportType2 = "VPrinting.Documents.VoucherPrintLayout276";
            printer.m_PrinterXmlFilePath = @"C:\PROJECTS\VPrint2\XmlConfigurations\print276_Type1.xml";
            printer.PrintAllocation(0, true);
        }

        [TestMethod]
        public void germany_print_format_Type_1_Raz()
        {
            VoucherPrinter printer = new VoucherPrinter();
            printer.m_PrinterName = Printers.Empty;
            printer.m_ReportType2 = "VPrinting.Documents.VoucherPrintLayoutRaz";
            printer.m_PrinterXmlFilePath = @"C:\PROJECTS\VPrint2\XmlConfigurations\print276_Type1_Raz.xml";
            printer.PrintAllocation(80882, false);
        }

        [TestMethod]
        public void germanySS_print_format_Type_1_Raz()
        {
            VoucherPrinter printer = new VoucherPrinter();
            printer.m_PrinterName = Printers.Empty;
            printer.m_ReportType2 = "VPrinting.Documents.VoucherPrintLayoutRaz";
            printer.m_PrinterXmlFilePath = @"C:\PROJECTS\VPrint2\XmlConfigurations\print905_Type1_Raz.xml";
            printer.PrintAllocation(377222, false);
        }

        [TestMethod]
        public void austria_print_format_Type_1_Raz()
        {
            VoucherPrinter printer = new VoucherPrinter();
            printer.m_PrinterName = Printers.Empty;
            printer.m_ReportType2 = "VPrinting.Documents.VoucherPrintLayoutRaz";
            printer.m_PrinterXmlFilePath = @"C:\PROJECTS\VPrint2\XmlConfigurations\print40_Type1_Raz.xml";
            printer.PrintAllocation(203931, false);
        }
    }
}
