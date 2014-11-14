using Microsoft.VisualStudio.TestTools.UnitTesting;
using VPrinting.Communication;

namespace ReceivingServiceTest
{
    [TestClass]
    public class FTPTest
    {
        [TestMethod]
        public void ftp_test()
        {
            FtpClient ftp = new FtpClient("ftp://192.168.53.143", @"fintrax\trsadmin", "BtServ1ce");

            var files = ftp.directoryListSimple("250");

            foreach (var file in files)
                if (!string.IsNullOrWhiteSpace(file))
                    ftp.delete("250/" + file);

            ftp.upload("250/test.pdf", "C:\\OWASP Top 10 - 2013.pdf");
        }
    }
}
