using System.Diagnostics;
using System.Globalization;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VPrinting;
using VPrinting.Documents;
using Zen.Barcode;

namespace VPrintTest
{
    [TestClass]
    public class CzechTest
    {
        static CzechTest()
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
        public void czech_print_format_Type_1()
        {
            //Oki
            VoucherPrinter printer = new VoucherPrinter();
            printer.m_PrinterName = Printers.DELL;
            printer.SimulatePrint = true;
            printer.PrintOnce = true;
            printer.UseLocalFormat = false;
            printer.UseLocalPrinter = true;
            printer.m_ReportType2 = "VPrinting.Documents.VoucherPrintLayoutRaz";
            printer.m_PrinterXmlFilePath = @"C:\PROJECTS\VPrint2\XmlConfigurations\print203_Type1_Raz.xml";
            printer.PrintAllocation(452159, false); //
        }

        [TestMethod]
        public void czech_print_format_TypeX_1()
        {
            VoucherPrinter printer = new VoucherPrinter();
            printer.m_PrinterName = Printers.DELL;
            printer.m_ReportType2 = "VPrinting.Documents.VoucherPrintLayoutRazX";
            printer.m_PrinterXmlFilePath = @"C:\PROJECTS\VPrint2\XmlConfigurations\print203_Type1_RazX.xml";
            printer.PrintAllocation(102223, false); //  
        }

        [TestMethod]
        public void Test_Barcode()
        {
            var barcode = BarcodeDrawFactory.GetSymbology(BarcodeSymbology.Code25InterleavedNC);
            var image = barcode.Draw("82620159918434718610", 40, 2);
            image.Save("C:\\TEST\\Test1.png");
        }

        [TestMethod]
        public void czech_print_format_TypeX_2()
        {
            var str = MTPL.NumToString("0A313233FF");
            var str2 = MTPL.NumToString("32323232", 10);
            //Oki
            var docText1 = Helper.ToChr(27, 16, 65, 8, 3, 0, 1, 2, 1, 1, 1, 2, 30, 31, 32, 33, 34, 35, 36, 37) + ASCII.LF + ASCII.FF;
            var docText2 = string.Format(@"{0}it1r1s0x70y00b{1}\\", (char)27, 123456789) + ASCII.LF + ASCII.FF;
            var docText3 = MTPL.PrintI2Of5Barcode("123456789", 10, "000") + ASCII.FF;
            var docText4 = MTPL.NumToString("494E5445534C45415645442032204F4620350A0D1B10410803000102010101021B10420A313233343536373839300FFF");
            var docText5 = ASCII.ESC + "it130r1s0x00y170b123456789012?+12345\\" + ASCII.LF + ASCII.FF;
            var docText6 =
                ASCII.ESC + "ih10w10x25y230lSample\\" + ASCII.LF +
                ASCII.ESC + "ix90y230s4h10w10f2g2e" + ASCII.LF +
                ASCII.ESC + "ix105y230s4h10w10v" + ASCII.LF + ASCII.FF;
            var docText7 = MTPL.PrintUSPSBarcode("123445") + ASCII.LF + ASCII.FF;
            var docText8 = MTPL.NumToString("271666101234567890") + ASCII.LF + ASCII.FF;
            var docText9 = MTPL.NumToString("1B 10 42 0A 31 32 33 34 35 36 37 38 39 30") + ASCII.LF + ASCII.FF;
            DirectHelper.SendStringToPrinter(Printers.Tally_T2365_2T, "test", docText9);
        }
    }
}
