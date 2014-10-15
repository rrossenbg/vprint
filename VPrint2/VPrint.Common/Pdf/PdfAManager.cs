/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.security;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.X509;
using VPrinting;
using VPrinting.Pdf;

namespace VPrint.Common.Pdf
{
    public class PdfAManager
    {
        public void CreatePdf(string destinationFileName, IList<System.Drawing.Image> images, PdfCreationInfo info)
        {
            var list = new List<iTextSharp.text.Image>();

            foreach (var img in images)
                list.Add(iTextSharp.text.Image.GetInstance(img.ToArray()));

            float width = list.Max(i => i.Width);
            float height = list.Max(i => i.Height) + (info.MetaData != null ? info.MetaData.Count * 20 : 0);

            Document doc = new Document(new iTextSharp.text.Rectangle(width, height), 5, 25, 25, 5);
            try
            {
                var pdfWriter = PdfWriter.GetInstance(doc, new FileStream(destinationFileName, FileMode.Create));
                pdfWriter.SetFullCompression();
                pdfWriter.StrictImageSequence = true;
                pdfWriter.SetLinearPageMode();

                doc.Open();
                doc.AddTitle(info.Title);
                doc.AddSubject(info.Subject);
                doc.AddAuthor(info.Author);
                doc.AddCreator(info.Creator);
                doc.AddCreationDate();
                doc.AddProducer();

                PdfPTable table = new PdfPTable(2);
                table.SetWidths(new int[] { 1, 2 });
                table.SpacingAfter = 5f;
                table.SpacingBefore = 5f;

                if (info.MetaData != null)
                {
                    lock (info.MetaData)
                    {
                        var bfTimes = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);
                        var times = new iTextSharp.text.Font(bfTimes, 7, iTextSharp.text.Font.ITALIC, BaseColor.BLACK);

                        foreach (Tuple<string, string> kv in info.MetaData)
                        {
                            table.AddCell(new Phrase(new Chunk(kv.Item1) { Font = times }));
                            table.AddCell(new Phrase(new Chunk(kv.Item2) { Font = times }));
                        }
                    }
                }
                
                //.BackgroundColor = new GrayColor(0.75f);
                doc.Add(table);
                doc.Add(new Phrase(" "));
                doc.Add(new Phrase(" "));
                //TODO: http://kuujinbo.info/iTextInAction2Ed/index.aspx?ch=Chapter08&ex=TextFields

                foreach (var img in list)
                    doc.Add(img);
            }
            finally
            {
                doc.Close();
            }
        }

