/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace PremiumTaxFree.Scan
{
    public class Gdip
    {
        private static ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

        private static bool GetCodecClsid(string filename, out Guid clsid)
        {
            clsid = Guid.Empty;
            string ext = Path.GetExtension(filename);
            if (ext == null)
                return false;
            ext = "*" + ext.ToUpper();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FilenameExtension.IndexOf(ext) >= 0)
                {
                    clsid = codec.Clsid;
                    return true;
                }
            }
            return false;
        }

        public static void SaveDIBAs(string picname, IntPtr bmpInfo, IntPtr pixDat)
        {
            Guid clsid;
            if (!GetCodecClsid(picname, out clsid))
                throw new Exception("Unknown picture format for extension " + Path.GetExtension(picname));

            IntPtr imgPtr = IntPtr.Zero;
            int st = GdipCreateBitmapFromGdiDib(bmpInfo, pixDat, ref imgPtr);
            if ((st != 0) || (imgPtr == IntPtr.Zero))
                throw new Exception("Gdi can not create bitmap. Code:" + st);

            st = GdipSaveImageToFile(imgPtr, picname, ref clsid, IntPtr.Zero);
            GdipDisposeImage(imgPtr);
            if (st != 0)
                throw new Exception("Gdi can not save the bitmap. Code:" + st);
        }

        [DllImport("gdiplus.dll", ExactSpelling = true)]
        private static extern int GdipCreateBitmapFromGdiDib(IntPtr bminfo, IntPtr pixdat, ref IntPtr image);

        [DllImport("gdiplus.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern int GdipSaveImageToFile(IntPtr image, string fileName, [In] ref Guid clsid, IntPtr encparams);

        [DllImport("gdiplus.dll", ExactSpelling = true)]
        private static extern int GdipDisposeImage(IntPtr image);
    }
}
