using System;
using System.Data;
using PremierTaxFree.PTFLib.Data;

namespace PremierTaxFree.Data.Objects
{
    public class DbClientVoucher: IReadable
    {
        public int CountryID { get; set; }
        public int RetailerID { get; set; }
        public string VoucherID { get; set; }
        public string SiteCode { get; set; }
        public string BarCode { get; set; }
        
        public byte[] VoucherImage { get; set; }
        public byte[] BarCodeImage { get; set; }
        public DateTime DateAllocated { get; set; }
        public DateTime DateScanned { get; set; }
        public DateTime DateInserted { get; set;}

        public void Load(IDataReader reader)
        {
            throw new NotImplementedException();
        }
    }
}
