/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using DTKBarReader;

namespace VPrinting
{
    public static class BarCodeEx
    {
        public static Barcode[] ReadFromBitmapRotateAll(this BarcodeReader reader, ref Bitmap bmp, out Bitmap barCodeImage, out Rectangle barCodeArea)
        {
            barCodeImage = null;
            barCodeArea = Rectangle.Empty;

            Barcode[] results = reader.ReadFromBitmap(bmp);

            for (int count = 0; count < 4 && (results == null || results.Length == 0); count++)
            {
                bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
                //bmp = bmp.RotateEx(90);
                results = reader.ReadFromBitmap(bmp);
            }

            if (results != null && results.Length > 0)
            {
                barCodeArea = Rectangle.FromLTRB(results[0].Left, results[0].Top, results[0].Right, results[0].Bottom);
                barCodeImage = bmp.CopyNoFree(barCodeArea);
            }
            return results;
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
        static public Bitmap CopyNoFree(this Image src, Rectangle section)
        {
            Bitmap bmp = new Bitmap(section.Width, section.Height);
            using (Graphics g = Graphics.FromImage(bmp))
                g.DrawImage(src, 0, 0, section, GraphicsUnit.Pixel);
            return bmp;
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
