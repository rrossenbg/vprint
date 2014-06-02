/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/

using System;
using System.Data;

namespace PremierTaxFree.PTFLib.Data.Objects
{
    /// <summary>
    /// Client data object
    /// </summary>
    /// <example>SELECT * FROM FILES WHERE FileID = @FileID;</example>
    public class DbClientFileInfo : IReadable
    {
        public int FileID { get; private set; }
        public int CountryID { get; private set; }
        public int RetailerID { get; private set; }
        public string VoucherID { get; private set; }
        public string SiteCode { get; private set; }
        public string BarCode { get; private set; }
        public byte[] VoucherImage { get; private set; }
        public byte[] BarCodeImage { get; private set; }
        public DateTime DateInserted { get; private set; }
        public DateTime DateAllocated { get; private set; }
        public string Comment { get; private set; }

        public void Load(IDataReader reader)
        {
            FileID = reader.GetValue<int>("FileID");
            CountryID = reader.GetValue<int>("CountryID");
            RetailerID = reader.GetValue<int>("RetailerID");
            VoucherID = reader.GetValue<string>("VoucherID");
            SiteCode = reader.GetValue<string>("SiteCode");
            BarCode = reader.GetValue<string>("BarCode");
            VoucherImage = reader.GetValue<byte[]>("VoucherImage");
            BarCodeImage = reader.GetValue<byte[]>("BarCodeImage");
            DateInserted = reader.GetValue<DateTime>("DateInserted");
            DateAllocated = reader.GetValue<DateTime>("DateAllocated");
            Comment = reader.GetValue<string>("Comment");
        }
    }
}
