using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime;
using VPrinting;

namespace MerchantSite
{
    public static class ImagesEx
    {
        [TargetedPatchingOptOut("na")]
        public static byte[] ToArray(this Image image)
        {
            using (var mem = new MemoryStream())
            {
                image.Save(mem, image.RawFormat);
                return mem.ToArray();
            }
        }

        [TargetedPatchingOptOut("na")]
        public static List<Image> TiffGetAllImages(this string tiffFilePath)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(tiffFilePath));
            Debug.Assert(File.Exists(tiffFilePath));

            var images = new List<Image>();

            using (var bitmap = (Bitmap)Image.FromFile(tiffFilePath))
            {
                int count = bitmap.GetFrameCount(FrameDimension.Page);

                for (int idx = 0; idx < count; idx++)
                {
                    bitmap.SelectActiveFrame(FrameDimension.Page, idx);

                    using (var byteStream = new MemoryStream())
                    {
                        bitmap.Save(byteStream, ImageFormat.Tiff);
                        images.Add(Image.FromStream(byteStream));
                    }
                }
            }
            return images;
        }

        public static List<string> TiffGetAllImages2(this string fullFileName, ImageFormat imageFormat, bool rotatePortrait = false)
        {
            var list = new List<string>();

            bool rotate =  false;

            using (Image image = Image.FromFile(fullFileName))
            {
                string directory = Path.GetDirectoryName(fullFileName);
                string fileName = Path.GetFileNameWithoutExtension(fullFileName);

                rotate = rotatePortrait && image.Width > image.Height;

                int pages = image.GetFrameCount(FrameDimension.Page);

                for (int index = 0; index < pages; index++)
                {
                    int activePage = index + 1;
                    image.SelectActiveFrame(FrameDimension.Page, index);
                    var path = Path.Combine(directory, string.Concat(fileName, activePage, imageFormat.GetFileExt()));
                    list.Add(path);
                    image.Save(path, imageFormat);
                }
            }
            if (rotate)
            {
                foreach (var path in list)
                {
                    using (Image image = Image.FromFile(path))
                    {
                        image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        image.Save(path, imageFormat);
                    }
                }
            }
            return list;
        }
    }
}