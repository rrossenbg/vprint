/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System.Collections.Generic;
using System.Drawing.Printing;

namespace VPrinting
{
    public static class PrintManager
    {
        public static string GetDefaultPrinterName()
        {
            PrinterSettings settings = new PrinterSettings();
            return settings.PrinterName;
        }

        public static IEnumerable<string> GetInstalledPrinters()
        {
            foreach (string printer in PrinterSettings.InstalledPrinters)
                yield return printer;
        }
    }
}
