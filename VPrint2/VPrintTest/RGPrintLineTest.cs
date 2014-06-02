using System.Diagnostics;
using System.Globalization;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VPrinting.Documents;
using System.Drawing;
using System.Xml.Serialization;
using VPrinting;
using System.Text;

namespace VPrintTest
{
    [TestClass]
    public class RGPrintLine
    {
        static RGPrintLine()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en");
            VoucherPrinter.Error += new ThreadExceptionEventHandler(VoucherPrinter_Error);
        }

        static void VoucherPrinter_Error(object sender, ThreadExceptionEventArgs e)
        {
            Debug.Fail(e.Exception.Message, e.Exception.ToString());
        }

        public RGPrintLine()
        {
            //XmlSerializer formatter = new XmlSerializer(typeof(Size));
            //var s = new Size(100, 100);
            //var str = formatter.FromObject(s);
            //var voucherPrintObj = formatter.ToObject<Size>("{Width=100, Height=100}");
            //var sstr = s.ToString();
        }

        [TestMethod]
        public void uk_RGPrintLine_format_test()
        {
            VoucherPrinter printer = new VoucherPrinter();
            printer.m_PrinterName = Printers.DELL;
            printer.m_ReportType2 = "VPrinting.Documents.VoucherPrintLayoutUnitRazX";
            printer.m_PrinterXmlFilePath = @"C:\PROJECTS\VPrint\RGPrintLineTest.xml";
            printer.PrintOnce = true;
            printer.SimulatePrint = false;
            printer.UseLocalFormat = true;
            printer.UseLocalPrinter = true;
            printer.PrintAllocation(246244, false);
        }
    }
}
