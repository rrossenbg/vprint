/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Diagnostics;
using System.ServiceModel;
using VPrinting.Common;
using VPrinting.Data;
using VPrinting.ScanServiceRef;
using VPrinting.Tools;

namespace VPrinting
{
    partial class MainForm
    {
        private void ScanFileAsync(string fullPath)
        {
            if (string.IsNullOrWhiteSpace(fullPath) || 
                !Global.IgnoreList.Add(fullPath) || 
                fullPath.Contains("barcode"))
                return;

            #region SCAN ORGANIZER

            m_ScanFileOrganizer.RunTask(new TaskProcessOrganizer<string>.TaskItem(fullPath, 
                DelegateHelper.CreateScanAction()));

            #endregion
        }

#if OLD_CODE

                new Action<TaskProcessOrganizer<string>.TaskItem>((o) =>
            {
                var fullFilePath = o.Item;

                Bitmap bmp = null;
                Bitmap bmpBarcode = null;
                var data = new BarcodeData();

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

                        item = m_StateManager.ProcessItem_Begin(!MainForm.ms_ImportCoversheet);

                        item.FileInfoList.Add(new FileInfo(fullFilePath)); // Scanned Image

                        if (!MainForm.ms_ImportCoversheet)
                        {
                            StateManager.VoucherItem vitem = (StateManager.VoucherItem)item;
                            FileInfo barcFilePath = null;
                            Rectangle rect = Rectangle.Empty;

                            CommonTools.ParseVoucherImage(ref bmp, ref bmpBarcode, out rect, ref barcode);

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

                                data = MainForm.ms_BarcodeConfig.ParseBarcode(barcode);
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
                            this.InvokeSf(() => cbCoversheet.Checked = false);
                        }

                        if (item.CountryID == 0)
                            item.CountryID = MainForm.ms_DefaultCountryId;

                        if (!ext.EqualNoCase(".tif"))
                            bmp.Save(fullFilePath, ImageFormat.Jpeg);

                        if (item.Thumbnail == null)
                            item.Thumbnail = bmp.GetThumbnailImage(45, 45, () => false, IntPtr.Zero);

                        CertificateSecurity sec = new CertificateSecurity(X509FindType.FindBySerialNumber, Strings.CERTNUMBER, StoreLocation.LocalMachine);
                        if (sec.Loaded)
                            item.Signature = sec.SignData(bmp.ToArray());

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

                    Program.OnThreadException(null, new ThreadExceptionEventArgs(ex));
                }
                finally
                {
                    bmp.DisposeSf();
                    bmpBarcode.DisposeSf();

                    m_MainContext.Post(ShowItemScannedCallback, item);

                    try
                    {
                        m_StateManager.ProcessItem_End(item);
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
                        Program.OnThreadException(null, new ThreadExceptionEventArgs(scex));
                    }
                }
            })));
        }
#endif

        private void SendToServerAsync(StateManager.Item item)
        {
            if (item != null)
            {
                if (item.State != StateManager.eState.OK)
                    return;

                if (item.FileInfoList.Count == 0)
                {
                    item.State = StateManager.eState.NA;
                }
                else
                {
                    #region SEND ORGANIZER

                    m_SendFileOrganizer.RunTask(new TaskProcessOrganizer<StateManager.Item>.TaskItem(item,
                        new Action<TaskProcessOrganizer<StateManager.Item>.TaskItem>(
                            (i) =>
                            {
                                var keys = Security.CreateInstance().GenerateSecurityKeys();
                                try
                                {
                                    var serverSessionId = i.Item.SessionID.ToString();
#if DEBUGGER
                                    Trace.WriteLine("Sending ".concat(serverSessionId), Strings.VRPINT);
#endif

                                    //copy voucher
                                    i.Item.FileInfoList.ForEach((f) =>
                                    {
                                        if (f.Exists())
                                            ServiceDataAccess.Instance.SendFile(f, serverSessionId, keys);
                                    });

                                    var srv = ServiceDataAccess.Instance;

                                    StateManager.VoucherItem vi = i.Item as StateManager.VoucherItem;
                                    if (vi != null)
                                    {
                                        srv.CommitVoucherChanges(serverSessionId, vi.JobID, vi.CountryID, vi.RetailerID, vi.VoucherID,
                                            Global.FolderID.HasValue ? Global.FolderID.Value : (int?)null, vi.SiteCode, vi.Barcode, keys);

                                        srv.SaveHistory(OperationHistory.Scan, vi.SessionID, vi.CountryID, vi.RetailerID,
                                                vi.VoucherID, 0, 0, "", keys);
                                    }
                                    else
                                    {
                                        srv.CommitFileChanges(serverSessionId, i.Item.CountryID,
                                            Global.FolderID.HasValue ? Global.FolderID.Value : (int?)null, keys);
                                    }
#if DEBUGGER
                                    Trace.WriteLine("Committed".concat(serverSessionId), Strings.VRPINT);
#endif
                                    i.Item.State = StateManager.eState.Sent;
                                }
                                catch (FaultException<MyApplicationFault> srex)
                                {
                                    i.Item.Message = srex.Message;
                                    i.Item.State = StateManager.eState.Err;

                                    int count = StateManager.Default.SetItemWithErr();
                                    DelegateHelper.PostShowItemsWithErrCallback(count);
                                    DelegateHelper.FireError(this, srex);

#if DEBUGGER
                                    Trace.WriteLine(srex, Strings.VRPINT);
#endif
                                }
                                catch (Exception ex)
                                {
                                    i.Item.Message = ex.Message;
                                    i.Item.State = StateManager.eState.Err;

                                    int count = StateManager.Default.SetItemWithErr();
                                    DelegateHelper.PostShowItemsWithErrCallback(count);
                                    DelegateHelper.FireError(this, ex);

#if DEBUGGER
                                    Trace.WriteLine(ex, Strings.VRPINT);
#endif
                                }
                                finally
                                {
                                    i.Item.FireUpdated();
                                    m_MainContext.Post(ShowItemCommitedCallback, i.Item);
                                }
                            })));

                    #endregion
                }
            }
        }
    }
}