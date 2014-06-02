/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using PremierTaxFree.PTFLib.Native;

namespace PremierTaxFree
{
    public static class ImageHelper
    {
        public struct SIZE
        {
            public int cx, cy;
        }

        /// <summary>
        /// Captures desktop
        /// </summary>
        /// <returns></returns>
        public static Bitmap GetDesktopImage()
        {
            IntPtr ptr = user32.GetDesktopWindow();
            return GetImage(ptr);
        }

        /// <summary>
        /// Creates bitmap by image pointer
        /// </summary>
        /// <param name="ptr"></param>
        /// <returns></returns>
        public static Bitmap GetImage(IntPtr ptr)
        {
            SIZE size;

            IntPtr hBitmap;

            IntPtr hDC = user32.GetDC(ptr);

            IntPtr hMemDC = Gdi32.CreateCompatibleDC(hDC);

            size.cx = user32.GetSystemMetrics(user32.SM_CXSCREEN);

            size.cy = user32.GetSystemMetrics(user32.SM_CYSCREEN);

            hBitmap = Gdi32.CreateCompatibleBitmap(hDC, size.cx, size.cy);

            if (hBitmap != IntPtr.Zero)
            {
                IntPtr hOld = (IntPtr)Gdi32.SelectObject(hMemDC, hBitmap);

                Gdi32.BitBlt(hMemDC, 0, 0, size.cx, size.cy, hDC, 0, 0, Gdi32.SRCCOPY);

                Gdi32.SelectObject(hMemDC, hOld);

                Gdi32.DeleteDC(hMemDC);

                user32.ReleaseDC(ptr, hDC);

                Bitmap bmp = Image.FromHbitmap(hBitmap);

                Gdi32.DeleteObject(hBitmap);

                GC.Collect();

                return bmp;
            }

            return null;
        }

        /// <summary>
        /// Offsets system cursor
        /// </summary>
        /// <param name="cursor"></param>
        /// <param name="offset"></param>
        public static void SetOffset(this Cursor cursor, Point offset)
        {
            Point p = Control.MousePosition;
            p.Offset(offset);
            Cursor.Position = p;
        }

        /// <summary>
        /// C# Converting 32bpp image to 8bpp
        /// </summary>
        /// <param name="oldbmp"></param>
        /// <returns></returns>
        /// <see cref="http://stackoverflow.com/questions/6355135/c-converting-32bpp-image-to-8bpp"/>
        public static Image Convert(Bitmap oldbmp)
        {
            using (var ms = new MemoryStream())
            {
                oldbmp.Save(ms, ImageFormat.Gif);
                ms.Position = 0;
                return Image.FromStream(ms);
            }
        }
    }
}
