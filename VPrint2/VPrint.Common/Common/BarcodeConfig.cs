using System;
using System.Collections.Generic;

namespace VPrinting.Common
{
    /// <summary>
    /// 
    /// </summary>
    /// <examle>
    /// CountryID   0, 3
    /// Buz         3, 2
    /// RetailerID  5, 6
    /// VoucherID   11,-1
    /// </examle>
    /// <example>
    /// Gucci
    /// VoucherID 0, 9
    /// CountryID 9, 3
    /// RetailerID 12, 6
    /// BuzType -1,-1
    /// </example>
    [Serializable]
    public class BarcodeConfig
    {
        public string Name { get; set; }
        /// <summary>
        /// With out check digits
        /// </summary>
        public int Length { get; set; }
        public bool HasCheckDigit { get; set; }
        public string Template { get; set; }
        public string Sample { get; set; }
        public Tuple<int, int> CountryID { get; set; }
        public Tuple<int, int> BuzType { get; set; }
        public Tuple<int, int> RetailerID { get; set; }
        public Tuple<int, int> VoucherID { get; set; }

        public override string ToString()
        {
            return string.Concat(Name, " {", Sample, "}");
        }

        public string ToString(int countryID, int bizType, int retailerID, int voucherId)
        {
            if (string.IsNullOrWhiteSpace(Template))
                return string.Empty;

            return string.Format(Template, countryID, bizType, retailerID, voucherId);
        }


        public void Test()
        {
            if (string.IsNullOrEmpty(Sample))
                throw new Exception("Barcode sample not valid");

            BarcodeData data = null;
            if (!ParseBarcode(Sample.Replace(" ", ""), ref data))
                throw new Exception("Barcode template not valid");

            data.Test();
        }

        /// <summary>
        /// 001977684 056 100353
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns></returns>
        public bool ParseBarcode(string barcode, ref BarcodeData data)
        {
            if (string.IsNullOrWhiteSpace(barcode))
                throw new ArgumentException("empty barcode", "barcode");

            if (barcode.Length != this.Length)
                return false;

            //Remove check digits if any
            var voucherId =
                this.VoucherID.Item2 <= 0 ?
                int.Parse(barcode.Substring(this.VoucherID.Item1)) :
                int.Parse(barcode.Substring(this.VoucherID.Item1, this.VoucherID.Item2));

            var countryId =
                this.CountryID.Item2 <= 0 ?
                int.Parse(barcode.Substring(this.CountryID.Item1)) :
                int.Parse(barcode.Substring(this.CountryID.Item1, this.CountryID.Item2));

            var retailerId = this.RetailerID != null ?
                this.RetailerID.Item2 <= 0 ?
                int.Parse(barcode.Substring(this.RetailerID.Item1)) :
                int.Parse(barcode.Substring(this.RetailerID.Item1, this.RetailerID.Item2)) :
                0;

            data = new BarcodeData(countryId, retailerId, voucherId, barcode);
            return true;
        }

        public bool ParseBarcodeSafe(string barcode, ref BarcodeData data)
        {
            try
            {
                return ParseBarcode(barcode, ref data);
            }
            catch
            {
                return false;
            }
        }

        public static void Init()
        {
            var list = new List<BarcodeConfig>()
            {
                 new BarcodeConfig()
                {
                    Name = "CCC-SS-RRRRRR-VVVVVVVVV",
                    Length = 20,
                    //iso, ty, br, voucher
                    Template = "{0:000}{1:00}{2:000000}{3:00000000}",
                    Sample = "012 01 012345 012345678",
                    CountryID = new Tuple<int,int>(0, 3),
                    BuzType = new Tuple<int,int>(3, 2),
                    RetailerID = new Tuple<int,int>(5, 6),
                    VoucherID = new Tuple<int,int>(11, 9),
                },
                new BarcodeConfig()
                {
                    Name = "CCC-RRRRRR-VVVVVVVVV",
                    Length = 18,
                    //iso, ty, br, voucher
                    Template = "{0:000}{2:000000}{3:00000000}",
                    Sample = "012 012345 012345678",
                    CountryID = new Tuple<int,int>(0, 3),
                    RetailerID = new Tuple<int,int>(3, 6),
                    VoucherID = new Tuple<int,int>(9, 9),
                },
                new BarcodeConfig()
                {
                    Name = "VVVVVVVVV-CCC-SS",
                    Length = 14,
                    //iso, ty, br, voucher
                    Template = "{3:000000000}{0:000}{1:00}",
                    Sample = "012345678 012 01",
                    VoucherID = new Tuple<int,int>(0, 9),
                    CountryID = new Tuple<int,int>(9, 3),
                    BuzType = new Tuple<int,int>(12,2),
                },
                new BarcodeConfig()
                {
                    Name = "CCC-SS-RRRRRR-VVVVVVVVV-AAAAAAAAAAA",
                    Length = 31,
                    //iso, ty, br, voucher
                    Template = "{0:000}{2:000000}{3:00000000}",
                    Sample = "012 01 012345 012345678 01234567890",
                    CountryID = new Tuple<int,int>(0, 3),
                    BuzType = new Tuple<int,int>(3, 2),
                    RetailerID = new Tuple<int,int>(5, 6),
                    VoucherID = new Tuple<int,int>(11, 9),
                },
            };
            StateSaver.Default.Set(Strings.LIST_OF_BARCODECONFIGS, list);
        }
    }
}
