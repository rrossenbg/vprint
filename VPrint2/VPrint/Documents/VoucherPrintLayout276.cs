/***************************************************
//  Copyright (c) Premium Tax Free 2012
/***************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;
using System.Xml.Serialization;

namespace VPrinting.Documents
{
    /// <summary>
    /// GDI+ printing. No barcodes
    /// </summary>
    [Serializable]
    public class VoucherPrintLayout276 : VoucherPrinterSettings, IVoucherLayout
    {
        protected const int LINE_LEN_LIMIT = 25;

        public string DocumentInitialization
        {
            get;
            set;
        }

        public int FormLength { get; set; }

        public Size PageSize { get; set; }
        public bool Landscape { get; set; }

        public GPrintLine RetailerID { get; set; }
        public GPrintLine Line1 { get; set; }
        public GPrintLine Line2 { get; set; }
        public GPrintLine Line5 { get; set; }
        public GPrintLine Line3 { get; set; }
        public GPrintLine Phone { get; set; }
        public GPrintLine VoucherID { get; set; }
        public GPrintLine VATNumber { get; set; }
        public GPrintLine ShopName { get; set; }

        public bool ShowRetailerVatRate
        {
            get
            {
                return m_ShowRetailerVatRate;
            }
            set
            {
                m_ShowRetailerVatRate = value;
            }
        }

        private List<IPrintLine> m_list;
        [XmlIgnore]
        public virtual List<IPrintLine> PrintLines
        {
            get
            {
                m_list = new List<IPrintLine>()
                    {
                        RetailerID,
                        Line1,
                        Line2,
                        Line5,
                        Line3,
                        Phone,
                        VATNumber,
                        VoucherID,
                        ShopName,
                    };
                m_list.Sort(new PrintLineComparer());
                return m_list;
            }
            set
            {
                m_list = value;
            }
        }

        public VoucherPrintLayout276()
        {
            RetailerID = new GPrintLine();
            Line1 = new GPrintLine();
            Line2 = new GPrintLine();
            Line5 = new GPrintLine();
            Line3 = new GPrintLine();
            Phone = new GPrintLine();
            VoucherID = new GPrintLine();
            VATNumber = new GPrintLine();
            ShopName = new GPrintLine();
        }

        public void Init()
        {
        }

        public virtual void DataBind(IDataProvider pr, string strVoucherNo, int voucher, bool printDemo)
        {
            if (printDemo)
            {
                RetailerID.Text = "11228";
                Line1.Text = "Hermann-Oberth-Starbe 9";
                Line2.Text = "Line2 Line2 Line2";
                Line3.Text = "85640 Putzbrunn/Munchen";
                Line5.Text = "Germany";
                Phone.Text = "0142 611 187";
                VoucherID.Text = "DE0021366878";
                if (this.m_ShowRetailerVatRate)
                    VATNumber.Text = "332037662";
                ShopName.Text = "PREMIER TAX FREE GmbH";
            }
            else
            {
                RetailerID.Text = pr.Retailer.Id.ToString();
                Line1.Text = pr.Retailer.RetailAddress.Line1.Limit(LINE_LEN_LIMIT, null);
                Line2.Text = pr.Retailer.RetailAddress.Line2.Limit(LINE_LEN_LIMIT, null);
                Line5.Text = pr.Retailer.RetailAddress.Line5.Limit(LINE_LEN_LIMIT, null);
                Line3.Text = pr.Retailer.RetailAddress.Line3.Limit(LINE_LEN_LIMIT, null);
                Phone.Text = pr.Retailer.Phone.TrimSafe();
                VoucherID.Text = voucher + pr.Printing.CalculateCheckDigit(voucher);
                if (this.m_ShowRetailerVatRate)
                    VATNumber.Text = pr.Retailer.VatNumber.TrimSafe();
                ShopName.Text = pr.Retailer.TradingName.TrimSafe();
            }
        }

        public virtual void Clear()
        {
            foreach (IPrintLine line in PrintLines)
                if (line != null)
                    line.Text = null;
        }

        public void InitPrinter(string printDoc)
        {
            //No implementation
        }

        public void PrintVoucher(string printerName, string printDocName, int length, string docInitialization, IList<IPrintLine> lines)
        {
            using (var doc = new PrintDocument())
            {
                doc.DocumentName = printDocName;
                doc.PrintController = new StandardPrintController();
                doc.PrinterSettings.PrinterName = printerName;
                doc.DefaultPageSettings.PaperSize = new PaperSize("CustomPaper", PageSize.Width, PageSize.Height);
                doc.DefaultPageSettings.Landscape = Landscape;

                doc.PrintPage += new PrintPageEventHandler(Document_PrintPage);
                doc.Print();
                doc.PrintPage -= new PrintPageEventHandler(Document_PrintPage);
            }
        }

        private void Document_PrintPage(object sender, PrintPageEventArgs e)
        {
            using (var brush = new SolidBrush(Color.Black))
            {
                foreach (IPrintLine line in PrintLines)
                {
                    if (line == null)
                        continue;

                    GPrintLine gline = line as GPrintLine;
                    if (gline != null && !gline.IsEmpty())
                    {
                        if (gline.Font == null)
                            throw new ArgumentNullException("Line.Font");
                        if (gline.Font.Value == null)
                            throw new ArgumentNullException("Line.Font.Value");

                        e.Graphics.DrawString(
                            string.IsNullOrEmpty(gline.Description) ? gline.Text : string.Format(gline.Description, gline.Text),
                            gline.Font.Value, brush, line.X, line.Y);
                    }
                }
            }
        }

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            foreach (var line in PrintLines)
                b.AppendLine(line.ToString());
            return b.ToString();
        }
    }
}