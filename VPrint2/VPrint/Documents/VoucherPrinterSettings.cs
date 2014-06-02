/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Configuration;

namespace VPrinting.Documents
{
    public abstract class VoucherPrinterSettings
    {
        #region SETTINGS

        public bool m_PrintHeadOfficeDetails = Convert.ToBoolean(ConfigurationManager.AppSettings["PrintHeadOfficeDetails"]);
        public bool m_ShowRetailerVatRate = Convert.ToBoolean(ConfigurationManager.AppSettings["ShowRetailerVatRate"]);
        public bool m_PrinterPrintBarcode = Convert.ToBoolean(ConfigurationManager.AppSettings["PrinterPrintBarcode"]);
        public string m_ReportType2 = ConfigurationManager.AppSettings["ReportType2"];
        public string m_PrinterName = ConfigurationManager.AppSettings["PrinterName"];
        public string m_PrinterXmlFilePath = ConfigurationManager.AppSettings["PrinterXmlFilePath"];

        #endregion
    }
}
