﻿using System.IO;
using System.Drawing;
using System.Diagnostics;

namespace ReceivingServiceLib
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
        /// @"C:\PROJECTS\VPrint2\ReceivingServiceLib.Common\PTF.pfx"
        /// <summary>
        ///  @"C:\PROJECTS\VPrint2\ReceivingServiceLib.Common\Resources\PTFLogo.jpg"

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bufferIn">ImageFileArray</param>
        /// <param name="barcode"></param>
        /// <param name="countryName">Spain</param>
        /// <param name="location">"Madrid, Spain"</param>
        /// <param name="retailerId"></param>
        /// <param name="voucherId"></param>
        /// <returns></returns>
        public byte[] CreateSignPdf(byte[] bufferIn, string barcode, string countryName, string location, int retailerId, int voucherId)
        {
            string pfxFileFullPath = Global.Strings.pfxFileFullPath;
            string PTFLogoFileFullPath = Global.Strings.PTFLogoFileFullPath;

            PdfManager manager = new PdfManager();

            var pdfFileName = Path.ChangeExtension(Path.GetTempFileName(), ".pdf");
            var signedPdfFileName = Path.ChangeExtension(Path.GetTempFileName(), ".pdf");

            try
            {
                using (var mem = new MemoryStream(bufferIn))
                using (var bitmap = (Bitmap)Image.FromStream(mem))
                {
                    var list = bitmap.GetAllPages(System.Drawing.Imaging.ImageFormat.Jpeg);

                    manager.CreatePdf(pdfFileName, list,
                        new PdfManager.CreationInfo()
                        {
                            Title = string.Concat("Voucher ", barcode),
                            Subject = string.Concat("Retailer ", retailerId),
                            Author = string.Concat("PTF ", countryName),
                            Creator = string.Concat("PTF ", countryName),
                        });
                }

                manager.SignPdfFile(
                    pdfFileName,
                    signedPdfFileName,
                new PdfManager.SignInfo()
                {
                    pfxFilePath = pfxFileFullPath,
                    pfxKeyPass = "",
                    docPass = null,
                    signImagePath = PTFLogoFileFullPath,
                    reasonForSigning = string.Concat("Voucher ", barcode),
                    location = location
                });

                var bufferOut = File.ReadAllBytes(signedPdfFileName);
                return bufferOut;
            }
            finally
            {
                try
                {
                    File.Delete(pdfFileName);
                    File.Delete(signedPdfFileName);
                }
                catch
                {
                }
            }
        }
    }
}
