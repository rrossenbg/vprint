/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime;
using System.Runtime.InteropServices;
using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Math.Geometry;
using DBitmap = System.Drawing.Bitmap;
using DImage = System.Drawing.Image;
using PPoint = System.Drawing.Point;

namespace CPrint2
{
    public static class ImageEx
    {
        /// <summary>
        /// http://www.codeproject.com/Articles/265354/Playing-Card-Recognition-Using-AForge-Net-Framewor
        /// http://www.aforgenet.com/framework/features/
        /// </summary>
        public static Bitmap CropRotateFree(this Bitmap source, Size minSizeInch, Size maxSizeInch)
        {
            using (source)
            {
                using (Graphics g = Graphics.FromImage(source))
                {
                    int w1 = source.Width - 1;
                    int w6 = source.Width - 6;
                    int h1 = source.Height - 1;
                    int h6 = source.Height - 6;

                    Rectangle[] rects = new Rectangle[] { 
                        Rectangle.FromLTRB( 0, 0, w1, 5),
                        Rectangle.FromLTRB( 0, 0, 5, h1),
                        Rectangle.FromLTRB( 0, h6, w1, h1),
                        Rectangle.FromLTRB( w6, 0, w1, h1),
                    };

                    g.FillRectangles(Brushes.Black, rects);

                    var monitor = new PictureModifier(source);
                    monitor.ApplyGrayscale();
                    monitor.ApplySobelEdgeFilter();
                    //monitor.ApplyThresholdedDifference();
                    //monitor.ApplyFilling();


                    var list = new List<Region>();
                    try
                    {
                        list.AddRange(monitor.EnumKnownForms());
                        list.Sort(new RegionComparer(g));

                        foreach (var r in list)
                        {
                            var rectange = Rectangle.Round(r.GetBounds(g));

                            if (rectange.Size.InBetween(minSizeInch, maxSizeInch) || 
                                rectange.Size.InBetween(maxSizeInch, minSizeInch))
                            {
                                using (var bmp3 = source.CropImageNoFree(rectange))
                                {
                                    if (bmp3.Width > bmp3.Height)
                                        bmp3.RotateFlip(RotateFlipType.Rotate90FlipNone);

                                    return bmp3.ToGrayscale4bpp();
                                }
                            }

#if TEST
                            Debug.WriteLine(rectange);
                            g.DrawRectangle(Pens.Red, rectange);
                            using (Font font = new Font("Arial", 10, FontStyle.Bold))
                                g.DrawString(rectange.Size.ToString(), font, Brushes.White, rectange.Location);
#endif
                        }
                    }
                    finally
                    {
                        list.ForEach((r) => r.DisposeSf());
                    }
                }
                return null;
            }
        }

        private class RegionComparer : IComparer<Region>
        {
            private readonly Graphics m_g;

            public RegionComparer(Graphics g)
            {
                m_g = g;
            }
            public int Compare(Region x, Region y)
            {
                return x.GetArea(m_g).CompareTo(y.GetArea(m_g));
            }
        }

        public static void SaveMultipage(this List<System.Drawing.Bitmap> list, string location, string type)
        {
            ImageCodecInfo codecInfo = getCodecForstring(type);

            if (list.Count == 1)
            {
                EncoderParameters iparams = new EncoderParameters(1);
                Encoder iparam = Encoder.Compression;
                EncoderParameter iparamPara = new EncoderParameter(iparam, (long)(EncoderValue.CompressionLZW));
                iparams.Param[0] = iparamPara;
                list[0].Save(location, codecInfo, iparams);
            }
            else if (list.Count > 1)
            {
                Encoder saveEncoder = Encoder.SaveFlag;
                Encoder compressionEncoder = Encoder.Compression;
                EncoderParameters EncoderParams = new EncoderParameters(2);

                // Save the first page (frame).
                EncoderParameter SaveEncodeParam = new EncoderParameter(saveEncoder, (long)EncoderValue.MultiFrame);
                EncoderParameter CompressionEncodeParam = new EncoderParameter(compressionEncoder, (long)EncoderValue.CompressionLZW);
                EncoderParams.Param[0] = CompressionEncodeParam;
                EncoderParams.Param[1] = SaveEncodeParam;

                File.Delete(location);
                list[0].Save(location, codecInfo, EncoderParams);

                for (int i = 1; i < list.Count; i++)
                {
                    if (list[i] == null)
                        break;

                    SaveEncodeParam = new EncoderParameter(saveEncoder, (long)EncoderValue.FrameDimensionPage);
                    CompressionEncodeParam = new EncoderParameter(compressionEncoder, (long)EncoderValue.CompressionLZW);
                    EncoderParams.Param[0] = CompressionEncodeParam;
                    EncoderParams.Param[1] = SaveEncodeParam;
                    list[0].SaveAdd(list[i], EncoderParams);

                }

                SaveEncodeParam = new EncoderParameter(saveEncoder, (long)EncoderValue.Flush);
                EncoderParams.Param[0] = SaveEncodeParam;
                list[0].SaveAdd(EncoderParams);
            }
        }

