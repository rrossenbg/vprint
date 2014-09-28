using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows.Media.Imaging;
using AForge.Imaging;
using AForge.Imaging.Filters;
using DTKBarReader;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VPrint.Common;
using VPrinting;
using VPrinting.Common;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Globalization;

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
            loader.Process(path, PluginLoader.Operation.Start);

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

                        System.Drawing.Rectangle rect = System.Drawing.Rectangle.Empty;
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

            loader.Process(path, PluginLoader.Operation.Stop);
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

                    System.Drawing.Rectangle rect = System.Drawing.Rectangle.Empty;
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

        [TestMethod]
        public void test_creditcardcover()
        {
            try
            {
                const string FILE1 = @"C:\IMAGES\PB\PB742030.jpg";
                const string FILE2 = @"C:\IMAGES\PB\PB742030_done2.jpg";
                const string FILE3 = @"C:\IMAGES\PB\PB742030_done3.jpg";
                const string FILE4 = @"C:\IMAGES\PB\PB742030_done4.jpg";

                //using (var image = (Bitmap)Bitmap.FromFile(FILE1))
                //{
                //    //Invert filter = new Invert();
                //    //filter.ApplyInPlace(image);
                //    using (var img = image.ConvertToBitonal())
                //    {
                //        img.Save(FILE2, ImageFormat.Jpeg);
                //    }
                //}

                using (var image = (Bitmap)Bitmap.FromFile(FILE1))
                {
                    EuclideanColorFiltering filter = new EuclideanColorFiltering();
                    filter.CenterColor = new AForge.Imaging.RGB(Color.BlueViolet); //Pure White
                    filter.Radius = 0; //Increase this to allow off-whites
                    filter.FillColor = new AForge.Imaging.RGB(Color.Red); //Replacement Colour
                    filter.ApplyInPlace(image);
                }

                return;

                //To gray scale
                using (var image = (Bitmap)Bitmap.FromFile(FILE1))
                {
                    FiltersSequence seq = new FiltersSequence();
                    seq.Add(Grayscale.CommonAlgorithms.BT709);  //First add  GrayScaling filter
                    seq.Add(new OtsuThreshold()); //Then add binarization(thresholding) filter
                    var img = seq.Apply(image); // Apply filters on source image
                    img.Save(FILE2, ImageFormat.Jpeg);
                    return;
                }

                using (var image = (Bitmap)Bitmap.FromFile(FILE1))
                {
                    Grayscale filter = new Grayscale(0.2125, 0.7154, 0.0721);
                    // apply the filter
                    using (Bitmap grayImage = filter.Apply(image))
                    {
                        VerticalIntensityStatistics vis = new VerticalIntensityStatistics(grayImage);
                        // get gray histogram (for grayscale image)
                        var histogram = vis.Gray;
                        // output some histogram's information
                        System.Diagnostics.Debug.WriteLine("Mean = " + histogram.Mean);
                        System.Diagnostics.Debug.WriteLine("Min = " + histogram.Min);
                        System.Diagnostics.Debug.WriteLine("Max = " + histogram.Max);
                    }
                }

                

                using (var image = (Bitmap)Bitmap.FromFile(FILE2))
                {
                    VerticalIntensityStatistics vis = new VerticalIntensityStatistics(image);
                    // get gray histogram (for grayscale image)
                    var histogram = vis.Gray;
                    // output some histogram's information
                    System.Diagnostics.Debug.WriteLine("Mean = " + histogram.Mean);
                    System.Diagnostics.Debug.WriteLine("Min = " + histogram.Min);
                    System.Diagnostics.Debug.WriteLine("Max = " + histogram.Max);
                }
                //using (var image = (Bitmap)Bitmap.FromFile(FILE2))
                //{
                //    //EuclideanColorFiltering filter = new EuclideanColorFiltering();
                //    //// set center colol and radius
                //    //filter.CenterColor = new RGB(82, 26, 39);
                //    //filter.Radius = 100;
                //    //// apply the filter
                //    //filter.ApplyInPlace(image);

                //    Median filter2 = new Median();
                //    // apply the filter
                //    filter2.ApplyInPlace(image);
                //    image.Save(FILE3, ImageFormat.Jpeg);
                //}

                using (var image = (Bitmap)Bitmap.FromFile(FILE2))
                {
                    //Median filter2 = new Median();
                    //// apply the filter
                    //filter2.ApplyInPlace(image);

                    //ConservativeSmoothing filter = new ConservativeSmoothing();
                    //filter.ApplyInPlace(image);

                    //BilateralSmoothing filter = new BilateralSmoothing();
                    //filter.KernelSize = 25;
                    //filter.SpatialFactor = 10;
                    //filter.ColorFactor = 60;
                    //filter.ColorPower = 0.5;
                    //// apply the filter
                    //filter.ApplyInPlace(image);

                    ///////////////////////////////////////////
                    
                    // create filter
                    BlobsFiltering filter3 = new BlobsFiltering();
                    // configure filter
                    //filter.CoupledSizeFiltering = true;
                    filter3.MinWidth = 20;
                    filter3.MinHeight = 20;
                    filter3.MaxWidth = 150;
                    filter3.MaxHeight = 150;
                    filter3.ApplyInPlace(image); //apply the filter
                    ////////////////////////////


                    //ExtractBiggestBlob filter = new ExtractBiggestBlob();
                    //// apply the filter
                    //Bitmap biggestBlobsImage = filter.Apply(image);
                    image.Save(FILE3, ImageFormat.Jpeg);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        [TestMethod]
        public void test_template_matching()
        {
            try
            {
                using (var sourceImage = ((Bitmap)Bitmap.FromFile(@"C:\IMAGES\PB\PB742007.jpg")).ToGrayScale())
                using (var template = ((Bitmap)Bitmap.FromFile(@"C:\IMAGES\PB\PB742007_cover2.jpg")).ToGrayScale())
                {
                    // create template matching algorithm's instance
                    // (set similarity threshold to 92.5%)
                    ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching(0.80f);
                    // find all matchings with specified above similarity
                    TemplateMatch[] matchings = tm.ProcessImage(sourceImage, template);
                    // highlight found matchings

                    BitmapData data = sourceImage.LockBits(new System.Drawing.Rectangle(0, 0, sourceImage.Width, sourceImage.Height),
                        ImageLockMode.ReadWrite, sourceImage.PixelFormat);
                    foreach (TemplateMatch m in matchings)
                    {
                        Drawing.Rectangle(data, m.Rectangle, Color.Red);
                        Debug.WriteLine(m.Rectangle.Location.ToString());
                    }
                    sourceImage.UnlockBits(data);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        [TestMethod]
        public void test_pdf_opening()
        {
            ExtractImagesFromPDF(@"C:\IMAGES\PORTUGAL\OF_68401.pdf", @"C:\test");
        }

        public static void ExtractImagesFromPDF(string sourcePdf, string outputPath)
        {
            // NOTE:  This will only get the first image it finds per page.
            PdfReader pdf = new PdfReader(sourcePdf);
            RandomAccessFileOrArray raf = new RandomAccessFileOrArray(sourcePdf);

            try
            {
                for (int pageNumber = 1; pageNumber <= pdf.NumberOfPages; pageNumber++)
                {
                    PdfDictionary pg = pdf.GetPageN(pageNumber);

                    // recursively search pages, forms and groups for images.
                    PdfObject obj = FindImageInPDFDictionary(pg);
                    if (obj != null)
                    {

                        int XrefIndex = Convert.ToInt32(((PRIndirectReference)obj).Number.ToString(CultureInfo.InvariantCulture));
                        PdfObject pdfObj = pdf.GetPdfObject(XrefIndex);
                        PdfStream pdfStrem = (PdfStream)pdfObj;
                        byte[] bytes = PdfReader.GetStreamBytesRaw((PRStream)pdfStrem);
                        if ((bytes != null))
                        {
                            using (var memStream = new MemoryStream(bytes))
                            {
                                memStream.Position = 0;
                                var img = System.Drawing.Image.FromStream(memStream);
                                if (!Directory.Exists(outputPath))
                                    Directory.CreateDirectory(outputPath);
                                img.Save("C:\\test.bmp");
                            }
                        }
                    }
                }
            }
            finally
            {
                pdf.Close();
                raf.Close();
            }
        }

        private static PdfObject FindImageInPDFDictionary(PdfDictionary pg)
        {
            PdfDictionary res =
                (PdfDictionary)PdfReader.GetPdfObject(pg.Get(PdfName.RESOURCES));


            PdfDictionary xobj =
              (PdfDictionary)PdfReader.GetPdfObject(res.Get(PdfName.XOBJECT));
            if (xobj != null)
            {
                foreach (PdfName name in xobj.Keys)
                {

                    PdfObject obj = xobj.Get(name);
                    if (obj.IsIndirect())
                    {
                        PdfDictionary tg = (PdfDictionary)PdfReader.GetPdfObject(obj);

                        PdfName type =
                          (PdfName)PdfReader.GetPdfObject(tg.Get(PdfName.SUBTYPE));

                        //image at the root of the pdf
                        if (PdfName.IMAGE.Equals(type))
                        {
                            return obj;
                        }// image inside a form
                        else if (PdfName.FORM.Equals(type))
                        {
                            return FindImageInPDFDictionary(tg);
                        } //image inside a group
                        else if (PdfName.GROUP.Equals(type))
                        {
                            return FindImageInPDFDictionary(tg);
                        }

                    }
                }
            }

            return null;

        }

        [TestMethod]
        public void A()
        {
            //var cu = Voodoo<CurrentUser>.NewInUnmanagedMemory();
            //cu.CountryID = 123;
            //cu.Username = "Rosen";
            //cu.Username = "Rosen";
            //Voodoo<CurrentUser>.FreeUnmanagedInstance(cu);

            using (var a = new ObjectHandle<A>())
            {
                a.Value.M1 = 0;
                a.Value.M2 = "Rosen";
                a.Value.M3 = 0;
            }
        }
    }

    public class A
    {
        public int M1 { get; set; }
        public string M2 { get; set; }
        public decimal M3 { get; set; }
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

    public static class ImageEx
    {
        public static Bitmap ToGrayScale(this Bitmap image)
        {
            var seq = new FiltersSequence();
            seq.Add(Grayscale.CommonAlgorithms.BT709);  //First add  GrayScaling filter
            seq.Add(new OtsuThreshold());               //Then add binarization(thresholding) filter
            return seq.Apply(image);                    // Apply filters on source image
        }
    }
}

