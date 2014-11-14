using Microsoft.VisualStudio.TestTools.UnitTesting;
using VPrint.Common.Pdf;
using VPrinting;
using VPrinting.Common;
using System.IO;

namespace VPrintTest.Scanning
{
    [TestClass]
    public class PDFTest
    {
        [TestMethod]
        public void compressPDF_test()
        {
            PdfAManager man = new PdfAManager();
            man.CompressPdf("C:\\tmp64C66c81c722_ca06_46b4_ac94_97b958110a5a.pdf", "C:\\result1.pdf"); 
        }

        [TestMethod]
        public void drawPDF_test()
        {
            new FileInfo(@"C:\IMAGES\PORTUGAL\New folder\OA_24993.pdf").DrawToImage(96);
        }

        [TestMethod]
        public void draw_parse_sitecode_test()
        {
            string code;
            int location;
            CommonTools.ParseSiteCode("OA_24464.pdf", out code, out location);
            CommonTools.ParseSiteCode("OA_24464.0.pdf", out code, out location);
        }
    }
}