        private static ImageCodecInfo getCodecForstring(string type)
        {
            ImageCodecInfo[] info = ImageCodecInfo.GetImageEncoders();

            for (int i = 0; i < info.Length; i++)
            {
                string EnumName = type.ToString();
                if (info[i].FormatDescription.Equals(EnumName, StringComparison.InvariantCultureIgnoreCase))
                    return info[i];
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bitmap"></param>
        /// <see cref="http://www.aforgenet.com/articles/shape_checker/"/>
        public static IEnumerable<Tuple<DetectedShape, PPoint[]>> FindMarks(this Bitmap bitmap, Size min, Size max)
        {
            // lock image
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);

            // step 1 - turn background to black
            ////ColorFiltering colorFilter = new ColorFiltering();

            ////colorFilter.Red = new IntRange(0, 64);
            ////colorFilter.Green = new IntRange(0, 64);
            ////colorFilter.Blue = new IntRange(0, 64);
            ////colorFilter.FillOutsideRange = false;

            ////colorFilter.ApplyInPlace(bitmapData);

            // step 2 - locating objects
            BlobCounter blobCounter = new BlobCounter();

            blobCounter.FilterBlobs = true;
            blobCounter.MinHeight = min.Height;
            blobCounter.MaxHeight = max.Height;
            blobCounter.MinWidth = min.Width;
            blobCounter.MaxWidth = max.Width;

            blobCounter.ProcessImage(bitmapData);
            Blob[] blobs = blobCounter.GetObjectsInformation();
            bitmap.UnlockBits(bitmapData);

            // step 3 - check objects' type and highlight
            SimpleShapeChecker shapeChecker = new SimpleShapeChecker();

            for (int i = 0, n = blobs.Length; i < n; i++)
            {
                List<IntPoint> edgePoints = blobCounter.GetBlobsEdgePoints(blobs[i]);

                AForge.Point center;
                float radius;

                // is circle ?
                if (shapeChecker.IsCircle(edgePoints, out center, out radius))
                {
                    //g.DrawEllipse(yellowPen,
                    //    (float)(center.X - radius), (float)(center.Y - radius),
                    //    (float)(radius * 2), (float)(radius * 2));
                }
                else
                {
                    // is triangle or quadrilateral

                    List<IntPoint> corners;

                    if (shapeChecker.IsConvexPolygon(edgePoints, out corners))
                    {
                        // get sub-type
                        PolygonSubType subType = shapeChecker.CheckPolygonSubType(corners);

                        DetectedShape shape;

                        if (subType == PolygonSubType.Unknown)
                            shape = (corners.Count == 4) ? DetectedShape.quadrilateral : DetectedShape.triangle;
                        else
                            shape = (corners.Count == 4) ? DetectedShape.quadrilateral_with_nown_sub_type : DetectedShape.known_triangle;

                        var polygon = ToPointsArray(corners);

                        yield return new Tuple<DetectedShape, PPoint[]>(shape, polygon);
                        //g.DrawPolygon(pen, ToPointsArray(corners));
                    }
                }
            }
        }        

        private static PPoint[] ToPointsArray(List<IntPoint> points)
        {
            PPoint[] array = new PPoint[points.Count];

            for (int i = 0, n = points.Count; i < n; i++)
                array[i] = new PPoint(points[i].X, points[i].Y);

            return array;
        }

        [TargetedPatchingOptOut("na")]
        public static void Pixellate(this Bitmap image, Rectangle rec)
        {
            Pixellate filter = new Pixellate();
            // apply the filter
            filter.ApplyInPlace(image, rec);
        }

        [TargetedPatchingOptOut("na")]
        public static void Pixellate(this Bitmap image, Rectangle rec, int pixelSize)
        {
            // look at every pixel in the rectangle while making sure we're within the image bounds
            for (int xx = rec.X; xx < rec.X + rec.Width && xx < image.Width; xx += pixelSize)
            {
                for (int yy = rec.Y; yy < rec.Y + rec.Height && yy < image.Height; yy += pixelSize)
                {
                    int offsetX = pixelSize / 2;
                    int offsetY = pixelSize / 2;

                    // make sure that the offset is within the boundry of the image
                    while (xx + offsetX >= image.Width)
                        offsetX--;

                    while (yy + offsetY >= image.Height)
                        offsetY--;

                    // get the pixel color in the center of the soon to be pixelated area
                    Color pixel = image.GetPixel(xx + offsetX, yy + offsetY);

                    // for each pixel in the pixelate size, set it to the center color
                    for (int x = xx; x < xx + pixelSize && x < image.Width; x++)
                        for (int y = yy; y < yy + pixelSize && y < image.Height; y++)
                            image.SetPixel(x, y, pixel);
                }
            }
        }

        /// <summary>
        /// Checks whether a rectangle structure is valid
        /// </summary>
        /// <param name="re"></param>
        /// <returns></returns>
        public static bool IsValid(this Rectangle re)
        {
            return re.X >= 0 && re.Y >= 0 && re.Height > 0 && re.Width > 0;
        }

        [TargetedPatchingOptOut("na")]
        public static byte[] ToArray(this DImage image, long compression = 50L)
        {
            Debug.Assert(image != null);

            using (MemoryStream mem = new MemoryStream())
            {
                ImageCodecInfo jgpEncoder = ImageFormat.Jpeg.GetEncoder();

                using (var encoderParameters = new EncoderParameters(1))
                using (var parameter1 = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, compression))
                {
                    encoderParameters.Param[0] = parameter1;
                    image.Save(mem, jgpEncoder, encoderParameters);
                }

                return mem.ToArray();
            }
        }

