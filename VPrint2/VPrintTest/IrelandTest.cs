using System.Diagnostics;
using System.Globalization;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VPrinting.Documents;

namespace VPrintTest
{
    [TestClass]
    public class IrelandTest
    {
        static IrelandTest()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en");
            VoucherPrinter.Error += new ThreadExceptionEventHandler(VoucherPrinter_Error);
        }

        static void VoucherPrinter_Error(object sender, ThreadExceptionEventArgs e)
        {
            Debug.Fail(e.Exception.Message, e.Exception.ToString());
        }

        [TestMethod]
        public void ireland_print_format_Type_1()
        {
            VoucherPrinter printer = new VoucherPrinter();
            printer.UseLocalFormat = true;
            printer.UseLocalPrinter = true;
            printer.PrintOnce = true;
            printer.m_PrinterName = Printers.DELL;
            printer.m_ReportType2 = "VPrinting.Documents.VoucherPrintLayoutRaz";
            printer.m_PrinterXmlFilePath = @"C:\PROJECTS\VPrint\XmlConfigurations\print372_Type1_Raz.xml";
            printer.PrintAllocation(250182, false);
        }

        [TestMethod]
        public void ireland_print_format_Type_2()
        {

        }
    }
}
