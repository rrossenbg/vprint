/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using DTKBarReader;
using PremierTaxFree.Data;
using PremierTaxFree.Data.Objects;
using PremierTaxFree.Extensions;
using PremierTaxFree.PTFLib;
using PremierTaxFree.PTFLib.Data;
using PremierTaxFree.PTFLib.Data.Objects;
using PremierTaxFree.PTFLib.Net;
using PremierTaxFree.PTFLib.Serialization;
using PremierTaxFree.PTFLib.Sys;
using PremierTaxFree.PTFLib.Threading;
using PremierTaxFree.Scan;

namespace PremierTaxFree.Utils
{
    public delegate void VoucherProcessDelegate(Voucher data, StateObj state);

    public static class DelegateUtils
    {
        /// <summary>
        /// Creates SelectAuditIds reloading delegate
        /// </summary>
        /// <returns></returns>
        public static Delegate CreateAuditIdReloadDelegate()
        {
            var method = new Action(() =>
            {
                int minimumAuditIdsInCache = SettingsTable.Get<int>(Strings.VScan_MinimumAuditIDsInCache, Program.ITEMS_IN_CACHE);
                AuditIDSTable.AddRange(ClientDataAccess.SelectAvailableSiteCodeIDs(minimumAuditIdsInCache), minimumAuditIdsInCache);
            });
            return method;
        }

        /// <summary>
        /// Creates Voucher processing delegate chan
        /// </summary>
        /// <returns></returns>
        public static Delegate GetProcessDelegateChain()
        {
            bool hideCardcodeUseBarcode = SettingsTable.Get<bool>(Strings.VScan_HideCardCodeDetailsBybarcode, true);
            bool printOnImage = SettingsTable.Get<bool>(Strings.VScan_PrintOnImage, false);
            bool insertInDatabase = SettingsTable.Get<bool>(Strings.VScan_AutoInsertDataAfterScan, true);

            var del =   CreateReadImageFromScannerDelegate().Add(//Read image from scanner
                        CreateRepaireImageDelegate(),     //Repair the image border
                        CreateExtractBarCodeDelegate(),     //Read barcode
                        //CreateExtractBarCodeDelegate2(),    //Read barcode2
#if DONTUSE
                        hideCardcodeUseBarcode ? CreateHideCardDetailsDelegateByBarcode() : CreateHideCardDetailsDelegateByMark(), //Hide card details
                        printOnImage ? CreatePrintOnImageDelegate() : CreateEmptyDelegate(),            //Print on image
#endif
                        insertInDatabase ? CreateInsertVoucherDelegate() : CreateSendToUIDelagete(),    //Insert voucher in database
#if DEBUG
                        CreateProcessLogDelegate(), //Log to file
#endif
                        CreateEndScanDelegate());             //Clean
            return del;
        }

        /// <summary>
        /// Creates ReadSacnnedImage delegate
        /// </summary>
        /// <returns></returns>
        public static VoucherProcessDelegate CreateReadImageFromScannerDelegate()
        {
            var method = new VoucherProcessDelegate((Voucher data, StateObj state) =>
            {
                using (BmpObj bmpObj = new BmpObj())
                {
                    bmpObj.Bind(state.Dib);
                    bmpObj.Fill();

                    data.VoucherImage = new Bitmap(bmpObj.Rect.Width, bmpObj.Rect.Height);
                    bmpObj.CopyTo(data.VoucherImage);
                }
            });
            return method;
        }

        /// <summary>
        /// Used to cut borders of the image
        /// </summary>
        /// <returns></returns>
        public static VoucherProcessDelegate CreateRepaireImageDelegate()
        {
            var method = new VoucherProcessDelegate((Voucher data, StateObj state) =>
            {
                Color color = SettingsTable.Get<Color>(Strings.VScan_ImageBorderColor, Color.White);
                const int COLOR_DISTANCE = 40;
                int colorDistance = SettingsTable.Get<int>(Strings.VScan_ImageBorderColorDistance, COLOR_DISTANCE);
                data.VoucherImage = data.VoucherImage.RemoveBorder(color, colorDistance);
            });
            return method;
        }

