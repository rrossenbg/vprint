/***************************************************
//  Copyright (c) Premium Tax Free 2012-2013
/***************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using DTKBarReader;
using VPrint.Common;
using VPrinting.Common;
using VPrinting.Documents;
using VPrinting.Extentions;
using VPrinting.ScaningProcessors;

namespace VPrinting.Tools
{
    public static class DelegateHelper
    {
        private const string DONEEVENTNAME = "DelegateHelper_Done";
        public static event ThreadExceptionEventHandler Error;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <example>
        /// EscapePrintDocument doc = sender as EscapePrintDocument;
        /// EscapePrintHelper hlp = new EscapePrintHelper(e.Graphics);
        /// </example>
        public static PrintPageEventHandler CreatePrintPageEventHandler(IList<IPrintLine> printLines)
        {
            return new PrintPageEventHandler((sender, e) =>
            {
                IVoucherLayout layout = (IVoucherLayout)CacheManager.Instance.Table[Strings.IVoucherLayout];

                Point moveAll = (layout != null) ? layout.MoveAll : Point.Empty;

                using (var brush = new SolidBrush(Color.Black))
                {
                    foreach (IPrintLine line in printLines)
                    {
                        if (line == null)
                            continue;

                        GPrintLine gline = line as GPrintLine;
                        if (gline != null && !gline.Text.IsNullOrEmpty() && (!gline.IsEmpty()))
                        {
                            if (gline.Font == null)
                                throw new ArgumentNullException("Line.Font");
                            if (gline.Font.Value == null)
                                throw new ArgumentNullException("Line.Font.Value");

                            var lines = gline.Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                            Size size;
                            for (int i = 0, y = (int)line.Y; i < lines.Length; i++, y += size.Height)
                            {
                                e.Graphics.DrawString(lines[i], gline.Font.Value, brush, line.X + moveAll.X, y + moveAll.Y);
                                size = Size.Round(e.Graphics.MeasureString(lines[i], gline.Font.Value));
                            }
                        }
                        else
                        {
                            BarPrintLine bline = line as BarPrintLine;
                            if (bline != null && !bline.IsEmpty())
                            {
                                using (var bmp = BarcodeTools.BinaryWritePicture(bline.Text, bline.Height, bline.Size))
                                {
                                    e.Graphics.DrawImage(bmp, bline.X + moveAll.X, bline.Y + moveAll.Y);

                                    if (bline.BarText == null)
                                        throw new ApplicationException("BarText empty");

                                    SizeF s = e.Graphics.MeasureString(bline.BarText.Text, bline.BarText.Font.Value);

                                    e.Graphics.DrawString(bline.BarText.Text, bline.BarText.Font.Value, brush,
                                        bline.X + ((bmp.Width - s.Width) / 2 + bline.BarText.X) + moveAll.X, // Middle
                                        bline.Y + (bmp.Height + bline.BarText.Y) + moveAll.Y);//Bottom
                                }
                            }
                            else
                            {
                                GPrintLineUnit inline = line as GPrintLineUnit;
                                if (inline != null && !inline.Text.IsNullOrEmpty() && (!inline.IsEmpty()))
                                {
                                    if (inline.Font == null)
                                        throw new ArgumentNullException("Line.Font");
                                    if (inline.Font.Value == null)
                                        throw new ArgumentNullException("Line.Font.Value");

                                    var lines = inline.Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                                    SizeF size;

                                    float X = (inline.Units == GraphicsUnit.Inch) ?
                                        inline.X.FromInch() :
                                        inline.X;
                                    float y = (inline.Units == GraphicsUnit.Inch) ?
                                        inline.Y.FromInch() :
                                        inline.Y;

                                    for (int i = 0; i < lines.Length; i++, y += size.Height)
                                    {
                                        e.Graphics.DrawString(lines[i], inline.Font.Value, brush, X + moveAll.X, y + moveAll.Y);
                                        size = e.Graphics.MeasureString(lines[i], inline.Font.Value);
                                    }
                                }
                                else
                                {
                                    BarPrintLineUnit inbline = line as BarPrintLineUnit;
                                    if (inbline != null && !inbline.IsEmpty())
                                    {
                                        using (var bmp = BarcodeTools.BinaryWritePicture(inbline.Text, inbline.Height, inbline.Size))
                                        {
                                            float X = (inline.Units == GraphicsUnit.Inch) ?
                                                inbline.X.FromInch() :
                                                inbline.X;
                                            float Y = (inline.Units == GraphicsUnit.Inch) ?
                                                inbline.Y.FromInch() :
                                                inbline.Y;

                                            e.Graphics.DrawImage(bmp, X + moveAll.X, Y + moveAll.Y);

                                            if (inbline.BarText == null)
                                                throw new ApplicationException("BarText empty");

                                            SizeF s = e.Graphics.MeasureString(inbline.BarText.Text, inbline.BarText.Font.Value);

                                            e.Graphics.DrawString(inbline.BarText.Text, inbline.BarText.Font.Value, brush,
                                                (X + ((bmp.Width - s.Width) / 2 + inbline.BarText.X) + moveAll.X), // Middle
                                                (Y + (bmp.Height + inbline.BarText.Y)) + moveAll.Y);//Bottom
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                e.HasMorePages = false;
            });
        }

        public static Action<TaskProcessOrganizer<string>.TaskItem> CreateScanAction()
        {
            if(MainForm.ms_ImportCoversheet)
                return CoversheetProcessor.Default.GetAction();

            switch (StateManager.Default.Mode)
            {
                case StateManager.eMode.Barcode:
                    return VoucherWithBarcodeAndNoDocumentProcessor.Default.GetAction();
                case StateManager.eMode.Sitecode:
                    return VoucherWithSiteCodeAndNoDocumentProcessor.Default.GetAction();
                case StateManager.eMode.TransferFile:
                    return VoucherWithTransferFileProcessor.Default.GetAction();
                case StateManager.eMode.TransferFileAndBarcode:
                    return VoucherWithBarcodeAndTransferFileProcessor.Default.GetAction();
                default:
                    throw new NotImplementedException();
            }
        }        

        private static EventWaitHandle ms_Done;

        static DelegateHelper()
        {
            bool created;
            ms_Done = new EventWaitHandle(false, EventResetMode.AutoReset, DONEEVENTNAME, out created);
        }

        public static EventWaitHandle GetEvent()
        {
            return EventWaitHandle.OpenExisting(DONEEVENTNAME);
        }

        public static void PostItemScannedCallback(StateManager.Item item)
        {
            MainForm.Default.m_MainContext.Post(MainForm.Default.ShowItemScannedCallback, item);
            ms_Done.Set();
        }

        public static void Close()
        {
            ms_Done.Close();
        }

        public static void FireError(object sender, Exception ex)
        {
            if (Error != null)
                Error(sender, new ThreadExceptionEventArgs(ex));
        }

        /// <summary>
        /// EXAMPLE! NOT IN USE NOW
        /// </summary>
        /// <returns></returns>
        private static Action<TaskProcessOrganizer<string>.TaskItem> CreateScanActionORIGINAL()
        {
            const int TRIES = 10;

            return new Action<TaskProcessOrganizer<string>.TaskItem>((o) =>
            {
                var fullFilePath = o.Item;

                Bitmap bmp = null;
                Bitmap bmpBarcode = null;
                BarcodeData data = null;

                string barcode = null;
                string siteCode = null;

                StateManager.Item item = null;

                try
                {
                    var info = new FileInfo(fullFilePath);

                    if (info.Exists && !info.IsReadOnly(TRIES))
                    {
                        // ".tif"
                        var ext = Path.GetExtension(info.FullName);

#if PROC_TIFF_AS_JPEG
                        if (ext == ".tif")
                        {
                            int c = 0;

                            foreach (var im in info.FullName.TiffGetAllImages())
                            {
                                var name = info.FullName.ChangeFilePath((n) => n.Replace(ext, c++ + ext));
                                im.Save(name);
                                im.DisposeSf();
                            }

                            return;
                        }
#endif
                        bmp = ((Bitmap)Bitmap.FromFile(info.FullName)).Crop2();

                        item = StateManager.Default.ProcessItem_Begin(!MainForm.ms_ImportCoversheet);
                        item.FullFileName = fullFilePath;
                        item.FileInfoList.Add(new FileInfo(fullFilePath)); // Scanned Image

                        if (!MainForm.ms_ImportCoversheet)
                        {
                            StateManager.VoucherItem vitem = (StateManager.VoucherItem)item;
                            FileInfo barcFilePath = null;
                            Rectangle rect = Rectangle.Empty;
                            CommonTools.ParseVoucherImage(ref bmp, ref bmpBarcode, out rect, ref barcode, BarcodeTypeEnum.BT_All);

                            vitem.Barcode = barcode;

                            if (vitem.HasBarcode)
                            {
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

                                List<BarcodeConfig> barcodeLayouts = StateSaver.Default.Get<List<BarcodeConfig>>(Strings.LIST_OF_BARCODECONFIGS);

                                foreach (var cfg in barcodeLayouts)
                                    if (cfg.ParseBarcode(barcode, ref data))
                                        break;

                                if (data == null)
                                    throw new ApplicationException("Can not find barcode");

                                item.CountryID = data.CountryID;
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
                        }
                        else
                        {
                            MainForm.ms_ImportCoversheet = false;
                            MainForm.Default.InvokeSf(() => MainForm.Default.cbCoversheet.Checked = false);
                        }

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
                        }

                        item.State = StateManager.eState.OK;
                        item.Message = "";
                    }
                }
                catch (Exception ex)
                {
                    if (item != null)
                    {
                        item.State = StateManager.eState.Err;
                        item.Message = ex.Message;
                    }
                    var scex = new ScanException(ex, data)
                    {
                        SiteCode = siteCode,
                        FilePath = fullFilePath
                    };

                    if (Error != null)
                        Error(null, new ThreadExceptionEventArgs(ex));
                }
                finally
                {
                    bmp.DisposeSf();
                    bmpBarcode.DisposeSf();

                    MainForm.Default.m_MainContext.Post(MainForm.Default.ShowItemScannedCallback, item);

                    try
                    {
                        StateManager.Default.ProcessItem_End(item);
                    }
                    catch (Exception ex0)
                    {
                        if (item != null)
                        {
                            item.State = StateManager.eState.Err;
                            item.Message = ex0.Message;
                        }

                        var scex = new ScanException(ex0, data)
                        {
                            SiteCode = siteCode,
                            FilePath = fullFilePath
                        };

                        if (Error != null)
                            Error(null, new ThreadExceptionEventArgs(scex));
                    }
                }
            });
        }
    }
}
