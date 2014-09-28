/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.Diagnostics;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace CPrint2
{
    public static class WinFormsEx
    {
        [TargetedPatchingOptOut("na")]
        public static void ShowInfo(this IWin32Window owner, string message)
        {
            ShowInfo(owner, message, Application.ProductName);
        }

        [TargetedPatchingOptOut("na")]
        public static void ShowInfo(this IWin32Window owner, string message, string caption)
        {
            MessageBox.Show(owner, message, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        [TargetedPatchingOptOut("na")]
        public static DialogResult ShowQuestion(this IWin32Window owner, string message, MessageBoxButtons btns)
        {
            return MessageBox.Show(owner, message, Application.ProductName, btns, MessageBoxIcon.Question);
        }

        [TargetedPatchingOptOut("na")]
        public static bool ShowQuestion(this IWin32Window owner, string message, MessageBoxButtons btns, DialogResult yes)
        {
            return MessageBox.Show(owner, message, Application.ProductName, btns, MessageBoxIcon.Question) == yes;
        }

        [TargetedPatchingOptOut("na")]
        public static void ShowExclamation(this IWin32Window owner, string message)
        {
            ShowExclamation(owner, message, Application.ProductName);
        }

        [TargetedPatchingOptOut("na")]
        public static void ShowExclamation(this IWin32Window owner, string message, string caption)
        {
            MessageBox.Show(owner, message, caption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        [TargetedPatchingOptOut("na")]
        public static void ShowError(this IWin32Window owner, string message)
        {
            ShowError(owner, message, Application.ProductName);
        }

        [TargetedPatchingOptOut("na")]
        public static void ShowError(this IWin32Window owner, string message, string caption)
        {
            MessageBox.Show(owner, message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        [TargetedPatchingOptOut("na")]
        public static void InvokeSf(this Control cnt, MethodInvoker del)
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

        [TargetedPatchingOptOut("na")]
        public static object InvokeSafe(this Control cnt, Delegate del, params object[] data)
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

        [TargetedPatchingOptOut("na")]
        public static void SetSelected<T>(this ComboBox combo, Func<T, bool> funct)
        {
            Debug.Assert(combo != null);
            Debug.Assert(funct != null);

            foreach (T item in combo.Items)
            {
                if (funct(item))
                {
                    combo.SelectedItem = item;
                    break;
                }
            }
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

        [TargetedPatchingOptOut("na")]
        public static void SetTopmost(this IntPtr hWnd)
        {
            SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
        }

        [TargetedPatchingOptOut("na")]
        public static void AddRow(this TableLayoutPanel panel, Control control)
        {
            int rowIndex = AddTableRow(panel);
            panel.Controls.Add(control, 0, rowIndex);
        }

        [TargetedPatchingOptOut("na")]
        public static void ResizeRows(this TableLayoutPanel panel)
        {
            Debug.Assert(panel != null);

            foreach (RowStyle row in panel.RowStyles)
            {
                row.SizeType = SizeType.Absolute;
                row.Height = panel.Height / panel.RowStyles.Count;
            }
        }

        public static int AddTableRow(TableLayoutPanel panel)
        {
            int index = panel.RowCount++;
            RowStyle style = new RowStyle(SizeType.AutoSize);
            panel.RowStyles.Add(style);
            return index;
        }
    }
}
