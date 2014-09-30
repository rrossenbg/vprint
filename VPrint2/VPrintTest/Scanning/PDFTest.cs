using Microsoft.VisualStudio.TestTools.UnitTesting;
using VPrint.Common.Pdf;

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
    }
}
