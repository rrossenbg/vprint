/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/

using System;
using System.Data;
using System.Text;

namespace PremierTaxFree.PTFLib.Data.Objects.Server
{
    public class DbVoucher : IReadable
    {
        public int FileID { get; set; }
        public int ClientID { get; set; }
        public int CountryID { get; set; }
        public int RetailerID { get; set; }
        public string VoucherID { get; set; }
        public string SiteCode { get; set; }
        public string Comment { get; set; }
        
        public byte[] VoucherImage { get; set; }
        public byte[] BarCodeImage { get; set; }
        public DateTime DateAllocated { get; set; }
        public DateTime DateScanned { get; set; }
        public DateTime DateInserted { get; set;}

        public void Load(IDataReader reader)
        {
            FileID = reader.GetValue<int>("FileID");
            ClientID = reader.GetValue<int>("ClientID");
            CountryID = reader.GetValue<int>("CountryID");
            RetailerID = reader.GetValue<int>("RetailerID");
            VoucherID = reader.GetValue<string>("VoucherID");

            SiteCode = reader.GetValue<string>("SiteCode");
            Comment = reader.GetValue<string>("Comment");
            
            VoucherImage = reader.GetBytes("VoucherImage");
            BarCodeImage = reader.GetBytes("BarCodeImage");

            DateAllocated = reader.GetValue<DateTime>("DateAllocated");
            DateScanned = reader.GetValue<DateTime>("DateScanned");
            DateInserted = reader.GetValue<DateTime>("DateInserted");
        }

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.AppendFormat("FileID:\t\t{0}\r\n", FileID);
            b.AppendFormat("ClientID:\t\t{0}\r\n", ClientID);
            b.AppendFormat("CountryID:\t\t{0}\r\n", CountryID); 
            b.AppendFormat("RetailerID:\t{0}\r\n", RetailerID);
            b.AppendFormat("VoucherID:\t{0}\r\n", VoucherID);
            b.AppendFormat("CountryCode:\t{0}\r\n", SiteCode);
            b.AppendFormat("DateScanned:\t{0}\r\n", DateScanned);
            b.AppendFormat("DateInserted:\t{0}\r\n", DateInserted);
            return b.ToString();
        }
    }
}
