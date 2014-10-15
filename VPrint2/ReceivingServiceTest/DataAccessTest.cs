using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReceivingServiceLib.Data;
using VPrinting;

namespace ReceivingServiceTest
{
    [TestClass]
    public class DataAccessTest
    {
        [TestMethod]
        public void test_addvoucher()
        {
            var zipInfo = new FileInfo("C:\\TEST2.zip");
            var binInfo = new FileInfo("C:\\TEST2.bin");
            zipInfo.EncriptFile(binInfo);
            var test2 = File.ReadAllBytes(binInfo.FullName);
            VoucherDataAccess.Instance.AddVoucher(0, 250, 12345, 123456789, 1, "AA12345", "122344567890", 0, 0, test2, test2.Length, "122343556", true, 2);
        }
    }
}