        /// <summary>
        /// Used to find and exctract the barcode
        /// </summary>
        /// <returns></returns>
        public static VoucherProcessDelegate CreateExtractBarCodeDelegate()
        {
            var method = new VoucherProcessDelegate((Voucher data, StateObj state) =>
            {
                Debug.Assert(state.Main != IntPtr.Zero);
                Debug.Assert(state.Scan != IntPtr.Zero);
                Debug.Assert(data != null);

                if (data.VoucherImage == null)
                    throw new AppWarningException("No image found");

                Barcode[] readBarCodes = null;

                lock (typeof(BarcodeReader))
                {
                    BarcodeReader reader = new BarcodeReader(Strings.VScan_BarcodeReaderSDKDeveloperLicenseKey);
                    reader.LicenseManager.AddLicenseKey(Strings.VScan_BarcodeReaderSDKUnlimitedRuntimeLicenseKey);
                    reader.BarcodesToRead = 1;
                    reader.BarcodeTypes = BarcodeTypeEnum.BT_Inter2of5;
                    var bmp = data.VoucherImage;
                    readBarCodes = reader.ReadFromBitmapRotateAll(ref bmp, data);
                    data.VoucherImage = bmp;
                }

                if (readBarCodes == null || readBarCodes.Length == 0)
                {
                    var ex = new ApplicationException("No bar code found");
                    ex.AddNext(new MethodInvoker(() =>
                    {
                        string id = Strings.VScan_EditItem.Uniqueue();
                        data.Message = ex.Message;
                        DataSlot.Set(id, data);
                        WinMsg.SendText(state.Scan, state.Main, id);
                    }));
                    throw ex;
                }
                else if (readBarCodes.Length > 1)
                {
                    var ex = new ApplicationException("More than one bar code on image.");
                    ex.AddNext(new MethodInvoker(() =>
                    {
                        string id = Strings.VScan_EditItem.Uniqueue();
                        data.Message = ex.Message;
                        DataSlot.Set(id, data);
                        WinMsg.SendText(state.Scan, state.Main, id);
                    }));
                    throw ex;
                }

                try
                {
                    data.Parse(readBarCodes[0].BarcodeString);
                }
                catch (Exception e)
                {
                    var ex = new ApplicationException("Can't process barcode string.", e);
                    ex.AddNext(new MethodInvoker(() =>
                    {
                        string id = Strings.VScan_EditItem.Uniqueue();
                        data.Message = ex.Message;
                        DataSlot.Set(id, data);
                        WinMsg.SendText(state.Scan, state.Main, id);
                        VoucherMonitorForm.ShowImage(id);
                    }));
                    throw ex;
                }
            });
            return method;
        }

