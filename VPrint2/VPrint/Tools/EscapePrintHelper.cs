/***************************************************
//  Copyright (c) Premium Tax Free 2012
/***************************************************/

using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using System.Text;

namespace VPrinting.Tools
{
    public class EscapeEventArgs : EventArgs
    {
        private EscapePrintDocument m_doc;
        public EscapePrintDocument Document
        {
            get
            {
                return m_doc;
            }
        }
        private EscapeEventArgs()
        {
        }
        public EscapeEventArgs(EscapePrintDocument doc)
        {
            m_doc = doc;
        }
    }

    public delegate void EscapeEventHandler(object sender, EscapeEventArgs e);

    public class EscapePrintDocument : PrintDocument
    {
        public event EscapeEventHandler AfterPrint;
        private EscapePrintHelper m_CommandSender;
        private bool m_printing;

        /// <summary>
        /// //"\x1b[COMMAND]NUMMER YourFaxNumber\x1b[END COMMAND]");
        /// </summary>
        /// <param name="passThroughCommand"></param>
        public void SendPassThroughCommand(string passThroughCommand)
        {
            if (m_printing)
            {
                m_CommandSender.SendPassThrough(passThroughCommand);
            }
            else
            {
                m_CommandSender.SendPassThroughExt(passThroughCommand);
            }
        }

        protected override void OnPrintPage(PrintPageEventArgs e)
        {
            m_printing = true;
            m_CommandSender = new EscapePrintHelper(e.Graphics);
            try
            {
                base.OnPrintPage(e);
            }
            finally
            {
                m_printing = false;
            }
        }

        protected virtual void onAfterPrint()
        {
            if (AfterPrint != null)
                AfterPrint(this, new EscapeEventArgs(this));
        }

        protected override void OnEndPrint(PrintEventArgs e)
        {
            try
            {
                onAfterPrint();
            }
            finally
            {
                base.OnEndPrint(e);
            }
        }
    }

    public class EscapePrintHelper
    {
        [DllImport("gdi32.dll", EntryPoint = "Escape", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int Escape(IntPtr Hdc, int nEscape, int ncount, IntPtr inData, IntPtr outData);

        private const int PASSTHROUGH = 19;
        private IntPtr m_Handle;
        private Graphics m_graphics;

        public EscapePrintHelper(Graphics g)
        {
            m_graphics = g;
            m_Handle = g.GetHdc();
            g.ReleaseHdc(m_Handle);
        }

        public bool SendPassThroughExt(string passthroughData)
        {
            return SendPassThroughImplementation(passthroughData, m_Handle);
        }

        private bool SendPassThroughImplementation(string passthroughdata, IntPtr handle)
        {
            IntPtr grp = handle;
            bool ok = false;
            IntPtr pData = String2HGlobal(passthroughdata, true);
            try
            {
                int id = Escape(grp, PASSTHROUGH, 0, pData, IntPtr.Zero);
                ok = id > 0;
            }
            finally
            {
                Marshal.FreeHGlobal(pData);
            }
            return ok;
        }

        public bool SendPassThrough(string passthroughData)
        {
            bool ok = false;
            IntPtr grp = m_graphics.GetHdc();
            try
            {
                ok = SendPassThroughImplementation(passthroughData, grp);
            }
            finally
            {
                m_graphics.ReleaseHdc(grp);
            }
            return ok;
        }

        private IntPtr String2HGlobal(string data, bool includeSize)
        {
            int length = data.Length;
            int offset = 0;
            if (includeSize)
            {
                length += 2;
                offset = 2;
            }
            IntPtr retVal = Marshal.AllocHGlobal(length);
            short value = (short)data.Length;
            byte[] buffer = new byte[length];
            if (includeSize)
            {
                buffer[1] = (byte)(value >> 8);
                buffer[0] = (byte)value;
            }
            Encoding.Default.GetBytes(data, 0, data.Length, buffer, offset);
            for (int i = 0; i < buffer.Length; i++)
                Marshal.WriteByte(retVal, i, buffer[i]);
            return retVal;
        }
    }
}
