/***************************************************
//  Copyright (c) Premium Tax Free 2012
/***************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using VPrinting.Common;

namespace VPrinting.Documents
{
    /// <summary>
    /// Direct simple printing. No barcode printing
    /// </summary>
    [Serializable]
    public class VoucherPrintLayout250 : VoucherPrinterSettings, IVoucherLayout
    {
        protected const int LINE_LEN_LIMIT = 25;

        public string DocumentInitialization
        {
            get;
            set;
        }

        public int FormLength
        {
            get;
            set;
        }

        public string Context { get; set; }

        public PrintLine BarCodeNo { get; set; }
        public PrintLine BarCodeText { get; set; }

        public PrintLine RetailerID { get; set; }
        public PrintLine Line1 { get; set; }
        public PrintLine Line5 { get; set; }
        public PrintLine Line3 { get; set; }
        public PrintLine Phone { get; set; }
        public PrintLine VoucherID { get; set; }
        public PrintLine VATNumber { get; set; }
        public PrintLine ShopName { get; set; }
        public PrintLine Line2 { get; set; }
        public Point MoveAll { get; set; }

        private List<IPrintLine> m_list;
        [XmlIgnore]
        public virtual List<IPrintLine> PrintLines
        {
            get
            {
                m_list = new List<IPrintLine>()
                    {
                        BarCodeNo,
                        BarCodeText,
                        RetailerID,
                        Line1,
                        Line5,
                        Line3,
                        Phone,
                        VATNumber,
                        VoucherID,
                        ShopName,
                        Line2,
                    };
                m_list.Sort(new PrintLineComparer());
                return m_list;
            }
            set
            {
                m_list = value;
            }
        }

        public VoucherPrintLayout250()
        {
            BarCodeNo = new PrintLine("BarCodeNo");
            BarCodeText = new PrintLine("BarCodeText");
            RetailerID = new PrintLine("RetailerID");
            Line1 = new PrintLine("Line1");
            Line5 = new PrintLine("Line5");
            Line3 = new PrintLine("Line3");
            Phone = new PrintLine("Phone");
            VoucherID = new PrintLine("VoucherID");
            VATNumber = new PrintLine("VATNumber");
            ShopName = new PrintLine("ShopName");
            Line2 = new PrintLine("Line2");
        }

        public void Init()
        {

        }

        public virtual void DataBind(IDataProvider pr, string strVoucherNo, int voucher, bool printDemo)
        {
            if (printDemo)
            {
                RetailerID.Text = "11228";
                Line1.Text = "1-3 Bld de la Madeleine";
                Line5.Text = "75001";
                Line3.Text = "PARIS";
                Line2.Text = "Place du casino";
                Phone.Text = "01 42 61 11 87";
                VoucherID.Text = "A0021366878";
                if (this.m_ShowRetailerVatRate)
                    VATNumber.Text = "332037662";
                ShopName.Text = "BOUTIQUE J-M WESTON";
            }
            else
            {
                if (this.m_PrinterPrintBarcode)
                {
                    BarCodeNo.Text = strVoucherNo.Replace(" ", "").Substring(3);
                    BarCodeText.Text = strVoucherNo;
                }

                RetailerID.Text = pr.Retailer.Id.ToString();
                Line1.Text = pr.Retailer.RetailAddress.Line1.Limit(LINE_LEN_LIMIT, null);
                Line5.Text = pr.Retailer.RetailAddress.Line5.Limit(LINE_LEN_LIMIT, null);
                Line3.Text = pr.Retailer.RetailAddress.Line3.Limit(LINE_LEN_LIMIT, null);
                Line2.Text = pr.Retailer.RetailAddress.Line2.Limit(LINE_LEN_LIMIT, null);
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
            StringBuilder builder = new StringBuilder();

            //Setting form length
            if (FormLength > 0)
                builder.Append(MTPL.SetFormLength(FormLength));

            builder.Append(Context);
            builder.Clean();

            //Parse commands
            var pattern1 = @"\[cmd\s*:\s*{([^}]*)\}\s*\]"; //{(char)27 + "[11w"}
            var re1 = new Regex(pattern1, RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | 
                RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
            re1.Replace(ref builder, (ma) =>
            {
                return ma.Groups[1].Success ?
                    CacheManager.Instance.Table.GetValueAdd<string>(ma.Groups[1].Value, 
                        new Func<string>(() => Convert.ToString(CommonTools.Eval(ma.Groups[1].Value)))) :
                    null;
            });

            //Replace spaces
            builder.Replace("<nbsp>", " ");

            //<ht><ht>[VATNumber,10]<ht>[VATNumber,-10]<ht>[VATNumber]<br>
            foreach (PrintLine line in lines)
            {
                //Parse params
                var pattern2 = string.Format(@"\[(?<name>{0})(?<value>,\-?\d+)?\]", line.Description);
                var re2 = new Regex(pattern2, RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | 
                    RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
                re2.Replace(ref builder, (ma) =>
                {
                    if(ma.Groups[2].Success)
                        return string.Format(string.Concat("{0", ma.Groups["value"].Value, "}"), line.Text);
                    else if(ma.Groups[1].Success)
                        return line.Text ?? "";
                    else 
                        return null;
                });
            }

            //Replace htabs
            builder.Replace("<ht>", ASCII.HT);
            //Replace vtabs
            builder.Replace("<vt>", ASCII.VT);
            //Replace line breaks
            builder.Replace("<br>", ASCII.LF + ASCII.CR);
            //End of form
            builder.Append(ASCII.FF);

            var docText = builder.ToString();
            PrinterQueue.AddJob(printerName, printDocName, docText);
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
////static void Main(string[] args)
////{
////    List<PrintLine> lines= new List<PrintLine>();
////    lines.Add(new PrintLine() { X = 600, Y = 000, Text = "11228", Description = "RetailerID" });
////    lines.Add(new PrintLine() { X = 1300, Y = 520, Text = "1-3 Bld de la Madeleine", Description = "Line1" });
////    lines.Add(new PrintLine() { X = 1300, Y = 620, Text = "75001", Description = "Line5" });
////    lines.Add(new PrintLine() { X = 1800, Y = 620, Text = "PARIS", Description = "Line3" });
////    lines.Add(new PrintLine() { X = 1300, Y = 720, Text = "Tel : 01 42 61 11 87", Description = "Phone" });

////    lines.Add(new PrintLine() { X = 3050, Y = 520, Text = "A0021366878", Description = "VoucherID" });
////    lines.Add(new PrintLine() { X = 1600, Y = 1020, Text = "332037662", Description = "VATNumber" });
////    lines.Add(new PrintLine() { X = 1300, Y = 1300, Text = "* BOUTIQUE J-M WESTON ******************", Description = "ShopName" });
////    lines.Add(new PrintLine() { X = 1300, Y = 1400, Text = "* 1-3 Bld de la Madeleine 75001 PARIS  *", Description = "TextLine2" });
            
////    VoucherPrintRaw voucherPrint = new VoucherPrintRaw();
////    voucherPrint.PrintVoucher("Tally T2365", "DGB8262015294824022390881", 6119, "", lines);
////    Console.Read();
////}
