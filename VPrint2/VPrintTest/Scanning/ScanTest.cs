using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Media.Imaging;
using DTKBarReader;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VPrinting;
using VPrinting.Common;

namespace VPrintTest
{
    [TestClass]
    public class ScanTest
    {
        [TestMethod]
        public void italy_barcode_scan_test()
        {
            PluginLoader loader = new PluginLoader();
            string path = @"C:\PROJECTS\VPrint2\VPrintTest\bin\x86\Debug";
            loader.Start(path);

            var files = Directory.GetFiles(@"C:\IMAGES\PN");

            StateSaver.Default.Path = "asd";
            StateSaver.Default.Load();
            List<BarcodeConfig> barcodeLayouts = StateSaver.Default.Get<List<BarcodeConfig>>(Strings.LIST_OF_BARCODECONFIGS);

            foreach (var fileName in files)
            {
                Debug.WriteLine(fileName);

                Bitmap bmp = (Bitmap)Bitmap.FromFile(fileName);
                {
                    Bitmap bmpBarcode = null;
                    {
                        var time = Stopwatch.StartNew();

                        Rectangle rect = Rectangle.Empty;
                        string barcode = null;
                        bool result = CommonTools.ParseVoucherImage(ref bmp, ref bmpBarcode, out rect, ref barcode, BarcodeTypeEnum.BT_All);
                        if (!result)
                        {
                            Debug.WriteLine("Cant read barcode");
                            continue;
                        }

                        BarcodeData data = null;

                        if (barcode != null)
                        {
                            foreach (var cfg in barcodeLayouts)
                                if (cfg.ParseBarcode(barcode, ref data))
                                    break;
                        }
                        else
                        {
                            Debug.WriteLine("Wrong barcode " + barcode);
                        }

                        if (data == null)
                            Debug.WriteLine("Wrong barcode " + barcode);

                        Debug.WriteLine(time.Elapsed);
                    }
                }

                Debug.WriteLine("===================================");
            }
        }


        [TestMethod]
        public void test_barcode_reader()
        {
            var files1 = new DirectoryInfo(@"C:\spain\BBVA\BBVA examples 18 digit").GetFiles();
            var files2 = new DirectoryInfo(@"C:\spain\Thales Examples\Thales Examples").GetFiles();
            var files3 = new DirectoryInfo(@"C:\spain\Verifone Examples\Verifone Examples").GetFiles();
            var files4 = new DirectoryInfo(@"C:\spain\Verifone Examples - TOPOS\Verifone Examples - TOPOS").GetFiles();

            var files = files1.Add(files2).Add(files3).Add(files4);

            StateSaver.Default.Path = "asd";
            StateSaver.Default.Load();
            List<BarcodeConfig> barcodeLayouts = StateSaver.Default.Get<List<BarcodeConfig>>(Strings.LIST_OF_BARCODECONFIGS);

            foreach (var fileName in files)
            {
                Debug.WriteLine(fileName);

                Bitmap bmp = (Bitmap)Bitmap.FromFile(fileName.FullName);
                Bitmap bmpBarcode = null;
                {
                    var time = Stopwatch.StartNew();

                    Rectangle rect = Rectangle.Empty;
                    string barcode = null;
                    bool result = CommonTools.ParseVoucherImage(ref bmp, ref bmpBarcode, out rect, ref barcode);
                    Assert.IsTrue(result);

                    BarcodeData data = null;

                    foreach (var cfg in barcodeLayouts)
                        if (cfg.ParseBarcode(barcode, ref data))
                            break;

                    if (data == null)
                        Debug.WriteLine("Wrong barcode " + barcode);

                    Debug.WriteLine(time.Elapsed);
                }

                Debug.WriteLine("===================================");
            }
        }

        [TestMethod]
        public void test_image_crop_whiteframe()
        {
            //string fileName = "C:\\IMAGES\\img-130628084640-0001.jpg";
            //string saveTofileName = "C:\\test.JPG";

            //using (Bitmap bmp = (Bitmap)Bitmap.FromFile(fileName))
            //{
            //    BoundingBoxFinder f = new BoundingBoxFinder();
            //    var r = f.FindBoxSobel(bmp, 100);

            //    using (var bmp2 = bmp.Clone(r, bmp.PixelFormat))
            //    {
            //        bmp2.Save(saveTofileName, ImageFormat.Jpeg);
            //    }
            //}
        }

