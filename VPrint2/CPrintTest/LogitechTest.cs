using System;
using System.Drawing;
using System.Drawing.Imaging;
using AForge.Imaging;
using AForge.Imaging.Filters;
using CPrint2;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace CPrintTest
{
    [TestClass]
    public class LogitechTest
    {
        [TestMethod]
        public void Logitech_test1()
        {
            double alpha = 0.003; //stores alpha for thread access
            int Threshold = 60; //stores threshold for thread access


            //var shooter = new CameraShooter();
            //shooter.ClickLogitechShootButton();

            var Background = new Image<Bgr, byte>("C:\\Picture 70.jpg").Convert<Gray, Byte>().Convert<Gray, float>();
            var Frame = new Image<Bgr, byte>("C:\\Picture 71.jpg");
            var Gray_Frame = Frame.Convert<Gray, Byte>().Convert<Gray, float>();
            var Difference = Background.AbsDiff(Gray_Frame);
            Difference.ToBitmap().Save("C:\\test_Difference.jpg");

            Background.RunningAvg(Difference, alpha);
            Background.ToBitmap().Save("C:\\test_Background.jpg");
        }

        [TestMethod]
        public void Logitech_test2()
        {
            //var shooter = new CameraShooter();
            //shooter.ClickLogitechShootButton();
            SubtractImages();
        }

        [TestMethod]
        public void A2()
        {
            RGBImageMaskBGSubtraction("C:\\Picture 72.jpg", false);
        }

        // background subtraction by converting the RGB image to binary to create the mask
        // then use this mask to copy foreground objects in the image
        private void RGBImageMaskBGSubtraction(string filename, bool displayResult)
        {
            // load the threshold value for grayscale image
            double threshold = double.Parse("60");

            // create new image
            Image<Bgr, Byte> img = new Image<Bgr, byte>(filename);

            //convert to grayscale
            Image<Gray, Byte> gray = img.Convert<Gray, Byte>();

            //convert to binary image using the threshold
            gray = gray.ThresholdBinary(new Gray(threshold), new Gray(255));

            // copy pixels from the original image where pixels in 
            // mask image is nonzero
            Image<Bgr, Byte> newimg = img.Copy(gray);


        }

        [TestMethod]
        public void B()
        {
            try
            {
                int numberOfIterations = 1;

                var frame = new Image<Bgr, byte>("C:\\Picture 71.jpg");

                System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, frame.Width, frame.Height);
                Image<Gray, byte> mask = frame.GrabCut(rect, numberOfIterations);

                mask = mask.ThresholdBinary(new Gray(2), new Gray(255));

                System.Drawing.Color cl = System.Drawing.Color.FromArgb(0, 255, 255, 255);
                Image<Bgr, Byte> processed_image = new Image<Bgr, Byte>("C:\\Picture 70.jpg");
                frame.Copy(processed_image, mask);

                // My way to display frame 
                processed_image.ToBitmap().Save("C:\\test_result.jpg", ImageFormat.Jpeg);
            }
            catch (Exception ex)
            {
            }
        }

        [TestMethod]
        public void C()
        {
            Image<Bgr, byte> image0 = new Image<Bgr, byte>("C:\\Picture 70.jpg");
            Image<Bgr, byte> image1 = new Image<Bgr, byte>("C:\\Picture 71.jpg");
            var img = image0.Or(image1);
            int w = img.Width;
        }

        public void SubtractImages()
        {
            Image<Bgr, Byte> Frame = null; //current Frame from camera
            Image<Gray, float> Gray_Frame; //gray_frame form camera
            Image<Gray, float> Background = null; //Average Background being formed
            Image<Gray, float> Previous_Frame; //Previiousframe aquired
            Image<Gray, float> Difference; //Difference between the two fra

            double alpha = 0.003; //stores alpha for thread access
            int Threshold = 60; //stores threshold for thread access

            using (var _capture = new Capture())
            {

                if (Frame == null) //we need at least one fram to work out running average so acquire one before doing anything
                {
                    //display the frame aquired
                    Frame = _capture.RetrieveBgrFrame(); //we could use RetrieveGrayFrame if we didn't care about displaying colour image
                    //DisplayImage(Frame.ToBitmap(), captureBox); //display the image using thread safe call

                    //copy the frame to previousframe
                    //Previous_Frame = Frame.Convert<Gray, Byte>().Convert<Gray, float>(); //we can only convert one aspect at a time so wel call convert twice
                    Background = Frame.Convert<Gray, Byte>().Convert<Gray, float>(); //we can only convert one aspect at a time so wel call convert twice
                }

                //acquire the frame
                Frame = _capture.RetrieveBgrFrame(); //we could use RetrieveGrayFrame if we didn't care about displaying colour image
                //DisplayImage(Frame.ToBitmap(), captureBox); //display the image using thread safe call

                //create a gray copy for processing
                Gray_Frame = Frame.Convert<Gray, Byte>().Convert<Gray, float>(); //we can only convert one aspect at a time so wel call convert twice

                //cvAbsDiff(pFrameMat, pBkMat, pFrMat);
                Difference = Background.AbsDiff(Gray_Frame); //find the absolute difference 

                Difference.ToBitmap().Save("C:\\test_Difference.jpg");

                //CvInvoke.cvRunningAvg(Difference, Background, 0.003, Background); 
                /*Play with the alpha weighting 0.001 */
                Background.RunningAvg(Difference, alpha); //performe the cvRunningAvg frame acumullation
                //DisplayImage(Background.ToBitmap(), resultbox); //display the image using thread safe call
                Background.ToBitmap().Save("C:\\test_Background.jpg");
            }
        }

        public void A()
        {
            Image<Bgr, Byte> Frame = null; //current Frame from camera
            Image<Gray, float> Gray_Frame; //gray_frame form camera
            Image<Gray, float> Background = null; //Average Background being formed
            Image<Gray, float> Previous_Frame; //Previiousframe aquired
            Image<Gray, float> Difference; //Difference between the two fra

            using (var _capture = new Capture())
            {
                if (Frame == null) //we need at least one fram to work out running average so acquire one before doing anything
                {
                    //display the frame aquired
                    Frame = _capture.RetrieveBgrFrame(); //we could use RetrieveGrayFrame if we didn't care about displaying colour image
                    //DisplayImage(Frame.ToBitmap(), captureBox); //display the image using thread safe call

                    //copy the frame to previousframe
                    //Previous_Frame = Frame.Convert<Gray, Byte>().Convert<Gray, float>(); //we can only convert one aspect at a time so wel call convert twice
                    Background = Frame.Convert<Gray, Byte>().Convert<Gray, float>(); //we can only convert one aspect at a time so wel call convert twice
                }

                //acquire the frame
                Frame = _capture.RetrieveBgrFrame(); //we could use RetrieveGrayFrame if we didn't care about displaying colour image
                //DisplayImage(Frame.ToBitmap(), captureBox); //display the image using thread safe call

                //    CvInvoke.cvDFT(Frame.Convert<Gray, Single>().Ptr, DFT.Ptr, Emgu.CV.CvEnum.CV_DXT.CV_DXT_FORWARD, -1);
                //CvInvoke.cvDFT(Background.Convert<Gray, Single>().Ptr, DFTBack.Ptr, Emgu.CV.CvEnum.CV_DXT.CV_DXT_FORWARD, -1);

                //CvInvoke.cvDFT((DFTBack - DFT).Convert<Gray, Single>().Ptr, originalLeft.Ptr, Emgu.CV.CvEnum.CV_DXT.CV_DXT_INVERSE, -1);
                //CvInvoke.cvDFT((DFT - DFTBack).Ptr, originalRight.Ptr, Emgu.CV.CvEnum.CV_DXT.CV_DXT_INVERSE, -1);

                ////create a gray copy for processing
                //Gray_Frame = Frame.Convert<Gray, Byte>().Convert<Gray, float>(); //we can only convert one aspect at a time so wel call convert twice

                ////cvAbsDiff(pFrameMat, pBkMat, pFrMat);
                //Difference = Background.AbsDiff(Gray_Frame); //find the absolute difference 

                //Difference.ToBitmap().Save("C:\\test_Difference.jpg");

                ////CvInvoke.cvRunningAvg(Difference, Background, 0.003, Background); 
                ///*Play with the alpha weighting 0.001 */
                //Background.RunningAvg(Difference, alpha); //performe the cvRunningAvg frame acumullation
                ////DisplayImage(Background.ToBitmap(), resultbox); //display the image using thread safe call
                //Background.ToBitmap().Save("C:\\test_Background.jpg");
            }
        }

        [TestMethod]
        ///http://www.aforgenet.com/framework/docs/html/2d04f587-3272-2ad5-f8bc-54ff407d41f2.htm
        public void SubtractAForge()
        {
            Bitmap back = (Bitmap)Bitmap.FromFile("C:\\Picture 70.jpg");

            Subtract filter = new Subtract(back);
            Bitmap sourceImage = (Bitmap)Bitmap.FromFile("C:\\Picture 71.jpg");
            var image = filter.Apply(sourceImage);

            ExtractBiggestBlob filter2 = new ExtractBiggestBlob();
            Bitmap biggestBlobsImage = filter2.Apply(image);
            var o = filter2.OriginalImage;
            var p = filter2.BlobPosition;

            biggestBlobsImage.Save("C:\\Picture 71-70 Result.jpg", ImageFormat.Jpeg);
        }

        // Returns a new image that is a cropped version (rectangular cut-out)
        // of the original image.
        public Image<Bgr, byte> cropImage(Image<Bgr, byte> img, Rectangle region)
        {

            // Set the desired region of interest.
            CvInvoke.cvSetImageROI(img, region);
            // Copy region of interest into a new iplImage and return it.
            Size size = region.Size;

            IntPtr imageCropped = CvInvoke.cvCreateImage(size, IPL_DEPTH.IPL_DEPTH_8U, img.NumberOfChannels);
            CvInvoke.cvCopy(img, imageCropped, IntPtr.Zero);	// Copy just the region.

            return null;
        }

        //[TestMethod]
        //public void BlobTest()
        //{
        //    Image<Gray, Byte> img = new Image<Gray, byte>(@"test.png");
        //    Image<Bgr, Byte> img2 = img.Convert<Bgr, Byte>();

        //    // Get the image blobs
        //    Blob[] blobs = img.GetBlobs(100, IntPtr.Zero, true, true);

        //    // Skip the one with the biggest area
        //    IEnumerable<Blob> filtered = blobs.OrderByDescending(b => b.Area).Skip(1);
        //    // And the one with the worst perimeter-area ratio
        //    filtered = filtered.OrderBy(b => b.PerimeterLength / b.Area).Skip(1);

        //    //// Draw them in img2
        //    img2.DrawBlobs(filtered.ToArray(),
        //                              true,                                     // Fill
        //                              true, new Bgr(255, 0, 0),       // Draw Bounding box
        //                              true, new Bgr(0, 255, 0),       // Draw Convex hull 
        //                              true, new Bgr(0, 0, 255),       // Draw Ellipse
        //                              true, new Bgr(255, 0, 0),       // Draw centroid
        //                              true, new Bgr(100, 100, 100) // Draw Angle
        //                              );

        //    String win1 = "Test Window1"; //The name of the window
        //    CvInvoke.cvNamedWindow(win1); //Create the window using the specific name
        //    CvInvoke.cvShowImage(win1, img); //Show the image
        //    String win2 = "Test Window2"; //The name of the window
        //    CvInvoke.cvNamedWindow(win2); //Create the window using the specific name
        //    CvInvoke.cvShowImage(win2, img2); //Show the image
        //    CvInvoke.cvWaitKey(0);  //Wait for the key pressing event

        //    CvInvoke.cvDestroyWindow(win1); //Destory the window
        //    CvInvoke.cvDestroyWindow(win2); //Destory the window

        //    // Release the blobs
        //    foreach (Blob b in blobs)
        //        b.Clear();
        //}

        //public void AAA()
        //{
        //    IntPtr vector = BlobsInvoke.CvGetBlobs(Ptr, threshold, maskImage, borderColor, findMoments, out count);
        //}

        [TestMethod]
        public void DDDD()
        {
            Image<Bgr, Byte> currentFrame = new Image<Bgr, Byte>(@"C:\test_a.png");

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

                using (MemStorage storage = new MemStorage()) //allocate storage for contour approximation
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
                        currentFrame.Draw(tr.BoundingRectangle, new Bgr(Color.Red), 3);

                        top = Math.Min(top, tr.BoundingRectangle.Top);
                        left = Math.Min(left, tr.BoundingRectangle.Left);
                        bottom = Math.Max(bottom, tr.BoundingRectangle.Bottom);
                        right = Math.Max(right, tr.BoundingRectangle.Right);
                    }
                }

                Rectangle result = Rectangle.FromLTRB(left, top, right, bottom);
                currentFrame.Draw(result, new Bgr(Color.Blue), 3);
            }
        }
    }


    public static class Ex
    {

    }
}
