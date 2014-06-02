/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using PremierTaxFree.PTFLib.Native;

namespace PremierTaxFree.Scan
{    
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public class BITMAPINFOHEADER
    {
        public int biSize;
        public int biWidth;
        public int biHeight;
        public short biPlanes;
        public short biBitCount;
        public int biCompression;
        public int biSizeImage;
        public int biXPelsPerMeter;
        public int biYPelsPerMeter;
        public int biClrUsed;
        public int biClrImportant; 
    }

    /// <summary>
    /// DIB bitmap object.
    /// Reads bitmap from driver(unmanaged memory)
    /// </summary>
    public class BmpObj : IDisposable
    {
        public BITMAPINFOHEADER Header = new BITMAPINFOHEADER();
        public IntPtr DibPtr;
        public IntPtr BmpPtr;
        public IntPtr PxPtr;
        public Rectangle Rect = new Rectangle();

        /// <summary>
        /// Free allocated memory
        /// </summary>
        public void Dispose()
        {
            if (DibPtr != IntPtr.Zero)
            {
                Kernel32.GlobalFree(DibPtr);
                DibPtr = IntPtr.Zero;
            }
        }

        /// <summary>
        /// Allocates global memory
        /// </summary>
        /// <param name="dibPtr"></param>
        public void Bind(IntPtr dibPtr)
        {
            Dispose();
            DibPtr = dibPtr;
            BmpPtr = Kernel32.GlobalLock(DibPtr);
        }
        
        /// <summary>
        /// Creates bitmap header
        /// </summary>
        public void Fill()
        {
            Debug.Assert(DibPtr != IntPtr.Zero);
            Debug.Assert(BmpPtr != IntPtr.Zero);

            Marshal.PtrToStructure(BmpPtr, Header);

            if (Header.biSizeImage == 0)
                Header.biSizeImage = ((((Header.biWidth * Header.biBitCount) + 31) & ~31) >> 3) 
                    * Header.biHeight;

            int p = Header.biClrUsed;
            if ((p == 0) && (Header.biBitCount <= 8))
                p = 1 << Header.biBitCount;

            p = (p * 4) + Header.biSize + (int)BmpPtr;

            PxPtr = (IntPtr)p;

            Rect.Width = Header.biWidth;
            Rect.Height = Header.biHeight;
        }

        /// <summary>
        /// Draws scanned image to an image object 
        /// </summary>
        /// <param name="image"></param>
        public void CopyTo(Image image)
        {
            Debug.Assert(image != null);

            using (Graphics g = Graphics.FromImage(image))
            {
                IntPtr hdc = g.GetHdc();
                Gdi32.SetDIBitsToDevice(hdc, 0, 0, image.Width, image.Height, 0, 0, 0, Rect.Height, PxPtr, BmpPtr, 0);
                g.ReleaseHdc(hdc);
            }
        }
    }
}
