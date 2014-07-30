/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using VPrinting.Common;
using VPrinting.Extentions;
using VPrinting.Tools;

namespace VPrinting.ScaningProcessors
{
    public class CoversheetProcessor : IScanProcessor
    {
        public static CoversheetProcessor Default = new CoversheetProcessor();

        public Action<TaskProcessOrganizer<string>.TaskItem> GetAction()
        {
            const int TRIES = 10;

            return new Action<TaskProcessOrganizer<string>.TaskItem>((o) =>
            {
                var fullFilePath = o.Item;

                Bitmap bmp = null;
                Bitmap bmpBarcode = null;
                BarcodeData data = null;

                string siteCode = null;

                StateManager.Item item = StateManager.Default.ProcessItem_Begin(false);

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

                        MainForm.ms_ImportCoversheet = false;
                        MainForm.Default.InvokeSf(() => MainForm.Default.cbCoversheet.Checked = false);

                        if (item.CountryID == 0)
                            item.CountryID = MainForm.ms_DefaultCountryId;

                        if (!ext.EqualNoCase(".tif"))
                            bmp.Save(fullFilePath, ImageFormat.Jpeg);

                        if (item.Thumbnail == null)
                            item.Thumbnail = bmp.GetThumbnailImage(45, 45, () => false, IntPtr.Zero);

                        item.State = StateManager.eState.OK;
                        item.Message = "";

                        StateManager.Default.CompleteCurrentItem();
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
                        SiteCode = "na",
                        FilePath = fullFilePath
                    };

                    DelegateHelper.FireError(this, ex);
                }
                finally
                {
                    bmp.DisposeSf();
                    bmpBarcode.DisposeSf();

                    DelegateHelper.PostItemScannedCallback(item);

                    try
                    {
                        StateManager.Default.AddNewItem(item);
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

                        DelegateHelper.FireError(this, scex);
                    }
                }
            });
        }
    }
}