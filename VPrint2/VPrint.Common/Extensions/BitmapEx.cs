/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Text;
using ImgEncoder = System.Drawing.Imaging.Encoder;

namespace VPrinting
{
    [Obfuscation(StripAfterObfuscation = true, ApplyToMembers = true)]
    public static class BitmapEx
    {
        [TargetedPatchingOptOut("na")]
        public static byte[][] GetRGB(this Bitmap bmp)
        {
            BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            IntPtr ptr = bmp_data.Scan0;
            int num_pixels = bmp.Width * bmp.Height, num_bytes = bmp_data.Stride * bmp.Height, padding = bmp_data.Stride - bmp.Width * 3, i = 0, ct = 1;

            byte[] r = new byte[num_pixels], g = new byte[num_pixels], b = new byte[num_pixels], rgb = new byte[num_bytes];

            Marshal.Copy(ptr, rgb, 0, num_bytes);

            for (int x = 0; x < num_bytes - 3; x += 3)
            {
                if (x == (bmp_data.Stride * ct - padding))
                    x += padding; ct++;

                r[i] = rgb[x];
                g[i] = rgb[x + 1];
                b[i] = rgb[x + 2];
                i++;
            }
            bmp.UnlockBits(bmp_data);
            return new byte[3][] { r, g, b };
        }

        /// <summary>
        ///  
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        [TargetedPatchingOptOut("na")]
        public static Bitmap Crop2(this Bitmap bmp)
        {
            using (bmp)
            {
                int w = bmp.Width;
                int h = bmp.Height;

                Func<int, bool> allWhiteRow = row =>
                {
                    for (int i = 0; i < w; i += 20)
                        if (bmp.GetPixel(i, row).Ans() < 127)
                            return false;
                    return true;
                };

                Func<int, bool> allWhiteColumn = col =>
                {
                    for (int i = 0; i < h; i += 20)
                        if (bmp.GetPixel(col, i).Ans() < 127)
                            return false;
                    return true;
                };

                int topmost = 0;
                for (int row = 0; row < h; ++row)
                {
                    if (allWhiteRow(row))
                        topmost = row;
                    else break;
                }

                int bottommost = 0;
                for (int row = h - 1; row >= 0; --row)
                {
                    if (allWhiteRow(row))
                        bottommost = row;
                    else break;
                }

                int leftmost = 0, rightmost = 0;
                for (int col = 0; col < w; ++col)
                {
                    if (allWhiteColumn(col))
                        leftmost = col;
                    else break;
                }

                for (int col = w - 1; col >= 0; --col)
                {
                    if (allWhiteColumn(col))
                        rightmost = col;
                    else break;
                }

                if (rightmost == 0) rightmost = w; // As reached left
                if (bottommost == 0) bottommost = h; // As reached top.

                int croppedWidth = rightmost - leftmost;
                int croppedHeight = bottommost - topmost;

                if (croppedWidth == 0) // No border on left or right
                {
                    leftmost = 0;
                    croppedWidth = w;
                }

                if (croppedHeight == 0) // No border on top or bottom
                {
                    topmost = 0;
                    croppedHeight = h;
                }

                try
                {
                    var target = new Bitmap(croppedWidth, croppedHeight);
                    using (Graphics g = Graphics.FromImage(target))
                    {
                        g.DrawImage(bmp,
                          new RectangleF(0, 0, croppedWidth, croppedHeight),
                          new RectangleF(leftmost, topmost, croppedWidth, croppedHeight),
                          GraphicsUnit.Pixel);
                    }
                    return target;
                }
                catch (Exception ex)
                {
                    throw new Exception(
                        string.Format("Values are topmost={0} btm={1} left={2} right={3} croppedWidth={4} croppedHeight={5}",
                        topmost, bottommost, leftmost, rightmost, croppedWidth, croppedHeight),
                      ex);
                }
            }
        }

        [TargetedPatchingOptOut("na")]
        public static void DrawOnImage(this Image img, Action<Graphics> funct)
        {
            Debug.Assert(img != null);
            Debug.Assert(funct != null);

            using (Graphics g = Graphics.FromImage(img))
                funct(g);
        }

        [TargetedPatchingOptOut("na")]
        public static void DrawOnImage(this Image img, Action<Graphics, object> funct, object data)
        {
            Debug.Assert(img != null);
            Debug.Assert(funct != null);

            using (Graphics g = Graphics.FromImage(img))
                funct(g, data);
        }

