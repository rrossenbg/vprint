/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime;
using AForge.Imaging;
using AForge.Imaging.Filters;
using SysImage = System.Drawing.Image;

namespace CardCodeCover
{
    public static class ImageEx
    {
        [TargetedPatchingOptOut("na")]
        public static Bitmap ToGrayScale(this Bitmap image)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            var seq = new FiltersSequence();
            seq.Add(Grayscale.CommonAlgorithms.BT709);  //First add  GrayScaling filter
            seq.Add(new OtsuThreshold());               //Then add binarization(thresholding) filter
            return seq.Apply(image);                    // Apply filters on source image
        }

        [TargetedPatchingOptOut("na")]
        public static TemplateMatch[] Match(this Bitmap sourceImage, Bitmap template, float similarityThreshold = 0.80f)
        {
            if (sourceImage == null)
                throw new ArgumentNullException("sourceImage");
            if (template == null)
                throw new ArgumentNullException("template");

            ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching(similarityThreshold);
            // find all matchings with specified above similarity
            TemplateMatch[] matchings = tm.ProcessImage(sourceImage, template);
            return matchings;
        }

        [TargetedPatchingOptOut("na")]
        public static void DrawMatches(this Bitmap sourceImage, TemplateMatch[] matchings)
        {
            if (sourceImage == null)
                throw new ArgumentNullException("sourceImage");
            if (matchings == null)
                throw new ArgumentNullException("matchings");

            BitmapData data = sourceImage.LockBits(new Rectangle(0, 0, sourceImage.Width, sourceImage.Height),
                    ImageLockMode.ReadWrite, sourceImage.PixelFormat);
            foreach (TemplateMatch m in matchings)
                Drawing.Rectangle(data, m.Rectangle, Color.Red);
            sourceImage.UnlockBits(data);
        }

        [TargetedPatchingOptOut("na")]
        public static Rectangle ScrollOffsetY(this Rectangle rect, float offsetY)
        {
            var r = rect;
            r.Offset(0, -(int)offsetY);
            return r;
        }

        [TargetedPatchingOptOut("na")]
        public static RectangleF Mul(this Rectangle r, float x, float y)
        {
            return RectangleF.FromLTRB(r.X * x, r.Y * y, (r.X + r.Width) * x, (r.Y + r.Height) * y);
        }

        [TargetedPatchingOptOut("na")]
        public static double Distance(this Rectangle rect1, Rectangle rect2)
        {
            var c1 = rect1.Center();
            var c2 = rect2.Center();
            return Math.Sqrt((c2.X - c1.X) * (c2.X - c1.X) + (c2.Y - c1.Y) * (c2.Y - c1.Y));
        }

        [TargetedPatchingOptOut("na")]
        public static double Distance(this Point p1, Point p2)
        {
            return Math.Sqrt((p2.X - p1.X) * (p2.X - p1.X) + (p2.Y - p1.Y) * (p2.Y - p1.Y));
        }

        [TargetedPatchingOptOut("na")]
        public static Point Center(this Rectangle rect)
        {
            return new Point((rect.Left + rect.Width) / 2, (rect.Top + rect.Height) / 2);
        }

        [TargetedPatchingOptOut("na")]
        public static long Length(this System.Drawing.Image image)
        {
            if (image == null)
                return 0;

            using (var ms = new MemoryStream())
            {
                image.Save(ms, ImageFormat.Jpeg);
                return ms.Length;
            }
        }

