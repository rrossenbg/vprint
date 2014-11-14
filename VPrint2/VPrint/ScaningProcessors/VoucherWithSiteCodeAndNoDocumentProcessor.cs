/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Reflection;
using VPrint.Common.Pdf;
using VPrinting.Common;
using VPrinting.Data;
using VPrinting.Forms;
using VPrinting.Interfaces;
using VPrinting.Pdf;
using VPrinting.Tools;

namespace VPrinting.ScaningProcessors
{
    public class VoucherWithSiteCodeAndNoDocumentProcessor : IScanProcessor, IServiceData
    {
        private const int TRIES = 10;

        public static VoucherWithSiteCodeAndNoDocumentProcessor Default = new VoucherWithSiteCodeAndNoDocumentProcessor();

        private readonly PartyManagement.PartyManagement Manager = new PartyManagement.PartyManagement();
        private readonly VoucherNumberingAllocationPrinting.VoucherNumberingAllocationPrinting Printing = new VoucherNumberingAllocationPrinting.VoucherNumberingAllocationPrinting();

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
                            var finfo = helper.Run(info, item);
                            fullFilePath = (finfo != null) ? finfo.FullName : "";
                        }

                        bmp = ((Bitmap)Bitmap.FromFile(fullFilePath));

                        var useCrop = StateSaver.Default.Get<bool>(Strings.CROPIMAGE);
                        if (useCrop)
                            bmp = bmp.Crop2();

                        item.FileInfoList.Add(new FileInfo(fullFilePath)); // Scanned Image

                        item.FullFileName = fullFilePath;

                        StateManager.VoucherItem vitem = (StateManager.VoucherItem)item;

#if DRAW_ON_VOUCHER
                        bmp.DrawOnImage((gr) =>
                        {
                            var str = string.Format("{0:dd-MM-yyyy hh:mm}", DateTime.Now);
                            ///string user = Program.currentUser.ToString();
                            var an = StateSaver.Default.Get<AssemblyName>(Strings.VERSION);

                            using (var font = new Font(FontFamily.GenericSansSerif, 10f, FontStyle.Regular))
                            {
                                gr.DrawString(str, font, Brushes.Black, new PointF(10, 10));
                                gr.DrawString(an.Name, font, Brushes.Black, new PointF(10, 25));
                                gr.DrawString(an.Version.ToString(), font, Brushes.Black, new PointF(10, 40));
                            }
                        });
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
                        if (!VPrinting.Common.CommonTools.ParseSiteCode(info.Name, out site, out location))
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
#if SAVE_VOUCHER
                        if (!ext.EqualNoCase(".tif"))
                            bmp.Save(fullFilePath, ImageFormat.Jpeg);
#endif

                        if (item.Thumbnail == null)
                            item.Thumbnail = bmp.GetThumbnailImage(45, 45, () => false, IntPtr.Zero);

                        if (StateSaver.Default.Get<bool>(Strings.USE_VCOVER))
                        {
                            using (WaitObject obj = new WaitObject(item.FileInfoList))
                            {
                                var ptr = StateSaver.Default.Get<IntPtr>(Strings.VCOVER_FUNC);
                                var time = StateSaver.Default.Get<TimeSpan>(Strings.VCOVER_TIMEOUT, TimeSpan.FromMinutes(10));
                                var dlg = ptr.GetDelegate<CallVCoverService_ReadDataDelegate>();
                                dlg.DynamicInvoke(obj);
                                if (!obj.WaitOne(time))
                                    throw new ApplicationException("VCover timeout");
                                if(obj.Err != null)
                                    throw obj.Err;
                            }
                        }

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
                                DocPass = null,
                                SignImagePath = StateSaver.Default.Get<string>(Strings.PTFLogoFileFullPath),
                                ReasonForSigning = string.Concat("Signing electronic copy of voucher ", vitem.Barcode),
                                Location = StateSaver.Default.Get<string>(Strings.Certigicate_LOCATION)
                            };

                            var prtGetMetaData = StateSaver.Default.Get<IntPtr>(Strings.Certigicate_METADATA_FUNC);
                            if (prtGetMetaData != IntPtr.Zero)
                            {
                                crInfo.MetaData = signInfo.MetaData = prtGetMetaData.GetDelegate<GetMetaDataDelegate>().DynamicInvoke(this, vinfo.IsoId, 0, vinfo.RetailerId).cast<ArrayList>();
                                var str = string.Format("{0:dd-MM-yyyy hh:mm}", DateTime.Now);
                                var an = StateSaver.Default.Get<AssemblyName>(Strings.VERSION);
                                crInfo.MetaData.Insert(0, new Tuple<string, string>("", ""));
                                crInfo.MetaData.Insert(0, new Tuple<string, string>("Voucher number", vinfo.VoucherId.ToString()));

                                crInfo.MetaData.Add(new Tuple<string, string>("", ""));
                                crInfo.MetaData.Add(new Tuple<string, string>("Operator", Program.currentUser.Username));
                                crInfo.MetaData.Add(new Tuple<string, string>("App.name", an.Name));
                                crInfo.MetaData.Add(new Tuple<string, string>("App.version", an.Version.ToString()));
                                crInfo.MetaData.Add(new Tuple<string, string>("Create at", str));
                            }

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

        #region CALL TRS SERVICES

        public object RetrievePtfOfficeDetail(int iso, int hoid)
        {
            return Manager.RetrievePtfOfficeDetail(iso, hoid);
        }

        public object RetrieveRetailerDetail(int iso, int rid)
        {
            return Manager.RetrieveRetailerDetail(iso, rid);
        }

        private readonly Hashtable m_CachedData = Hashtable.Synchronized(new Hashtable());

        public object[] RetrieveTableData(string fieldsList, string tableName, string where)
        {
            string key = string.Concat(fieldsList.GetHashCode(), tableName, where);
            object[] value = null;
            if (!m_CachedData.ContainsKey(key))
                m_CachedData[key] = value = Manager.RetrieveTableData(fieldsList, tableName, where);
            else
                value = (object[])m_CachedData[key];
            return value;
        }

        #endregion
    }
}
