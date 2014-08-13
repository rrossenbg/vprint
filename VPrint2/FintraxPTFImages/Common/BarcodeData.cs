
using System;

namespace FintraxPTFImages.Common
{
    public class BarcodeData
    {
        public int CountryID;
        public int RetailerID;
        public int VoucherID;
        public string Barcode;

        public BarcodeData(int countryId, int retailerId, int voucherId, string barcode)
        {
            CountryID = countryId;
            RetailerID = retailerId;
            VoucherID = voucherId;
            Barcode = barcode;
        }

        public void Test()
        {
            if (CountryID != 012)
                throw new ArgumentException("CountryID");
            if (RetailerID != 0 && RetailerID != 012345)
                throw new ArgumentException("RetailerID");
            if (VoucherID != 012345678)
                throw new ArgumentException("VoucherID");
        }
    }
}