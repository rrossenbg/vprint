using System.Diagnostics;
using System.Globalization;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VPrinting.Documents;

namespace VPrintTest
{
    [TestClass]
    public class HollandTest
    {
        static HollandTest()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en");
            VoucherPrinter.Error += new ThreadExceptionEventHandler(VoucherPrinter_Error);
        }

        static void VoucherPrinter_Error(object sender, ThreadExceptionEventArgs e)
        {
            Debug.Fail(e.Exception.Message, e.Exception.ToString());
        }

        [TestMethod]
        public void holland_print_format_Type_1()
        {
            VoucherPrinter printer = new VoucherPrinter();
            printer.UseLocalFormat = false;
            printer.UseLocalPrinter = true;
            printer.PrintOnce = true;
            printer.m_PrinterName = Printers.DELL;
            printer.m_ReportType2 = "VPrinting.Documents.VoucherPrintLayoutRaz";
            printer.m_PrinterXmlFilePath = @"C:\PROJECTS\VPrint2\XmlConfigurations\print528_Type1_Raz.xml";
            printer.SimulatePrint = false;
            printer.PrintAllocation(457312, false);
        }

        [TestMethod]
        public void holland_print_format_Type_2()
        {
            VoucherPrinter printer = new VoucherPrinter();
            printer.UseLocalFormat = true;
            printer.UseLocalPrinter = false;
            printer.PrintOnce = true;
            printer.m_PrinterName = Printers.DELL;
            printer.m_ReportType2 = "VPrinting.Documents.VoucherPrintLayoutRazX";
            printer.m_PrinterXmlFilePath = @"C:\PROJECTS\VPrint2\XmlConfigurations\print528_Type2_RazX.xml";
            printer.SimulatePrint = false;
            printer.PrintAllocation(457312, false);
        }
    }
}
