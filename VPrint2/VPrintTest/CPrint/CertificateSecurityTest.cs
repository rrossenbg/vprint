using System.Drawing;
using System.Security.Cryptography.X509Certificates;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VPrint.Common;
using VPrinting;

namespace VPrintTest.CPrint
{
    [TestClass]
    public class CertificateSecurityTest
    {
        private string FILENAME = "C:\\img-130628084640-0001 - Copy.jpg";

        [TestMethod]
        public void test_find_certificate()
        {
            CertificateSecurity sec = new CertificateSecurity(X509FindType.FindBySerialNumber, Strings.CERTNUMBER, StoreLocation.LocalMachine);
            Assert.IsNotNull(sec.Loaded);
        }

        [TestMethod]
        public void test_image_encription()
        {
            var img = (Bitmap)Bitmap.FromFile(FILENAME);
            CertificateSecurity sec = new CertificateSecurity(X509FindType.FindBySerialNumber, Strings.CERTNUMBER, StoreLocation.LocalMachine);
            var data = sec.SignData(img.ToArray());
            Assert.IsNotNull(data);
        }
    }
}
