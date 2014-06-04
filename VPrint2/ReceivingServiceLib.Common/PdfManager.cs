using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.security;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.X509;

namespace ReceivingServiceLib
{
    public class PdfManager
    {
        public class CreationInfo
        {
            public string Title { get; set; }
            public string Subject { get; set; }
            public string Author { get; set; }
            public string Creator { get; set; }
        }

        public void CreatePdf(string destinationFileName, IList<System.Drawing.Image> images, CreationInfo info)
        {
            List<Image> list = new List<Image>();

            foreach (var img in images)
            {
                using (var mem = new MemoryStream())
                {
                    img.Save(mem, img.RawFormat);
                    list.Add(Image.GetInstance(mem));
                }
            }

            float width = list.Max(i => i.Width);
            float height = list.Max(i => i.Height);

            Document doc = new Document(new Rectangle(width, height), 25, 25, 25, 25);
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

                foreach (var img in list)
                    doc.Add(img);
            }
            finally
            {
                doc.Close();
            }
        }

        public class SignInfo
        {
            public string pfxFilePath;
            public string pfxKeyPass;
            public byte[] docPass;
            public string signImagePath;
            public string reasonForSigning;
            public string location;
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
        public void SignPdfFile(string sourceDocument, string destinationDocument, SignInfo info)
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
                    if (info.docPass != null)
                        stamper.SetEncryption(info.docPass, info.docPass, PdfWriter.ALLOW_SCREENREADERS, PdfWriter.STRENGTH128BITS);

                    var img = new iTextSharp.text.Jpeg(new Uri(info.signImagePath));
                    PdfSignatureAppearance appearance = stamper.SignatureAppearance;
                    appearance.Image = img;
                    appearance.Reason = info.reasonForSigning;
                    appearance.Location = info.location;
                    const float x = 20, y = 10;
                    appearance.SetVisibleSignature(new iTextSharp.text.Rectangle(x, y, x + img.Width, y + img.Width), 1, "Icsi-Vendor");

                    IExternalSignature es = new PrivateKeySignature(pk, "SHA-256");
                    MakeSignature.SignDetached(appearance, es,
                        new X509Certificate[] { pk12.GetCertificate(alias).Certificate }, null, null, null, 0, CryptoStandard.CMS);
                    stamper.Close();
                }
            }
        }
    }
}