        [TargetedPatchingOptOut("na")]
        public static ImageCodecInfo GetEncoder(this ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
                if (codec.FormatID == format.Guid)
                    return codec;
            return null;
        }
    }

    public static class ImageEx2
    {
        /// <summary>
        /// Copies an image. Frees the original.
        /// </summary>
        /// <param name="src"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        static public Bitmap CopyFree(this DImage src, Rectangle section)
        {
            using (src)
            {
                Bitmap bmp = new Bitmap(section.Width, section.Height);
                using (Graphics g = Graphics.FromImage(bmp))
                    g.DrawImage(src, 0, 0, section, GraphicsUnit.Pixel);
                return bmp;
            }
        }

        public static Bitmap CropImageNoFree(this Bitmap img, Rectangle cropArea)
        {
            Bitmap bmpImage = new Bitmap(img);
            Bitmap bmpCrop = bmpImage.Clone(cropArea, bmpImage.PixelFormat);
            return bmpCrop;
        }

        /// <summary>
        /// Copies an image. Doesn't free the original.
        /// </summary>
        /// <param name="src"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        static public Bitmap CopyNoFree(this DImage src, Rectangle section)
        {
            Bitmap bmp = new Bitmap(section.Width, section.Height);
            using (Graphics g = Graphics.FromImage(bmp))
                g.DrawImage(src, 0, 0, section, GraphicsUnit.Pixel);
            return bmp;
        }