        /// <summary>
        /// Signs a PDF document using iTextSharp library
        /// </summary>
        /// <param name="sourceDocument">The path of the source pdf document which is to be signed</param>
        /// <param name="destinationDocument">The path at which the signed pdf document should be generated</param>
        /// <param name="privateKeyStream">A Stream containing the private/public key in .pfx format which would be used to sign the document</param>
        /// <param name="pfxKeyPass">The password for the private key</param>
        /// <param name="reasonForSigning">String describing the reason for signing, would be embedded as part of the signature</param>
        /// <param name="location">Location where the document was signed, would be embedded as part of the signature</param>
        public void SignPdfFile(string sourceDocument, string destinationDocument, PdfSignInfo info)
        {
            using (var cpfxFile = new FileStream(info.pfxFilePath, FileMode.Open, FileAccess.Read))
            {
                Pkcs12Store pk12 = new Pkcs12Store(cpfxFile, info.pfxKeyPass.ToCharArray());

                string alias = null;

                foreach (string tAlias in pk12.Aliases)
                {
                    if (pk12.IsKeyEntry(tAlias))
                    {
                        alias = tAlias;
                        break;
                    }
                }

                var pk = pk12.GetKey(alias).Key;
                using (var reader = new PdfReader(sourceDocument))
                using (var fout = new FileStream(destinationDocument, FileMode.Create, FileAccess.ReadWrite))
                using (var stamper = PdfStamper.CreateSignature(reader, fout, '\0'))
                {
                    if (info.DocPass != null)
                        stamper.SetEncryption(info.DocPass, info.DocPass, PdfWriter.ALLOW_SCREENREADERS, PdfWriter.STRENGTH128BITS);

                    var img = new iTextSharp.text.Jpeg(new Uri(info.SignImagePath));
                    PdfSignatureAppearance appearance = stamper.SignatureAppearance;
                    appearance.Image = img;
                    appearance.Reason = info.ReasonForSigning;
                    appearance.Location = info.Location;
                    const float x = 20, y = 10;
                    appearance.SetVisibleSignature(new iTextSharp.text.Rectangle(x, y, x + img.Width, y + img.Width), 1, "Icsi-Vendor");

                    IExternalSignature es = new PrivateKeySignature(pk, "SHA-256");
                    MakeSignature.SignDetached(appearance, es,
                        new X509Certificate[] { pk12.GetCertificate(alias).Certificate }, null, null, null, 0, CryptoStandard.CMS);

                    //http://www.phronesisweb.com/blog/filling-pdf-acrofields-in-c-using-itextsharp/
                    AcroFields form = stamper.AcroFields;
                    form.GenerateAppearances = true; 
                    ////form.SetField("name", "John Doe");
                    ////form.SetField("address", "xxxxx, yyyy");
                    ////form.SetField("postal_code", "12345");
                    ////form.SetField("email", "johndoe@xxx.com");
                    if (info.MetaData != null)
                    {
                        lock (info.MetaData)
                        {
                            foreach (Tuple<string, string> kv in info.MetaData)
                            {
                                form.SetField(kv.Item1, kv.Item2);
                                //form.SetFieldProperty(kv.Item1.Compress(), "fflags", 0, null);
                            }
                        }
                    }

                    //http://forums.asp.net/t/1846462.aspx?PDF+form+contents+are+not+visible+iTextSharp
                    //Dictionary<string, string> inf = reader.Info;
                    ////inf.Add("Title", "Hello World stamped");
                    ////inf.Add("Subject", "Hello World with changed metadata");
                    ////inf.Add("Keywords", "iText in Action, PdfStamper");
                    ////inf.Add("Creator", "Silly standalone example");
                    ////inf.Add("Author", "Also Bruno Lowagie");

                    //if (info.MetaData != null)
                    //{
                    //    lock (info.MetaData)
                    //        foreach (Tuple<string, string> kv in info.MetaData)
                    //            inf.Add(kv.Item1, kv.Item2);
                    //    stamper.MoreInfo = inf;
                    //}

                    //stamper.SetFullCompression();
                    //stamper.Writer.SetFullCompression();
                    stamper.FormFlattening = true;
                    stamper.Close();
                }
            }
        }

