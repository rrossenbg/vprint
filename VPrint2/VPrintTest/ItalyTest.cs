using System;
using System.Linq;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VPrinting;
using VPrinting.Documents;
using System.Drawing;

namespace VPrintTest
{
    [TestClass]
    public class ItalyTest
    {
        static ItalyTest()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en");
            VoucherPrinter.Error += new ThreadExceptionEventHandler(VoucherPrinter_Error);
        }

        static void VoucherPrinter_Error(object sender, ThreadExceptionEventArgs e)
        {
            Debug.Fail(e.Exception.Message, e.Exception.ToString());
        }

        public ItalyTest()
        {

        }        

        [TestMethod]
        public void italy_print_format_Type_1()
        {
            VoucherPrinter printer = new VoucherPrinter();
            printer.UseLocalFormat = false;
            printer.UseLocalPrinter = false;
            printer.PrintOnce = true;
            printer.m_PrinterName = Printers.DELL;
            printer.m_ReportType2 = "VPrinting.Documents.VoucherPrintLayoutRaz";
            printer.m_PrinterXmlFilePath = @"C:\PROJECTS\VPrint2\XmlConfigurations\print380_Type1_Raz.xml";
            printer.Test += new EventHandler(printer_Test);
            printer.SimulatePrint = true;
            printer.PrintAllocation(589331, false);//384920//211771
        }

        [TestMethod]
        public void italy_print_format_Type_2()
        {
            //SMALL
            VoucherPrinter printer = new VoucherPrinter();
            printer.UseLocalFormat = false;
            printer.UseLocalPrinter = true;
            printer.SimulatePrint = false;
            printer.PrintOnce = true;
            printer.m_PrinterName = Printers.DELL;
            printer.m_ReportType2 = "VPrinting.Documents.VoucherPrintLayoutRazX";
            printer.m_PrinterXmlFilePath = @"C:\PROJECTS\VPrint2\XmlConfigurations\print380_Type2_RazX.xml";
            printer.PrintAllocation(360303, false);//384920//384920//211771
        }

        [TestMethod]
        public void italy_print_format_Type_3()
        {
            //BIG
            VoucherPrinter printer = new VoucherPrinter();
            printer.UseLocalFormat = true;
            printer.UseLocalPrinter = true;
            printer.PrintOnce = true;
            printer.SimulatePrint = true;
            printer.m_PrinterName = Printers.DELL;
            printer.m_ReportType2 = "VPrinting.Documents.VoucherPrintLayoutRazX";
            printer.m_PrinterXmlFilePath = @"C:\PROJECTS\VPrint2\XmlConfigurations\print380_Type3_RazX.xml";
            printer.PrintAllocation(360303, false);
            //435438//384920//211771
            //441786//435438//428314---•	Sephora  head office 106793
        }

        [TestMethod]
        public void italy_print_format_Type_4()
        {
            //BIG
            VoucherPrinter printer = new VoucherPrinter();
            printer.Test += new System.EventHandler(printer_Test);
            printer.UseLocalFormat = false;
            printer.UseLocalPrinter = true;
            printer.PrintOnce = true;
            printer.SimulatePrint = true;
            printer.m_PrinterName = Printers._3TH_FLOOR_PRINTER;
            printer.m_ReportType2 = "VPrinting.Documents.VoucherPrintLayoutRazX";
            printer.m_PrinterXmlFilePath = @"C:\PROJECTS\VPrint2\XmlConfigurations\print380_Type4_RazX.xml";
            printer.PrintAllocation(589331, false); //569366  360303
            //435438//384920//211771
            //441786//435438//428314---•	Sephora  head office 106793
        }

