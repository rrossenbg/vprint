using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VPrinting;
using VPrinting.Documents;
using VPrinting.Tools;

namespace VPrintTest
{
    [TestClass]
    public class GreeceTest
    {
        static GreeceTest()
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
        public void greece_print_format_TypeX_1()
        {
            //Oki
            VoucherPrinter printer = new VoucherPrinter();
            printer.m_PrinterName = Printers.DELL;
            printer.m_ReportType2 = "VPrinting.Documents.VoucherPrintLayoutRazX";
            printer.m_PrinterXmlFilePath = @"C:\PROJECTS\VPrint2\XmlConfigurations\print300_Type1_RazX.xml";
            printer.UseLocalPrinter = true;
            printer.UseLocalFormat = true;
            printer.PrintAllocation(389298, false); //388608
        }

        [TestMethod]
        public void greece_print_format_TypeX_2()
        {
            //HP - RID - 361456
            VoucherPrinter printer = new VoucherPrinter();
            printer.m_PrinterName = Printers.DELL;
            printer.m_ReportType2 = "VPrinting.Documents.VoucherPrintLayoutRazX";
            printer.m_PrinterXmlFilePath = @"C:\PROJECTS\VPrint2\XmlConfigurations\print300_Type2_RazX.xml";
            printer.UseLocalPrinter = true;
            printer.PrintAllocation(388608, false); //361456
        }

        [TestMethod]
        public void greece_print_format_TypeX_3()
        {
            //Oki
            VoucherPrinter printer = new VoucherPrinter();
            printer.m_PrinterName = Printers.DELL;
            printer.m_ReportType2 = "VPrinting.Documents.VoucherPrintLayoutRazX";
            printer.m_PrinterXmlFilePath = @"C:\PROJECTS\VPrint\print300_Type3_RazX.xml";
            printer.UseLocalPrinter = true;
            printer.UseLocalFormat = true;
            printer.PrintOnce = true;
            printer.PrintAllocation(389298, false); //388608
        }

        [TestMethod]
        public void serialization_test()
        {
            VoucherPrintRazX obj = new VoucherPrintRazX();
            obj.Lines = new List<GPrintLine>();
            obj.Lines.Add(new GPrintLine() { Font = new FontWrapper(new Font("Arial", 10, FontStyle.Bold)), X = 10, Y = 10, Text = "test" });
            obj.Lines.Add(new GPrintLine() { Font = new FontWrapper(new Font("Arial", 10, FontStyle.Bold)), X = 10, Y = 10, Text = "test" });
            obj.Lines.Add(new GPrintLine() { Font = new FontWrapper(new Font("Arial", 10, FontStyle.Bold)), X = 10, Y = 10, Text = "test" });
            //obj.Lines.Add(new BarPrintLine() { Font = new FontWrapper(new Font("Arial", 10)), X = 10, Y = 10, Text = "123456", Text2 = "test2" });

            using (MemoryStream mem = new MemoryStream())
            using (StreamWriter wr = new StreamWriter(mem))
            {
                XmlSerializer formatter = new XmlSerializer(typeof(VoucherPrintRazX));
                formatter.Serialize(wr, obj);
                string str = Encoding.UTF8.GetString(mem.ToArray());
                Debug.WriteLine(str);
            }
        }

        [TestMethod]
        public void test_decimal_test()
        {
            decimal d = 100.1m;
            var f = decimal.Ceiling(d) - decimal.Floor(d);
        }

        [TestMethod]
        public void DeserializeTest()
        {
            string a = File.ReadAllText(@"C:\TEST\greceetest.txt");
            XmlSerializer formatter = new XmlSerializer(typeof(VoucherPrintRazX));
            var voucherPrintObj = formatter.ToObject<VoucherPrintRazX>(a.TrimStart());
        }

        [TestMethod]
        public void DeserializeTest2()
        {
            string a = File.ReadAllText(@"C:\PROJECTS\VPrint\grecee_test.txt");
            XmlSerializer formatter = new XmlSerializer(typeof(VoucherPrintRazX));
            var voucherPrintObj = formatter.ToObject<VoucherPrintRazX>(a.TrimStart());
        }

        [TestMethod]
        public void print_test()
        {
            double d = 111111.223332;
            Debug.WriteLine(d.ToString("0.##"));
        }
    }
}