         /// <summary>
        /// Used to find and exctract the barcode
        /// </summary>
        /// <returns></returns>
        public static VoucherProcessDelegate CreateExtractBarCodeDelegate2()
        {
            var method = new VoucherProcessDelegate((Voucher data, StateObj state) =>
            {
                Debug.Assert(state.Main != IntPtr.Zero);
                Debug.Assert(state.Scan != IntPtr.Zero);
                Debug.Assert(data != null);

                if (data.VoucherImage == null)
                    throw new AppWarningException("No image found");

                Guid g = Guid.NewGuid();
                var buffer = data.VoucherImage.ToArray();
                ClientDataAccess.InsertBarcodeInfo(g, buffer);

                string path = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "ReaderProc.exe");
                var p = Process.Start(path, g.ToString());

                if (!p.WaitForExit((int)TimeSpan.FromSeconds(10).TotalMilliseconds))
                    throw new TimeoutException("Reader timeout");

                var buffer2 = ClientDataAccess.SelectBarcodeInfoData(g);
                ClientDataAccess.DeleteBarcodeInfo(g);

                ObjectSerializer ser = new ObjectSerializer(true);
                BarcodeInfoArray barArray = ser.Deserialize<BarcodeInfoArray>(buffer2);
                if (barArray.Count == 0)
                    throw new IndexOutOfRangeException("No barcode found");

                if (barArray == null || barArray.Count == 0)
                {
                    var ex = new ApplicationException("No bar code found");
                    ex.AddNext(new MethodInvoker(() =>
                    {
                        string id = Strings.VScan_EditItem.Uniqueue();
                        data.Message = ex.Message;
                        DataSlot.Set(id, data);
                        WinMsg.SendText(state.Scan, state.Main, id);
                    }));
                    throw ex;
                }
                else if (barArray.Count > 1)
                {
                    var ex = new ApplicationException("More than one bar code on image.");
                    ex.AddNext(new MethodInvoker(() =>
                    {
                        string id = Strings.VScan_EditItem.Uniqueue();
                        data.Message = ex.Message;
                        DataSlot.Set(id, data);
                        WinMsg.SendText(state.Scan, state.Main, id);
                    }));
                    throw ex;
                }

                try
                {
                    data.Parse(barArray[0].String);
                }
                catch (Exception e)
                {
                    var ex = new ApplicationException("Can't parse barcode string.", e);
                    ex.AddNext(new MethodInvoker(() =>
                    {
                        string id = Strings.VScan_EditItem.Uniqueue();
                        data.Message = ex.Message;
                        DataSlot.Set(id, data);
                        WinMsg.SendText(state.Scan, state.Main, id);
                        VoucherMonitorForm.ShowImage(id);
                    }));
                    throw ex;
                }
            });
            return method;
        }

        /// <summary>
        /// Used to find and cover the cardcode by using the barcode location
        /// </summary>
        /// <returns></returns>
        public static VoucherProcessDelegate CreateHideCardDetailsDelegateByBarcode()
        {
            var method = new VoucherProcessDelegate((Voucher data, StateObj state) =>
            {
                var distanceToHiddenArea = SettingsTable.Get<Point>(Strings.VScan_DistanceFromBarcodeBottomLeftToHiddenArea, Point.Empty);
                if (distanceToHiddenArea == Point.Empty)
                {
                    var ex = new ApplicationException("Cannot find chunk configuration.");
                    ex.AddNext(new MethodInvoker(() =>
                    {
                        string id = Strings.VScan_EditItem.Uniqueue();
                        data.Message = ex.Message;
                        DataSlot.Set(id, data);
                        WinMsg.SendText(state.Scan, state.Main, id);
                    }));
                    throw ex;
                }

                Size hideAreaSize = SettingsTable.Get<Size>(Strings.VScan_HiddenAreaSize, Size.Empty);
                Point hiddenAreaLocation = new Point(data.BarCodeArea.Left, data.BarCodeArea.Bottom);

                hiddenAreaLocation.Offset(distanceToHiddenArea);
                Rectangle hiddenArea = new Rectangle(hiddenAreaLocation, hideAreaSize);

                var hiddenAreaDraw = SettingsTable.Get<HiddenAreaDrawingCfg>(Strings.VScan_HiddenAreaDrawingCfg, HiddenAreaDrawingCfg.Default);

                using (var g = Graphics.FromImage(data.VoucherImage))
                    g.FillRectangleHatch(hiddenArea, hiddenAreaDraw.ForeColor, hiddenAreaDraw.BackColor, hiddenAreaDraw.Style);               

            });
            return method;
        }