        private void printer_Test(object sender, System.EventArgs e)
        {
            VoucherPrinter Model = (VoucherPrinter)sender;

            var id = Model.Retailer.Id;
            var barcodeNumber = Model.StrVoucherNo.Replace(" ", "").Substring(3);
            var barcodeText = Model.StrVoucherNo.Replace(" ", "");

            //RETAILER
            var shopName = Model.Retailer.Name;
            var voucher = Model.VoucherNo;
            var checkDig = Model.Printing.CalculateCheckDigit(Model.VoucherNo);
            var voucherNumber = string.Concat(Model.VoucherNo, checkDig);
            var line1 = Model.Retailer.RetailAddress.Line1;
            var line2 = Model.Retailer.RetailAddress.Line2;
            var line3 = Model.Retailer.RetailAddress.Line3;
            var line5 = Model.Retailer.RetailAddress.Line5;

            var officeData = Model.Manager.RetrieveTableData("ho_pfs, ho_Certificate_1, ho_Certificate_2, ho_Certificate_3, ho_category_title,ho_add_id", "HeadOffice",
                "where ho_id={0} and ho_iso_id={1}".format(Model.Retailer.HeadOfficeId, Model.Retailer.CountryId));
            var branchData = Model.Manager.RetrieveTableData("br_category, br_pfs", "Branch",
                "where br_id={0} and br_iso_id={1}".format(Model.Retailer.Id, Model.Retailer.CountryId));

            //OFFICE 
            var officeName = Convert.ToString(Model.Manager.RetrieveTableData("ho_trading_name", "HeadOffice",
               "where ho_id={0} and ho_iso_id={1}".format(Model.Retailer.HeadOfficeId, Model.Retailer.CountryId)).FirstOrDefault()).EscapeXml();

            var hoData = Model.Manager.RetrieveTableData("hoa_add_1,hoa_add_2,hoa_add_3,hoa_add_4,hoa_add_5,hoa_add_6", "HeadOfficeAddress",
                "where hoa_id = {0} ".format(officeData.Length > 5 ? officeData[5] : "0"));

            var oLine1 = Convert.ToString(hoData.Length > 0 ? hoData[0] : "");
            var oLine2 = Convert.ToString(hoData.Length > 1 ? hoData[1] : "");
            var oLine3 = Convert.ToString(hoData.Length > 2 ? hoData[2] : "");
            var oLine5 = Convert.ToString(hoData.Length > 3 ? hoData[4] : "");

            //CERTIFICATES
            var certificate1 = Convert.ToString(officeData.Length > 1 ? officeData[1] : "");
            var certificate2 = Convert.ToString(officeData.Length > 2 ? officeData[2] : "");
            var certificate3 = Convert.ToString(officeData.Length > 3 ? officeData[3] : "");
            var category = Convert.ToString(officeData.Length > 4 ? officeData[4] : "");
            var vatNumber = Model.Retailer.VatNumber;

            var value = string.Format(
            "Shop Code: {0}\r\n" +
            "Doc nr: S-IT-380-10-{0}-{1}-{2}\r\n" +
            "{3}\r\n" +
            "{4}\r\n" +
            "{5} {6}\r\n" +
            "{7}\r\n" +
            "{8}\r\n" +
            "{9}\t{10}\r\n" +
            "P.I: {11}\r\n" +
            "C.F./REG.IMP.: {12}\r\n" +
            "N.REA: {13,-30} CAP.SOC: {14}",
                                          id, voucher, checkDig,
                                          shopName,
                                          line1,
                                          line5, line3,
                                          officeName,
                                          oLine1,
                                          oLine5, oLine3,
                                          vatNumber,
                                          certificate1,
                                          certificate3, certificate2).EscapeXml();
        }

        private void printer_Test2(object sender, System.EventArgs e)
        {
            VoucherPrinter Model = (VoucherPrinter)sender;

            var id = Model.Retailer.Id;
            var barcodeNumber = Model.StrVoucherNo.Replace(" ", "").Substring(3);
            var barcodeText = Model.StrVoucherNo.Replace(" ", "");

            //RETAILER
            var shopName = Model.Retailer.Name;
            var voucher = Model.VoucherNo;
            var checkDig = Model.Printing.CalculateCheckDigit(Model.VoucherNo);
            var voucherNumber = string.Concat(Model.VoucherNo, checkDig);
            var line1 = Model.Retailer.RetailAddress.Line1;
            var line2 = Model.Retailer.RetailAddress.Line2;
            var line3 = Model.Retailer.RetailAddress.Line3;
            var line5 = Model.Retailer.RetailAddress.Line5;

            var officeData = Model.Manager.RetrieveTableData("ho_pfs, ho_Certificate_1, ho_Certificate_2, ho_Certificate_3, ho_category_title,ho_add_id", "HeadOffice",
                "where ho_id={0} and ho_iso_id={1}".format(Model.Retailer.HeadOfficeId, Model.Retailer.CountryId));
            var branchData = Model.Manager.RetrieveTableData("br_category, br_pfs", "Branch",
                "where br_id={0} and br_iso_id={1}".format(Model.Retailer.Id, Model.Retailer.CountryId));

            //OFFICE
            var officeName = Convert.ToString( Model.Manager.RetrieveTableData("ho_trading_name", "HeadOffice",
                "where ho_id={0} and ho_iso_id={1}".format(Model.Retailer.HeadOfficeId, Model.Retailer.CountryId)).FirstOrDefault());

            var hoData = Model.Manager.RetrieveTableData("hoa_add_1,hoa_add_2,hoa_add_3,hoa_add_4,hoa_add_5,hoa_add_6", "HeadOfficeAddress",
                "where hoa_id = {0} ".format(officeData.Length > 5 ? officeData[5] : "0"));

            var oLine1 = Convert.ToString(hoData.Length > 0 ? hoData[0] : "");
            var oLine2 = Convert.ToString(hoData.Length > 1 ? hoData[1] : "");
            var oLine3 = Convert.ToString(hoData.Length > 2 ? hoData[2] : "");
            var oLine5 = Convert.ToString(hoData.Length > 3 ? hoData[4] : "");

            //CERTIFICATES
            var certificate1 = Convert.ToString(officeData.Length > 1 ? officeData[1] : "");
            var certificate2 = Convert.ToString(officeData.Length > 2 ? officeData[2] : "");
            var certificate3 = Convert.ToString(officeData.Length > 3 ? officeData[3] : "");
            var category = Convert.ToString(officeData.Length > 4 ? officeData[4] : "");
            var vatNumber = Model.Retailer.VatNumber;
        }
    }
}