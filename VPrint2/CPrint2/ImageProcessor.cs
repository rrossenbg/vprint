using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
        public static event ThreadExceptionEventHandler Error;

        public static ImageProcessor Instance
        {
            get
            {
                return new ImageProcessor();
            }
        }

        private static IgnoreList<string> ms_Files = new IgnoreList<string>();

        private static volatile DataObj ms_DataObj = new DataObj();

        private class Lock1 { }
        private class Lock2 { }
        private class Lock3 { }

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

                            if (!obj.Equals(ms_DataObj))
                            {
                                ms_DataObj.DeleteFiles();
                                ms_DataObj = obj;
                            }

                            ms_DataObj.Submit = obj.Submit;

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
                Thread.Sleep(Config.ImagePickupDelay);

                string fullFileName = Convert.ToString(o);

                if (ms_Files.Add(fullFileName))
                {
                    var obj = ms_DataObj;

                    try
                    {
                        var file = new FileInfo(fullFileName);

                        if (file.Exists)
                        {
                            obj.Files.Add(file);

                            if (obj.Submit)
                            {
                                var keys = Security.CreateInstance().GenerateSecurityKeys();
                                var serverSessionId = Guid.NewGuid();
                                var sserverSessionId = serverSessionId.ToString();

                                string tifFullFileName = Path.Combine(
                                    Path.GetDirectoryName(((FileInfo)obj.Files[0]).FullName), 
                                    string.Format("{0}_{1}_{2}.tif", obj.Iso, obj.BrId, obj.VId));

                                var tifFile = new FileInfo(tifFullFileName);
                                var images = new List<Bitmap>();

                                lock (obj.Files)
                                {
                                    foreach (FileInfo fl in obj.Files)
                                    {
                                        using (var bmp = (Bitmap)Bitmap.FromFile(fl.FullName))
                                        {
                                            //var img = bmp.CropRotateGray(Config.MinWidth, Config.MaxWidth, Config.MinHeight,
                                            //    Config.MaxHeight, true, true).FirstOrDefault();
                                            var img = bmp.CropRotateFree();
                                            if (img != null)
                                                images.Add(img);
                                        }
                                    }
                                }

                                images.SaveMultipage(tifFile.FullName, "TIFF");
                                images.ForEach(i => i.DisposeSf());
                                images.Clear();

                                CertificateSecurity sec = new CertificateSecurity(X509FindType.FindBySerialNumber,
                                    Strings.CERTNUMBER, StoreLocation.LocalMachine);
                                //if (sec.Loaded)
                                //    item.Signature = sec.SignData(bmp.ToArray());

                                //copy voucher
                                var srv = ServiceDataAccess.Instance;

                                srv.SendFile(tifFile, sserverSessionId, keys);

                                srv.CommitVoucherChanges(sserverSessionId, 0, obj.Iso, obj.BrId, obj.VId,
                                    Global.FolderID.HasValue ? Global.FolderID.Value : (int?)null, "", "", keys);

                                srv.SaveHistory(OperationHistory.Scan, serverSessionId, obj.Iso, obj.BrId, obj.VId,
                                    0, 0, "", keys);

#warning TEST UNCOMMENT ON LIVE
                                
                                //tifFile.DeleteSafe();
                                //obj.DeleteFiles();
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
            ms_Files.Clear();
        }
    }
}
