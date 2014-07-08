/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Drawing;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace VPrinting.Extentions
{
    public static class FormsEx
    {
        [TargetedPatchingOptOut("na")]
        public static void InvokeSf(this Control cnt, MethodInvoker del)
        {
            try
            {
                if (cnt == null)
                    return;
                if (cnt.IsDisposed || !cnt.IsHandleCreated)
                    return;
                if (cnt.InvokeRequired)
                    cnt.Invoke(del);
                else
                    del();
            }
            catch
            {
            }
        }

        [TargetedPatchingOptOut("na")]
        public static object InvokeSafe(this Control cnt, Delegate del, params object[] data)
        {
            try
            {
                if (cnt == null)
                    return null;
                if (cnt.IsDisposed || !cnt.IsHandleCreated)
                    return null;
                if (cnt.InvokeRequired)
                    return cnt.Invoke(del, data);
                else
                    return del.DynamicInvoke(data);
            }
            catch
            {
                return null;
            }
        }

        [TargetedPatchingOptOut("na")]
        public static Bitmap TakeSnapshot(this Control ctl)
        {
            Bitmap bmp = new Bitmap(ctl.Size.Width, ctl.Size.Height);
            using (Graphics g = System.Drawing.Graphics.FromImage(bmp))
                g.CopyFromScreen(ctl.PointToScreen(ctl.ClientRectangle.Location), new Point(0, 0), ctl.ClientRectangle.Size);
            return bmp;
        }

        [TargetedPatchingOptOut("na")]
        public static bool IsDefault(this DateTimePicker dt, DateTime @default)
        {
            return dt.Value == @default;
        }

        static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        static readonly IntPtr HWND_TOP = new IntPtr(0);
        static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
        const UInt32 SWP_NOSIZE = 0x0001;
        const UInt32 SWP_NOMOVE = 0x0002;
        const UInt32 TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [TargetedPatchingOptOut("na")]
        public static void SetTopmost(this Form form)
        {
            SetWindowPos(form.Handle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
        }
    }
}