        [TargetedPatchingOptOut("na")]
        public static void Print(this Bitmap bmp, StreamWriter writer)
        {
            for (int i = 0; i < bmp.Width; i++)
            {
                StringBuilder b = new StringBuilder();
                for (int j = 0; j < bmp.Height; j++)
                    b.AppendFormat("{0}  ", bmp.GetPixel(i, j));

                writer.WriteLine(b.ToString());
            }
        }

        [TargetedPatchingOptOut("na")]
        public static void ConvolutionFilter(this Bitmap sourceBitmap, double[,] filterMatrix, double factor = 1, int bias = 0)
        {
            BitmapData sourceData = sourceBitmap.LockBits(new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height), 
                ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height];
            byte[] resultBuffer = new byte[sourceData.Stride * sourceData.Height];

            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);

            double blue = 0.0;
            double green = 0.0;
            double red = 0.0;

            int filterWidth = filterMatrix.GetLength(1);
            int filterHeight = filterMatrix.GetLength(0);

            int filterOffset = (filterWidth - 1) / 2;
            int calcOffset = 0;

            int byteOffset = 0;

            for (int offsetY = filterOffset; offsetY < sourceBitmap.Height - filterOffset; offsetY++)
            {
                for (int offsetX = filterOffset; offsetX < sourceBitmap.Width - filterOffset; offsetX++)
                {
                    blue = 0;
                    green = 0;
                    red = 0;

                    byteOffset = offsetY * sourceData.Stride + offsetX * 4;

                    for (int filterY = -filterOffset; filterY <= filterOffset; filterY++)
                    {
                        for (int filterX = -filterOffset; filterX <= filterOffset; filterX++)
                        {
                            calcOffset = byteOffset + (filterX * 4) + (filterY * sourceData.Stride);

                            blue += (double)(pixelBuffer[calcOffset]) * filterMatrix[filterY + filterOffset, filterX + filterOffset];

                            green += (double)(pixelBuffer[calcOffset + 1]) * filterMatrix[filterY + filterOffset, filterX + filterOffset];

                            red += (double)(pixelBuffer[calcOffset + 2]) * filterMatrix[filterY + filterOffset, filterX + filterOffset];
                        }
                    }

                    blue = factor * blue + bias;
                    green = factor * green + bias;
                    red = factor * red + bias;

                    blue = (blue > 255 ? 255 : (blue < 0 ? 0 : blue));
                    green = (green > 255 ? 255 : (green < 0 ? 0 : green));
                    red = (red > 255 ? 255 : (red < 0 ? 0 : blue));

                    resultBuffer[byteOffset] = (byte)(blue);
                    resultBuffer[byteOffset + 1] = (byte)(green);
                    resultBuffer[byteOffset + 2] = (byte)(red);
                    resultBuffer[byteOffset + 3] = 255;
                }
            }

            Marshal.Copy(resultBuffer, 0, sourceData.Scan0, resultBuffer.Length);  

