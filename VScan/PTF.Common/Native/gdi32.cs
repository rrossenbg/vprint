/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Runtime.InteropServices;

namespace PremierTaxFree.PTFLib.Native
{
    public static class Gdi32
	{
		public  const int SRCCOPY = 13369376;

		[DllImport("gdi32.dll",EntryPoint="DeleteDC")]
		public static extern IntPtr DeleteDC(IntPtr hDc);

		[DllImport("gdi32.dll",EntryPoint="DeleteObject")]
		public static extern IntPtr DeleteObject(IntPtr hDc);

		[DllImport("gdi32.dll",EntryPoint="BitBlt")]
		public static extern bool BitBlt(IntPtr hdcDest,int xDest,int yDest,int wDest,int hDest,IntPtr hdcSource,int xSrc,int ySrc,int RasterOp);

		[DllImport ("gdi32.dll",EntryPoint="CreateCompatibleBitmap")]
		public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc,	int nWidth, int nHeight);

		[DllImport ("gdi32.dll",EntryPoint="CreateCompatibleDC")]
		public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

		[DllImport ("gdi32.dll",EntryPoint="SelectObject")]
		public static extern IntPtr SelectObject(IntPtr hdc,IntPtr bmp);

        [DllImport("gdi32.dll", ExactSpelling = true)]
        public static extern int SetDIBitsToDevice(IntPtr hdc, 
                    int xDst, int yDst, int widthDst, int heightDst, 
                    int xSrc, int ySrc, int start, int lines,
                    IntPtr bitsPtr, IntPtr bmiPtr, int color);
	}
}
