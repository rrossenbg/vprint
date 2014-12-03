/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CPrint2.Common;
using CPrint2.Data;
using CPrint2.ScanServiceRef;
using Emgu.CV;
using Emgu.CV.Structure;

namespace CPrint2
{
    /// <summary>
    /// SAMPLES
    /// http://www.emgu.com/wiki/index.php?title=Camera_Capture
    /// </summary>
    public class CameraCapture
    {
        /// <summary>
        /// 2sec
        /// </summary>
        private readonly TimeSpan LOOP = TimeSpan.FromSeconds(2);

        public class NewImageEventArgs : EventArgs
        {
            public Image<Bgr, Byte> Image { get; set; }
            public object Param { get; set; }
        }

        public event EventHandler<NewImageEventArgs> NewImage;

        private readonly int m_Source;

        public CameraCapture(int source = 0)
        {
            m_Source = source;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <remarks>
        /// This code should always be called in main thread
        /// </remarks>
        public void RunWait(object @param)
        {
            using (var capture = new Capture(m_Source))
            {
                //capture.DuplexQueryFrame();
                capture.SetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FRAME_COUNT, 10);//
                capture.SetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FRAME_WIDTH, Config.FRAME_WIDTH);//5168, 1280, 2304, 
                capture.SetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FRAME_HEIGHT, Config.FRAME_HEIGHT);//2907, 720, 1536 
                capture.SetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FOURCC, CvInvoke.CV_FOURCC('U', '2', '6', '3')); //622,3730
                capture.SetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_MONOCROME, 1);

                CheckErrorThrow();

                var task = Task.Factory.StartNew((o) =>
                {
                    var sw = Stopwatch.StartNew();

                    for (; ; )
                    {
                        using (Image<Bgr, byte> img = capture.QueryFrame())
                        {
                            CheckErrorThrow();

                            if (img != null && NewImage != null && sw.Elapsed > LOOP)
                            {
                                var ne = new NewImageEventArgs() { Image = img.Copy(), Param = o };
                                NewImage(this, ne);
                                break;
                            }
                        }
                        Thread.Sleep(50);
                    }
                }, @param, TaskCreationOptions.AttachedToParent);
                Task.WaitAll(task);
            }
        }

        private static void CheckErrorThrow()
        {
            var stat = CvInvoke.cvGetErrStatus();
            if (stat != 0)
            {
                string message = CvInvoke.cvErrorStr(stat);
                throw new Exception(message);
            }
            //int err = Marshal.GetLastWin32Error();
            //if (err > 0)
            //    throw new Win32Exception(err);
        }
    }

    /// <summary>
    /// SAMPLES
    /// http://www.emgu.com/wiki/index.php?title=Camera_Capture
    /// </summary>
    public class CameraCaptureN : IDisposable
    {
        private readonly Hashtable m_Images0 = Hashtable.Synchronized(new Hashtable());
        private readonly Hashtable m_Images = Hashtable.Synchronized(new Hashtable());
        private volatile bool m_isInit;
        private readonly static Hashtable ms_CaptureData = Hashtable.Synchronized(new Hashtable());

        public class NewImageEventArgs : EventArgs
        {
            public bool Init { get; set; }
            public int Index { get; set; }
            public Capture Capture { get; set; }
            public Image<Gray, Byte> Image { get; set; }
            public object Param { get; set; }
        }

        private readonly int[] m_Sources;

        public CameraCaptureN(params int[] sources)
        {
            m_Sources = sources;
        }

        public void UpdateImage0(int index, FileInfo file)
        {
            FileInfo info = (FileInfo)m_Images0[index];
            if (info != null)
                info.DeleteSafe();
            m_Images0[index] = file;
        }

        public void UpdateImage(int index, FileInfo file)
        {
            FileInfo info = (FileInfo)m_Images[index];
            if (info != null)
                info.DeleteSafe();
            m_Images[index] = file;
        }

        public FileInfo RetrieveImage0(int index)
        {
            FileInfo info = (FileInfo)m_Images0[index];
            return info;
        }

        public IEnumerable<FileInfo> RetrieveImages()
        {
            lock (m_Images.SyncRoot)
            {
                foreach (DictionaryEntry info in m_Images)
                    yield return (FileInfo)info.Value;
            }
        }

        public bool IsInitialized()
        {
            return m_Images0.Count == m_Sources.Length;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <remarks>
        /// This code should always be called in main thread
        /// </remarks>
        public void RunWait(bool isInit, object @param = null)
        {
            Clear(isInit);

            m_isInit = isInit;

            Application.Idle += new EventHandler(Application_Idle);

            foreach (int source in m_Sources)
            {
                if(ms_CaptureData.ContainsKey(source))
                    using (((Tuple<int, Capture, object>)ms_CaptureData[source]).Item2) ;

                var capture = new Capture(source);
                capture.SetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FRAME_COUNT, 10);//
                capture.SetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FRAME_WIDTH, 2304);//Config.FRAME_WIDTH);//5168, 1280, 2304, 
                capture.SetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FRAME_HEIGHT, 1536);//Config.FRAME_HEIGHT);//2907, 720, 1536 
                capture.SetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FOURCC, CvInvoke.CV_FOURCC('U', '2', '6', '3')); //622,3730

                capture.CheckErrorThrow();

                ms_CaptureData[source] = new Tuple<int, Capture, object>(source, capture, @param);
            }
        }

        private void Application_Idle(object sender, EventArgs e)
        {
            foreach (int source in m_Sources)
            {
                var tuple = (Tuple<int, Capture, object>)ms_CaptureData[source];

                ///http://www.emgu.com/forum/viewtopic.php?f=7&t=6
                ///http://wenku.baidu.com/view/75485602de80d4d8d15a4f1c
                ///http://stackoverflow.com/questions/17392395/background-subtaction-using-emgu-cv
                for (; ; )
                {
                    using (Image<Bgr, byte> img = tuple.Item2.QueryFrame())
                    {
                        if (img != null)
                        {
                            #region INNER

                            var imgc = img.Copy();

                            var file2 = ((FileInfo)null).Temp();

                            Image<Bgr, byte> result = null;

                            try
                            {
                                if (m_isInit)
                                {
                                    imgc.Bitmap.Save(file2.FullName, ImageFormat.Jpeg);
                                    imgc.Bitmap.Save(tuple.Item1 == 0 ? "C:\\test0_0.jpg" : "C:\\test0_1.jpg", ImageFormat.Jpeg);

                                    this.UpdateImage0(tuple.Item1, file2);

                                    MainForm.Default.ShowImageAsynch(file2.FullName);
                                }
                                else
                                {
                                    var obj = (DataObj)tuple.Item3;

                                    var file0 = this.RetrieveImage0(tuple.Item1);

                                    if (file0 == null)
                                        throw new Exception(string.Format("Image0 is null for camera '{0}'. Create one.", tuple.Item1));

                                    using (var img0 = new Image<Bgr, byte>(file0.FullName))
                                    using (var diffImage = imgc.AbsDiff(img0))
                                    {
                                        diffImage.Bitmap.Save(file2.FullName, ImageFormat.Jpeg);
                                        diffImage.Bitmap.Save(tuple.Item1 == 0 ? "C:\\test1_0.jpg" : "C:\\test1_1.jpg", ImageFormat.Jpeg);
                                    }
                                    
                                    this.UpdateImage(tuple.Item1, file2);

                                    if (tuple.Item1 + 1 == m_Sources.Length)
                                    {
                                        foreach (var file in this.RetrieveImages())
                                            if (result != null)
                                                result = result.JoinFree(new Image<Bgr, byte>(file.FullName));
                                            else
                                                result = new Image<Bgr, byte>(file.FullName);

                                        var file3 = ((FileInfo)null).Temp();

                                        try
                                        {
                                            result.Bitmap.Save(file3.FullName, ImageFormat.Jpeg);
                                            result.Bitmap.Save("C:\\test_result.jpg", ImageFormat.Jpeg);

                                            MainForm.Default.ShowImageAsynch(file3.FullName);

                                            //copy voucher
                                            var keys = Security.CreateInstance().GenerateSecurityKeys();
                                            var serverSessionId = obj.Id;
                                            var sserverSessionId = obj.Id.ToString();

                                            var srv = ServiceDataAccess.Instance;

                                            srv.SendFile(file3, sserverSessionId, keys);

                                            srv.CommitVoucherChanges(sserverSessionId, 0, obj.Iso, obj.BrId, obj.VId,
                                                Global.FolderID.HasValue ? Global.FolderID.Value : (int?)null, "", "", keys);

                                            srv.SaveHistory(OperationHistory.Scan, serverSessionId, obj.Iso, obj.BrId, obj.VId, 0, 0, "", keys);
                                        }
                                        finally
                                        {
                                            //file3.DeleteSafe();
                                        }
                                    }
                                }
                                break;
                            }
                            catch (Exception ex)
                            {
                                AppContext.Default.FireError(ex);
                            }
                            finally
                            {
                                imgc.DisposeSf();
                                result.DisposeSf();
                                Application.Idle -= new EventHandler(Application_Idle);
                            }
                            #endregion
                        }
                    }
                }
            }
        }

        private void Clear(bool init)
        {
            if (init)
            {
                lock (m_Images0.SyncRoot)
                {
                    foreach (DictionaryEntry info in m_Images0)
                        ((FileInfo)info.Value).DeleteSafe();
                    m_Images0.Clear();
                }
            }

            lock (m_Images.SyncRoot)
            {
                foreach (DictionaryEntry info in m_Images)
                    ((FileInfo)info.Value).DeleteSafe();
                m_Images.Clear();
            }
        }

        public void Dispose()
        {
            foreach (int source in m_Sources)
            {
                if (ms_CaptureData.ContainsKey(source))
                    using (((Tuple<int, Capture, object>)ms_CaptureData[source]).Item2) ;
            }
        }
    }
}