using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime;
using System;
using System.Text;

namespace FintraxPTFImages
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
        public static byte[] ToArray(this Image image, ImageFormat format, long compression = 50L)
        {
            Debug.Assert(image != null);

            using (var mem = new MemoryStream())
            {
                ImageCodecInfo enc = format.GetEncoder();

                using (var encoderParameters = new EncoderParameters(1))
                using (var parameter1 = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, compression))
                {
                    encoderParameters.Param[0] = parameter1;
                    image.Save(mem, enc, encoderParameters);
                }

                return mem.ToArray();
            }
        }

        [TargetedPatchingOptOut("na")]
        public static ImageCodecInfo GetEncoder(this ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
                if (codec.FormatID == format.Guid)
                    return codec;
            return null;
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

        public static List<string> TiffGetAllImages2(this string fullFileName, ImageFormat imageFormat)
        {
            List<string> list = new List<string>();

            using (Image image = Image.FromFile(fullFileName))
            {
                int activePage;

                string directory = Path.GetDirectoryName(fullFileName);
                string fileName = Path.GetFileNameWithoutExtension(fullFileName);

                int pages = image.GetFrameCount(FrameDimension.Page);

                for (int index = 0; index < pages; index++)
                {
                    activePage = index + 1;

                    image.SelectActiveFrame(FrameDimension.Page, index);

                    var path = Path.Combine(directory, string.Concat(fileName, activePage, imageFormat.GetFileExt()));

                    list.Add(path);

                    image.Save(path, imageFormat);
                }
            }
            return list;
        }

        public static string GetFileExt(this ImageFormat format)
        {
            if (format == null)
                throw new ArgumentNullException("format");

            if (format.Guid == ImageFormat.Jpeg.Guid)
                return ".jpg";
            else if (format.Guid == ImageFormat.Bmp.Guid)
                return ".bmp";
            else if (format.Guid == ImageFormat.Tiff.Guid)
                return ".tif";
            else if (format.Guid == ImageFormat.Png.Guid)
                return ".png";
            else if (format.Guid == ImageFormat.Emf.Guid)
                return ".emf";
            else if (format.Guid == ImageFormat.Exif.Guid)
                return ".exif";
            else if (format.Guid == ImageFormat.Gif.Guid)
                return ".gif";
            else if (format.Guid == ImageFormat.Icon.Guid)
                return ".ico";
            else if (format.Guid == ImageFormat.Wmf.Guid)
                return ".wmf";
            throw new NotImplementedException();
        }
    }
}