/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.Drawing;
using System.Runtime;
using System.Runtime.InteropServices;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace CPrint2
{
    public static class ImagesCVEx
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="img1"></param>
        /// <param name="img2"></param>
        /// <returns></returns>
        /// <see cref="//http://opencv-users.1802565.n2.nabble.com/Combine-2-images-into-1-larger-image-td4175190.html"/>
        public static Image<Bgr, byte> JoinFree(this Image<Bgr, byte> img1, Image<Bgr, byte> img2)
        {
            if (img1 == null)
                throw new ArgumentNullException("img1");

            if (img2 == null)
                throw new ArgumentNullException("img2");

            try
            {

                var size = new Size(img1.Width + img2.Width + 3, Math.Max(img1.Height, img2.Height));

                var stacked = CvInvoke.cvCreateImage(size, IPL_DEPTH.IPL_DEPTH_8U, 3);

                CvInvoke.cvZero(stacked);

                CvInvoke.cvSetImageROI(stacked, new Rectangle(0, 0, img1.Width, img1.Height));
                CvInvoke.cvCopy(img1.Ptr, stacked, IntPtr.Zero);

                CvInvoke.cvResetImageROI(stacked);
                CvInvoke.cvSetImageROI(stacked, new Rectangle(img1.Width + 3, 0, img2.Width, img2.Height));
                CvInvoke.cvCopy(img2.Ptr, stacked, IntPtr.Zero);

                MIplImage mIpl = (MIplImage)Marshal.PtrToStructure(stacked, typeof(MIplImage));
                var result = new Image<Bgr, byte>(mIpl.width, mIpl.height, mIpl.widthStep, mIpl.imageData);
                return result;
            }
            finally
            {
                using (img1) ;
                using (img2) ;
            }
        }

        [TargetedPatchingOptOut("na")]
        public static void CheckErrorThrow(this Capture cap)
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

        [TargetedPatchingOptOut("na")]
        public static Image<Bgr, byte> DrawBorder(this Image<Bgr, byte> image, int offset, Color color, int tickness = 5)
        {
            MCvBox2D box = new MCvBox2D(new PointF(image.Width / 2, image.Height / 2), new SizeF(image.Width - offset, image.Height - offset), 0);
            image.Draw(box, new Bgr(color), tickness);
            return image;
        }

        /// <summary>
        /// WORKS OK! USE IT!
        /// </summary>
        /// <param name="currentFrame"></param>
        /// <param name="draw"></param>
        /// <returns></returns>
        public static Rectangle FindBiggestBlob(this Image<Bgr, Byte> currentFrame, bool draw = false)
        {
            using (Image<Gray, Byte> img1 = currentFrame.Convert<Gray, Byte>())
            using (Image<Gray, Byte> img2 = img1.PyrDown())
            using (Image<Gray, Byte> grayImage = img2.PyrUp())
            using (Image<Gray, Byte> cannyImage = new Image<Gray, Byte>(grayImage.Size))
            {
                CvInvoke.cvCanny(grayImage, cannyImage, 10, 60, 3);

                using (StructuringElementEx kernel = new StructuringElementEx(3, 3, 1, 1, Emgu.CV.CvEnum.CV_ELEMENT_SHAPE.CV_SHAPE_RECT))
                    CvInvoke.cvDilate(cannyImage, cannyImage, kernel, 1);

                IntPtr cont = IntPtr.Zero;

                int top = int.MaxValue, left = int.MaxValue, bottom = 0, right = 0;

                //allocate storage for contour approximation
                using (MemStorage storage = new MemStorage())
                {
                    for (Contour<Point> contours = cannyImage.FindContours(CHAIN_APPROX_METHOD.CV_LINK_RUNS, RETR_TYPE.CV_RETR_LIST);
                        contours != null; contours = contours.HNext)
                    {
#if KNOW_HOW
                        IntPtr seq = CvInvoke.cvConvexHull2(contours, storage.Ptr, Emgu.CV.CvEnum.ORIENTATION.CV_CLOCKWISE, 0);
                        IntPtr defects = CvInvoke.cvConvexityDefects(contours, seq, storage);
#endif
                        Seq<Point> tr = contours.GetConvexHull(Emgu.CV.CvEnum.ORIENTATION.CV_CLOCKWISE);

#if KNOW_HOW
                        Seq<Emgu.CV.Structure.MCvConvexityDefect> te = contours.GetConvexityDefacts(storage, Emgu.CV.CvEnum.ORIENTATION.CV_CLOCKWISE);
#endif
                        if (draw)
                            currentFrame.Draw(tr.BoundingRectangle, new Bgr(Color.Red), 3);

                        top = Math.Min(top, tr.BoundingRectangle.Top);
                        left = Math.Min(left, tr.BoundingRectangle.Left);
                        bottom = Math.Max(bottom, tr.BoundingRectangle.Bottom);
                        right = Math.Max(right, tr.BoundingRectangle.Right);
                    }
                }

                Rectangle result = Rectangle.FromLTRB(left, top, right, bottom);
                if (draw)
                    currentFrame.Draw(result, new Bgr(Color.Blue), 3);
                return result;
            }
        }
    }
}