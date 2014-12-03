/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using CPrint2.Data;
using Emgu.CV;
using Emgu.CV.Structure;

namespace CPrint2.Controls
{
    public partial class CameraControl : UserControl
    {
        public enum ShowMode
        {
            Normal,
            ShowOrigin,
            Thresholded,
            HSV
        }

        public static ShowMode Mode { get; set; }

        public delegate void ImageTakenDelegate(object sender, EventArgs e);
        public event ImageTakenDelegate ImageTaken;

        private Capture m_Cap = default(Capture);

        public FileInfo Cap_Fore_Info { get; set; }
        public int CameraIndex { get; set; }

        private volatile bool m_RunCommand;

        public CameraControl()
        {
            InitializeComponent();
        }

        public CameraControl(int index)
        {
            InitializeComponent();
            CameraIndex = index;

            m_Cap = new Capture(index);
            m_Cap.SetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FRAME_COUNT, Config.FRAME_COUNT);//
            m_Cap.SetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FRAME_WIDTH, Config.FRAME_WIDTH);//5168, 1280, 2304, 
            m_Cap.SetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FRAME_HEIGHT, Config.FRAME_HEIGHT);//2907, 720, 1536 
            m_Cap.SetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FOURCC, CvInvoke.CV_FOURCC('U', '2', '6', '3')); //622,3730
            m_Cap.ImageGrabbed += cap_ImageGrabbed;
        }

        public void Start()
        {
            m_Cap.Start();
        }

        public void Run()
        {
            m_RunCommand = true;
        }

        public void Stop()
        {
            m_Cap.Stop();
        }

        private unsafe void cap_ImageGrabbed(object sender, EventArgs e)
        {
            using (var img = m_Cap.RetrieveBgrFrame())
            {
                using (Image<Bgr, byte> normal = img.Copy())
                using (Image<Gray, byte> thresholded = new Image<Gray, byte>(img.Width, img.Height))
                using (Image<Gray, byte> hsv = new Image<Gray, byte>(img.Width, img.Height))
                {
                    var Origin = (MIplImage)Marshal.PtrToStructure(img.Ptr, typeof(MIplImage));
                    var Thresholded = (MIplImage)Marshal.PtrToStructure(thresholded.Ptr, typeof(MIplImage));
                    var Hsv = (MIplImage)Marshal.PtrToStructure(hsv.Ptr, typeof(MIplImage));
                    Rectangle roi = new Rectangle();
                    VCamLib.ProcessImage(Origin, Thresholded, Hsv, ref roi);
                    if (Mode == ShowMode.Normal)
                        captureBox.Image = normal;
                    else if (Mode == ShowMode.ShowOrigin)
                        captureBox.Image = img;
                    else if (Mode == ShowMode.Thresholded)
                        captureBox.Image = thresholded;
                    else if (Mode == ShowMode.HSV)
                        captureBox.Image = hsv;

                    if (m_RunCommand)
                    {
                        //Save
                        m_RunCommand = false;

                        Cap_Fore_Info = ((FileInfo)null).Temp().IfDebug(string.Format("C:\\test{0}_fore.jpg", CameraIndex));

                        using (Image<Bgr, byte> result = normal.Copy(roi))
                            result.ToBitmap().Save(Cap_Fore_Info.FullName, ImageFormat.Jpeg);

                        if (ImageTaken != null)
                            ImageTaken(this, EventArgs.Empty);
                    }
                }
            }
        }
    }
}
