/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using PremierTaxFree.Data.Objects;

namespace PremierTaxFree.PTFLib.Data
{
    /// <summary>
    /// Voucher data and images
    /// </summary>
    public class Voucher : IDisposable
    {
        /// <summary>
        /// 50L
        /// </summary>
        private const long DEFAULT_COMPRESION_LEVEL = 50L;
        private static readonly byte[] EmptyData = { 0 };

        #region DATA PROPERTIES
       
        /// <summary>
        /// Barcode string
        /// </summary>
        public string BarCodeString { get; set; }

        /// <summary>
        /// Country code number
        /// </summary>
        public int CountryID { get; set; }

        /// <summary>
        /// RetailerID
        /// </summary>
        public int RetailerID { get; set; }

        /// <summary>
        /// VoucherID
        /// </summary>
        public string VoucherID { get; set; }

        /// <summary>
        /// SiteCode
        /// </summary>
        public string SiteCode { get; set; }

        /// <summary>
        /// Operator's commend before scanning starts
        /// </summary>
        public string Comment { get; set; }
        
        /// <summary>
        /// Image of voucher
        /// </summary>
        public Bitmap VoucherImage { get; set; }

        /// <summary>
        /// Image of barcode
        /// </summary>
        public Image BarCodeImage { get; set; }

        /// <summary>
        /// Aditional data. Not used.
        /// </summary>
        public byte[] Data { get; set; }
        
        #endregion

        #region HELPING PROPERTIES

        public Rectangle BarCodeArea { get; set; }
        public string Message { get; set; }
        public long CompressionLevel { get; set; }
        public Stopwatch ProcessTime { get; set; }

        #endregion

        public Voucher()
        {
            CompressionLevel = DEFAULT_COMPRESION_LEVEL;
            Comment = string.Empty;
            SiteCode = string.Empty;
            ProcessTime = Stopwatch.StartNew();
            BarCodeImage = null;
            Data = EmptyData;
        }

        ~Voucher()
        {
            Dispose(false);
        }

        /// <summary>
        /// Validate object data
        /// </summary>
        public void Validate()
        {
            if (SiteCode == null)
                throw new ApplicationException("SiteCode is null.");
            if (CountryID <= 0)
                throw new ApplicationException("CountryCode supposed to be more than 0.");
            if (RetailerID <= 0)
                throw new ApplicationException("RetailerID supposed to be more than 0.");
            if (VoucherID == null)
                throw new ApplicationException("VoucherID is null.");
            if (VoucherImage == null)
                throw new ApplicationException("VoucherImage is null.");
        }

        public static explicit operator DbClientVoucher(Voucher voucher)
        {
            DbClientVoucher dbVoucher = new DbClientVoucher()
            {
                CountryID = voucher.CountryID,
                RetailerID = voucher.RetailerID,
                VoucherID = voucher.VoucherID,
                BarCode = voucher.BarCodeString,
                VoucherImage = voucher.VoucherImage.ToArray(),
                BarCodeImage = voucher.BarCodeImage.ToArray(),
                SiteCode = voucher.SiteCode,
            };
            return dbVoucher;
        }

        /// <summary>
        /// For debugging 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.AppendLine("============================");
            b.AppendFormat("CountryCode:\t{0}", CountryID);
            b.AppendFormat("Business:\t{0}", "TODO");
            b.AppendFormat("RetailerID:\t{0}", RetailerID);
            b.AppendFormat("VoucherID:\t{0}", VoucherID);
            b.AppendFormat("SiteCode:\t{0}", SiteCode);
            b.AppendLine(Message);
            b.AppendLine("============================");
            return b.ToString();
        }

        public void Parse(string str)
        {
            if (string.IsNullOrEmpty(str))
                throw new ApplicationException("Wrong or missing barcode string.");

            this.CountryID = int.Parse(str.Substring(0, 3));
            //this.Business = int.Parse(str.Substring(3, 2));
            this.RetailerID = int.Parse(str.Substring(5, 6));
            this.VoucherID = str.Substring(11);
            this.BarCodeString = str;
        }

        /// <summary>
        /// Free object
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            VoucherImage = VoucherImage.DisposeSf();
            BarCodeImage = BarCodeImage.DisposeSf();

            if (disposing)
                GC.SuppressFinalize(this);
        }
    }
}