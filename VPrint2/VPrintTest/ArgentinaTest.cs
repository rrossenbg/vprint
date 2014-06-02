using System.Diagnostics;
using System.Globalization;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VPrinting.Documents;

namespace VPrintTest
{
    [TestClass]
    public class ArgentinaTest
    {
        static ArgentinaTest()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en");
            VoucherPrinter.Error += new ThreadExceptionEventHandler(VoucherPrinter_Error);
        }

        static void VoucherPrinter_Error(object sender, ThreadExceptionEventArgs e)
        {
            Debug.Fail(e.Exception.Message, e.Exception.ToString());
        }

        public ArgentinaTest()
        {

        }

        [TestMethod]
        //Should be built in Release
        public void argentina_print_format_Type_1_Live()
        {
            VoucherPrinter printer = new VoucherPrinter();
            printer.m_PrinterName = Printers.DELL;
            printer.m_ReportType2 = "VPrinting.Documents.VoucherPrintLayout620";
            printer.m_PrinterXmlFilePath = @"C:\PROJECTS\VPrint\XmlConfigurations\print32_Type1.xml";
            //5 MUNDOS, LDA
            //BUBBLE-QUINTA SHOPPING LJ43 8135-862: id(141690)
            printer.PrintAllocation(332970, false);
        }
    }
}