        [TargetedPatchingOptOut("na")]
        public static SysImage ToImageFile(this byte[] array)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            var name = Path.GetRandomFileName();
            try
            {
                File.WriteAllBytes(name, array);
                return SysImage.FromFile(name);
            }
            finally
            {
                try
                {
                    if (File.Exists(name))
                        File.Delete(name);
                }
                catch
                {
                    //No errors during delete
                }
            }
        }

        [TargetedPatchingOptOut("na")]
        public static byte[] ToArrayFile(this SysImage image)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            var name = Path.GetRandomFileName();
            try
            {
                image.Save(name, ImageFormat.Jpeg);
                return File.ReadAllBytes(name);
            }
            finally
            {
                try
                {
                    if (File.Exists(name))
                        File.Delete(name);
                }
                catch
                {
                    //No errors during delete
                }
            }
        }

        [TargetedPatchingOptOut("na")]
        public static byte[] ToArray(this SysImage image, long compression = 50L)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            using (MemoryStream mem = new MemoryStream())
            {
                ImageCodecInfo jgpEncoder = ImageFormat.Jpeg.GetEncoder();

                if (jgpEncoder == null)
                    throw new Exception("Cannot find encoder");

                using (var encoderParameters = new EncoderParameters(1))
                using (var parameter1 = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, compression))
                {
                    encoderParameters.Param[0] = parameter1;
                    image.Save(mem, jgpEncoder, encoderParameters);
                }

                return mem.ToArray();
            }
        }

        /// <summary>
        /// Gets encoder by image format
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        [TargetedPatchingOptOut("na")]
        public static ImageCodecInfo GetEncoder(this ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
                if (codec.FormatID == format.Guid)
                    return codec;
            return null;
        }

        public static Bitmap RotateEx(this Bitmap bmp, double angle)
        {
            using (bmp)
            {
                PointF[] rotationPoints = { new PointF(0, 0), new PointF(bmp.Width, 0), new PointF(0, bmp.Height), new PointF(bmp.Width, bmp.Height) };
                PointMath.RotatePoints(rotationPoints, new PointF(bmp.Width / 2.0f, bmp.Height / 2.0f), angle);

                Rectangle bounds = PointMath.GetBounds(rotationPoints);
                Bitmap newBmp = new Bitmap(bounds.Width, bounds.Height);
                using (Graphics g = Graphics.FromImage(newBmp))
                {
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    Matrix mtx = new Matrix();
                    mtx.RotateAt((float)angle, new PointF(bmp.Width / 2.0f, bmp.Height / 2.0f));
                    mtx.Translate(-bounds.Left, -bounds.Top, MatrixOrder.Append);
                    g.Transform = mtx;
                    g.DrawImage(bmp, 0, 0);
                }
                return newBmp;
            }
        }

        /// <summary>
        /// Copies an image. Doesn't free the original.
        /// </summary>
        /// <param name="src"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        [TargetedPatchingOptOut("na")]
        public static Bitmap CloneNoFree(this Bitmap src, Rectangle section)
        {
            Bitmap cropped = (Bitmap)src.Clone(section, src.PixelFormat);
            return cropped;
        }

        [TargetedPatchingOptOut("na")]
        public static Bitmap CopyNoFree(this SysImage src, Rectangle section, float scale)
        {
            var bmp = new Bitmap(section.Width, section.Height);

            using (Graphics g = Graphics.FromImage(bmp))
                g.DrawImageUnscaledAndClipped(src, new Rectangle(Point.Empty, section.Size));

            return bmp;
        }

        [TargetedPatchingOptOut("na")]
        public static Bitmap ConvertTo24bppFree(this SysImage img)
        {
            if (img.PixelFormat == PixelFormat.Format24bppRgb)
                return (Bitmap)img;

            using (img)
            {
                var bmp = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
                using (var gr = Graphics.FromImage(bmp))
                    gr.DrawImage(img, new Rectangle(0, 0, img.Width, img.Height));
                return bmp;
            }
        }

        [TargetedPatchingOptOut("na")]
        public static byte[] ConverterToArray(this SysImage image)
        {
            ImageConverter imageConverter = new ImageConverter();
            byte[] array = (byte[])imageConverter.ConvertTo(image, typeof(byte[]));
            return array;
        }
    }

    internal static class PointMath
    {
        /// <summary>
        /// Converts an angle from degree to radians
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        private static double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        /// <summary>
        /// Rotates a point by angle in degree
        /// </summary>
        /// <param name="pnt"></param>
        /// <param name="degreeAngle"></param>
        /// <returns></returns>
        public static PointF RotatePoint(PointF pnt, double degreeAngle)
        {
            return RotatePoint(pnt, new PointF(0, 0), degreeAngle);
        }

        /// <summary>
        /// Rotates a point by angle in degree
        /// </summary>
        /// <param name="pnt"></param>
        /// <param name="origin"></param>
        /// <param name="degreeAngle"></param>
        /// <returns></returns>
        public static PointF RotatePoint(PointF pnt, PointF origin, double degreeAngle)
        {
            double radAngle = DegreeToRadian(degreeAngle);

            PointF newPoint = new PointF();

            double deltaX = pnt.X - origin.X;
            double deltaY = pnt.Y - origin.Y;

            newPoint.X = (float)(origin.X + (Math.Cos(radAngle) * deltaX - Math.Sin(radAngle) * deltaY));
            newPoint.Y = (float)(origin.Y + (Math.Sin(radAngle) * deltaX + Math.Cos(radAngle) * deltaY));

            return newPoint;
        }

        /// <summary>
        /// Rotates an array of points
        /// </summary>
        /// <param name="pnts"></param>
        /// <param name="degreeAngle"></param>
        public static void RotatePoints(PointF[] pnts, double degreeAngle)
        {
            for (int i = 0; i < pnts.Length; i++)
            {
                pnts[i] = RotatePoint(pnts[i], degreeAngle);
            }
        }

        /// <summary>
        /// Rotates an array of points
        /// </summary>
        /// <param name="pnts"></param>
        /// <param name="origin"></param>
        /// <param name="degreeAngle"></param>
        public static void RotatePoints(PointF[] pnts, PointF origin, double degreeAngle)
        {
            for (int i = 0; i < pnts.Length; i++)
            {
                pnts[i] = RotatePoint(pnts[i], origin, degreeAngle);
            }
        }

        /// <summary>
        /// Computes rectangle by array of points
        /// </summary>
        /// <param name="pnts"></param>
        /// <returns></returns>
        public static Rectangle GetBounds(PointF[] pnts)
        {
            RectangleF boundsF = GetBoundsF(pnts);
            return new Rectangle((int)Math.Round(boundsF.Left),
                                 (int)Math.Round(boundsF.Top),
                                 (int)Math.Round(boundsF.Width),
                                 (int)Math.Round(boundsF.Height));
        }

        /// <summary>
        /// Computes rectangle by array of points
        /// </summary>
        /// <param name="pnts"></param>
        /// <returns></returns>
        public static RectangleF GetBoundsF(PointF[] pnts)
        {
            float left = pnts[0].X;
            float right = pnts[0].X;
            float top = pnts[0].Y;
            float bottom = pnts[0].Y;

            for (int i = 1; i < pnts.Length; i++)
            {
                if (pnts[i].X < left)
                    left = pnts[i].X;
                else if (pnts[i].X > right)
                    right = pnts[i].X;

                if (pnts[i].Y < top)
                    top = pnts[i].Y;
                else if (pnts[i].Y > bottom)
                    bottom = pnts[i].Y;
            }

            return new RectangleF(left,
                                  top,
                                 (float)Math.Abs(right - left),
                                 (float)Math.Abs(bottom - top));
        }
    }
}