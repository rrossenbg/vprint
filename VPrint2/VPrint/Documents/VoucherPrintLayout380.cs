using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace VPrinting.Documents
{
    /// <summary>
    /// an old code
    /// </summary>
    [Serializable]
    public class VoucherPrintLayout380 : VoucherPrintLayout826
    {
        public PrintLine VATLine { get; set; }
        public PrintLine RegLine { get; set; }
        public PrintLine CapLine { get; set; }
        public PrintLine ReaLine { get; set; }

        [XmlIgnore]
        public override List<IPrintLine> PrintLines
        {
            get
            {
                List<IPrintLine> list = new List<IPrintLine>(base.PrintLines)
                    {
                        VATLine,
                        RegLine,
                        CapLine,
                        ReaLine,
                    };
                list.Sort(new PrintLineComparer());
                return list;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public VoucherPrintLayout380()
            : base()
        {
            LINE_LEN_LIMIT = 25;

            VATLine = new PrintLine("VAT");
            RegLine = new PrintLine("Reg");
            CapLine = new PrintLine("Cap");
            ReaLine = new PrintLine("Rea");
        }

        public override void DataBind(IDataProvider pr, string strVoucherNo, int voucher, bool printDemo)
        {
            string[] strings;

            if (this.m_PrinterPrintBarcode)
            {
                BarCodeNo.Text = strVoucherNo.Replace(" ", "").Substring(3);
                BarCodeText.Text = strVoucherNo;
            }

            ShopName.Text = pr.Retailer.TradingName.TrimSafe();
            VoucherID.Text = voucher + pr.Printing.CalculateCheckDigit(voucher);
            ShopNo.Text = pr.Retailer.Id.ToString();

            strings = new string[] { 
                            pr.Retailer.RetailAddress.Line1.Limit(LINE_LEN_LIMIT, null), 
                            pr.Retailer.RetailAddress.Line2.Limit(LINE_LEN_LIMIT, null), 
                              /*Line4 is not used here*/ 
                            (pr.Retailer.RetailAddress.Line5 + "-" + pr.Retailer.RetailAddress.Line3).Limit(LINE_LEN_LIMIT, null)};

            new PrintLine[] { 
                        Line0, 
                        Line1, 
                        Line2 }.ProcessPairs(
                strings,
                new Predicate<string>((str) => string.IsNullOrEmpty(str.TrimSafe())),
                new Action<PrintLine, string>((pl, str) => pl.Text = str.TrimSafe()));

            if (this.m_PrintHeadOfficeDetails)
            {
                var HeadOffice = pr.Manager.RetrieveHeadOfficeDetail(pr.Allocation.CountryId, pr.Allocation.HeadOfficeId);

                RegLine.Text = "Reg. Imp.:".concat(HeadOffice.CertificationCode1);
                CapLine.Text = "Cap. Soc.:".concat(HeadOffice.CertificationCode2);
                ReaLine.Text = "N. Rea   :".concat(HeadOffice.CertificationCode3);
            }

            if (this.m_ShowRetailerVatRate)
            {
                VATLine.Text = "VAT No ".concat(pr.Retailer.VatNumber);
            }
        }
    }
}
