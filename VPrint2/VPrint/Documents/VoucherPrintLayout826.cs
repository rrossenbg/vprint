/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Xml.Serialization;

namespace VPrinting.Documents
{
    /// <summary>
    /// Direct complex printing. Prints barcodes
    /// </summary>
    ///<remarks>
    /// Contains document settings
    /// </remarks>
    /// <example>
    /// [4032`[1d[?11~!C1;000:82620152948233631823;[?10~
    /// [4304`[221dDGB82620152948233631823
    /// [4002`[2901d1 Et 1 Font 3
    /// [5845`[2950d233631823
    /// [4002`[3001d54 Ledbury Road
    /// [4002`[3103dWestbourne Grove
    /// [4002`[3204dLondon
    /// [5845`[3207d152948
    /// [4002`[3306dW11 2AG
    /// </example>
    [Serializable]
    public class VoucherPrintLayout826 : VoucherPrinterSettings, IVoucherLayout
    {
        /// <summary>
        /// 15 chars
        /// </summary>
        protected int LINE_LEN_LIMIT = 15; //Chars

        public string DocumentInitialization { get; set; }

        public int FormLength { get; set; }
        public BarCodeLine BarCodeNo { get; set; }
        public PrintLine BarCodeText { get; set; }
        public PrintLine ShopName { get; set; }
        public PrintLine VoucherID { get; set; }
        public PrintLine ShopNo { get; set; }
        public PrintLine Line0 { get; set; }
        public PrintLine Line1 { get; set; }
        public PrintLine Line2 { get; set; }
        public PrintLine Line3 { get; set; }
        public PrintLine VATNumber { get; set; }
        public Point MoveAll { get; set; }

        [XmlIgnore]
        public virtual List<IPrintLine> PrintLines
        {
            get
            {
                List<IPrintLine> list = new List<IPrintLine>()
                    {
                        BarCodeNo,
                        BarCodeText,
                        ShopName,
                        Line0,
                        Line1,
                        Line2,
                        Line3,
                        VoucherID,
                        ShopNo,
                        VATNumber
                    };
                list.Sort(new PrintLineComparer());
                return list;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public VoucherPrintLayout826()
        {
            BarCodeNo = new BarCodeLine("BarCodeNo");
            BarCodeText = new PrintLine("BarCodeText");
            ShopName = new PrintLine("ShopName");
            VoucherID = new PrintLine("VoucherID");
            ShopNo = new PrintLine("ShopNo");
            Line0 = new PrintLine("Line0");
            Line1 = new PrintLine("Line1");
            Line2 = new PrintLine("Line2");
            Line3 = new PrintLine("Line3");
            VATNumber = new PrintLine("VATNumber");
            DocumentInitialization = string.Empty;
        }

        public void Init()
        {
        }

        public virtual void Clear()
        {
            BarCodeNo.Text =
            BarCodeText.Text =
            ShopName.Text =
            Line0.Text =
            Line1.Text =
            Line2.Text =
            Line3.Text =
            VoucherID.Text =
            VATNumber.Text =
            ShopNo.Text = string.Empty;
        }

        /// <summary>
        /// Loads this object with concrete data
        /// </summary>
        /// <param name="pr"></param>
        /// <param name="strVoucherNo">DGB8262015294824022390881</param>
        /// <param name="voucher">152948240</param>
        /// <param name="printDemo">false</param>
        public virtual void DataBind(IDataProvider pr, string strVoucherNo, int voucher, bool printDemo)
        {
            string[] strings;

            if (printDemo)
            {
                BarCodeNo.Text = "82620152948233631823";
                BarCodeText.Text = "DGB82620152948233631823";
                ShopName.Text = "Demo Shop Name";
                VoucherID.Text = "1234567";
                ShopNo.Text = "123456";
                VATNumber.Text = "VAT No 0123456789";

                strings = new string[] { 
                            "Address line number 1", 
                            "Address line number 2", 
                            "Address line number 3",
                            "Address line number 4"};
            }
            else
            {
                BarCodeNo.Text = strVoucherNo.Replace(" ", "").Substring(3);
                BarCodeText.Text = strVoucherNo.Replace(" ", "");
                ShopName.Text = pr.Retailer.TradingName.TrimSafe();
                VoucherID.Text = voucher + pr.Printing.CalculateCheckDigit(voucher);
                ShopNo.Text = pr.Retailer.Id.ToString();
                VATNumber.Text = "VAT No {0}".format(pr.Retailer.VatNumber);

                strings = new string[] { 
                            pr.Retailer.RetailAddress.Line1.Limit(LINE_LEN_LIMIT, null), 
                            pr.Retailer.RetailAddress.Line2.Limit(LINE_LEN_LIMIT, null), 
                            pr.Retailer.RetailAddress.Line3.Limit(LINE_LEN_LIMIT, null),
                            /*Line4 is not in use here*/ 
                            pr.Retailer.RetailAddress.Line5.Limit(LINE_LEN_LIMIT, null)};
            }

            new PrintLine[] { 
                        Line0, 
                        Line1, 
                        Line2, 
                        Line3 }.ProcessPairs(
                strings,
                new Predicate<string>((str) => string.IsNullOrEmpty(str.TrimSafe())),
                new Action<PrintLine, string>((pl, str) => pl.Text = str.TrimSafe()));
        }

        public void InitPrinter(string printDoc)
        {
        }

        public void PrintVoucher(string printerName, string printDocumentName, int length, string documentInitialization, IList<IPrintLine> lines)
        {
            Debug.Assert(!string.IsNullOrEmpty(printerName));

            var b = new StringBuilder();

            b.Append(MTPL.PUMOn(true));
            b.Append(MTPL.SetFormLength(length));//6119
            b.AppendLine(documentInitialization);

            PrintLine lastLine = null;

            const int MIN_V_STEP = 50;

            foreach (PrintLine line in lines)
            {
                if (lastLine == null || Math.Abs(line.Y - lastLine.Y) > MIN_V_STEP)
                {
                    b.Append(MTPL.SetAbsolutePosition((int)line.X, (int)line.Y));
                }
                else
                {
                    b.Append(MTPL.SetAbsoluteHorizontalPosition((int)line.X));
                }

                line.Print(b);
                lastLine = line;
            }

            b.Append(ASCII.FF);

            string text = b.ToString();
            PrinterQueue.AddJob(printerName, printDocumentName, text);
        }

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            foreach (var line in PrintLines)
                b.AppendLine(line.ToString());
            return b.ToString();
        }

        public void PrintVouchers(string printerName, string printDocName, int length, string docInitialization, List<IList<IPrintLine>> multilines)
        {
            throw new NotImplementedException();
        }
    }
}
