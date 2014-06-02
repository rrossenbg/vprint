using Microsoft.VisualStudio.TestTools.UnitTesting;
using VPrinting;

namespace VPrintTest
{
    [TestClass]
    public class PostScriptTest
    {
        [TestMethod]
        public void postscript_print_file_test()
        {
            string fileName = @"C:\Users\Rosen.rusev\Desktop\PostScript with samples\CODE\PROG_01.PS";
            RawPrinterHelper.SendFileTcp(fileName, "192.168.44.158", "test", "rosen", false);
        }
    }
}
