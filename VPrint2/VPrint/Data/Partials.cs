namespace VPrinting.VoucherNumberingAllocationPrinting
{
    public partial class PrinterFormat
    {
        public override string ToString()
        {
            return this.Name;
        }
    }
    public partial class VoucherNumberingAllocationPrinting
    {
    }
}

namespace VPrinting.PartyManagement
{
    public partial class PrinterDetails : IKeyable
    {
        public int IsoID { get; set; }

        public int RetailerID { get; set; }

        public bool IsEmpty
        {
            get
            {
                return string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Xml) || string.IsNullOrEmpty(Type2);
            }
        }

        public System.Guid GetKey()
        {
            return VPrinting.Common.CommonTools.ToGuid(IsoID, RetailerID);
        }

        public void SetKey(System.Guid key)
        {
            int iso_id, ret_id, i1, i2;
            VPrinting.Common.CommonTools.FromGuid(key, out iso_id, out ret_id, out i1, out i2);
            IsoID = iso_id;
            RetailerID = ret_id;
        }
    }

    public partial class CountryDetail
    {
        public override string ToString()
        {
            return string.Concat(this.Iso3, " (", this.Number, ")");
        }
    }

    public partial class HeadOffice
    {
        public override string ToString()
        {
            return string.Concat(this.Name, " (", this.Id, ")");
        }
    }

    public partial class Retailer
    {
        public override string ToString()
        {
            return string.Concat(this.Name, " (", this.Id, ")");
        }
    }

    public partial class PartyManagment
    {
    }
}
