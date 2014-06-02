/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

namespace PremierTaxFree.PTFLib
{
    public enum eVoucherScanType
    {
        /// <summary>
        /// Continue previous job
        /// </summary>
        ContinuePreviousScan = 0,
        /// <summary>
        /// Brand new vouchers
        /// </summary>
        BrandNew,
        /// <summary>
        /// Domestic rescan vouchers
        /// </summary>
        DomesticRescan,
        /// <summary>
        /// Foreign rescan vouchers
        /// </summary>
        ForeignRescan,
    }
}
