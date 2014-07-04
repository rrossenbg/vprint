using System;
using System.Linq;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VPrinting;
using VPrinting.Documents;
using VPrinting.Attributes;
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
        //    printer.m_PrinterXmlFilePath = @"C:\PROJECTS\VPrint2\XmlConfigurations\print620_Type1.xml";
        //    printer.PrintAllocation(0, true);
        //}

        //[TestMethod]
        ////Should be built in Release
        //public void portugal_print_format_Type_1_Live()
        //{
        //    VoucherPrinter printer = new VoucherPrinter();
        //    printer.m_PrinterName = Printers.DELL;
        //    printer.m_ReportType2 = "VPrinting.Documents.VoucherPrintLayout620";
        //    printer.m_PrinterXmlFilePath = @"C:\PROJECTS\VPrint2\XmlConfigurations\print620_Type1.xml";
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
        //    printer.m_PrinterXmlFilePath = @"C:\PROJECTS\VPrint2\XmlConfigurations\print620_Type2.xml";
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
        //    printer.m_PrinterXmlFilePath = @"C:\PROJECTS\VPrint2\XmlConfigurations\print620_Type3.xml";
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
            printer.m_PrinterXmlFilePath = @"C:\PROJECTS\VPrint2\XmlConfigurations\print724_ss_RazX.xml";
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
            printer.m_PrinterXmlFilePath = @"C:\PROJECTS\VPrint2\XmlConfigurations\print901_RazX.xml";
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
            printer.m_PrinterXmlFilePath = @"C:\PROJECTS\VPrint2\XmlConfigurations\print901_Type3_RazX.xml";
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
            printer.UseLocalFormat = false;
            printer.UseLocalPrinter = true;
            printer.SimulatePrint = false;
            printer.PrintOnce = true;
            printer.Test += new EventHandler(printer_Test);
            printer.PrintAllocation(547200, false);
        }

        void printer_Test(object sender, EventArgs e)
        {
            VoucherPrinter Model = (VoucherPrinter)sender;

            //Please no code into the body tag!
            //Only variables

            //Voucher
            var voucherNumber = Model.VoucherNo + Model.Printing.CalculateCheckDigit(Model.VoucherNo);


            var barcodeNumber = Model.StrVoucherNo.Replace(" ", "").Substring(3);
            var barcodeText = Model.StrVoucherNo.Replace(" ", "");

            //Retailer
            var id = Model.Retailer.Id;
            var retailerName = Model.Retailer.TradingName.EscapeXml();
            var rLine1 = Model.Retailer.RetailAddress.Line1;
            var rLine2 = Model.Retailer.RetailAddress.Line2;
            var rLine3 = Model.Retailer.RetailAddress.Line3;
            var rLine5 = Model.Retailer.RetailAddress.Line5;
            var retailerAddress = string.Concat(rLine1, '\n', rLine2, '\n', rLine5, '-', rLine3).EscapeXml();
            var retailerPhone = Model.Retailer.Phone.EscapeXml();

            var officeData = Model.Manager.RetrieveTableData("ho_pfs, ho_Certificate_1, ho_Certificate_2, ho_Certificate_3, ho_category_title,ho_add_id", "HeadOffice",
                "where ho_id={0} and ho_iso_id={1}".format(Model.Retailer.HeadOfficeId, Model.Retailer.CountryId));
            var branchData = Model.Manager.RetrieveTableData("br_category, br_pfs", "Branch",
                "where br_id={0} and br_iso_id={1}".format(Model.Retailer.Id, Model.Retailer.CountryId));

            var ho = "HO: " + Model.Retailer.HeadOfficeId;

            //Office
            var officeName = Model.Retailer.HeadOfficeName.EscapeXml();
            var hoData = Model.Manager.RetrieveTableData("hoa_add_1,hoa_add_2,hoa_add_3,hoa_add_4,hoa_add_5,hoa_add_6", "HeadOfficeAddress",
                "where hoa_id = {0} ".format(officeData.Length > 5 ? officeData[5] : "0"));
            var oLine1 = hoData.Length > 0 ? hoData[0] : "";
            var oLine2 = hoData.Length > 1 ? hoData[1] : "";
            var oLine3 = hoData.Length > 2 ? hoData[2] : "";
            var oLine5 = hoData.Length > 4 ? hoData[4] : "";
            var officeAddress = string.Concat(oLine1, '\n', oLine2, '\n', oLine5, '-', oLine3).EscapeXml();

            var vatNumber = string.Concat("", Model.Retailer.VatNumber);


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
