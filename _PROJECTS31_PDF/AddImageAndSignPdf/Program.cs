using System;
using System.IO;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.security;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.X509;

namespace AddImageAndSignPdf
{
    /// <summary>
    /// 
    /// </summary>
    /// <see cref="http://stackoverflow.com/questions/84847/how-do-i-create-a-self-signed-certificate-for-code-signing-on-windows">Certificates</see>
    /// <see cref="http://www.mikesdotnetting.com/Article/87/iTextSharp-Working-with-images">PDF Images</see>
    /// <example>
    /// makecert -r -pe -n "CN=PTF-Spain" -ss CA -sr CurrentUser -a sha256 -cy authority -sky signature -sv "D:\TEST\PTF.pvk" "D:\TEST\PTF.cer"
    /// pvk2pfx -pvk "D:\TEST\PTF.pvk" -spc "D:\TEST\PTF.cer" -pfx "D:\TEST\PTF.pfx"
    /// </example>
    class Program
    {
        static void Main(string[] args)
        {
            const string PATH = @"D:\PROJECTS\_PROJECTS31_PDF\AddImageAndSignPdf\Others\";

            CreatePdf(PATH + "test123.pdf", PATH + "NewFile.JPG", new CreationInfo()
            {
                Title = "Voucher 1234567",
                Subject = "Retailer 1342526",
                Author = "PTF Spain",
                Creator = "PTF Spain"
            });

            byte[] pass = UTF8Encoding.Default.GetBytes("rosen123$");

            SignPdfFile(
                PATH + "test123.pdf",
                PATH + "test123_Signed.pdf",
                new SignInfo()
            {
                pfxFilePath = PATH + "PTF.pfx",
                pfxKeyPass = "",
                docPass = pass,
                signImagePath = PATH + "..\\Images\\PTFLogo.jpg",
                reasonForSigning = "Voucher No. 123452321",
                location = "Madrid, Spain"
            });
        }

        public class CreationInfo
        {
            public string Title { get; set; }
            public string Subject { get; set; }
            public string Author { get; set; }
            public string Creator { get; set; }
        }

        public static void CreatePdf(string destinationFileName, string imageFilePath, CreationInfo info)
        {
            Image gif = Image.GetInstance(imageFilePath);
            Document doc = new Document(new Rectangle(gif.Width, gif.Height), 25, 25, 25, 25);
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
                doc.Add(gif);
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
        public static void SignPdfFile(string sourceDocument, string destinationDocument,SignInfo i)
        {
            using (var cpfxFile = new FileStream(i.pfxFilePath, FileMode.Open, FileAccess.Read))
            {
                Pkcs12Store pk12 = new Pkcs12Store(cpfxFile, i.pfxKeyPass.ToCharArray());

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
                    stamper.SetEncryption(i.docPass, i.docPass, PdfWriter.ALLOW_SCREENREADERS, PdfWriter.STRENGTH128BITS);

                    var img = new iTextSharp.text.Jpeg(new Uri(i.signImagePath));
                    PdfSignatureAppearance appearance = stamper.SignatureAppearance;
                    appearance.Image = img;
                    appearance.Reason = i.reasonForSigning;
                    appearance.Location = i.location;
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