            sourceBitmap.UnlockBits(sourceData);
        }

        /// <summary>
        /// (g)=>{ var rect = new Rectangle(20, 20, 200, 100); g.FillRectangle(Brushes.Red, rect); }
        /// 
        /// float[][] ptsArray = { new float[] {1, 0, 0, 0, 0}, new float[] {0, 1, 0, 0, 0}, new float[] {0, 0, 1, 0, 0}, 
        ///                        new float[] {0, 0, 0, 0.31f, 0}, new float[] {0, 0, 0, 0, 1}};
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="ptsArray"></param>
        /// <param name="funct"></param>
        /// <returns></returns>
        /// <see cref="http://www.c-sharpcorner.com/UploadFile/mahesh/DrawTransparentImageUsingAB10102005010514AM/DrawTransparentImageUsingAB.aspx"/>
        [TargetedPatchingOptOut("na")]
        public static Bitmap AlphaBlend(this Bitmap bitmap, float[][] ptsArray, Action<Graphics> funct)
        {
            Debug.Assert(bitmap != null);
            Debug.Assert(ptsArray != null);
            Debug.Assert(funct != null);

            var result = new Bitmap(bitmap.Width, bitmap.Height);

            using (var g = Graphics.FromImage(result))
            {
                var matrix = new ColorMatrix(ptsArray);

                var attrib = new ImageAttributes();
                attrib.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                funct(g);
                g.DrawImage(bitmap, new Rectangle(0, 0, bitmap.Width, bitmap.Height), 0, 0, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel, attrib);
            }
            return result;
        }

        [TargetedPatchingOptOut("na")]
        public static void Pixellate(this Bitmap image, Rectangle rectangle, int pixelSize)
        {
            // look at every pixel in the rectangle while making sure we're within the image bounds
            for (int xx = rectangle.X; xx < rectangle.X + rectangle.Width && xx < image.Width; xx += pixelSize)
            {
                for (int yy = rectangle.Y; yy < rectangle.Y + rectangle.Height && yy < image.Height; yy += pixelSize)
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

        [TargetedPatchingOptOut("na")]
        public static List<Image> TiffGetAllImages(this string tiffFilePath)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(tiffFilePath));
            Debug.Assert(File.Exists(tiffFilePath));

            var images = new List<Image>();

            using (var bitmap = (Bitmap)Image.FromFile(tiffFilePath))
            {
                int count = bitmap.GetFrameCount(FrameDimension.Page);

                for (int idx = 0; idx < count; idx++)
                {
                    bitmap.SelectActiveFrame(FrameDimension.Page, idx);

                    using (var byteStream = new MemoryStream())
                    {
                        bitmap.Save(byteStream, ImageFormat.Tiff);
                        images.Add(Image.FromStream(byteStream));
                    }
                }
            }
            return images;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="fullFilePath"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <see cref="http://www.codeproject.com/Articles/16904/Save-images-into-a-multi-page-TIFF-file-or-add-ima"/>
        /// <example>saveMultipage(scannedImages, loc, "TIFF");</example>
        [TargetedPatchingOptOut("na")]
        public static bool SaveMultipage(this Image[] bmp, string fullFilePath, string type = "TIFF")
        {
            if (bmp == null)
                throw new ArgumentNullException("bmp");

            if (string.IsNullOrWhiteSpace(fullFilePath))
                throw new ArgumentException("fullFilePath");

            try
            {
                ImageCodecInfo codecInfo = GetCodecForString(type);

                for (int i = 0; i < bmp.Length; i++)
                {
                    if (bmp[i] == null)
                        break;
                    bmp[i] = (Image)ConvertToBitonal((Bitmap)bmp[i]);
                }

                if (bmp.Length == 1)
                {
                    EncoderParameters iparams = new EncoderParameters(1);
                    ImgEncoder iparam = ImgEncoder.Compression;
                    EncoderParameter iparamPara = new EncoderParameter(iparam, (long)(EncoderValue.CompressionCCITT4));
                    iparams.Param[0] = iparamPara;
                    bmp[0].Save(fullFilePath, codecInfo, iparams);
                }
                else if (bmp.Length > 1)
                {
                    ImgEncoder saveEncoder;
                    ImgEncoder compressionEncoder;
                    EncoderParameter SaveEncodeParam;
                    EncoderParameter CompressionEncodeParam;
                    EncoderParameters EncoderParams = new EncoderParameters(2);

                    saveEncoder = ImgEncoder.SaveFlag;
                    compressionEncoder = ImgEncoder.Compression;

                    // Save the first page (frame).
                    SaveEncodeParam = new EncoderParameter(saveEncoder, (long)EncoderValue.MultiFrame);
                    CompressionEncodeParam = new EncoderParameter(compressionEncoder, (long)EncoderValue.CompressionCCITT4);
                    EncoderParams.Param[0] = CompressionEncodeParam;
                    EncoderParams.Param[1] = SaveEncodeParam;

                    File.Delete(fullFilePath);
                    bmp[0].Save(fullFilePath, codecInfo, EncoderParams);

                    for (int i = 1; i < bmp.Length; i++)
                    {
                        if (bmp[i] == null)
                            break;

                        SaveEncodeParam = new EncoderParameter(saveEncoder, (long)EncoderValue.FrameDimensionPage);
                        CompressionEncodeParam = new EncoderParameter(compressionEncoder, (long)EncoderValue.CompressionCCITT4);
                        EncoderParams.Param[0] = CompressionEncodeParam;
                        EncoderParams.Param[1] = SaveEncodeParam;
                        bmp[0].SaveAdd(bmp[i], EncoderParams);
                    }

                    SaveEncodeParam = new EncoderParameter(saveEncoder, (long)EncoderValue.Flush);
                    EncoderParams.Param[0] = SaveEncodeParam;
                    bmp[0].SaveAdd(EncoderParams);
                }
                return true;
            }
            catch (Exception ee)
            {
                throw new Exception("Error in saving as multipage", ee);
            }
        }

        private static ImageCodecInfo GetCodecForString(string encType)
        {
            ImageCodecInfo[] info = ImageCodecInfo.GetImageEncoders();
            for (int i = 0; i < info.Length; i++)
                if (info[i].FormatDescription.Equals(encType))
                    return info[i];
            return null;
        }

        private static Bitmap ConvertToBitonal(Bitmap original)
        {
            Bitmap source = null;

            // If original bitmap is not already in 32 BPP, ARGB format, then convert
            if (original.PixelFormat != PixelFormat.Format32bppArgb)
            {
                source = new Bitmap(original.Width, original.Height, PixelFormat.Format32bppArgb);
                source.SetResolution(original.HorizontalResolution, original.VerticalResolution);
                using (Graphics g = Graphics.FromImage(source))
                    g.DrawImageUnscaled(original, 0, 0);
            }
            else
            {
                source = original;
            }

            // Lock source bitmap in memory
            BitmapData sourceData = source.LockBits(new Rectangle(0, 0, source.Width, source.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            // Copy image data to binary array
            int imageSize = sourceData.Stride * sourceData.Height;
            byte[] sourceBuffer = new byte[imageSize];
            Marshal.Copy(sourceData.Scan0, sourceBuffer, 0, imageSize);

            // Unlock source bitmap
            source.UnlockBits(sourceData);

            // Create destination bitmap
            Bitmap destination = new Bitmap(source.Width, source.Height, PixelFormat.Format1bppIndexed);

            // Lock destination bitmap in memory
            BitmapData destinationData = destination.LockBits(new Rectangle(0, 0, destination.Width, destination.Height), ImageLockMode.WriteOnly, PixelFormat.Format1bppIndexed);

            // Create destination buffer
            imageSize = destinationData.Stride * destinationData.Height;
            byte[] destinationBuffer = new byte[imageSize];

            int sourceIndex = 0;
            int destinationIndex = 0;
            int pixelTotal = 0;
            byte destinationValue = 0;
            int pixelValue = 128;
            int height = source.Height;
            int width = source.Width;
            int threshold = 500;

            // Iterate lines
            for (int y = 0; y < height; y++)
            {
                sourceIndex = y * sourceData.Stride;
                destinationIndex = y * destinationData.Stride;
                destinationValue = 0;
                pixelValue = 128;

                // Iterate pixels
                for (int x = 0; x < width; x++)
                {
                    // Compute pixel brightness (i.e. total of Red, Green, and Blue values)
                    pixelTotal = sourceBuffer[sourceIndex + 1] + sourceBuffer[sourceIndex + 2] + sourceBuffer[sourceIndex + 3];
                    if (pixelTotal > threshold)
                    {
                        destinationValue += (byte)pixelValue;
                    }
                    if (pixelValue == 1)
                    {
                        destinationBuffer[destinationIndex] = destinationValue;
                        destinationIndex++;
                        destinationValue = 0;
                        pixelValue = 128;
                    }
                    else
                    {
                        pixelValue >>= 1;
                    }
                    sourceIndex += 4;
                }
                if (pixelValue != 128)
                {
                    destinationBuffer[destinationIndex] = destinationValue;
                }
            }

            // Copy binary image data to destination bitmap
            Marshal.Copy(destinationBuffer, 0, destinationData.Scan0, imageSize);

            // Unlock destination bitmap
            destination.UnlockBits(destinationData);

            // Return
            return destination;
        }
    }

    public class ConvMatrix
    {
        public int TopLeft = 0, TopMid = 0, TopRight = 0;
        public int MidLeft = 0, Pixel = 1, MidRight = 0;
        public int BottomLeft = 0, BottomMid = 0, BottomRight = 0;
        public int Factor = 1;
        public int Offset = 0;

        public void SetAll(int nVal)
        {
            TopLeft = TopMid = TopRight = MidLeft = Pixel = MidRight = BottomLeft = BottomMid = BottomRight = nVal;
        }
    }
}
