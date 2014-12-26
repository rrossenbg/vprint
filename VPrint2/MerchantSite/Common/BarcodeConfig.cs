using System;

namespace MerchantSite.Common
{
    public class HouseOfFrazerBarcodeConfig : BarcodeConfig
    {
        public HouseOfFrazerBarcodeConfig()
        {
            Length = 20;
            VoucherID = new Tuple<int, int>(0, 9);
            CountryID = new Tuple<int, int>(9, 3);
            BuzType = new Tuple<int, int>(12, 2);
            RetailerID = new Tuple<int, int>(14, 6);
            Template = "{0:00000000}{1:000}{2:00}{3:000000}";
            Sample = "012345678 826 20 012345";
        }

        public override bool ParseBarcode(string barcode, ref BarcodeData data)
        {
            if (barcode.IndexOf("82620") != 9)
                return false;

            return base.ParseBarcode(barcode, ref data);
        }
    }

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
        public virtual bool ParseBarcode(string barcode, ref BarcodeData data)
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
    }
}
