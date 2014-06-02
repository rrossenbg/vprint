/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PremierTaxFree.PTFLib
{
    public enum ColorFilterTypes
    {
        Red,
        Blue,
        Green,
    }

    public static class ImageEx
    {
        /// <summary>
        /// Converts the image to jpg having Compression
        /// </summary>
        /// <param name="image"></param>
        /// <param name="compression">50L</param>
        /// <returns></returns>
        public static byte[] ToArray(this Image image, long compression = 50L)
        {
            Debug.Assert(image != null);

            using (MemoryStream mem = new MemoryStream())
            {
                ImageCodecInfo jgpEncoder = ImageFormat.Jpeg.GetEncoder();

                using (var encoderParameters = new EncoderParameters(1))
                using (var parameter1 = new EncoderParameter(Encoder.Quality, compression))
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
        public static ImageCodecInfo GetEncoder(this ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
                if (codec.FormatID == format.Guid)
                    return codec;
            return null;
        }

        /// <summary>
        /// Creates a cursor by image object
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="xHotSpot"></param>
        /// <param name="yHotSpot"></param>
        /// <returns></returns>
        public static Cursor CreateCursor(this Bitmap bmp, int xHotSpot, int yHotSpot)
        {
            IconInfo tmp = new IconInfo();
            GetIconInfo(bmp.GetHicon(), ref tmp);
            tmp.xHotspot = xHotSpot;
            tmp.yHotspot = yHotSpot;
            tmp.fIcon = false;
            return new Cursor(CreateIconIndirect(ref tmp));
        }

        /// <summary>
        /// Resizes a bitmap
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="nWidth"></param>
        /// <param name="nHeight"></param>
        /// <returns></returns>
        public static Bitmap Resize(this Bitmap bmp, int nWidth, int nHeight)
        {
            if (nWidth <= 0 || nHeight <= 0)
                return bmp;

            using (bmp)
            {
                Bitmap copy = new Bitmap(nWidth, nHeight);
                using (Graphics g = Graphics.FromImage(copy))
                    g.DrawImage(bmp, 0, 0, nWidth, nHeight);
                return copy;
            }
        }

        /// <summary>
        /// Copies an image. Frees the original.
        /// </summary>
        /// <param name="src"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        static public Bitmap Copy(this Image src, Rectangle section)
        {
            using (src)
            {
                Bitmap bmp = new Bitmap(section.Width, section.Height);
                using (Graphics g = Graphics.FromImage(bmp))
                    g.DrawImage(src, 0, 0, section, GraphicsUnit.Pixel);
                return bmp;
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

        /// <summary>
        /// Rotates an image by angle.
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Bitmap Rotate(this Bitmap bmp, float angle)
        {
            if (float.IsNaN(angle))
                return null;

            using (bmp)
            {
                //create a new empty bitmap to hold rotated image
                Bitmap copy = new Bitmap(bmp.Width, bmp.Height);
                //make a graphics object from the empty bitmap
                using (Graphics g = Graphics.FromImage(copy))
                {
                    //move rotation point to center of image
                    g.TranslateTransform((float)bmp.Width / 2, (float)bmp.Height / 2);
                    //rotate
                    g.RotateTransform(angle);
                    //move image back
                    g.TranslateTransform(-(float)bmp.Width / 2, -(float)bmp.Height / 2);
                    //draw passed in image onto graphics object
                    g.DrawImage(bmp, new Point(0, 0));
                    return copy;
                }
            }
        }

        /// <summary>
        /// Removes white border around the image.
        /// Disposes the input bitmap.
        /// </summary>
        /// <param name="bmp">This</param>
        /// <param name="color">Color.White</param>
        /// <param name="colorDistance">40</param>
        /// <returns></returns>
        /// <remarks>2098x1273->00:00:00.0514984</remarks>
        public static Bitmap RemoveBorder(this Bitmap bmp, Color color, int colorDistance)
        {
            using (bmp)
            {
                #region SCAN UP-DOWN
                var lx1 = new LinkedList<Point>();
                var lx2 = new LinkedList<Point>();

                for (int y = 11; y < bmp.Height; y += bmp.Height / 11)
                {
                    bool flx1 = false, flx2 = false;

                    for (int xa = 0, xb = bmp.Width - 1; xa <= xb && xa < bmp.Width; xa++, xb--)
                    {
                        if (!flx1)
                        {
                            Color c1 = bmp.GetPixel(xa, y);
                            if (Distance(color, c1) > colorDistance)
                            {
                                lx1.AddLast(new Point(xa, y));
                                flx1 = true;
                            }
                        }

                        if (!flx2)
                        {
                            Color c2 = bmp.GetPixel(xb, y);
                            if (Distance(color, c2) > colorDistance)
                            {
                                lx2.AddFirst(new Point(xb, y));
                                flx2 = true;
                            }
                        }
                    }
                }

                #endregion

                #region SCAN LEFT-RIGHT

                var ly1 = new LinkedList<Point>();
                var ly2 = new LinkedList<Point>();

                for (int x = 11; x < bmp.Width; x += bmp.Width / 11)
                {
                    bool fly1 = false, fly2 = false;

                    for (int ya = 0, yb = bmp.Height - 1; ya <= yb && ya < bmp.Height; ya++, yb--)
                    {
                        if (!fly1)
                        {
                            Color c1 = bmp.GetPixel(x, ya);
                            if (Distance(color, c1) > colorDistance)
                            {
                                ly1.AddLast(new Point(x, ya));
                                fly1 = true;
                            }
                        }

                        if (!fly2)
                        {
                            Color c2 = bmp.GetPixel(x, yb);
                            if (Distance(color, c2) > colorDistance)
                            {
                                ly2.AddFirst(new Point(x, yb));
                                fly2 = true;
                            }
                        }
                    }
                }

                #endregion

                var points = new List<Point>(100);
                points.AddRange(lx1);
                points.AddRange(ly1);
                points.AddRange(lx2);
                points.AddRange(ly2);

                using (var pa = new GraphicsPath())
                {
                    pa.AddPolygon(points.ToArray());
                    Rectangle re = Rectangle.Round(pa.GetBounds());
                    Debug.Assert(re.IsValid(), "Rectange is invalid.");
                    var result = bmp.Copy(re);
                    return result;
                }
            }
        }
        
        /// <summary>
        /// Computes a color distance
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        private static int Distance(Color c1, Color c2)
        {
            int dr = Math.Abs(c1.R - c2.R);
            int dg = Math.Abs(c1.G - c2.G);
            int db = Math.Abs(c1.B - c2.B);
            return Math.Max(Math.Max(dr, dg), db);
        }

        /// <summary>
        /// ChangePixelFormat
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="newFormat"></param>
        /// <returns></returns>
        /// <see cref="http://stackoverflow.com/questions/483014/generate-image-file-with-low-bit-depths"/>
        public static Bitmap ChangePixelFormat(this Bitmap bmp, PixelFormat newFormat)
        {
            Debug.Assert(bmp != null);
            using (bmp)
            {
                Bitmap copy = new Bitmap(bmp.Width, bmp.Height, newFormat);
                using (Graphics g = Graphics.FromImage(copy))
                {
                    g.DrawImage(bmp, 0, 0);
                }
                return copy;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="colorFilterType"></param>
        /// <returns></returns>
        /// <see cref="http://www.codeproject.com/KB/GDI-plus/ImageProcessing2.aspx"/>
        public static Bitmap SetColorFilter(this Bitmap bmp, ColorFilterTypes colorFilterType)
        {
            Debug.Assert(bmp != null);
            using (bmp)
            {
                Bitmap clone = (Bitmap)bmp.Clone();

                for (int i = 0; i < clone.Width; i++)
                {
                    for (int j = 0; j < clone.Height; j++)
                    {
                        Color c = clone.GetPixel(i, j);

                        int nPixelR = 0;
                        int nPixelG = 0;
                        int nPixelB = 0;

                        if (colorFilterType == ColorFilterTypes.Red)
                        {
                            nPixelR = c.R;
                            nPixelG = c.G - 255;
                            nPixelB = c.B - 255;
                        }
                        else if (colorFilterType == ColorFilterTypes.Green)
                        {
                            nPixelR = c.R - 255;
                            nPixelG = c.G;
                            nPixelB = c.B - 255;
                        }
                        else if (colorFilterType == ColorFilterTypes.Blue)
                        {
                            nPixelR = c.R - 255;
                            nPixelG = c.G - 255;
                            nPixelB = c.B;
                        }

                        nPixelR = Math.Max(nPixelR, 0);
                        nPixelR = Math.Min(255, nPixelR);

                        nPixelG = Math.Max(nPixelG, 0);
                        nPixelG = Math.Min(255, nPixelG);

                        nPixelB = Math.Max(nPixelB, 0);
                        nPixelB = Math.Min(255, nPixelB);

                        clone.SetPixel(i, j, Color.FromArgb((byte)nPixelR, (byte)nPixelG, (byte)nPixelB));
                    }
                }
                return clone;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        /// <see cref="http://www.codeproject.com/KB/GDI-plus/ImageProcessing2.aspx"/>
        public static Bitmap SetGamma(this Bitmap bmp, double red, double green, double blue)
        {
            Debug.Assert(bmp != null);
            using (bmp)
            {
                Bitmap clone = (Bitmap)bmp.Clone();

                byte[] redGamma = CreateGammaArray(red);
                byte[] greenGamma = CreateGammaArray(green);
                byte[] blueGamma = CreateGammaArray(blue);

                for (int i = 0; i < clone.Width; i++)
                {
                    for (int j = 0; j < clone.Height; j++)
                    {
                        Color c = clone.GetPixel(i, j);
                        clone.SetPixel(i, j, Color.FromArgb(redGamma[c.R], greenGamma[c.G], blueGamma[c.B]));
                    }
                }
                return clone;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        /// <see cref="http://www.codeproject.com/KB/GDI-plus/ImageProcessing2.aspx"/>
        public static Bitmap SetBrightness(this Bitmap bmp, int brightness)
        {
            Debug.Assert(bmp != null);
            using (bmp)
            {
                Bitmap clone = (Bitmap)bmp.Clone();
                if (brightness < -255) brightness = -255;
                if (brightness > 255) brightness = 255;

                for (int i = 0; i < clone.Width; i++)
                {
                    for (int j = 0; j < clone.Height; j++)
                    {
                        Color c = clone.GetPixel(i, j);

                        int cR = c.R + brightness;
                        int cG = c.G + brightness;
                        int cB = c.B + brightness;

                        if (cR < 0) cR = 1;
                        if (cR > 255) cR = 255;

                        if (cG < 0) cG = 1;
                        if (cG > 255) cG = 255;

                        if (cB < 0) cB = 1;
                        if (cB > 255) cB = 255;

                        clone.SetPixel(i, j, Color.FromArgb((byte)cR, (byte)cG, (byte)cB));
                    }
                }
                return clone;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        /// <see cref="http://www.codeproject.com/KB/GDI-plus/ImageProcessing2.aspx"/>
        public static Bitmap SetContrast(this Bitmap bmp, double contrast)
        {
            Debug.Assert(bmp != null);

            using (bmp)
            {
                Bitmap clone = (Bitmap)bmp.Clone();
                if (contrast < -100) contrast = -100;
                if (contrast > 100) contrast = 100;
                contrast = (100.0 + contrast) / 100.0;
                contrast *= contrast;

                for (int i = 0; i < clone.Width; i++)
                {
                    for (int j = 0; j < clone.Height; j++)
                    {
                        Color c = clone.GetPixel(i, j);
                        double pR = c.R / 255.0;
                        pR -= 0.5;
                        pR *= contrast;
                        pR += 0.5;
                        pR *= 255;
                        if (pR < 0) pR = 0;
                        if (pR > 255) pR = 255;

                        double pG = c.G / 255.0;
                        pG -= 0.5;
                        pG *= contrast;
                        pG += 0.5;
                        pG *= 255;
                        if (pG < 0) pG = 0;
                        if (pG > 255) pG = 255;

                        double pB = c.B / 255.0;
                        pB -= 0.5;
                        pB *= contrast;
                        pB += 0.5;
                        pB *= 255;
                        if (pB < 0) pB = 0;
                        if (pB > 255) pB = 255;

                        clone.SetPixel(i, j, Color.FromArgb((byte)pR, (byte)pG, (byte)pB));
                    }
                }
                return clone;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        /// <see cref="http://www.codeproject.com/KB/GDI-plus/ImageProcessing2.aspx"/>
        public static Bitmap SetGrayscale(this Bitmap bmp)
        {
            Debug.Assert(bmp != null);
            using (bmp)
            {
                Bitmap clone = (Bitmap)bmp.Clone();

                for (int i = 0; i < clone.Width; i++)
                {
                    for (int j = 0; j < clone.Height; j++)
                    {
                        Color c = clone.GetPixel(i, j);
                        byte gray = (byte)(.299 * c.R + .587 * c.G + .114 * c.B);
                        clone.SetPixel(i, j, Color.FromArgb(gray, gray, gray));
                    }
                }
                return clone;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        /// <see cref="http://www.codeproject.com/KB/GDI-plus/ImageProcessing2.aspx"/>
        public static Bitmap SetInvert(this Bitmap bmp)
        {
            Debug.Assert(bmp != null);
            using (bmp)
            {
                Bitmap clone = (Bitmap)bmp.Clone();

                for (int i = 0; i < clone.Width; i++)
                {
                    for (int j = 0; j < clone.Height; j++)
                    {
                        Color c = clone.GetPixel(i, j);
                        clone.SetPixel(i, j, Color.FromArgb(255 - c.R, 255 - c.G, 255 - c.B));
                    }
                }
                return clone;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        /// <see cref="http://www.codeproject.com/KB/GDI-plus/ImageProcessing2.aspx"/>
        public static Bitmap SetSize(this Bitmap bmp, int newWidth, int newHeight)
        {
            Debug.Assert(bmp != null);
            Debug.Assert(newWidth != 0 && newHeight != 0);

            using (bmp)
            {
                Bitmap copy = new Bitmap(newWidth, newHeight, bmp.PixelFormat);

                double nWidthFactor = (double)bmp.Width / (double)newWidth;
                double nHeightFactor = (double)bmp.Height / (double)newHeight;

                for (int x = 0; x < copy.Width; ++x)
                {
                    for (int y = 0; y < copy.Height; ++y)
                    {

                        int fr_x = (int)Math.Floor(x * nWidthFactor);
                        int fr_y = (int)Math.Floor(y * nHeightFactor);
                        int cx = fr_x + 1;
                        if (cx >= bmp.Width) cx = fr_x;
                        int cy = fr_y + 1;
                        if (cy >= bmp.Height) cy = fr_y;
                        double fx = x * nWidthFactor - fr_x;
                        double fy = y * nHeightFactor - fr_y;
                        double nx = 1.0 - fx;
                        double ny = 1.0 - fy;

                        Color color1 = bmp.GetPixel(fr_x, fr_y);
                        Color color2 = bmp.GetPixel(cx, fr_y);
                        Color color3 = bmp.GetPixel(fr_x, cy);
                        Color color4 = bmp.GetPixel(cx, cy);

                        // Blue
                        byte bp1 = (byte)(nx * color1.B + fx * color2.B);

                        byte bp2 = (byte)(nx * color3.B + fx * color4.B);

                        byte nBlue = (byte)(ny * (double)(bp1) + fy * (double)(bp2));

                        // Green
                        bp1 = (byte)(nx * color1.G + fx * color2.G);

                        bp2 = (byte)(nx * color3.G + fx * color4.G);

                        byte nGreen = (byte)(ny * (double)(bp1) + fy * (double)(bp2));

                        // Red
                        bp1 = (byte)(nx * color1.R + fx * color2.R);

                        bp2 = (byte)(nx * color3.R + fx * color4.R);

                        byte nRed = (byte)(ny * (double)(bp1) + fy * (double)(bp2));

                        copy.SetPixel(x, y, Color.FromArgb(255, nRed, nGreen, nBlue));
                    }
                }
                return copy;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        /// <see cref="http://www.codeproject.com/KB/GDI-plus/ImageProcessing2.aspx"/>
        public static Bitmap RotateFlipEx(this Bitmap bmp, RotateFlipType rotateFlipType)
        {
            Debug.Assert(bmp != null);
            using (bmp)
            {
                Bitmap clone = (Bitmap)bmp.Clone();
                clone.RotateFlip(rotateFlipType);
                return clone;
            }
        }

        public static Bitmap RotateEx(this Bitmap bmp, double angle)
        {
            using (bmp)
            {
                PointF[] rotationPoints = { new PointF(0, 0), new PointF(bmp.Width, 0), new PointF(0, bmp.Height), new PointF(bmp.Width, bmp.Height)};
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
        /// 
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        /// <see cref="http://www.codeproject.com/KB/GDI-plus/ImageProcessing2.aspx"/>
        public static Bitmap DrawOutCropArea(this Bitmap bmp, int xPosition, int yPosition, int width, int height)
        {
            using (bmp)
            {
                Bitmap clone = (Bitmap)bmp.Clone();

                using (Graphics gr = Graphics.FromImage(clone))
                {
                    using (var pen = new Pen(Color.FromArgb(150, Color.White)))
                    {
                        Rectangle rect1 = new Rectangle(0, 0, bmp.Width, yPosition);
                        Rectangle rect2 = new Rectangle(0, yPosition, xPosition, height);
                        Rectangle rect3 = new Rectangle(0, (yPosition + height), bmp.Width, bmp.Height);
                        Rectangle rect4 = new Rectangle((xPosition + width), yPosition, (bmp.Width - xPosition - width), height);
                        gr.FillRectangle(pen.Brush, rect1);
                        gr.FillRectangle(pen.Brush, rect2);
                        gr.FillRectangle(pen.Brush, rect3);
                        gr.FillRectangle(pen.Brush, rect4);
                    }
                    return clone;
                }
            }
        }

        private static readonly float[][] ms_DefaultAttribs = { 
                                                           new float[] {1, 0, 0, 0, 0}, 
                                                           new float[] {0, 1, 0, 0, 0}, 
                                                           new float[] {0, 0, 1, 0, 0}, 
                                                           new float[] {0, 0, 0, 1, 0}, 
                                                           new float[] {0, 0, 0, 0, 1} 
                                                       };
        private static readonly float[][] ms_BlackWhiteAttribs = {
                                                           new float[] {.3f, .3f, .3f, 0, 0},
                                                           new float[] {.59f, .59f, .59f, 0, 0},
                                                           new float[] {.11f, .11f, .11f, 0, 0},
                                                           new float[] {0, 0, 0, 1, 0},
                                                           new float[] {0, 0, 0, 0, 1}
                                                       };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="attribs"></param>
        /// <returns></returns>
        /// <see cref="http://www.gutgames.com/post/Converting-Image-to-Black-and-White-in-C.aspx"/>
        public static Bitmap ToGrayScale(this Bitmap bmp)
        {
            Bitmap outbmp = new Bitmap(bmp.Width, bmp.Height);

            using (Graphics g = Graphics.FromImage(outbmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                ColorMatrix NewColorMatrix = new ColorMatrix(ms_BlackWhiteAttribs);

                using (ImageAttributes attr = new ImageAttributes())
                {
                    attr.SetColorMatrix(NewColorMatrix);
                    g.DrawImage(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, attr);
                }
            }
            return outbmp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="threshold">128</param>
        /// <returns></returns>
        public static Bitmap ToBlackWhite(this Bitmap bmp, int threshold)
        {
#warning Optimize for speed
            Bitmap outbmp = new Bitmap(bmp.Width, bmp.Height);

            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    Color c = bmp.GetPixel(i, j);

                    int ans = (Convert.ToInt16(c.R) + Convert.ToInt16(c.G) + Convert.ToInt16(c.B)) / 3;

                    if (ans > threshold)
                    {
                        c = Color.FromArgb(255, 255, 255);
                    }
                    else
                    {
                        c = Color.FromArgb(0, 0, 0);
                    }

                    outbmp.SetPixel(i, j, c);
                }
            }
            return outbmp;
        }

        public static Bitmap Crop(this Bitmap bmp, Rectangle rect)
        {
            return bmp.Crop(rect.X, rect.Y, rect.Width, rect.Height);
        }

        public static byte[] ToArray(this Bitmap bmp)
        {
            using (var mem = new MemoryStream())
            {
                bmp.Save(mem, bmp.RawFormat);
                return mem.ToArray();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        /// <see cref="http://www.codeproject.com/KB/GDI-plus/ImageProcessing2.aspx"/>
        public static Bitmap Crop(this Bitmap bmp, int xPosition, int yPosition, int width, int height)
        {
            Debug.Assert(bmp != null);

            using(bmp)
            using (Bitmap clone = (Bitmap)bmp.Clone())
            {
                if (xPosition + width > bmp.Width)
                    width = bmp.Width - xPosition;

                if (yPosition + height > bmp.Height)
                    height = bmp.Height - yPosition;

                Rectangle rect = new Rectangle(xPosition, yPosition, width, height);

                return (Bitmap)clone.Clone(rect, clone.PixelFormat);
            }
        }

        private static byte[] CreateGammaArray(double color)
        {
            byte[] gammaArray = new byte[256];
            for (int i = 0; i < 256; ++i)
                gammaArray[i] = (byte)Math.Min(255, (int)((255.0 * Math.Pow(i / 255.0, 1.0 / color)) + 0.5));
            return gammaArray;
        }

        public struct IconInfo
        {
            public bool fIcon;
            public int xHotspot;
            public int yHotspot;
            public IntPtr hbmMask;
            public IntPtr hbmColor;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr CreateIconIndirect(ref IconInfo icon);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetIconInfo(IntPtr hIcon, ref IconInfo pIconInfo);

        public static Image ToJpeg(this Bitmap img, long quality)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

            ImageCodecInfo jpegCodec = null;

            for (int i = 0; i < codecs.Length; i++)
            {
                if (codecs[i].MimeType == "image/jpeg")
                {
                    jpegCodec = codecs[i];
                    break;
                }
            }

            if (jpegCodec == null)
                throw new AppStopException("Can't find jpeg encoder");

            EncoderParameter qualityParam = new EncoderParameter(Encoder.Quality, (long)quality);
            EncoderParameters encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = qualityParam;

            using (MemoryStream memory = new MemoryStream())
            {
                img.Save(memory, jpegCodec, encoderParams);
                memory.Position = 0;
                return Image.FromStream(memory);
            }
        }

#if NOT_USE_THIS
        public static Bitmap ToBlackWhite2(this Bitmap bmp, int threshold)
        {
            Bitmap bmpCpy = (Bitmap)bmp.Clone();

            // GDI+ still lies to us - the return format is BGR, NOT RGB.
            BitmapData bdOrg = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            BitmapData bdCpy = bmpCpy.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int scanline = bdOrg.Stride;

            IntPtr orgScan0 = bdOrg.Scan0;
            IntPtr cpyScan0 = bdCpy.Scan0;

            unsafe
            {
                byte* pOrg = (byte*)(void*)orgScan0;
                byte* pCpy = (byte*)(void*)cpyScan0;

                int nOffset = bdOrg.Stride - bmp.Width * 3;
                int nWidth = bmp.Width;
                int nHeight = bmp.Height;

                int xOffset = 0, yOffset = 0;

                for (int y = 0; y < nHeight; ++y)
                {
                    for (int x = 0; x < nWidth; ++x)
                    {
                        Color c = GetPixel(pOrg, x, y, xOffset, yOffset, scanline);

                        int ans = (Convert.ToInt16(c.R) + Convert.ToInt16(c.G) + Convert.ToInt16(c.B)) / 3;

                        if (ans > threshold)
                        {
                            c = Color.FromArgb(255, 255, 255);
                        }
                        else
                        {
                            c = Color.FromArgb(0, 0, 0);
                        }

                        SetPixel(pCpy, x, y, xOffset, yOffset, scanline, c);

                        pOrg += 3;
                    }
                    pOrg += nOffset;
                }
            }

            bmp.UnlockBits(bdOrg);
            bmpCpy.UnlockBits(bdCpy);

            return bmpCpy;
        }

        public static Rectangle FindRectangle2(this Bitmap bmp, Point start, Color backColor, Size min, Size max, float threshold)
        {
            BitmapData bmData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int scanline = bmData.Stride;

            IntPtr Scan0 = bmData.Scan0;

            unsafe
            {
                byte* p = (byte*)(void*)Scan0;

                int nOffset = bmData.Stride - bmp.Width * 3;
                int xOffset = 0, yOffset = 0;

                // GDI+ still lies to us - the return format is BGR, NOT RGB.
                for (int y = start.Y; y < bmp.Height; y++)
                {
                    for (int x = start.X; x < bmp.Width; x++)
                    {
                        //Color c = bmp.GetPixel(x, y);
                        Color c = GetPixel(p, x, y, xOffset, yOffset, scanline);
                        if (c != backColor)
                        {
                            int y1 = y;

                            for (; y1 < bmp.Height; y1++)
                            {
                                c = GetPixel(p, x, y1, xOffset, yOffset, scanline);
                                if (c == backColor)
                                    break;
                            }

                            int x1 = x + 1, y2 = 0;
                            int missingDots = 0;

                            for (; x1 < bmp.Width; x1++)
                            {
                                for (y2 = y; y2 < y1; y2++)
                                {
                                    c = GetPixel(p, x1, y2, xOffset, yOffset, scanline);
                                    if (c == backColor)
                                        missingDots++;

                                    if (((missingDots * 0.01f) / (y1 - y)) > threshold)
                                        break;
                                }
                                if (c == backColor)
                                    break;
                            }

                            Rectangle r = Rectangle.FromLTRB(x, y, x1, y1);
                            if (r.IsBetween(min, max))
                                return r;
                        }

                        p += 3;
                    }

                    p += nOffset;
                }
            }

            bmp.UnlockBits(bmData);
            return Rectangle.Empty;
        }

        private unsafe static Color GetPixel(byte* p, int x, int y, int xOffset, int yOffset, int scanline)
        {
            byte b = p[((y + yOffset) * scanline) + ((x + xOffset) * 3)];
            byte g = p[((y + yOffset) * scanline) + ((x + xOffset) * 3) + 1];
            byte r = p[((y + yOffset) * scanline) + ((x + xOffset) * 3) + 2];
            //byte r = p[((y + yOffset) * scanline) + ((x + xOffset) * 4) + 3];

            Color c = Color.FromArgb(255, r, g, b);
            return c;
        }

        private unsafe static void SetPixel(byte* p, int x, int y, int xOffset, int yOffset, int scanline, Color c)
        {
            //p[((y + yOffset) * scanline) + ((x + xOffset) * 4)] = Convert.ToByte(c.A);//A
            p[((y + yOffset) * scanline) + ((x + xOffset) * 4)] = Convert.ToByte(c.B);//B
            p[((y + yOffset) * scanline) + ((x + xOffset) * 4) + 1] = Convert.ToByte(c.G);//G
            p[((y + yOffset) * scanline) + ((x + xOffset) * 4) + 2] = Convert.ToByte(c.R);//R
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="foreColor"></param>
        /// <param name="backColor"></param>
        /// <see cref="http://stackoverflow.com/questions/1218986/jpeg-artifacts-removal-in-c-sharp"/>
        public static void RemoveBorder(this Bitmap bmp, Color foreColor, Color backColor)
        {
            int width = bmp.Width;
            int height = bmp.Height;
            Dictionary<Point, int> currentLayer = new Dictionary<Point, int>();
            currentLayer[new Point(0, 0)] = 0;
            currentLayer[new Point(width - 1, height - 1)] = 0;

            while (currentLayer.Count != 0)
            {
                foreach (Point p in currentLayer.Keys)
                    bmp.SetPixel(p.X, p.Y, backColor);

                Dictionary<Point, int> newLayer = new Dictionary<Point, int>();

                foreach (Point p in currentLayer.Keys)
                {
                    foreach (Point p1 in Neighbors(p, width, height))
                    {
                        if (Distance(bmp.GetPixel(p1.X, p1.Y), foreColor) < 40)
                            newLayer[p1] = 0;
                    }
                }

                currentLayer = newLayer;
            }
        }

        private static List<Point> Neighbors(Point p, int maxX, int maxY)
        {
            List<Point> points = new List<Point>();
            if (p.X + 1 < maxX)
                points.Add(new Point(p.X + 1, p.Y));
            if (p.X - 1 >= 0)
                points.Add(new Point(p.X - 1, p.Y));
            if (p.Y + 1 < maxY)
                points.Add(new Point(p.X, p.Y + 1));
            if (p.Y - 1 >= 0)
                points.Add(new Point(p.X, p.Y - 1));
            return points;
        }
#endif

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="start">Point to start from</param>
        /// <param name="backColor">Back color value</param>
        /// <param name="min">Minimum size to look for</param>
        /// <param name="max">Maximum size to look for</param>
        /// <param name="threshold">Missing dots allowed. 0.20f->20%</param>
        /// <returns></returns>
        /// <example>
        ///  using (var bmp2 = bmp.ToBlackWhite(128))
        ///  {
        ///      var r = bmp2.FindRectangle(Point.Empty, Color.FromArgb(255, 255, 255, 255), new Size(20, 20), new Size(50, 50), 0.25f);
        ///      if (r != Rectangle.Empty)
        ///      {
        ///          using (var g = Graphics.FromImage(bmp2))
        ///          {
        ///              g.DrawRectangle(Pens.Red, r);
        ///          }
        ///          Debug.WriteLine("Done");
        ///      }
        ///  }
        /// </example>
        public static Rectangle FindRectangle(this Bitmap bmp, Point start, Color backColor, Size min, Size max, float threshold)
        {
#warning Optimize for speed

            for (int y = start.Y; y < bmp.Height; y++)
            {
                for (int x = start.X; x < bmp.Width; x++)
                {
                    Color c = bmp.GetPixel(x, y);
                    if (c != backColor)
                    {
                        int y1 = y;

                        for (; y1 < bmp.Height; y1++)
                        {
                            c = bmp.GetPixel(x, y1);
                            if (c == backColor)
                                break;
                        }

                        int x1 = x + 1, y2 = 0;
                        int missingDots = 0;

                        for (; x1 < bmp.Width; x1++)
                        {
                            for (y2 = y; y2 < y1; y2++)
                            {
                                c = bmp.GetPixel(x1, y2);
                                if (c == backColor)
                                    missingDots++;

                                if (((missingDots * 0.01f) / (y1 - y)) > threshold)
                                    break;
                            }
                            if (c == backColor)
                                break;
                        }

                        Rectangle r = Rectangle.FromLTRB(x, y, x1, y1);
                        if (r.IsBetween(min, max))
                            return r;
                    }
                }
            }

            return Rectangle.Empty;
        }
    }
}