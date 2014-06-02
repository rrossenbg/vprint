/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Threading;

namespace PremierTaxFree.Data
{
    /// <summary>
    /// Saves current scan operation data
    /// </summary>
    public class CurrentScanObj
    {
        private eVouchersScanType m_VoucherScanType;
        private string m_SiteCodeString;
        private int m_VoucherStartNumber;
        public string Comment { get; set; }
        public Guid BatchID { get; set; }

        public string SiteCode
        {
            get
            {
                return m_VoucherScanType == eVouchersScanType.AfterInsertionDomestic ? m_SiteCodeString : string.Empty;
            }
        }

        public CurrentScanObj()
        {
        }

        public CurrentScanObj(eVouchersScanType type, string siteCode, int startFrom)
        {
            m_VoucherScanType = type;
            m_SiteCodeString = siteCode;
            Interlocked.Exchange(ref m_VoucherStartNumber, startFrom);
        }

        public int GetNext()
        {
            return Interlocked.Increment(ref m_VoucherStartNumber);
        }

        public void SetNew(eVouchersScanType type, string siteCode, int startFrom, string comment)
        {
            m_VoucherScanType = type;
            m_SiteCodeString = siteCode;
            BatchID = Guid.NewGuid();
            Interlocked.Exchange(ref m_VoucherStartNumber, startFrom);
            Comment = comment;
        }
    }

    public enum eVouchersScanType
    {
        NotSet = 0,
        BeforeInsertion = 1,
        AfterInsertionDomestic = 2,
        AfterInsertionForeign = 3
    }
}