        [TestMethod]
        public void test_spanish_barcode()
        {
            var cfg = new BarcodeConfig()
              {
                  Name = "Spain",
                  Length = 18,
                  HasCheckDigit = true,
                  //iso, ty, br, voucher
                  Template = "{0:000}{1:00}{2:00000}{3:0000000}",
                  Sample = "724 21 43101 4876719 9",
                  CountryID = new Tuple<int, int>(0, 3),
                  BuzType = new Tuple<int, int>(3, 2),
                  RetailerID = new Tuple<int, int>(5, 5),
                  VoucherID = new Tuple<int, int>(10, 7),
              };
            BarcodeData data = null;
            cfg.ParseBarcode("724214310148767199", ref data);
        }

        [TestMethod]
        public void test_edgedetecthomogenity()
        {
            //var file = (Bitmap)Bitmap.FromFile(@"C:\test.png");
            //var bmp = ImageHelper.CropUnwantedBackground(file);
        }

        [TestMethod]
        public void test_multipage_tif()
        {
            // Define the image palette
            var fileName = @"C:\Users\Rosen.rusev\Pictures\Presenter\2014-05-02_0039.jpg";

            FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);

            var img = new System.Windows.Media.Imaging.BitmapImage();
            img.BeginInit();
            img.StreamSource = fileStream;
            img.EndInit();

            FileStream stream = new FileStream(@"C:\Users\Rosen.rusev\Pictures\Presenter\new.tif", FileMode.Create);
            TiffBitmapEncoder encoder = new TiffBitmapEncoder();
            Debug.WriteLine(encoder.CodecInfo.Author);
            encoder.Compression = TiffCompressOption.Ccitt4 | TiffCompressOption.Lzw;
            encoder.Frames.Add(BitmapFrame.Create(img));
            encoder.Frames.Add(BitmapFrame.Create(img));
            encoder.Frames.Add(BitmapFrame.Create(img));
            encoder.Save(stream);
        }

