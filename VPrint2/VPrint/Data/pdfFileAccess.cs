using System.Drawing;
using System.IO;
using VPrinting;
using VPrinting.Pdf;

namespace VPrint.Common.Pdf
{
    public class pdfFileAccess
    {
        public static pdfFileAccess Instance
        {
            get
            {
                return new pdfFileAccess();
            }
        }

        /// <summary>
        /// string pfxFileFullPath = Global.Strings.pfxFileFullPath;
        /// @"C:\PROJECTS\VPrint2\ReceivingServiceLib.Common\PTF.pfx"
        /// string PTFLogoFileFullPath = Global.Strings.PTFLogoFileFullPath;
        /// @"C:\PROJECTS\VPrint2\ReceivingServiceLib.Common\Resources\PTFLogo.jpg"
        /// </summary>
        /// <param name="fileStream">ImageFileArray</param>
        /// <param name="barcode"></param>
        /// <param name="countryName">Spain</param>
        /// <param name="location">"Madrid, Spain"</param>
        /// <param name="retailerId"></param>
        /// <param name="voucherId"></param>
        /// <returns></returns>
        public string CreateSignPdf(Bitmap bitmap, string barcode, int retailerId, int voucherId, PdfCreationInfo creationInfo, PdfSignInfo signInfo)
        {
            PdfAManager manager = new PdfAManager();

            var pdfFileName = Path.ChangeExtension(Path.GetTempFileName(), ".pdf");
            var signedPdfFileName = Path.ChangeExtension(Path.GetTempFileName(), ".pdf");

            try
            {
                manager.CreatePdf(pdfFileName, new Bitmap[] {bitmap}, creationInfo);

                manager.SignPdfFile(pdfFileName, signedPdfFileName, signInfo);

                return signedPdfFileName;
            }
            finally
            {
                try
                {
                    File.Delete(pdfFileName);
                }
                catch
                {
                }
            }
        }
    }
}