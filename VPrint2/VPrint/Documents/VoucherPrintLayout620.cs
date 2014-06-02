/***************************************************
//  Copyright (c) Premium Tax Free 2012
/***************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Xml.Serialization;
using VPrinting.Tools;
using VPrinting.Common;

namespace VPrinting.Documents
{
    /// <summary>
    /// GDI+ printing. Direct barcode printing
    /// </summary>
    public class VoucherPrintLayout620 : VoucherPrinterSettings, IVoucherLayout
    {
        protected const int LINE_LEN_LIMIT = 25;

        public string DocumentInitialization { get; set; }

        public int FormLength { get; set; }
        public Size PageSize { get; set; }
        public bool Landscape { get; set; }
        public bool PrinterPrintBarcode
        {
            get { return m_PrinterPrintBarcode; }
            set { m_PrinterPrintBarcode = value; }
        }
        public bool ShowRetailerVatRate
        {
            get { return m_ShowRetailerVatRate; }
            set { m_ShowRetailerVatRate = value; }
        }

        /// <summary>
        /// [Name]&lt;br&gt;[Line1]&lt;br&gt;[Line2]&lt;br&gt;[Line3]&lt;br&gt;[Line5]
        /// </summary>
        public string HeadOfficeAddressFormat { get; set; }

        /// <summary>
        /// [Name]&lt;br&gt;[Line1]&lt;br&gt;[Line2]&lt;br&gt;[Line3]&lt;br&gt;[Line5]
        /// </summary>
        public string RetailerAddressFormat { get; set; }

        //HO address
        public GPrintLine HOAddress { get; set; }

        //Retailer address flags
        public GPrintLine RetailerAddress { get; set; }

        public GPrintLine VoucherID { get; set; }
        /// <summary>
        /// Shop No
        /// </summary>
        public GPrintLine RetailerID { get; set; }
        /// <summary>
        /// RetailerID + SubVoucherRange
        /// </summary>
        public GPrintLine StoreCode { get; set; }
        public GPrintLine VATNumber { get; set; }

        public CommPrintLine BarCodeNo { get; set; }
        public BarPrintLine BarCodeImg { get; set; }

        private List<IPrintLine> m_list;

        [XmlIgnore]
        public List<IPrintLine> PrintLines
        {
            get
            {
                m_list = new List<IPrintLine>()
                    {
                        BarCodeNo,
                        BarCodeImg,
                        RetailerAddress,
                        HOAddress,

                        StoreCode,
                        VoucherID,
                        RetailerID,
                        VATNumber,
                    };
                m_list.Sort(new PrintLineComparer());
                return m_list;
            }
            set
            {
                m_list = value;
            }
        }

        public VoucherPrintLayout620()
        {
            BarCodeNo = new CommPrintLine();
            BarCodeImg = new BarPrintLine();
        }

        public void Init()
        {
        }

        public virtual void Clear()
        {
            foreach (IPrintLine line in PrintLines)
                if (line != null)
                    line.Text = null;
        }

        public void DataBind(IDataProvider pr, string voucherNo, int voucher, bool printDemo)
        {
            if (printDemo)
            {
                RetailerID.Text = "141690";

                StoreCode.Text = "141690-123";

                if (!HeadOfficeAddressFormat.IsNullOrEmpty())
                {
                    HOAddress.Text =
    @"PTF Portugal
Rua Castilho, no 39-8F
Edificio Castil 8º Piso Sala F
1250-068,Lisboa";
                }

                if (!RetailerAddressFormat.IsNullOrEmpty())
                {
                    RetailerAddress.Text =
    @"BUBBLE
QUINTA SHOPPING LJ43
ALMANCIL
8135-862";
                }

                VoucherID.Text = "507680030";

                if (this.m_ShowRetailerVatRate)
                    VATNumber.Text = "62020141690013853533";
                if (this.m_PrinterPrintBarcode)
                {
                    BarCodeImg.Text = 
                    BarCodeNo.Text = "62020141690013853533";
                    //BarCodeImg.Text2 =
                    //BarCodeNo.Text2 = "D PT 620 20 141690 013853533";
                }
            }
            else
            {
                
                RetailerID.Text = pr.Retailer.Id.ToString();

                var subVoucherRange = CacheManager.Instance.Table[Strings.SubRangeFrom].Cast<int>();

                StoreCode.Text = string.Format("{0} - {1}", pr.Retailer.Id, subVoucherRange);

                if (!HeadOfficeAddressFormat.IsNullOrEmpty())
                {
                    //Constructing HeadOffice Address
                    Dictionary<string, string> dict = new Dictionary<string, string> 
                    { 
                        { "Name", pr.Office.Name },
                        { "Line1", pr.Office.OfficeAddress.Line1 },
                        { "Line2", pr.Office.OfficeAddress.Line2 },
                        { "Line3", pr.Office.OfficeAddress.Line3 },
                        { "Line5", pr.Office.OfficeAddress.Line5 },
                    };
                    HOAddress.Text = HeadOfficeAddressFormat.format(dict, false).Replace("<br><br>", null).Replace("<br>", Environment.NewLine);
                }

                if (!RetailerAddressFormat.IsNullOrEmpty())
                {
                    //
                    //Constructing Retailer Address
                    Dictionary<string, string> dict = new Dictionary<string, string> 
                    { 
                        { "Name", pr.Retailer.TradingName },
                        { "Line1", pr.Retailer.RetailAddress.Line1 },
                        { "Line2", pr.Retailer.RetailAddress.Line2 },
                        { "Line3", pr.Retailer.RetailAddress.Line3 },
                        { "Line5", pr.Retailer.RetailAddress.Line5 },
                    };
                    RetailerAddress.Text = RetailerAddressFormat.format(dict, false).Replace("<br><br>", null).Replace("<br>", Environment.NewLine);
                }

                VoucherID.Text = voucher + pr.Printing.CalculateCheckDigit(voucher);

                if (this.m_ShowRetailerVatRate)
                    VATNumber.Text = pr.Retailer.VatNumber.TrimSafe();
                if (this.m_PrinterPrintBarcode)
                {
                    BarCodeImg.Text =
                    BarCodeNo.Text = voucherNo.Replace(" ", "").Substring(3);
                    //BarCodeImg.Text2 = BarCodeNo.Text2 = voucherNo;
                }
            }
        }

        public void InitPrinter(string printDoc)
        {
        }

        public void PrintVoucher(string printerName, string printDocName, int length, string docInitialization, IList<IPrintLine> lines)
        {
            using (var doc = new EscapePrintDocument())
            {
                doc.DocumentName = printDocName;
                doc.PrintController = new StandardPrintController();
                doc.PrinterSettings.PrinterName = printerName;
                doc.DefaultPageSettings.PaperSize = new PaperSize("CustomPaper", PageSize.Width, PageSize.Height);
                doc.DefaultPageSettings.Landscape = Landscape;

                var handler = DelegateHelper.CreatePrintPageEventHandler(lines);
                doc.PrintPage += handler;
                doc.Print();
                doc.PrintPage -= handler;
            }
        }
    }
}
