/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using VPrint.Common.Pdf;
using VPrinting.Common;
using VPrinting.Data;
using VPrinting.Forms;
using VPrinting.Pdf;
using VPrinting.Tools;
using VPrint.Common;

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
                        Global.IgnoreList.Add(fullFilePath);
                        // ".tif"
                        var ext = Path.GetExtension(info.FullName);
                        if (ext.EqualNoCase(".pdf"))
                        {
                            var helper = new PDFFileHelper();
                            fullFilePath = helper.Run(info, item);
                        }

                        bmp = ((Bitmap)Bitmap.FromFile(fullFilePath));
                        bmp = bmp.Crop2();

                        item.FileInfoList.Add(new FileInfo(fullFilePath)); // Scanned Image

                        item.FullFileName = fullFilePath;

                        StateManager.VoucherItem vitem = (StateManager.VoucherItem)item;
#if DRAW_ON_VOUCHER
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

#endif
#if COVER_CCD
                        var coverArea = StateSaver.Default.Get<Rectangle>(Strings.VOUCHERCOVERREGION);
                        if (!coverArea.IsEmpty)
                        {
                            var size = StateSaver.Default.Get<int>(Strings.PIXELSIZE, 5);
                            bmp.Pixellate(coverArea);
                        }
#endif
                        string site;
                        int location;
                        if (!CommonTools.ParseSiteCode(info.Name, out site, out location))
                            throw new Exception("Wrong sitecode");

                        var vinfo = ServiceDataAccess.Instance.FindVoucherTRSBySiteCode(site, location);
                        if (vinfo == null || !vinfo.IsValid)
                        {
                            vinfo = ServiceDataAccess.Instance.FindVoucherPRBySiteCode(site, location);
                            if (!vinfo.IsValid)
                                throw new Exception("Cannot find voucher by sitecode");
                        }

                        item.CountryID = vinfo.IsoId;
                        vitem.RetailerID = vinfo.RetailerId;
                        vitem.VoucherID = vinfo.VoucherId;
                        vitem.Barcode = "";
                        vitem.SiteCode = string.Concat(site, location);

                        if (item.CountryID == 0)
                            item.CountryID = MainForm.ms_DefaultCountryId;

                        if (!ext.EqualNoCase(".tif"))
                            bmp.Save(fullFilePath, ImageFormat.Jpeg);

                        if (item.Thumbnail == null)
                            item.Thumbnail = bmp.GetThumbnailImage(45, 45, () => false, IntPtr.Zero);

                        var certificateSigning = StateSaver.Default.Get<bool>(Strings.CERTIFICATE_SIGNING_AVAILABLE);
                        if (certificateSigning)
                        {
                            var crInfo = new PdfCreationInfo()
                            {
                                Title = string.Concat("Voucher ", vitem.VoucherID),
                                Subject = string.Concat("Retailer ", vitem.RetailerID),
                                Author = string.Concat("PTF ", StateSaver.Default.Get<string>(Strings.Certigicate_COUNTRY)),
                                Creator = string.Concat("PTF ", StateSaver.Default.Get<string>(Strings.Certigicate_LOCATION)),
                            };

                            var signInfo = new PdfSignInfo()
                            {
                                pfxFilePath = StateSaver.Default.Get<string>(Strings.COUNTRY_CERTIFICATE_PATH),
                                pfxKeyPass = StateSaver.Default.Get<string>(Strings.COUNTRY_CERTIFICATE_PASS),
                                docPass = null,
                                signImagePath = StateSaver.Default.Get<string>(Strings.PTFLogoFileFullPath),
                                reasonForSigning = string.Concat("Signing electronic copy of voucher ", vitem.Barcode),
                                location = StateSaver.Default.Get<string>(Strings.Certigicate_LOCATION)
                            };

                            var pdfFileName = pdfFileAccess.Instance.CreateSignPdf(bmp, vitem.Barcode, vitem.RetailerID, vitem.VoucherID, crInfo, signInfo);
                            item.FileInfoList.Add(new FileInfo(pdfFileName)); // Signed Image
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

                    int count = StateManager.Default.SetItemWithErr();
                    DelegateHelper.PostShowItemsWithErrCallback(count);
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