        /// <summary>
        /// Used to find and cover the card code by a mark on the voucher image
        /// </summary>
        /// <returns></returns>
        public static VoucherProcessDelegate CreateHideCardDetailsDelegateByMark()
        {
            var method = new VoucherProcessDelegate((Voucher data, StateObj state) =>
            {
                var markArea = SettingsTable.Get<MarkAreaCfg>(Strings.VScan_MarkAreaConfiguration, MarkAreaCfg.Empty);

                if (markArea.Equals(MarkAreaCfg.Empty))
                {
                    var ex = new ApplicationException("Cannot find 'hide area' configuration.");
                    ex.AddNext(new MethodInvoker(() =>
                    {
                        string id = Strings.VScan_EditItem.Uniqueue();
                        data.Message = ex.Message;
                        DataSlot.Set(id, data);
                        WinMsg.SendText(state.Scan, state.Main, id);
                    }));
                    throw ex;
                }

                Rectangle hiddenArea;

                using (var bmp2 = data.VoucherImage.ToBlackWhite(markArea.BlackWhiteThreshold))
                    hiddenArea = bmp2.FindRectangle(Point.Empty, markArea.BackColor, markArea.MinAreaSize, markArea.MaxAreaSize, markArea.Threshold);

                var hiddenAreaDraw = SettingsTable.Get<HiddenAreaDrawingCfg>(Strings.VScan_DistanceFromBarcodeBottomLeftToHiddenArea, HiddenAreaDrawingCfg.Default);

                using (var g = Graphics.FromImage(data.VoucherImage))
                    g.FillRectangleHatch(hiddenArea, hiddenAreaDraw.ForeColor, hiddenAreaDraw.BackColor, hiddenAreaDraw.Style);  

            });
            return method;
        }

        /// <summary>
        /// Used to print over the voucher image 
        /// </summary>
        /// <returns></returns>
        public static VoucherProcessDelegate CreatePrintOnImageDelegate()
        {
            var method = new VoucherProcessDelegate((Voucher data, StateObj state) =>
            {
                var printLocation = SettingsTable.Get<Point>(Strings.VScan_PrintAreaLocation, Point.Empty);
                if (printLocation == Point.Empty)
                {
                    var ex = new ApplicationException("Cannot find 'print on image' configuration.");
                    ex.AddNext(new MethodInvoker(() =>
                    {
                        string id = Strings.VScan_EditItem.Uniqueue();
                        data.Message = ex.Message;
                        DataSlot.Set(id, data);
                        WinMsg.SendText(state.Scan, state.Main, id);
                    }));
                    throw ex;
                }

                var aliases = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
                aliases["[DATE]"] = new Func<string>(() => DateTime.Now.ToString());
                aliases["[OPERATOR]"] = new Func<string>(() => SettingsTable.Get<UserAuth>(Strings.Transferring_AuthObject, UserAuth.Default).Name);
                aliases["[PCNAME]"] = new Func<string>(() => PTFUtils.GetMachine());
                aliases["[SITEID]"] = new Func<string>(() => data.SiteCode);
                aliases["[NUMBER]"] = new Func<string>(() => Convert.ToString(SettingsTable.Get<int>(Strings.VScan_ScanCount, 0)));

                string voucherTemplate = SettingsTable.Get<string>(Strings.VScan_ImPrinterTemplate, Strings.VScan_ImPrinterTemplateDefault);
                string voucherText = voucherTemplate.format(aliases);

                using (var g = Graphics.FromImage(data.VoucherImage))
                    g.DrawString(voucherText, SystemFonts.CaptionFont, Brushes.Red, printLocation);

            });
            return method;
        }

