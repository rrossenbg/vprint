using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace VPrinting.Common
{
    public unsafe class RawBitmap : IDisposable
    {
        private Bitmap m_originBitmap;
        private BitmapData m_bitmapData;
        private byte* m_begin;

        public RawBitmap(Bitmap originBitmap)
        {
            m_originBitmap = originBitmap;
            m_bitmapData = m_originBitmap.LockBits(new Rectangle(0, 0, m_originBitmap.Width, m_originBitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            m_begin = (byte*)(void*)m_bitmapData.Scan0;
        }

        public void Dispose()
        {
            m_originBitmap.UnlockBits(m_bitmapData);
        }

        public unsafe byte* Begin
        {
            get { return m_begin; }
        }

        public unsafe byte* this[int x, int y]
        {
            get
            {
                return m_begin + y * (m_bitmapData.Stride) + x * 3;
            }
        }

        public unsafe byte* this[int x, int y, int offset]
        {
            get
            {
                return m_begin + y * (m_bitmapData.Stride) + x * 3 + offset;
            }
        }

        public unsafe void SetColor(int x, int y, int color)
        {
            *(int*)(m_begin + y * (m_bitmapData.Stride) + x * 3) = color;
        }

        public int Stride
        {
            get { return m_bitmapData.Stride; }
        }

        public int Width
        {
            get { return m_bitmapData.Width; }
        }

        public int Height
        {
            get { return m_bitmapData.Height; }
        }

        public int GetOffset()
        {
            return m_bitmapData.Stride - m_bitmapData.Width * 3;
        }

        public Bitmap OriginBitmap
        {
            get { return m_originBitmap; }
        }
    }
}
