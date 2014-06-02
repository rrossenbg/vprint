/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PremierTaxFree.PTFLib
{
    public delegate void DrawDelegate(Graphics g);

    public static class GdiEx
    {
        public static void SetBackground(this Control control, Bitmap bitmap)
        {
            /*Parametri accettati dalla procedura:
            control = il form o controllo da renderizzare
            bitmap = la PNG da utilizzare come sfondo*/

            // Imposta le dimensioni del controllo come quelle della bitmap
            control.Width = bitmap.Width;
            control.Height = bitmap.Height;

            if (bitmap.PixelFormat != PixelFormat.Format32bppArgb)
                throw new ApplicationException("L'immagine deve essere in formato a 32 bpp con canale alpha");

            IntPtr hBitmap = IntPtr.Zero;
            IntPtr oldBitmap = IntPtr.Zero;

            IntPtr screenDc = GetDC(IntPtr.Zero);
            IntPtr memDc = CreateCompatibleDC(screenDc);
            try
            {
                hBitmap = bitmap.GetHbitmap(Color.FromArgb(0));
                oldBitmap = SelectObject(memDc, hBitmap);

                size size = new size(bitmap.Width, bitmap.Height);
                point pointSource = new point(control.Left, control.Top);
                point topPos = new point(0, 0);

                BLENDFUNCTION blend = new BLENDFUNCTION();
                blend.BlendOp = 0;
                blend.BlendFlags = 0;
                blend.SourceConstantAlpha = byte.MaxValue;
                blend.AlphaFormat = 1;

                UpdateLayeredWindow(control.Handle, screenDc, ref topPos, ref size,
                    memDc, ref pointSource, 0, ref blend, 2);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                ReleaseDC(IntPtr.Zero, screenDc);

                if (hBitmap != IntPtr.Zero)
                {
                    SelectObject(memDc, oldBitmap);
                    DeleteObject(hBitmap);
                }

                DeleteDC(memDc);
            }
        }

        /// <summary>
        /// Draws to any window by drawing delegate and handler
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="drawFunct"></param>
        public static void DrawToWnd(this IntPtr hWnd, DrawDelegate drawFunct)
        {
            using (Graphics g = Graphics.FromHwnd(hWnd))
            {
                drawFunct(g);
            }
        }

        /// <summary>
        /// Draws to any window by drawing delegate and handler
        /// </summary>
        /// <param name="drawFunct"></param>
        /// <param name="hWnd"></param>
        public static void DrawToWnd(this DrawDelegate drawFunct, IntPtr hWnd)
        {
            using (Graphics g = Graphics.FromHwnd(hWnd))
            {
                drawFunct(g);
            }
        }

        /// <summary>
        /// Draws line from point to point
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="penColor"></param>
        public static void DrawLine(this Bitmap bmp, point start, point end, Color penColor)
        {
            DrawLine(bmp, start, end, penColor, 20, false);
        }

        /// <summary>
        /// Draws line
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="penColor"></param>
        /// <param name="penWidth"></param>
        /// <param name="erase"></param>
        public static void DrawLine(this Bitmap bmp, point start, point end, Color penColor, int penWidth, bool erase)
        {
            using (Graphics g = Graphics.FromImage(bmp))
            {
                IntPtr pTarget = g.GetHdc();
                IntPtr pDc = CreateCompatibleDC(pTarget);

                IntPtr hBitmap = bmp.GetHbitmap();
                IntPtr pOrig = SelectObject(pDc, hBitmap);

                IntPtr pen = CreatePen(PenStyle.PS_SOLID | PenStyle.PS_GEOMETRIC |
                    PenStyle.PS_ENDCAP_ROUND, penWidth, (uint)ColorTranslator.ToWin32(penColor));

                // select the pen into the device context
                IntPtr oldpen = SelectObject(pDc, pen);
                MoveToEx(pDc, start.x, start.y, IntPtr.Zero);
                LineTo(pDc, end.x, end.y);

                // select the old pen back
                DeleteObject(SelectObject(pDc, oldpen));
                SelectObject(pDc, pOrig);
                g.ReleaseHdc();
            }
        }

        /// <summary>
        /// Creates a snap shot of control
        /// </summary>
        /// <param name="cnt"></param>
        /// <returns></returns>
        public static Bitmap SnapShot(this Control cnt)
        {
            Bitmap bmp = new Bitmap(cnt.Width, cnt.Height);

            using (Graphics gTo = Graphics.FromImage(bmp),
                            gFrom = cnt.CreateGraphics())
            {
                IntPtr dcTo = gTo.GetHdc();
                IntPtr dcFrom = gFrom.GetHdc();
                BitBlt(dcTo, 0, 0, cnt.Width, cnt.Height, dcFrom, 0, 0, 13369376);
                gTo.ReleaseHdc(dcTo);
                //bmp.Save(@"c:\snapshot.bmp", ImageFormat.Bmp);
                return bmp;
            }
        }

        public enum Bool : int
        {
            @False = 0,
            @True = 1
        }

        public struct point
        {
            public int x;
            public int y;
            public point(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        public struct size
        {
            public int cx;
            public int cy;
            public size(int cx, int cy)
            {
                this.cx = cx;
                this.cy = cy;
            }
        }

        public struct BLENDFUNCTION
        {
            public byte BlendOp;
            public byte BlendFlags;
            public byte SourceConstantAlpha;
            public byte AlphaFormat;
        }

        private const int ULW_ALPHA = 2;
        private const byte AC_SRC_OVER = 0;
        private const byte AC_SRC_ALPHA = 1;

        [DllImport("user32.dll")]
        private extern static Bool UpdateLayeredWindow(IntPtr handle, IntPtr hdcDst, ref point pptDst,
            ref size psize, IntPtr hdcSrc, ref point pprSrc, int crKey, ref BLENDFUNCTION pblend, int dwFlags);

        [DllImport("user32.dll")]
        private extern static IntPtr GetDC(IntPtr handle);

        [DllImport("coredll.dll", EntryPoint = "GetWindowDC", SetLastError = true)]
        private static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("user32.dll", ExactSpelling = true)]
        private extern static int ReleaseDC(IntPtr handle, IntPtr hDC);

        [DllImport("gdi32.dll")]
        private extern static IntPtr CreateCompatibleDC(IntPtr hDC);

        [DllImport("gdi32.dll")]
        private extern static Bool DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        private extern static IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

        [DllImport("gdi32.dll")]
        private extern static Bool DeleteObject(IntPtr hObject);

        [DllImport("gdi32.dll")]
        private static extern bool LineTo(IntPtr hdc, int nXEnd, int nYEnd);

        public enum PenStyle : int
        {
            PS_SOLID = 0, //The pen is solid.
            PS_DASH = 1, //The pen is dashed.
            PS_DOT = 2, //The pen is dotted.
            PS_DASHDOT = 3, //The pen has alternating dashes and dots.
            PS_DASHDOTDOT = 4, //The pen has alternating dashes and double dots.
            PS_NULL = 5, //The pen is invisible.
            PS_INSIDEFRAME = 6,// Normally when the edge is drawn, it’s centred on the outer edge meaning that half the width of the pen is drawn
            // outside the shape’s edge, half is inside the shape’s edge. When PS_INSIDEFRAME is specified the edge is drawn
            //completely inside the outer edge of the shape.
            PS_USERSTYLE = 7,
            PS_ALTERNATE = 8,
            PS_STYLE_MASK = 0x0000000F,

            PS_ENDCAP_ROUND = 0x00000000,
            PS_ENDCAP_SQUARE = 0x00000100,
            PS_ENDCAP_FLAT = 0x00000200,
            PS_ENDCAP_MASK = 0x00000F00,

            PS_JOIN_ROUND = 0x00000000,
            PS_JOIN_BEVEL = 0x00001000,
            PS_JOIN_MITER = 0x00002000,
            PS_JOIN_MASK = 0x0000F000,

            PS_COSMETIC = 0x00000000,
            PS_GEOMETRIC = 0x00010000,
            PS_TYPE_MASK = 0x000F0000
        };

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreatePen(PenStyle fnPenStyle, int nWidth, uint crColor);

        [DllImport("gdi32.dll")]
        private static extern bool MoveToEx(IntPtr hdc, int X, int Y, IntPtr lpPoint);

        public enum TernaryRasterOperations : uint
        {
            /// <summary>dest = source</summary>
            SRCCOPY = 0x00CC0020,
            /// <summary>dest = source OR dest</summary>
            SRCPAINT = 0x00EE0086,
            /// <summary>dest = source AND dest</summary>
            SRCAND = 0x008800C6,
            /// <summary>dest = source XOR dest</summary>
            SRCINVERT = 0x00660046,
            /// <summary>dest = source AND (NOT dest)</summary>
            SRCERASE = 0x00440328,
            /// <summary>dest = (NOT source)</summary>
            NOTSRCCOPY = 0x00330008,
            /// <summary>dest = (NOT src) AND (NOT dest)</summary>
            NOTSRCERASE = 0x001100A6,
            /// <summary>dest = (source AND pattern)</summary>
            MERGECOPY = 0x00C000CA,
            /// <summary>dest = (NOT source) OR dest</summary>
            MERGEPAINT = 0x00BB0226,
            /// <summary>dest = pattern</summary>
            PATCOPY = 0x00F00021,
            /// <summary>dest = DPSnoo</summary>
            PATPAINT = 0x00FB0A09,
            /// <summary>dest = pattern XOR dest</summary>
            PATINVERT = 0x005A0049,
            /// <summary>dest = (NOT dest)</summary>
            DSTINVERT = 0x00550009,
            /// <summary>dest = BLACK</summary>
            BLACKNESS = 0x00000042,
            /// <summary>dest = WHITE</summary>
            WHITENESS = 0x00FF0062,
            /// <summary>
            /// Capture window as seen on screen.  This includes layered windows
            /// such as WPF windows with AllowsTransparency="true"
            /// </summary>
            CAPTUREBLT = 0x40000000
        }

        [DllImport("gdi32.dll")]
        private static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest,
            int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, TernaryRasterOperations dwRop);

        [DllImport("gdi32.dll")]
        private static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest,
            int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, uint dwRop);

        [DllImport("gdi32.dll")]
        private static extern bool FillRgn(IntPtr hDc, IntPtr hRgn, IntPtr hBrush);

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateSolidBrush(uint crColor);

        [DllImport("gdi32", EntryPoint = "CreateFontW")]
        private static extern IntPtr CreateFontW(
                    [In] Int32 nHeight,
                    [In] Int32 nWidth,
                    [In] Int32 nEscapement,
                    [In] Int32 nOrientation,
                    [In] Int32 fnWeight,
                    [In] UInt32 fdwItalic,
                    [In] UInt32 fdwUnderline,
                    [In] UInt32 fdwStrikeOut,
                    [In] UInt32 fdwCharSet,
                    [In] UInt32 fdwOutputPrecision,
                    [In] UInt32 fdwClipPrecision,
                    [In] UInt32 fdwQuality,
                    [In] UInt32 fdwPitchAndFamily,
                    [In] IntPtr lpszFace);
    }
}