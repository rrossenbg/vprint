/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Collections.Generic;

namespace DEMATLib
{
    /// <summary>
    /// Build xml
    /// </summary>
    /// <example>
    /// var b = new BDV_InvoiceBuilder();
    /// 
    /// b.SetBuyer("test1", "test2", "add1", "add2", "add3", "city", "code", "123456");
    /// b.SetError("Test error message");
    ///        
    /// b.SetRetailer("ret123", "rname1", "rname2", "addr1", "addr2", "addr3", "city", "rpcode", "rvar");
    /// b.SetInvoiceTotal(100);
    /// b.SetTotal(1.1m, 2.2m, 3.3m, 4.4m);
    /// b.SetVoucherDetails("12345678", DateTime.Now);
    ///    
    /// for (int i = 0; i < 3; i++)
    ///     b.SetVoucherLine(i.ToString(), string.Format("{0} line {0}", i), 2.2m, "2", 1.1m, 2.2m, 3.3m, 4.4m);
    ///    
    /// var xml = b.CreateXML();
    /// File.WriteAllText("C:\\text.xml", xml);
    /// </example>
    public class BDV_InvoiceBuilder : IXMLBuilder
    {
        private int m_VoucherNumber;
        private readonly BDV_Invoice invoice = new BDV_Invoice();

        public BDV_InvoiceBuilder(bool valid)
        {
            invoice.FileHeader = new BDV_InvoiceFileHeader();
            invoice.FileHeader.FileType = valid ? filetype.FACTURE : filetype.AVOIR;
            invoice.FileHeader.FileCreationDate = DateTime.Now;
            invoice.FileHeader.FileCreationDateSpecified = true;
            invoice.FileHeader.FileFormatVersion = "1.0";

            invoice.Error = new BDV_InvoiceError();
            invoice.Retailer = new BDV_InvoiceRetailer();
            invoice.Buyer = new BDV_InvoiceBuyer();
            invoice.VoucherDetails = new BDV_InvoiceVoucherDetails();
            invoice.LineDetails = new List<BDV_InvoiceLineItem>();
            invoice.Total = new BDV_InvoiceTotal();
            invoice.Total.TotalPerVAT = new List<BDV_InvoiceTotalTotalPerVAT>();
            invoice.TouristDetails = new BDV_InvoiceTouristDetails();
        }

        public void SetError(string message)
        {
            invoice.Error.Description = message;
        }

        public void SetRetailer(int code, string name1, string name2,
            string addr1, string addr2, string addr3, string city, string postCode,
            string vatNumber)
        {
            invoice.Retailer.Retailer_Code = code.ToString(6);
            invoice.Retailer.Retailer_Name1 = name1;
            invoice.Retailer.Retailer_Name2 = name2;
            invoice.Retailer.Retailer_Address1 = addr1;
            invoice.Retailer.Retailer_Address2 = addr2;
            invoice.Retailer.Retailer_Address3 = addr3;
            invoice.Retailer.Retailer_City = city;
            invoice.Retailer.Retailer_PostalCode = postCode;
            invoice.Retailer.Retailer_VATNumber = vatNumber;
        }

        public void SetBuyer(string name1, string name2, 
            string addr1, string addr2, string addr3, string city, string postCode, 
            string vatNumber)
        {
            invoice.Buyer.Buyer_Name1 = name1;
            invoice.Buyer.Buyer_Name2 = name2;
            invoice.Buyer.Buyer_Address1 = addr1;
            invoice.Buyer.Buyer_Address2 = addr2;
            invoice.Buyer.Buyer_Address3 = addr3;
            invoice.Buyer.Buyer_City = city;
            invoice.Buyer.Buyer_PostalCode = postCode;
            invoice.Buyer.Buyer_VATNumber = vatNumber;
        }

        public void SetVoucherDetails(int number, DateTime date)
        {
            m_VoucherNumber = number;
            invoice.VoucherDetails.VoucherNumber = number.ToString();
            invoice.VoucherDetails.PurchaseDate = date;
        }

        public void SetTouristDetails(string touristName, string touristCountry, string touristPassport, 
            decimal refundAmount, bool refundAmountSpecified, string refundDescription)
        {
            invoice.TouristDetails.TouristName = touristName;
            invoice.TouristDetails.TouristCountry = touristCountry;
            invoice.TouristDetails.TouristPassport = touristPassport;
            invoice.TouristDetails.RefundAmount = refundAmount;
            invoice.TouristDetails.RefundAmountSpecified = refundAmountSpecified;
            invoice.TouristDetails.RefundDescription = refundDescription;
        }

        public void SetVoucherLine(string number, string description,
            decimal lineUnitPrice, string lineQuantity,
            decimal vatRate, decimal exVAT_LineAmount, decimal total_LineAmount, decimal vat_LineAmount)
        {
            var item = new BDV_InvoiceLineItem()
            {
                LineNumber = number,
                Description = description,
                LineUnitPrice = lineUnitPrice,
                LineQuantity = lineQuantity,
                VATRate = vatRate,
                ExVAT_LineAmount = exVAT_LineAmount,
                Total_LineAmount = total_LineAmount,
                VAT_LineAmount = vat_LineAmount,
            };

            invoice.LineDetails.Add(item);
        }

        public void SetInvoiceTotal(decimal value)
        {
            invoice.Total.InvoiceTotal = value;
        }

        public void SetTotalPerVAT(decimal vatRate, decimal vat_Amount, decimal exVAT_Amount, decimal total_Amount)
        {
            var item = new BDV_InvoiceTotalTotalPerVAT();
            item.VATRate = vatRate;
            item.VAT_Amount = vat_Amount;
            item.ExVAT_Amount = exVAT_Amount;
            item.Total_Amount = total_Amount;
            invoice.Total.TotalPerVAT.Add(item);
        }

        public string CreateFileName(long voucherSequenceNumber)
        {
#if OLD
            var fileName = string.Concat(invoice.Retailer.Retailer_Code.ToString(6), voucherSequenceNumber.ToString(8), ".xml");
            return fileName;
#else
            var vnumberNoCd = m_VoucherNumber / 10;

            if ((vnumberNoCd).CheckDigit() == m_VoucherNumber)
            {
                //It has check digit
                var fileName = string.Concat(vnumberNoCd, ".xml");
                return fileName;
            }
            else
            {
                //It hasn't got check digit
                var fileName = string.Concat(m_VoucherNumber, ".xml");
                return fileName;
            }

#endif
        }

        public string CreateXML()
        {
            var xml = invoice.Serialize();
            return xml;
        }
    }
}