        public IEnumerable<System.Drawing.Image> ExtractImagesFromPDF(string sourcePdf)
        {
            // NOTE:  This will only get the first image it finds per page.
            var pdf = new PdfReader(sourcePdf);
            var raf = new RandomAccessFileOrArray(sourcePdf);

            try
            {
                for (int pageNum = 1; pageNum <= pdf.NumberOfPages; pageNum++)
                {
                    PdfDictionary pg = pdf.GetPageN(pageNum);

                    // recursively search pages, forms and groups for images.
                    PdfObject obj = ExtractImagesFromPDF_FindImageInPDFDictionary(pg);
                    if (obj != null)
                    {
                        int XrefIndex = Convert.ToInt32(((PRIndirectReference)obj).Number.ToString(CultureInfo.InvariantCulture));
                        PdfObject pdfObj = pdf.GetPdfObject(XrefIndex);
                        PdfStream pdfStrem = (PdfStream)pdfObj;

                        byte[] bytes = PdfReader.GetStreamBytesRaw((PRStream)pdfStrem);
                        if ((bytes != null))
                        {
                            using (var mem = new MemoryStream(bytes))
                            {
                                mem.Position = 0;
                                var img = System.Drawing.Image.FromStream(mem);
                                yield return img;
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

        private PdfObject ExtractImagesFromPDF_FindImageInPDFDictionary(PdfDictionary pg)
        {
            PdfDictionary res = (PdfDictionary)PdfReader.GetPdfObject(pg.Get(PdfName.RESOURCES));
            PdfDictionary xobj = (PdfDictionary)PdfReader.GetPdfObject(res.Get(PdfName.XOBJECT));
            if (xobj != null)
            {
                foreach (PdfName name in xobj.Keys)
                {
                    PdfObject obj = xobj.Get(name);
                    if (obj.IsIndirect())
                    {
                        PdfDictionary tg = (PdfDictionary)PdfReader.GetPdfObject(obj);
                        PdfName type = (PdfName)PdfReader.GetPdfObject(tg.Get(PdfName.SUBTYPE));

                        //image at the root of the pdf
                        if (PdfName.IMAGE.Equals(type))
                        {
                            return obj;
                        }// image inside a form
                        else if (PdfName.FORM.Equals(type))
                        {
                            return ExtractImagesFromPDF_FindImageInPDFDictionary(tg);
                        } //image inside a group
                        else if (PdfName.GROUP.Equals(type))
                        {
                            return ExtractImagesFromPDF_FindImageInPDFDictionary(tg);
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// NOTE: This code only deals with page 1, you'd want to loop more for your code
        /// </summary>
        /// <param name="fromFileName"></param>
        /// <param name="toFileName"></param>
        public void CompressPdf(string fromFileName, string toFileName)
        {
            //Bind a reader to our large PDF
            PdfReader reader = new PdfReader(fromFileName);

            using (FileStream fs = new FileStream(toFileName, FileMode.Create, FileAccess.Write, FileShare.None))
            using (PdfStamper stamper = new PdfStamper(reader, fs))
            {
                //NOTE: This code only deals with page 1, you'd want to loop more for your code
                //Get page 1
                PdfDictionary page = reader.GetPageN(1);
                //Get the xobject structure
                PdfDictionary resources = (PdfDictionary)PdfReader.GetPdfObject(page.Get(PdfName.RESOURCES));
                PdfDictionary xobject = (PdfDictionary)PdfReader.GetPdfObject(resources.Get(PdfName.XOBJECT));
                if (xobject != null)
                {
                    PdfObject obj;
                    //Loop through each key
                    foreach (PdfName name in xobject.Keys)
                    {
                        obj = xobject.Get(name);
                        if (obj.IsIndirect())
                        {
                            //Get the current key as a PDF object
                            PdfDictionary imgObject = (PdfDictionary)PdfReader.GetPdfObject(obj);
                            //See if its an image
                            if (imgObject.Get(PdfName.SUBTYPE).Equals(PdfName.IMAGE))
                            {
                                //NOTE: There's a bunch of different types of filters, 
                                //I'm only handing the simplest one here which is basically raw JPG, you'll have to research others
                                if (imgObject.Get(PdfName.FILTER).Equals(PdfName.DCTDECODE))
                                {
                                    //Get the raw bytes of the current image
                                    byte[] oldBytes = PdfReader.GetStreamBytesRaw((PRStream)imgObject);
                                    //Will hold bytes of the compressed image later
                                    byte[] newBytes;
                                    //Wrap a stream around our original image
                                    using (MemoryStream sourceMS = new MemoryStream(oldBytes))
                                    using (System.Drawing.Image oldImage = Bitmap.FromStream(sourceMS))
                                    using (System.Drawing.Image newImage = ShrinkImage(oldImage, 0.9f))
                                        newBytes = ConvertImageToBytes(newImage, 85);

                                    //Create a new iTextSharp image from our bytes
                                    iTextSharp.text.Image compressedImage = iTextSharp.text.Image.GetInstance(newBytes);
                                    //Kill off the old image
                                    PdfReader.KillIndirect(obj);
                                    //Add our image in its place
                                    stamper.Writer.AddDirectImageSimple(compressedImage, (PRIndirectReference)obj);
                                }
                            }
                        }
                    }
                }
            }
        }

        private static byte[] ConvertImageToBytes(System.Drawing.Image image, long compressionLevel)
        {
            if (compressionLevel < 0 || compressionLevel > 100)
                throw new ArgumentOutOfRangeException();

            ImageCodecInfo jgpEncoder = GetEncoder(ImageFormat.Jpeg);

            var myEncoder = System.Drawing.Imaging.Encoder.Quality;
            EncoderParameters myEncoderParameters = new EncoderParameters(1);
            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, compressionLevel);
            myEncoderParameters.Param[0] = myEncoderParameter;
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, jgpEncoder, myEncoderParameters);
                return ms.ToArray();
            }
        }

        //standard code from MSDN
        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
                if (codec.FormatID == format.Guid)
                    return codec;
            return null;
        }

        //Standard high quality thumbnail generation from
        //http://weblogs.asp.net/gunnarpeipman/archive/2009/04/02/resizing-images-without-loss-of-quality.aspx
        private static System.Drawing.Image ShrinkImage(System.Drawing.Image sourceImage, float scaleFactor)
        {
            int newWidth = Convert.ToInt32(sourceImage.Width * scaleFactor);
            int newHeight = Convert.ToInt32(sourceImage.Height * scaleFactor);

            var thumbnailBitmap = new Bitmap(newWidth, newHeight);
            using (Graphics g = Graphics.FromImage(thumbnailBitmap))
            {
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                System.Drawing.Rectangle imageRectangle = new System.Drawing.Rectangle(0, 0, newWidth, newHeight);
                g.DrawImage(sourceImage, imageRectangle);
            }
            return thumbnailBitmap;
        }
    }
}
