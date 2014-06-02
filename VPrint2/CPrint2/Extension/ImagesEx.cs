/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime;
using System.Runtime.InteropServices;
using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Math.Geometry;
using PPoint = System.Drawing.Point;

namespace CPrint2
{
    public static class ImageEx
    {
        /// <summary>
        /// http://www.codeproject.com/Articles/265354/Playing-Card-Recognition-Using-AForge-Net-Framewor
        /// </summary>
        /// <param name="source"></param>
        public static IEnumerable<Bitmap> CropRotateGray(this Bitmap source, int minWidth, int maxWidth, int minHeight, int maxHeight,
            bool rotate = false, bool bbp4 = false)
        {
            BlobCounter extr = new BlobCounter();
            FiltersSequence seq = new FiltersSequence();
            seq.Add(Grayscale.CommonAlgorithms.BT709);
            seq.Add(new OtsuThreshold());

            using (var temp = seq.Apply(source))
            {
                extr.FilterBlobs = true;
                extr.MinWidth = minWidth;
                extr.MaxWidth = maxWidth;
                extr.MinHeight = minHeight;
                extr.MaxHeight = maxHeight;
                extr.ProcessImage(temp);

                //ResizeBilinear resizer = new ResizeBilinear(NewWidth, NewHeight);

                foreach (Blob blob in extr.GetObjectsInformation())
                {
                    List<IntPoint> edgePoints = extr.GetBlobsEdgePoints(blob);
                    List<IntPoint> corners = PointsCloud.FindQuadrilateralCorners(edgePoints);
                    if (corners.Count < 4)
                        continue;

                    var quadTransf = new QuadrilateralTransformation(corners);
                    Bitmap voucherImg = quadTransf.Apply(source);

                    if (voucherImg == null)
                        continue;

                    if (rotate && voucherImg.Width > voucherImg.Height)
                        voucherImg.RotateFlip(RotateFlipType.Rotate90FlipNone);

                    if (bbp4)
                    {
                        //yield return voucherImg.CopyToBpp(8);
                        yield return voucherImg.ToGrayscale4bpp();
                        voucherImg.DisposeSf();
                    }
                    else
                        yield return voucherImg;
                    //cardImg = resizer.Apply(cardImg); //Normalize card size
                }
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
            ColorFiltering colorFilter = new ColorFiltering();

            colorFilter.Red = new IntRange(0, 64);
            colorFilter.Green = new IntRange(0, 64);
            colorFilter.Blue = new IntRange(0, 64);
            colorFilter.FillOutsideRange = false;

            colorFilter.ApplyInPlace(bitmapData);

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
    }

    public static class ImageEx2
    {
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
}