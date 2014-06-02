/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using PremierTaxFree.PTFLib;
using PremierTaxFree.PTFLib.Data;

using PremierTaxFree.PTFLib.Serialization;
using PremierTaxFree.PTFLib.Sys;

namespace PremierTaxFree
{
    public static class UIUtils
    {
        /// <summary>
        /// Starts a windows service asynchronously and safe
        /// </summary>
        public static void TryStartTransferringServiceAsync()
        {
            new MethodInvoker(() =>
            {
                if (!OS.StartService(Strings.TransferringService, TimeSpan.FromMinutes(1)))
                    throw new IOException("Can't start transferring service");

            }).FireAndForget();
        }

        /// <summary>
        /// Unstall a windows service asynchronously and safe
        /// </summary>
        public static void TryUnInstallTransferringServiceAsync()
        {
            new MethodInvoker(() =>
            {
                string servicePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Strings.TransferringService + ".exe");
                OS.UnInstallService(servicePath, TimeSpan.FromMinutes(5));

            }).FireAndForget();
        }

        public static Process StartBarcodeReader(Bitmap img, Guid g)
        {
            var buffer = img.ToArray();
            ClientDataAccess.InsertBarcodeInfo(g, buffer);

            string path = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "ReaderProc.exe");
            var p = Process.Start(path, g.ToString());
            return p;
        }

        public static BarcodeInfo ReadBarcodeReaderResults(Process p, Guid g)
        {
            if (!p.WaitForExit((int)TimeSpan.FromSeconds(10).TotalMilliseconds))
                throw new TimeoutException("Reader timeout");

            var buffer2 = ClientDataAccess.SelectBarcodeInfoData(g);
            ClientDataAccess.DeleteBarcodeInfo(g);
            ObjectSerializer ser = new ObjectSerializer(true);
            var barArray = ser.Deserialize<BarcodeInfoArray>(buffer2);
            if (barArray.Count == 0)
                throw new IndexOutOfRangeException("No barcode found");
            return barArray[0];
        }
    }
}