        [TestMethod]
        public void test_BarcodeConfig_List()
        {
            var list = new List<BarcodeConfig>()
            {
                 new BarcodeConfig()
                {
                    Name = "CCC-SS-RRRRRR-VVVVVVVVV",
                    Length = 20,
                    //iso, ty, br, voucher
                    Template = "{0:000}{1:00}{2:000000}{3:00000000}",
                    Sample = "012 01 012345 012345678",
                    CountryID = new Tuple<int,int>(0, 3),
                    BuzType = new Tuple<int,int>(3, 2),
                    RetailerID = new Tuple<int,int>(5, 6),
                    VoucherID = new Tuple<int,int>(11, 9),
                },
                new BarcodeConfig()
                {
                    Name = "CCC-RRRRRR-VVVVVVVVV",
                    Length = 18,
                    //iso, ty, br, voucher
                    Template = "{0:000}{2:000000}{3:00000000}",
                    Sample = "012 012345 012345678",
                    CountryID = new Tuple<int,int>(0, 3),
                    RetailerID = new Tuple<int,int>(3, 6),
                    VoucherID = new Tuple<int,int>(9, 9),
                },
                new BarcodeConfig()
                {
                    Name = "VVVVVVVVV-CCC-SS",
                    Length = 14,
                    //iso, ty, br, voucher
                    Template = "{3:000000000}{0:000}{1:00}",
                    Sample = "012345678 012 01",
                    VoucherID = new Tuple<int,int>(0, 9),
                    CountryID = new Tuple<int,int>(9, 3),
                    BuzType = new Tuple<int,int>(12,2),
                },
                new BarcodeConfig()
                {
                    Name = "CCC-SS-RRRRRR-VVVVVVVVV-AAAAAAAAAAA",
                    Length = 31,
                    //iso, ty, br, voucher
                    Template = "{0:000}{2:000000}{3:00000000}",
                    Sample = "012 01 012345 012345678 01234567890",
                    CountryID = new Tuple<int,int>(0, 3),
                    BuzType = new Tuple<int,int>(3, 2),
                    RetailerID = new Tuple<int,int>(5, 6),
                    VoucherID = new Tuple<int,int>(11, 9),
                },
            };

            foreach (var c in list)
            {
                try
                {
                    c.Test();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
        }


        public ThreadExceptionEventHandler OnThreadException { get; set; }
    }

    //public class ImageHelper
    //{
    //    #region CropUnwantedBackground

    //    public static Bitmap CropUnwantedBackground(Bitmap bmp)
    //    {
    //        var backColor = GetMatchedBackColor(bmp);
    //        if (backColor.HasValue)
    //        {
    //            var bounds = GetImageBounds(bmp, backColor);
    //            var diffX = bounds[1].X - bounds[0].X + 1;
    //            var diffY = bounds[1].Y - bounds[0].Y + 1;
    //            var croppedBmp = new Bitmap(diffX, diffY);
    //            var g = Graphics.FromImage(croppedBmp);
    //            var destRect = new Rectangle(0, 0, croppedBmp.Width, croppedBmp.Height);
    //            var srcRect = new Rectangle(bounds[0].X, bounds[0].Y, diffX, diffY);
    //            g.DrawImage(bmp, destRect, srcRect, GraphicsUnit.Pixel);
    //            return croppedBmp;
    //        }
    //        else
    //        {
    //            return null;
    //        }
    //    }


        //#region Private Methods

        //#region GetImageBounds

        //private static Point[] GetImageBounds(Bitmap bmp, Color? backColor)
        //{
        //    //--------------------------------------------------------------------
        //    // Finding the Bounds of Crop Area bu using Unsafe Code and Image Proccesing
        //    Color c;
        //    int width = bmp.Width, height = bmp.Height;
        //    bool upperLeftPointFounded = false;
        //    var bounds = new Point[2];
        //    for (int y = 0; y < height; y++)
        //    {
        //        for (int x = 0; x < width; x++)
        //        {
        //            c = bmp.GetPixel(x, y);
        //            bool sameAsBackColor = ((c.R <= backColor.Value.R * 1.1 && c.R >= backColor.Value.R * 0.9) &&
        //                                    (c.G <= backColor.Value.G * 1.1 && c.G >= backColor.Value.G * 0.9) &&
        //                                    (c.B <= backColor.Value.B * 1.1 && c.B >= backColor.Value.B * 0.9));
        //            if (!sameAsBackColor)
        //            {
        //                if (!upperLeftPointFounded)
        //                {
        //                    bounds[0] = new Point(x, y);
        //                    bounds[1] = new Point(x, y);
        //                    upperLeftPointFounded = true;
        //                }
        //                else
        //                {
        //                    if (x > bounds[1].X)
        //                        bounds[1].X = x;
        //                    else if (x < bounds[0].X)
        //                        bounds[0].X = x;
        //                    if (y >= bounds[1].Y)
        //                        bounds[1].Y = y;
        //                }
        //            }
        //        }
        //    }
        //    return bounds;
        //}
    //    #endregion

    //    #region GetMatchedBackColor

    //    //private static Color? GetMatchedBackColor(Bitmap bmp)
    //    //{
    //    //    // Getting The Background Color by checking Corners of Original Image
    //    //    var corners = new Point[]{
    //    //    new Point(0, 0),
    //    //    new Point(0, bmp.Height - 1),
    //    //    new Point(bmp.Width - 1, 0),
    //    //    new Point(bmp.Width - 1, bmp.Height - 1)
    //    //}; // four corners (Top, Left), (Top, Right), (Bottom, Left), (Bottom, Right)
    //    //    for (int i = 0; i < 4; i++)
    //    //    {
    //    //        var cornerMatched = 0;
    //    //        var backColor = bmp.GetPixel(corners[i].X, corners[i].Y);
    //    //        for (int j = 0; j < 4; j++)
    //    //        {
    //    //            var cornerColor = bmp.GetPixel(corners[j].X, corners[j].Y);// Check RGB with some offset
    //    //            if ((cornerColor.R <= backColor.R * 1.1 && cornerColor.R >= backColor.R * 0.9) &&
    //    //                (cornerColor.G <= backColor.G * 1.1 && cornerColor.G >= backColor.G * 0.9) &&
    //    //                (cornerColor.B <= backColor.B * 1.1 && cornerColor.B >= backColor.B * 0.9))
    //    //            {
    //    //                cornerMatched++;
    //    //            }
    //    //        }
    //    //        if (cornerMatched > 2)
    //    //        {
    //    //            return backColor;
    //    //        }
    //    //    }
    //    //    return null;
    //    //}

    //    #endregion

    //    #endregion
    //}

    //public class BoundingBoxFinder
    //{
    //    /// <summary>
    //    /// Returns a rectangle inside 'lookInside' that bounds any energy greater than 'threshold'. 
    //    /// </summary>
    //    /// <param name="image"></param>
    //    /// <param name="lookInside">A rectangle of 'image' to look inside. </param>
    //    /// <param name="threshold">1-255, the energy threshold to detect activity. 80-150 is a good range.</param>
    //    /// <returns></returns>
    //    public Rectangle FindBoxSobel(Bitmap originalImage, Rectangle lookInside, byte threshold)
    //    {

    //        Bitmap image = originalImage;
    //        try
    //        {
    //            //Convert if needed (makes an extra copy)
    //            if (image.PixelFormat != PixelFormat.Format24bppRgb &&
    //                image.PixelFormat != PixelFormat.Format32bppArgb &&
    //                image.PixelFormat != PixelFormat.Format32bppRgb)
    //            {
    //                image = AForge.Imaging.Image.Clone(image, PixelFormat.Format24bppRgb);
    //            }

    //            //Crop if needed (makes an extra copy unless we converted too, then only 1 extra copy)
    //            if (!lookInside.Equals(new Rectangle(0, 0, image.Width, image.Height)))
    //            {
    //                Bitmap oldImage = image;
    //                try
    //                {
    //                    image = new Crop(lookInside).Apply(image);
    //                }
    //                finally
    //                {
    //                    if (oldImage != originalImage) oldImage.Dispose(); //Dispose the cloned 
    //                }
    //            }


    //            //Makes 1 more copy at 1/3rd the size, in grayscale
    //            Rectangle result = FindBoxSobel(image, threshold);
    //            return new Rectangle(lookInside.X + result.X, lookInside.Y + result.Y, result.Width, result.Height);


    //        }
    //        finally
    //        {
    //            if (image != originalImage) image.Dispose();
    //        }

    //    }
    //    /// <summary>
    //    /// Requires 24 bit or 32 bit (A) RGB image. 
    //    /// </summary>
    //    /// <param name="rgb"></param>
    //    /// <param name="threshold"></param>
    //    /// <returns></returns>
    //    public Rectangle FindBoxSobel(Bitmap rgb, byte threshold)
    //    {
    //        using (Bitmap gray = Grayscale.CommonAlgorithms.Y.Apply(rgb))
    //        {
    //            //Apply sobel operator to grayscale image
    //            new SobelEdgeDetector().Apply(gray);
    //            //Threshold into black and white.
    //            new Threshold(threshold).Apply(gray);
    //            //Trim only exact black pixels
    //            // lock source bitmap data
    //            BitmapData data = gray.LockBits(new Rectangle(0, 0, gray.Width, gray.Height), ImageLockMode.ReadOnly, gray.PixelFormat);
    //            try
    //            {
    //                return FindBoxExactGrayscale(data, 0);
    //            }
    //            finally
    //            {
    //                gray.UnlockBits(data);
    //            }
    //        }
    //    }
    //    /// <summary>
    //    /// Returns a bounding box that only excludes the specified color. 
    //    /// Only works on 8-bit images.
    //    /// </summary>
    //    /// <param name="sourceData"></param>
    //    /// <param name="colorToRemove">The palette index to remove.</param>
    //    /// <returns></returns>
    //    public Rectangle FindBoxExactGrayscale(BitmapData sourceData, byte indexToRemove)
    //    {
    //        if (sourceData.PixelFormat != PixelFormat.Format8bppIndexed) throw new ArgumentOutOfRangeException("FindBoxExact only operates on 8-bit grayscale images");
    //        // get source image size
    //        int width = sourceData.Width;
    //        int height = sourceData.Height;
    //        int offset = sourceData.Stride - width;

    //        int minX = width;
    //        int minY = height;
    //        int maxX = 0;
    //        int maxY = 0;

    //        // find rectangle which contains something except color to remove
    //        unsafe
    //        {
    //            byte* src = (byte*)sourceData.Scan0;

    //            for (int y = 0; y < height; y++)
    //            {
    //                if (y > 0) src += offset; //Don't adjust for offset until after first row
    //                for (int x = 0; x < width; x++)
    //                {
    //                    if (x > 0 || y > 0) src++; //Don't increment until after the first pixel.
    //                    if (*src != indexToRemove)
    //                    {
    //                        if (x < minX)
    //                            minX = x;
    //                        if (x > maxX)
    //                            maxX = x;
    //                        if (y < minY)
    //                            minY = y;
    //                        if (y > maxY)
    //                            maxY = y;
    //                    }
    //                }
    //            }
    //        }

    //        // check
    //        if ((minX == width) && (minY == height) && (maxX == 0) && (maxY == 0))
    //        {
    //            minX = minY = 0;
    //        }

    //        return new Rectangle(minX, minY, maxX - minX + 1, maxY - minY + 1);
    //    }
    //}
}

