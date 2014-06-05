/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

namespace CPrint2.PartyManagementRef
{
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
