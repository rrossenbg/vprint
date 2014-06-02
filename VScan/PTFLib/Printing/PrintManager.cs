/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Runtime.InteropServices;
using PremierTaxFree.PTFLib.Native;

namespace PremierTaxFree.PTFLib.Printing
{
    public static class PrintManager
    {
        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetDefaultPrinter(string Name);

        /// <summary>
        /// Gets default printer name
        /// </summary>
        /// <returns></returns>
        public static string GetDefaultPrinterName()
        {
            PrinterSettings settings = new PrinterSettings();
            return settings.PrinterName;
        }

        /// <summary>
        /// Gets all installed printers
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetInstalledPrinters()
        {
            foreach (string printer in PrinterSettings.InstalledPrinters)
                yield return printer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="szPrinterName"></param>
        /// <param name="docName">My C#.NET RAW Document</param>
        /// <param name="szFileName"></param>
        /// <returns></returns>
        public static bool SendFileToPrinter(string szPrinterName, string docName, string szFileName)
        {
            using (FileStream file = new FileStream(szFileName, FileMode.Open))
            {
                using (BinaryReader br = new BinaryReader(file))
                {
                    int nLength = Convert.ToInt32(file.Length);
                    byte[] bytes = br.ReadBytes(nLength);
                    // Allocate some unmanaged memory for those bytes.
                    IntPtr pUnmanagedBuffer = Marshal.AllocCoTaskMem(nLength);
                    // Copy the managed byte array into the unmanaged array.
                    Marshal.Copy(bytes, 0, pUnmanagedBuffer, nLength);
                    // Send the unmanaged bytes to the printer.
                    bool bSuccess = SendBytesToPrinter(szPrinterName, docName, pUnmanagedBuffer, nLength);
                    // Free the unmanaged memory that you allocated earlier.
                    Marshal.FreeCoTaskMem(pUnmanagedBuffer);
                    return bSuccess;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="szPrinterName"></param>
        /// <param name="docName"></param>
        /// <param name="szString">My C#.NET RAW Document</param>
        /// <returns></returns>
        public static bool SendStringToPrinter(string szPrinterName, string docName, string szString)
        {
            Int32 dwCount = szString.Length;
            IntPtr pBytes = Marshal.StringToCoTaskMemAuto(szString);
            SendBytesToPrinter(szPrinterName, docName, pBytes, dwCount);
            Marshal.FreeCoTaskMem(pBytes);
            return true;
        }

        /// <summary>
        /// Sends document to the printer
        /// </summary>
        /// <param name="szPrinterName"></param>
        /// <param name="docName"></param>
        /// <param name="pBytes"></param>
        /// <param name="dwCount"></param>
        /// <returns></returns>
        private static bool SendBytesToPrinter(string szPrinterName, string docName, IntPtr pBytes, Int32 dwCount)
        {
            Int32 dwError = 0, dwWritten = 0;
            IntPtr hPrinter = new IntPtr(0);
            winspool.DOCINFO di = new winspool.DOCINFO();
            bool bSuccess = false; // Assume failure unless you specifically succeed.

            di.pDocName = docName;
            di.pDataType = "RAW";

            // Open the printer.
            if (winspool.OpenPrinter(szPrinterName.Normalize(), out hPrinter, IntPtr.Zero))
            {
                // Start a document.
                if (winspool.StartDocPrinter(hPrinter, 1, di))
                {
                    // Start a page.
                    if (winspool.StartPagePrinter(hPrinter))
                    {
                        // Write your bytes.
                        bSuccess = winspool.WritePrinter(hPrinter, pBytes, dwCount, out dwWritten);
                        winspool.EndPagePrinter(hPrinter);
                    }
                    winspool.EndDocPrinter(hPrinter);
                }
                winspool.ClosePrinter(hPrinter);
            }
            // If you did not succeed, GetLastError may give more information
            // about why not.
            if (bSuccess == false)
            {
                dwError = Marshal.GetLastWin32Error();
            }
            return bSuccess;
        }
    }
}
