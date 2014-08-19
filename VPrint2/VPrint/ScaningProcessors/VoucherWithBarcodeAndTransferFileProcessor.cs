/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using DTKBarReader;
using VPrint.Common;
using VPrinting.Common;
using VPrinting.Extentions;
using VPrinting.Forms;
using VPrinting.Tools;

namespace VPrinting.ScaningProcessors
{
    public class VoucherWithBarcodeAndTransferFileProcessor : IScanProcessor
    {
        private const int TRIES = 10;

        public static readonly VoucherWithBarcodeAndTransferFileProcessor Default = new VoucherWithBarcodeAndTransferFileProcessor();

        public Action<TaskProcessOrganizer<string>.TaskItem> GetAction()
        {
            return new Action<TaskProcessOrganizer<string>.TaskItem>((o) =>
            {
                var fullFilePath = o.Item;

                Bitmap bmp = null;
                Bitmap bmpBarcode = null;
                BarcodeData data = null;

                string barcode = null;
                string siteCode = null;

                StateManager.VoucherItem item = (StateManager.VoucherItem)StateManager.Default.ProcessItem_Begin(true);

                try
                {
                    var info = new FileInfo(fullFilePath);

                    if (info.Exists && !info.IsReadOnly(TRIES))
                    {
                        // ".tif"
                        var ext = Path.GetExtension(info.FullName);

                        bmp = ((Bitmap)Bitmap.FromFile(info.FullName)).Crop2();
                        item.FullFileName = fullFilePath;
                        item.FileInfoList.Add(new FileInfo(fullFilePath)); // Scanned Image

                        StateManager.VoucherItem vitem = (StateManager.VoucherItem)item;
                        FileInfo barcFilePath = null;
                        Rectangle rect = Rectangle.Empty;
                        CommonTools.ParseVoucherImage(ref bmp, ref bmpBarcode, out rect, ref barcode, BarcodeTypeEnum.BT_All);
                        vitem.Barcode = barcode;

                        string user = string.Concat("Country: ", Program.currentUser.CountryID, " User: ", Program.currentUser.UserID);
                        bmp.DrawOnImage((gr, u) =>
                        {
                            using (var font = new Font(FontFamily.GenericSansSerif, 10f, FontStyle.Regular))
                            {
                                var str = string.Format("{0:dd-MM-yyyy hh:mm}", DateTime.Now);
                                gr.DrawString(str, font, Brushes.Black, new PointF(10, 10));
                                gr.DrawString(u.Cast<string>(), font, Brushes.Black, new PointF(10, 25));
                            }
                        }, user);

                        var coverArea = StateSaver.Default.Get<Rectangle>(Strings.VOUCHERCOVERREGION);
                        if (!coverArea.IsEmpty)
                        {
                            var size = StateSaver.Default.Get<int>(Strings.PIXELSIZE, 5);
                            bmp.Pixellate(coverArea, size);
                        }

                        if (vitem.HasBarcode)
                        {
                            List<BarcodeConfig> barcodeLayouts = StateSaver.Default.Get<List<BarcodeConfig>>(Strings.LIST_OF_BARCODECONFIGS);

                            foreach (var cfg in barcodeLayouts)
                                if (cfg.ParseBarcode(barcode, ref data))
                                    break;

                            if (data == null)
                                throw new ApplicationException("Barcode invalid");

                            vitem.CountryID = data.CountryID;
                            vitem.RetailerID = data.RetailerID;
                            vitem.VoucherID = data.VoucherID;
                            vitem.Barcode = barcode;

                            var barcodePath = fullFilePath.ChangeFilePath((name) => name.Replace(".", "_barcode."));
                            Global.IgnoreList.Add(barcodePath);

                            bmpBarcode.DrawOnImage((gr, s) =>
                            {
                                using (var font = new Font(FontFamily.GenericSansSerif, 10f, FontStyle.Regular))
                                {
                                    var str = Convert.ToString(s);
                                    gr.DrawString(str, font, Brushes.Red, new PointF(10, 10));
                                }
                            }, barcode);

                            bmpBarcode.Save(barcodePath, bmp.RawFormat);

                            barcFilePath = new FileInfo(barcodePath); // Scanned Barcode
                            vitem.FileInfoList.Add(barcFilePath); // Scanned Barcode Image
                        }

                        item = StateManager.Default.AddTransferFileItem(item);

                        if (item.CountryID == 0)
                            item.CountryID = MainForm.ms_DefaultCountryId;

                        if (!ext.EqualNoCase(".tif"))
                            bmp.Save(fullFilePath, ImageFormat.Jpeg);

                        if (item.Thumbnail == null)
                            item.Thumbnail = bmp.GetThumbnailImage(45, 45, () => false, IntPtr.Zero);

                        var sec = new CertificateSecurity(X509FindType.FindBySerialNumber, Strings.CERTNUMBER, StoreLocation.LocalMachine);
                        if (sec.Loaded)
                        {
                            var signFilePath = new FileInfo(Path.ChangeExtension(fullFilePath, ".sgn"));
                            item.Signature = sec.SignData(bmp.ToArray());
                            File.WriteAllBytes(signFilePath.FullName, item.Signature);
                            item.FileInfoList.Add(signFilePath);
                            Global.IgnoreList.Add(signFilePath.FullName);
                        }

                        item.State = StateManager.eState.OK;
                        item.Message = "";
                        StateManager.Default.CompleteItem(item);
                    }
                }
                catch (Exception ex)
                {
                    item.State = StateManager.eState.Err;
                    item.Message = ex.Message;

                    var scex = new ScanException(ex, data)
                    {
                        SiteCode = siteCode,
                        FilePath = fullFilePath
                    };

                    DelegateHelper.FireError(this, ex);
                }
                finally
                {
                    bmp.DisposeSf();
                    bmpBarcode.DisposeSf();
                    DelegateHelper.PostItemScannedCallback(item);
                    StateManager.Default.ShowNextItemExpected();
                }
            });
        }
    }
}
