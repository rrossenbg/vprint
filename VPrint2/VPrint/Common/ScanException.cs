/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;

namespace VPrinting.Common
{
    [Serializable]
    public class ScanException : ApplicationException
    {
        public BarcodeData BarData { get; set; }
        public string SiteCode { get; set; }
        public string FilePath { get; set; }

        public ScanException(Exception ex, BarcodeData data)
            : base(ex.Message, ex)
        {
            BarData = data;
            SiteCode = "";
            FilePath = "";
        }
    }
}
