/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Reflection;
using System.Runtime;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using VPrinting.Tools;

namespace VPrinting
{
    [Obfuscation(StripAfterObfuscation = true, ApplyToMembers = true)]
    public static class DrawingEx
    {
        [TargetedPatchingOptOut("na")]
        public static Rectangle ToRect(this Point point, Size size)
        {
            return new Rectangle(point, size);
        }

        [TargetedPatchingOptOut("na")]
        public static Point[] ToPointArray(this Rectangle r, Size s)
        {
            return new Point[] { new Point(r.Left, r.Top), new Point(r.Right - s.Width, r.Top), 
                new Point(r.Right - s.Width, r.Bottom - s.Height), new Point(r.Left, r.Bottom - s.Height) };
        }

        [TargetedPatchingOptOut("na")]
        public static IEnumerable<Rectangle> ToRectArray(this Rectangle r, Size size)
        {
            foreach (var p in r.ToPointArray(size))
                yield return p.ToRect(size);
        }

        [TargetedPatchingOptOut("na")]
        public static int HResolution(this Image image)
        {
            return (int)(image.Width / image.HorizontalResolution);
        }

        [TargetedPatchingOptOut("na")]
        public static int VResolution(this Image image)
        {
            return (int)(image.Height / image.VerticalResolution);
        }

        [TargetedPatchingOptOut("na")]
        public static PaperSize ToPaperSize(this Image image, string name)
        {
            if (image == null)
                return new PaperSize("A4", 1654, 2339);
            return new PaperSize(name, image.Width, image.Height);
        }

        [TargetedPatchingOptOut("na")]
        public static void DrawStringEx(this Graphics g, string text, Font font, float x, float y)
        {
            g.DrawString(text, font, Brushes.Black, x, y);
        }

        [TargetedPatchingOptOut("na")]
        public static string Serialize(this Font font)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(FontWrapper));
            using (var memory = new MemoryStream())
            {
                formatter.Serialize(memory, new FontWrapper(font));
                string str = UTF8Encoding.Default.GetString(memory.ToArray());
                return str;
            }
        }

        [TargetedPatchingOptOut("na")]
        public static byte[] ToArray(this Image image, long compression = 50L)
        {
            Debug.Assert(image != null);

            ImageCodecInfo jgpEncoder = ImageFormat.Jpeg.GetEncoder();

            using (MemoryStream mem = new MemoryStream())
            using (var encoderParameters = new EncoderParameters(1))
            using (var parameter1 = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, compression))
            {
                encoderParameters.Param[0] = parameter1;
                image.Save(mem, jgpEncoder, encoderParameters);
                return mem.ToArray();
            }
        }

        [TargetedPatchingOptOut("na")]
        public static Font ToFont(this string text)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(FontWrapper));
            using (var str = XmlReader.Create(new StringReader(text)))
            {
                Debug.Assert(str != null);
                FontWrapper font = ((FontWrapper)formatter.Deserialize(str));
                Debug.Assert(font != null);
                return font.Value;
            }
        }

        /// <summary>
        /// Gets encoder by image format
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
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

        [TargetedPatchingOptOut("na")]
        public static int Ans(this Color c)
        {
            return (Convert.ToInt16(c.R) + Convert.ToInt16(c.G) + Convert.ToInt16(c.B)) / 3;
        }

        [TargetedPatchingOptOut("na")]
        public static Point Invert(this Point point)
        {
            return new Point(-point.X, -point.Y);
        }        
    }
}