        /// <summary>
        /// Used to insert voucher data to the database
        /// </summary>
        /// <returns></returns>
        public static VoucherProcessDelegate CreateInsertVoucherDelegate()
        {
            var method = new VoucherProcessDelegate((Voucher data, StateObj state) =>
            {
                Debug.Assert(state.Dib != IntPtr.Zero);
                Debug.Assert(state.Main != IntPtr.Zero);
                Debug.Assert(state.Scan != IntPtr.Zero);
                Debug.Assert(data != null);

                try
                {

                    data.Validate();

                    ClientDataAccess.UpdateFileAsync((DbClientVoucher)data,
                        new EventHandler((o, e) =>
                        {
                            string fileName = Path.ChangeExtension(
                                string.Format("{0}{1}", PTFUtils.GetMachine(), DateTime.Now).ReplaceAll(new char[] { '.', '/', '\\', '-' }, '_'), ".xml");

                            //Data has been saved successfully. Show message
                            string id = string.Concat(Strings.VScan_ItemSaved, fileName);
                            DataSlot.Set(id, data);
                            WinMsg.SendText(state.Main, id);
                        }),
                        new ThreadExceptionEventHandler((o, e) =>
                        {
                            //Data has failed to save.
                            var ex = new ApplicationException("Can't insert: ".concat(e.Exception.Message), e.Exception);
                            ex.AddNext(new MethodInvoker(() =>
                            {
                                string id = Strings.VScan_EditItem.Uniqueue();
                                data.Message = ex.Message;
                                DataSlot.Set(id, data);
                                WinMsg.SendText(state.Scan, state.Main, id);
                            }));
                            throw ex;
                        }));
                }
                catch (Exception e)
                {
                    var ex = new ApplicationException("Err: ".concat(e.Message), e);
                    ex.AddNext(new MethodInvoker(() =>
                    {
                        string id = Strings.VScan_EditItem.Uniqueue();
                        data.Message = ex.Message;
                        DataSlot.Set(id, data);
                        WinMsg.SendText(state.Main, id);
                    }));
                    throw ex;
                }
            });
            return method;
        }

        /// <summary>
        /// Send data to UI editor directly
        /// </summary>
        /// <returns></returns>
        public static VoucherProcessDelegate CreateSendToUIDelagete()
        {
            var method = new VoucherProcessDelegate((Voucher data, StateObj state) =>
            {
                string id = Strings.VScan_EditItem.Uniqueue();
                data.Message = "Insert data to db is switched off";
                DataSlot.Set(id, data);
                WinMsg.SendText(state.Scan, state.Main, id);
            });
            return method;
        }

        /// <summary>
        /// Used to clean up the system when job is node
        /// </summary>
        /// <returns></returns>
        public static VoucherProcessDelegate CreateEndScanDelegate()
        {
            var method = new VoucherProcessDelegate((Voucher data, StateObj state) =>
            {
                DelegateUtils.CreateAuditIdReloadDelegate().FireAndForget();
                int number = SettingsTable.Get<int>(Strings.VScan_ScanCount, 0);
                SettingsTable.Set(Strings.VScan_ScanCount, number + 1);
                TimeSpan cleanTime = SettingsTable.Get<TimeSpan>(Strings.VScan_SleepBeforeCleanTime, TimeSpan.FromSeconds(15));       
                Thread.Sleep(cleanTime);
                //Do clean up here
            });
            return method;
        } 

        /// <summary>
        /// Empty delegate. Doesn't do anything.
        /// </summary>
        /// <returns></returns>
        public static VoucherProcessDelegate CreateEmptyDelegate()
        {
            var method = new VoucherProcessDelegate((Voucher data, StateObj state) => { });
            return method;
        }

        /// <summary>
        /// Not Implemented delegate. 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static VoucherProcessDelegate CreateThrowDelegate()
        {
            var method = new VoucherProcessDelegate((Voucher data, StateObj state) => { throw new NotImplementedException("This case is not implemented yet"); });
            return method;
        }

        /// <summary>
        /// Log delegate. Logs to file.
        /// </summary>
        /// <returns></returns>
        public static VoucherProcessDelegate CreateProcessLogDelegate()
        {
            var method = new VoucherProcessDelegate((Voucher data, StateObj state) =>
            {
                var str = data.BarCodeString.concat(
                    " -> Width: ", data.VoucherImage.Width, 
                    " Height: ", data.VoucherImage.Height, 
                    " ProcessTime: ", data.ProcessTime.Elapsed);
                FileLogger.LogInfo(str, "VOUCHER");
            });
            return method;
        }
    }
}
