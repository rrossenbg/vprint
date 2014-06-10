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
            if (!ms_Files.Add(fileName))
                return;

            Task.Factory.StartNew((o) =>
            {
                Thread.Sleep(300);

                string fullFileName = Convert.ToString(o);

                try
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
                            if (obj.Submit)
                            {
                                #region SUBMIT
                                try
                                {
                                    DataObj obj3 = null;
                                    ms_Queue.TryGetValue(obj.Id, out obj3);

                                    string tifFullFileName = Path.Combine(
                                        Path.GetDirectoryName(((FileInfo)obj3.Files[0]).FullName),
                                        string.Format("{0}_{1}_{2}.tif", obj3.Iso, obj3.BrId, obj3.VId));

                                    var keys = Security.CreateInstance().GenerateSecurityKeys();
                                    var serverSessionId = obj.Id;
                                    var sserverSessionId = obj.Id.ToString();

                                    var tifFile = new FileInfo(tifFullFileName);
                                    var images = new List<Bitmap>();

                                    lock (obj.Files)
                                    {
                                        foreach (FileInfo fl in obj3.Files)
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

                                        srv.CommitVoucherChanges(sserverSessionId, 0, obj3.Iso, obj3.BrId, obj3.VId,
                                            Global.FolderID.HasValue ? Global.FolderID.Value : (int?)null, "", "", keys);

                                        srv.SaveHistory(OperationHistory.Scan, serverSessionId, obj3.Iso, obj3.BrId, obj3.VId,
                                            0, 0, "", keys);
                                    }

                                    tifFile.DeleteSafe();
                                    return;
                                }
                                finally
                                {
                                    DataObj obj3;
                                    ms_Queue.TryRemove(obj.Id, out obj3);
                                }
                                #endregion
                            }
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
                finally
                {
                    new Action(() => File.Delete(fullFileName)).RunSafe();
                }

            }, fileName, TaskCreationOptions.LongRunning);
        }

        public void ProcessReadyImage(string fileName)
        {
            if (!ms_Files.Add(fileName))
                return;

            Task.Factory.StartNew((o) =>
            {
                string fullFileName = Convert.ToString(o);
                Guid current = (Guid)ms_CurrentDataObj;

                Thread.Sleep(Config.ImagePickupDelay);

                try
                {
                    var file1 = new FileInfo(fullFileName);
                    var file2 = new FileInfo(Path.Combine(file1.DirectoryName, Path.GetFileNameWithoutExtension(file1.Name) + "_2.jpg"));
                    ms_Files.Add(file2.FullName);

                    DataObj obj = null;

                    if (file1.Exists && ms_Queue.TryGetValue(current, out obj) && obj != null)
                    {
                        using (var bmp = (Bitmap)Bitmap.FromFile(file1.FullName))
                        {
                            var img = bmp.CropRotateFree(Config.MinSize, Config.MaxSize);
                            if (img != null)
                            {
                                var jpegCodec = ImageCodecInfo.GetImageEncoders().First(enc => enc.FormatID == ImageFormat.Jpeg.Guid);
                                var jpegParams = new EncoderParameters(1);
                                jpegParams.Param = new[] { new EncoderParameter(Encoder.Quality, 100L) };
                                img.Save(file2.FullName, jpegCodec, jpegParams);

                                obj.Files.Add(file2);
                                file1.DeleteSafe();

                                if (VoucherProcessCompleted != null)
                                    VoucherProcessCompleted(this, new ValueEventArgs<string>(file2.FullName));
                            }
                            else
                            {
                                if (VoucherProcessCompleted != null)
                                    VoucherProcessCompleted(this, new ValueEventArgs<string>(file1.FullName));
                            }
                        }                        
                    }
                }
                catch (Exception ex)
                {
                    if (Error != null)
                        Error(this, new ThreadExceptionEventArgs(ex));
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