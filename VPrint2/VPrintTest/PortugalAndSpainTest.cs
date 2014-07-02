using System.Diagnostics;
using System.Globalization;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VPrinting.Attributes;
using VPrinting.Documents;
using System;
using System.Collections;

namespace VPrintTest
{
    [TestClass]
    public class PortugalAndSpainTest
    {
        static PortugalAndSpainTest()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en");
            VoucherPrinter.Error += new ThreadExceptionEventHandler(VoucherPrinter_Error);
        }

        static void VoucherPrinter_Error(object sender, ThreadExceptionEventArgs e)
        {
            Debug.Fail(e.Exception.Message, e.Exception.ToString());
        }

        public PortugalAndSpainTest()
        {

        }

        //[TestMethod]
        //public void portugal_print_format_Type_1_Demo()
        //{
        //    VoucherPrinter printer = new VoucherPrinter();
        //    printer.m_PrinterName = Printers.Tally_T2365_2T;
        //    printer.m_ReportType2 = "VPrinting.Documents.VoucherPrintLayout620";
        //    printer.m_PrinterXmlFilePath = @"C:\PROJECTS\VPrint\XmlConfigurations\print620_Type1.xml";
        //    printer.PrintAllocation(0, true);
        //}

        //[TestMethod]
        ////Should be built in Release
        //public void portugal_print_format_Type_1_Live()
        //{
        //    VoucherPrinter printer = new VoucherPrinter();
        //    printer.m_PrinterName = Printers.DELL;
        //    printer.m_ReportType2 = "VPrinting.Documents.VoucherPrintLayout620";
        //    printer.m_PrinterXmlFilePath = @"C:\PROJECTS\VPrint\XmlConfigurations\print620_Type1.xml";
        //    //5 MUNDOS, LDA
        //    //BUBBLE-QUINTA SHOPPING LJ43 8135-862: id(141690)
        //    printer.PrintAllocation(332970, false);
        //}

        //[TestMethod]
        ////Should be built in Release
        //public void portugal_print_format_Type_2_Live()
        //{
        //    VoucherPrinter printer = new VoucherPrinter();
        //    printer.m_PrinterName = Printers.DELL;
        //    printer.m_ReportType2 = "VPrinting.Documents.VoucherPrintLayout620";
        //    printer.m_PrinterXmlFilePath = @"C:\PROJECTS\VPrint\XmlConfigurations\print620_Type2.xml";
        //    //5 MUNDOS, LDA
        //    //BUBBLE-QUINTA SHOPPING LJ43 8135-862: id(141690)
        //    printer.PrintAllocation(332970, false);
        //}

        //[TestMethod]
        ////Should be built in Release
        //public void spain_print_format_Type_3_Live()
        //{
        //    VoucherPrinter printer = new VoucherPrinter();
        //    printer.m_PrinterName = Printers.DELL;
        //    printer.m_ReportType2 = "VPrinting.Documents.VoucherPrintLayout620";
        //    printer.m_PrinterXmlFilePath = @"C:\PROJECTS\VPrint\XmlConfigurations\print620_Type3.xml";
        //    //5 MUNDOS, LDA
        //    //BUBBLE-QUINTA SHOPPING LJ43 8135-862: id(141690)
        //    printer.PrintAllocation(332970, false);
        //}

        [TestMethod]
        [OnLiveAttribute("11/09/2013")]
        public void spain_print724_format_ss_only_1_Demo()
        {
            //724
            VoucherPrinter printer = new VoucherPrinter();
            printer.m_PrinterName = Printers.Tally_T2365_2T;
            printer.m_ReportType2 = "VPrinting.Documents.VoucherPrintLayoutRazX";
            printer.m_PrinterXmlFilePath = @"C:\PROJECTS\VPrint\XmlConfigurations\print724_ss_RazX.xml";
            printer.UseLocalFormat = true;
            printer.UseLocalPrinter = true;
            printer.SimulatePrint = false;
            printer.PrintOnce = true;
            printer.PrintAllocation(422651, false);//335934
        }

        [TestMethod]
        [OnLiveAttribute("11/09/2013")]
        public void portugal_print901_format_ss_1_Demo()
        {
            //CountryID = 901
            VoucherPrinter printer = new VoucherPrinter();
            printer.m_PrinterName = Printers.DELL;
            printer.m_ReportType2 = "VPrinting.Documents.VoucherPrintLayoutRazX";
            printer.m_PrinterXmlFilePath = @"C:\PROJECTS\VPrint\XmlConfigurations\print901_RazX.xml";
            printer.UseLocalFormat = true;
            printer.UseLocalPrinter = true;
            printer.SimulatePrint = false;
            printer.PrintOnce = true;
            printer.PrintAllocation(421141, false);
        }

        [TestMethod]
        [OnLiveAttribute("")]
        public void portugal_print901_format_ssnew_1_Demo()
        {
            //CountryID = 901
            VoucherPrinter printer = new VoucherPrinter();
            printer.m_PrinterName = Printers.novaPDF_Lite_v7;
            printer.m_ReportType2 = "VPrinting.Documents.VoucherPrintLayoutRazX";
            printer.m_PrinterXmlFilePath = @"C:\PROJECTS\VPrint\XmlConfigurations\print901_Type3_RazX.xml";
            printer.UseLocalFormat = true;
            printer.UseLocalPrinter = true;
            printer.SimulatePrint = false;
            printer.PrintOnce = true;
            printer.PrintAllocation(421141, false);
        }

        [TestMethod]
        [OnLiveAttribute("")]
        public void portugal_print901_format_ssnew_2_Demo()
        {
            //CountryID = 901
            var printer = new VoucherPrinter();
            printer.m_PrinterName = Printers._3TH_FLOOR_PRINTER;
            printer.m_ReportType2 = "VPrinting.Documents.VoucherPrintLayoutRazX";
            printer.m_PrinterXmlFilePath = @"C:\PROJECTS\VPrint2\XmlConfigurations\print901_Type4_RazX.xml";
            printer.UseLocalFormat = true;
            printer.UseLocalPrinter = true;
            printer.SimulatePrint = false;
            printer.PrintOnce = true;
            printer.PrintAllocation(421141, false);
        }
    }


    [TestClass]
    public class OtherTests
    {
        [TestMethod]
        [OnLiveAttribute("11/09/2013")]
        public void RevertTest()
        {
            var arr = new int[] { 1, 2, 3, 4, 5, 6, 7 };
            arr.Revert();
        }
    }

    public class SimpleThreadSafeCache
    {
        private readonly Hashtable m_Cache = Hashtable.Synchronized(new Hashtable(StringComparer.InvariantCultureIgnoreCase));

        public void Add(string key, object data)
        {
            m_Cache[key] = data;
        }

        public object Get(string key)
        {
            return m_Cache[key];
        }

        public void Clear()
        {
            m_Cache.Clear();
        }
    }

    public static class Ex
    {
        /// <summary>
        ///  var arr = new int[] { 1, 2, 3, 4, 5, 6, 7 };
        ///  arr.Revert();
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        public static void Revert<T>(this T[] arr)
        {
            if (arr == null)
                throw new ArgumentNullException("arr");

            for (int i = 0; i < arr.Length / 2; i++)
            {
                T c = arr[i];
                arr[i] = arr[arr.Length - 1 - i];
                arr[arr.Length - 1 - i] = c;
            }
        }
    }
}
