using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using VPrinting;
using System.Drawing.Imaging;
using VPrint.Common;

namespace VPrintTest.Scanning
{
    [TestClass]
    public class DrawingTest
    {
        [TestMethod]
        public void Pixellate_Test()
        {
            using (Bitmap bmp = (Bitmap)Bitmap.FromFile("C:\\Images\\img-130628084640-0001 - Copy (4) - Copy20be36a8_7dae_4e34_8005_7349d5c955eb.jpg"))
            {
                using (LockBitmap lockBmp = new LockBitmap(bmp))
                    lockBmp.Pixellate(Rectangle.FromLTRB(200, 500, 1000, 700), 5);
                bmp.Save("C:\\Images\\TEST2.jpg", ImageFormat.Jpeg);
            }
        }
    }
}
