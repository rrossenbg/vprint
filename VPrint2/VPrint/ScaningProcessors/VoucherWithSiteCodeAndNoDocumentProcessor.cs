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
using VPrinting.Data;

namespace VPrinting.ScaningProcessors
{
    public class VoucherWithSiteCodeAndNoDocumentProcessor : IScanProcessor
    {
        private const int TRIES = 10;

        public static VoucherWithSiteCodeAndNoDocumentProcessor Default = new VoucherWithSiteCodeAndNoDocumentProcessor();

        public Action<TaskProcessOrganizer<string>.TaskItem> GetAction()
        {
            return new Action<TaskProcessOrganizer<string>.TaskItem>((o) =>
            {
                var fullFilePath = o.Item;

                Bitmap bmp = null;
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

                        string site;
                        int location;
                        if (!CommonTools.ParseSiteCode(info.Name, out site, out location))
                            throw new Exception("Wrong sitecode");

                        var vinfo = ServiceDataAccess.Instance.FindVoucherTRSBySiteCode(site, location);
                        item.CountryID = vinfo.IsoId;
                        vitem.RetailerID = vinfo.RetailerId;
                        vitem.VoucherID = vinfo.VoucherId;
                        vitem.Barcode = "";

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
                        StateManager.Default.CompleteItem(item);
                    }
                }
                catch (Exception ex)
                {
                    item.State = StateManager.eState.Err;
                    item.Message = ex.Message;
                    DelegateHelper.FireError(this, ex);
                }
                finally
                {
                    bmp.DisposeSf();

                    DelegateHelper.PostItemScannedCallback(item);

                    try
                    {
                        if (!item.IsSetup)
                        {
                            using (var mngr = new AsyncFormManager<RetailerForm>("Enter voucher details"))
                            {
                                mngr.Result = item;
                                mngr.RunWait();
                                if (!item.IsSetup)
                                    throw new ApplicationException("Cannot find barcode.");
                            }
                        }

                        StateManager.Default.AddNewItem(item);
                    }
                    catch (Exception ex0)
                    {
                        item.State = StateManager.eState.Err;
                        item.Message = ex0.Message;
                        DelegateHelper.FireError(this, ex0);
                    }
                }
            });
        }
    }
}
