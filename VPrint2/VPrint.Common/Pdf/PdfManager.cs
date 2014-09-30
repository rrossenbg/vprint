using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using iTextSharp.text.pdf;

namespace VPrint.Common.Pdf
{
    public class PdfManager
    {
        private string m_sourcePdf;

        public PdfManager(string sourcePdf)
        {
            m_sourcePdf = sourcePdf;
        }

        public IEnumerable<Image> ExtractImagesFromPDF()
        {
            // NOTE:  This will only get the first image it finds per page.
            var pdf = new PdfReader(m_sourcePdf);
            var raf = new RandomAccessFileOrArray(m_sourcePdf);

            try
            {
                for (int pageNum = 1; pageNum <= pdf.NumberOfPages; pageNum++)
                {
                    PdfDictionary pg = pdf.GetPageN(pageNum);

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

        private PdfObject FindImageInPDFDictionary(PdfDictionary pg)
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
    }
}
