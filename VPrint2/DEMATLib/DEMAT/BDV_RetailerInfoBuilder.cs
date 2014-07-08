/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;

namespace DEMATLib
{
    public class BDV_RetailerInfoBuilder : IXMLBuilder
    {
        private readonly BDV_RetailerInfo invoice = new BDV_RetailerInfo();

        public BDV_RetailerInfoBuilder()
        {
            invoice.FileHeader = new BDV_RetailerInfoFileHeader
            {
                FileCreationDate = DateTime.Now,
                FileFormatVersion = 1.0m,
                FileType = "INFO",
            };
        }

        public void AddError(string message)
        {
            invoice.Error = new BDV_RetailerInfoError
            {
                Description = message
            };
        }

        public void AddRetailer(int retailerId, DateTime? date, string email, bool? enabled)
        {
            var item = new BDV_RetailerInfoRetailer
            {
                Retailer_Code = retailerId.ToString(),
                Retailer_Contract_Date = date.HasValue ? date.ToString() : "",
                Retailer_Contract_Email = email,
                Retailer_Enable_Export = enabled.GetValueOrDefault(),
            };
            invoice.Retailer.Add(item);
        }

        public string CreateFileName(long voucherSequenceNumber)
        {
            return string.Format("RetailerInfo_{0:yyyy_MM_dd}.xml", DateTime.Now);
        }

        public string CreateXML()
        {
            return invoice.Serialize();
        }
    }
}