        public static Bitmap CopyToBpp(this Bitmap b, int bpp)
        {
            if (bpp != 1 && bpp != 8)
                throw new System.ArgumentException("1 or 8", "bpp");

            int w = b.Width, h = b.Height;
            IntPtr hbm = b.GetHbitmap();

            BITMAPINFO bmi = new BITMAPINFO();
            bmi.biSize = 40;
            bmi.biWidth = w;
            bmi.biHeight = h;
            bmi.biPlanes = 1;
            bmi.biBitCount = (short)bpp;
            bmi.biCompression = BI_RGB;
            bmi.biSizeImage = (uint)(((w + 7) & 0xFFFFFFF8) * h / 8);
            bmi.biXPelsPerMeter = 1000000;
            bmi.biYPelsPerMeter = 1000000;
            // Now for the colour table.
            uint ncols = (uint)1 << bpp;
            bmi.biClrUsed = ncols;
            bmi.biClrImportant = ncols;
            bmi.cols = new uint[256];
            if (bpp == 1)
            {
                bmi.cols[0] = MAKERGB(0, 0, 0); 
                bmi.cols[1] = MAKERGB(255, 255, 255);
            }
            else
            {
                for (int i = 0; i < ncols; i++) 
                    bmi.cols[i] = MAKERGB(i, i, i);
            }
            IntPtr bits0;
            IntPtr hbm0 = CreateDIBSection(IntPtr.Zero, ref bmi, DIB_RGB_COLORS, out bits0, IntPtr.Zero, 0);
            IntPtr sdc = GetDC(IntPtr.Zero);
            IntPtr hdc = CreateCompatibleDC(sdc); SelectObject(hdc, hbm);
            IntPtr hdc0 = CreateCompatibleDC(sdc); SelectObject(hdc0, hbm0);
            BitBlt(hdc0, 0, 0, w, h, hdc, 0, 0, SRCCOPY);
            System.Drawing.Bitmap b0 = System.Drawing.Bitmap.FromHbitmap(hbm0);

            DeleteDC(hdc);
            DeleteDC(hdc0);
            ReleaseDC(IntPtr.Zero, sdc);
            DeleteObject(hbm);
            DeleteObject(hbm0);
            //
            return b0;
        }

