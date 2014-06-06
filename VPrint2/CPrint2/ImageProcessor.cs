/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CPrint2.Colections;
using CPrint2.Common;
using CPrint2.Data;
using CPrint2.ScanServiceRef;

namespace CPrint2
{
    public class ImageProcessor
    {
        public static event EventHandler NewVoucherStarted;
        public static event EventHandler<ValueEventArgs<string>> VoucherProcessCompleted;
        public static event ThreadExceptionEventHandler Error;

        public static ImageProcessor Instance
        {
            get
            {
                return new ImageProcessor();
            }
        }

        private static readonly IgnoreList<string> ms_Files = new IgnoreList<string>();

        private static readonly ConcurrentDictionary<Guid, DataObj> ms_Queue = new ConcurrentDictionary<Guid, DataObj>();

        private static volatile object ms_CurrentDataObj;

        private class Lock1 { }
        private class Lock2 { }
        private class Lock3 { }

        static ImageProcessor()
        {
            ms_CurrentDataObj = (object)Guid.Empty;
        }

        public void ProcessCommandFile(string fileName)
        {
            Task.Factory.StartNew((o) =>
            {
                Thread.Sleep(300);

                string fullFileName = Convert.ToString(o);

                try
                {
                    if (ms_Files.Add(fullFileName))
                    {
                        try
                        {
                            string text = File.ReadAllText(fullFileName);

                            var obj = DataObj.Parse(text);

                            if (obj == null || !obj.IsValid)
                                return;

                            ms_CurrentDataObj = obj.Id;

                            if (ms_Queue.ContainsKey(obj.Id))
                            {
                                ms_Queue[obj.Id].Submit = obj.Submit;
                            }
                            else  
                            {
                                ms_Queue.TryAdd(obj.Id, obj);

                                if (NewVoucherStarted != null)
                                    NewVoucherStarted(this, EventArgs.Empty);
                            }

                            PresenterCameraShooter shooter = new PresenterCameraShooter();
                            shooter.TryStartPresenter(Config.PresenterPath);
                            shooter.ClickCameraButton();
                        }
                        catch (Exception ex)
                        {
                            if (Error != null)
                                Error(this, new ThreadExceptionEventArgs(ex));
                        }
                    }
                }
                finally
                {
                    new Action(() => File.Delete(fullFileName)).RunSafe();
                }

            }, fileName, TaskCreationOptions.LongRunning);
        }

        public void ProcessReadyImage(string fileName)
        {
            Task.Factory.StartNew((o) =>
            {
                string fullFileName = Convert.ToString(o);

                if (ms_Files.Add(fullFileName))
                {
                    Guid current = (Guid)ms_CurrentDataObj;

                    Thread.Sleep(Config.ImagePickupDelay);

                    try
                    {
                        var file = new FileInfo(fullFileName);

                        DataObj obj = null;

                        if (file.Exists && ms_Queue.TryGetValue(current, out obj))
                        {
                            obj.Files.Add(file);

                            using (var bmp = (Bitmap)Bitmap.FromFile(file.FullName))
                            {
                                var img = bmp.CropRotateFree();
                                if (img != null)
                                {
                                    #if HIGH_CMPRESION
                                    var jpegCodec = ImageCodecInfo.GetImageEncoders().First(enc => enc.FormatID == ImageFormat.Jpeg.Guid);
                                    var jpegParams = new EncoderParameters(1);
                                    jpegParams.Param = new[] { new EncoderParameter(Encoder.Quality, 100L) };
                                    img.Save(file.FullName, jpegCodec, jpegParams);
                                    #endif
                                    img.Save(file.FullName, ImageFormat.Jpeg);
                                }
                            }

                            if (VoucherProcessCompleted != null)
                                VoucherProcessCompleted(this, new ValueEventArgs<string>(file.FullName));

                            if (obj.Submit)
                            {
                                try
                                {
                                    string tifFullFileName = Path.Combine(
                                        Path.GetDirectoryName(((FileInfo)obj.Files[0]).FullName),
                                        string.Format("{0}_{1}_{2}.tif", obj.Iso, obj.BrId, obj.VId));

                                    var keys = Security.CreateInstance().GenerateSecurityKeys();
                                    var serverSessionId = Guid.NewGuid();
                                    var sserverSessionId = serverSessionId.ToString();

                                    var tifFile = new FileInfo(tifFullFileName);
                                    var images = new List<Bitmap>();

                                    lock (obj.Files)
                                    {
                                        foreach (FileInfo fl in obj.Files)
                                            if (fl.Exists(true))
                                                images.Add((Bitmap)Bitmap.FromFile(fl.FullName));
                                    }

                                    if (images.Count > 0)
                                    {
                                        var tif = TiffConverter.WrapJpegs(images.ConvertAll<byte[]>((b) => b.ToArray()));
                                        File.WriteAllBytes(tifFullFileName, tif);

                                        images.ForEach(i => i.DisposeSf());
                                        images.Clear();

                                        //copy voucher
                                        var srv = ServiceDataAccess.Instance;

                                        srv.SendFile(tifFile, sserverSessionId, keys);

                                        srv.CommitVoucherChanges(sserverSessionId, 0, obj.Iso, obj.BrId, obj.VId,
                                            Global.FolderID.HasValue ? Global.FolderID.Value : (int?)null, "", "", keys);

                                        srv.SaveHistory(OperationHistory.Scan, serverSessionId, obj.Iso, obj.BrId, obj.VId,
                                            0, 0, "", keys);
                                    }

                                    tifFile.DeleteSafe();
                                }
                                finally
                                {
                                    DataObj obj3;
                                    ms_Queue.TryRemove(current, out obj3);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (Error != null)
                            Error(this, new ThreadExceptionEventArgs(ex));
                    }
                }
            }, fileName, TaskCreationOptions.LongRunning);
        }

        public static void Clear()
        {
            ms_Queue.Clear();
            ms_Files.Clear();
            ms_CurrentDataObj = (object)Guid.Empty;
        }
    }
}