        [DllImport("gdi32.dll")]
        static extern bool DeleteObject(IntPtr hObject);

        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("gdi32.dll")]
        static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("user32.dll")]
        static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);

        [DllImport("gdi32.dll")]
        static extern int DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("gdi32.dll")]
        static extern int BitBlt(IntPtr hdcDst, int xDst, int yDst, int w, int h, IntPtr hdcSrc, int xSrc, int ySrc, int rop);
        static int SRCCOPY = 0x00CC0020;

        [DllImport("gdi32.dll")]
        static extern IntPtr CreateDIBSection(IntPtr hdc, ref BITMAPINFO bmi, uint Usage, out IntPtr bits, IntPtr hSection, uint dwOffset);
        static uint BI_RGB = 0;
        static uint DIB_RGB_COLORS = 0;
        [StructLayout(LayoutKind.Sequential)]
        struct BITMAPINFO
        {
            public uint biSize;
            public int biWidth, biHeight;
            public short biPlanes, biBitCount;
            public uint biCompression, biSizeImage;
            public int biXPelsPerMeter, biYPelsPerMeter;
            public uint biClrUsed, biClrImportant;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public uint[] cols;
        }

        static uint MAKERGB(int r, int g, int b)
        {
            return ((uint)(b & 255)) | ((uint)((r & 255) << 8)) | ((uint)((g & 255) << 16));
        }
    }

    public static class ImageEx3
    {
        public static bool InBetween(this Size s, Size min, Size max)
        {
            return min.LessOrEqual(s) && s.LessOrEqual(max);
        }

        public static bool LessOrEqual(this Size s, Size s1)
        {
            return s.Height <= s1.Height && s.Width <= s1.Width;
        }

        public static float GetArea(this Region r, Graphics g)
        {
            var r1 = r.GetBounds(g);
            return r1.Width * r1.Height;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        /// <see cref="http://stackoverflow.com/questions/2703947/reducing-bitmap-bit-size-in-c-sharp"/>
        public unsafe static Bitmap ToGrayscale(this Bitmap source)
        {
            // Create target image.
            int width = source.Width;
            int height = source.Height;
            Bitmap target = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
            // Set the palette to discrete shades of gray
            ColorPalette palette = target.Palette;
            for (int i = 0; i < palette.Entries.Length; i++)
            {
                palette.Entries[i] = Color.FromArgb(0, i, i, i);
            }
            target.Palette = palette;

            // Lock bits so we have direct access to bitmap data
            BitmapData targetData = target.LockBits(new Rectangle(0, 0, width, height),
                                                    ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
            BitmapData sourceData = source.LockBits(new Rectangle(0, 0, width, height),
                                                    ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

            unsafe
            {
                for (int r = 0; r < height; r++)
                {
                    byte* pTarget = (byte*)(targetData.Scan0 + r * targetData.Stride);
                    byte* pSource = (byte*)(sourceData.Scan0 + r * sourceData.Stride);
                    for (int c = 0; c < width; c++)
                    {
                        byte colorIndex = (byte)(((*pSource) * 0.3 + *(pSource + 1) * 0.59 + *(pSource + 2) * 0.11));
                        *pTarget = colorIndex;
                        pTarget++;
                        pSource += 3;
                    }
                }
            }

            target.UnlockBits(targetData);
            source.UnlockBits(sourceData);
            return target;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        /// <see cref="http://stackoverflow.com/questions/2703947/reducing-bitmap-bit-size-in-c-sharp"/>
        public static Bitmap ToGrayscale4bpp(this Bitmap source)
        {
            // Create target image.
            int width = source.Width;
            int height = source.Height;

            Bitmap target = new Bitmap(width, height, PixelFormat.Format4bppIndexed);
            // Set the palette to discrete shades of gray
            ColorPalette palette = target.Palette;
            for (int i = 0; i < palette.Entries.Length; i++)
            {
                int cval = 17 * i;
                palette.Entries[i] = Color.FromArgb(0, cval, cval, cval);
            }
            target.Palette = palette;

            // Lock bits so we have direct access to bitmap data
            BitmapData targetData = target.LockBits(new Rectangle(0, 0, width, height),
                                                    ImageLockMode.ReadWrite, PixelFormat.Format4bppIndexed);

            BitmapData sourceData = source.LockBits(new Rectangle(0, 0, width, height),
                                                    ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

            unsafe
            {
                for (int r = 0; r < height; r++)
                {
                    byte* pTarget = (byte*)(targetData.Scan0 + r * targetData.Stride);
                    byte* pSource = (byte*)(sourceData.Scan0 + r * sourceData.Stride);
                    byte prevValue = 0;
                    for (int c = 0; c < width; c++)
                    {
                        byte colorIndex = (byte)((((*pSource) * 0.3 + *(pSource + 1) * 0.59 + *(pSource + 2) * 0.11)) / 16);
                        if (c % 2 == 0)
                            prevValue = colorIndex;
                        else
                            *(pTarget++) = (byte)(prevValue | colorIndex << 4);

                        pSource += 3;
                    }
                }
            }

            target.UnlockBits(targetData);
            source.UnlockBits(sourceData);
            return target;
        }
    }

    public enum DetectedShape
    {
        circle,
        quadrilateral,
        triangle,
        quadrilateral_with_nown_sub_type,
        known_triangle
    }

    /// <summary>
    /// 
    /// </summary>
    /// <see cref="http://leakingmemory.wordpress.com/2012/03/17/shape-recognition-using-c-and-aforge/"/>
    public class PictureModifier
    {
        private DBitmap m_currentImage;

        public PictureModifier(DBitmap currentImage)
        {
            this.m_currentImage = currentImage;
        }

        public void ApplySobelEdgeFilter()
        {
            var filter = new SobelEdgeDetector();
            filter.ApplyInPlace(this.m_currentImage);
        }

        public void ApplyGrayscale()
        {
            // create grayscale filter (BT709)
            var filter = new Grayscale(0.2125, 0.7154, 0.0721);
            m_currentImage = filter.Apply(m_currentImage);
        }

        public void ApplyFilling()
        {
            // create filter
            PointedColorFloodFill filter = new PointedColorFloodFill();
            // configure the filter
            filter.Tolerance = Color.FromArgb(150, 150, 150);
            filter.FillColor = Color.FromArgb(255, 255, 255);
            filter.StartingPoint = new IntPoint(m_currentImage.Size.Width / 2, m_currentImage.Size.Height / 2);
            // apply the filter
            filter.ApplyInPlace(m_currentImage);
        }

        public void ApplyThresholdedDifference(int threshold = 250)
        {
            ThresholdedDifference diff = new ThresholdedDifference(threshold);
            diff.OverlayImage = CreateBlackImage(m_currentImage.Size, m_currentImage.PixelFormat);
            m_currentImage = diff.Apply(m_currentImage);
        }

        public IEnumerable<Region> EnumKnownForms()
        {
            Debug.Assert(m_currentImage != null);

            DBitmap image = new DBitmap(this.m_currentImage);

            // lock image
            BitmapData bmData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.ReadWrite, image.PixelFormat);

            // turn background to black
            ColorFiltering cFilter = new ColorFiltering();
            cFilter.Red = new IntRange(0, 64);
            cFilter.Green = new IntRange(0, 64);
            cFilter.Blue = new IntRange(0, 64);
            cFilter.FillOutsideRange = false;
            cFilter.ApplyInPlace(bmData);

            // locate objects
            BlobCounter bCounter = new BlobCounter();

            bCounter.FilterBlobs = true;
            bCounter.BackgroundThreshold = Color.Black;
            bCounter.ObjectsOrder = ObjectsOrder.Size;
            bCounter.MinHeight = 30;
            bCounter.MinWidth = 30;

            bCounter.ProcessImage(bmData);
            Blob[] baBlobs = bCounter.GetObjectsInformation();
            image.UnlockBits(bmData);

            // coloring objects
            SimpleShapeChecker shapeChecker = new SimpleShapeChecker();

            for (int i = 0, n = baBlobs.Length; i < n; i++)
            {
                List<IntPoint> edgePoints = bCounter.GetBlobsEdgePoints(baBlobs[i]);

                using (var path = new System.Drawing.Drawing2D.GraphicsPath())
                {
                    path.AddPolygon(ToPointsArray(edgePoints));
                    var region = new Region(path);
                    yield return region;
                }      

                //AForge.Point center;
                //float radius;

                //if (shapeChecker.IsCircle(edgePoints, out center, out radius))
                //{
                //    //g.DrawEllipse(yellowPen, (float)(center.X - radius), (float)(center.Y - radius),
                //    //    (float)(radius * 2), (float)(radius * 2));
                //}
                //else
                //{
                //    List<IntPoint> corners;
                //    // is triangle or quadrilateral
                //    if (shapeChecker.IsConvexPolygon(edgePoints, out corners))
                //    {
                //        using (var path = new System.Drawing.Drawing2D.GraphicsPath())
                //        {
                //            path.AddPolygon(ToPointsArray(edgePoints));
                //            var region = new Region(path);
                //            yield return region;
                //        }                        
                //    }
                //}
            }

            this.m_currentImage = image;
        }

        private Bitmap CreateBlackImage(Size s, PixelFormat format)
        {
            Bitmap bmp = new DBitmap(s.Width, s.Height, format);
            using (Graphics g = Graphics.FromImage(bmp))
                g.FillRectangle(Brushes.Black, new Rectangle(PPoint.Empty, s));
            return bmp;
        }

        private System.Drawing.Point[] ToPointsArray(List<IntPoint> points)
        {
            System.Drawing.Point[] array = new System.Drawing.Point[points.Count];
            for (int i = 0, n = points.Count; i < n; i++)
                array[i] = new System.Drawing.Point(points[i].X, points[i].Y);
            return array;
        }

        public Bitmap GetCurrentImage()
        {
            return m_currentImage;
        }
    }
}