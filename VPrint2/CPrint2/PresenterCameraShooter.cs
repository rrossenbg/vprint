/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace CPrint2
{
    public class PresenterCameraShooter
    {
        public void TryStartPresenter(string path)
        {
            if (!File.Exists(path))
                throw new ApplicationException("Can not find Presenter executable.\r\nPlease make sure it's installed and path is correct.");

            IntPtr hWnd = FindWindow(null, "IPEVO Presenter");
            if (hWnd == IntPtr.Zero)
            {
                Process.Start(new ProcessStartInfo(path));
                Thread.Sleep(500);
                hWnd = FindWindow(null, "IPEVO Presenter");
            }
            SetWindowPos(hWnd, IntPtr.Zero, -1000, -1000, 100, 100, 0);
        }

        public void TryStopPresenter()
        {
            var ps = Process.GetProcessesByName("Presenter");
            if (ps.Length > 0)
            {
                foreach (var p1 in ps)
                    p1.Kill();
            }
        }

        public void ClickCameraButton()
        {
            IntPtr hWnd = FindWindow(null, "IPEVO Presenter");
            SetWindowPos(hWnd, IntPtr.Zero, -1000, -1000, 100, 100, 0);

            RECT r = new RECT()
            {
                Bottom = -452,
                Left = -486,
                Right = -320,
                Top = -484
            };

            IntPtr btnhWnd = GetChildWindowHandle("", hWnd, r);
            SendMessage(btnhWnd, WM_LBUTTONDOWN, IntPtr.Zero, IntPtr.Zero);
            SendMessage(btnhWnd, WM_LBUTTONUP, IntPtr.Zero, IntPtr.Zero);
        }

        private IntPtr GetChildWindowHandle(string windowText, IntPtr parentHandle, RECT rect)
        {
            var searchData = new SearchData { ParentHandle = parentHandle, WindowText = windowText, Rect = rect };

            EnumWindows(Callback, ref searchData);

            return searchData.ResultHandle;
        }

        private static bool Callback(IntPtr currentWindowHandle, ref SearchData searchData)
        {
            IntPtr currentWindowParentHandle = GetParent(currentWindowHandle);

            if (currentWindowParentHandle == searchData.ParentHandle)
            {
                var windowText = new StringBuilder(1024);

                GetWindowText(currentWindowHandle, windowText, windowText.Capacity);

                if (windowText.ToString() == searchData.WindowText)
                {
                    RECT lpRect;
                    if (GetWindowRect(currentWindowHandle, out lpRect) && lpRect == searchData.Rect)
                    {
                        searchData.ResultHandle = currentWindowHandle;
                        return false;
                    }
                }
            }

            return true;
        }

        #region WINDOWS API

        public const int BM_CLICK = 0x00F5;
 
        public const int WM_LBUTTONDOWN = 0x0201; 
        public const int WM_LBUTTONUP = 0x0202;

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(String sClassName, String sAppName);

        struct SearchData
        {
            public string WindowText;
            public IntPtr ParentHandle;
            public IntPtr ResultHandle;
            public RECT Rect;
        }

        delegate bool EnumWindowsCallback(IntPtr currentWindowHandle, ref SearchData searchData);

        [DllImport("user32.dll")]
        static extern bool EnumWindows(EnumWindowsCallback callback, ref SearchData searchData);

        [DllImport("user32.dll")]
        static extern IntPtr GetParent(IntPtr childHandle);

        [DllImport("user32.dll")]
        static extern void GetWindowText(IntPtr handle, StringBuilder resultWindowText, int maxTextCapacity);
        
        /*************************************************************************************/

        [DllImport("user32.dll")]
        public static extern int SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner

            public override string ToString()
            {
                return string.Format("Left {0} Top {1} Right {2} Bottom {3}", Left, Top, Right, Bottom);
            }

            public static bool operator ==(RECT r1, RECT r2)
            {
                return r1.Left == r2.Left && r1.Top == r2.Top && r1.Right == r2.Right && r1.Bottom == r2.Bottom;
            }

            public static bool operator !=(RECT r1, RECT r2)
            {
                return r1.Left != r2.Left || r1.Top != r2.Top || r1.Right != r2.Right || r1.Bottom != r2.Bottom;
            }
        }

        /*************************************************************************************/

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

        //This is a replacement for Cursor.Position in WinForms
        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        public const int MOUSEEVENTF_LEFTDOWN = 0x02;
        public const int MOUSEEVENTF_LEFTUP = 0x04;

        //This simulates a left mouse click
        public static void LeftMouseClick(int xpos, int ypos)
        {
            SetCursorPos(xpos, ypos);
            mouse_event(MOUSEEVENTF_LEFTDOWN, xpos, ypos, 0, 0);
            Thread.Sleep(300);
            mouse_event(MOUSEEVENTF_LEFTUP, xpos, ypos, 0, 0);
        }

        /*************************************************************************************/

        public delegate bool Win32Callback(IntPtr hwnd, IntPtr lParam);

        [DllImport("user32.Dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumChildWindows(IntPtr parentHandle, Win32Callback callback, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static public extern IntPtr GetClassName(IntPtr hWnd, System.Text.StringBuilder lpClassName, int nMaxCount);

        private static bool EnumWindow(IntPtr handle, IntPtr pointer)
        {
            GCHandle gch = GCHandle.FromIntPtr(pointer);
            List<IntPtr> list = gch.Target as List<IntPtr>;
            if (list == null)
                throw new InvalidCastException("GCHandle Target could not be cast as List<IntPtr>");
            list.Add(handle);
            return true;
        }

        private static List<IntPtr> GetChildWindows(IntPtr parent)
        {
            List<IntPtr> result = new List<IntPtr>();
            GCHandle listHandle = GCHandle.Alloc(result);
            try
            {
                Win32Callback childProc = new Win32Callback(EnumWindow);
                EnumChildWindows(parent, childProc, GCHandle.ToIntPtr(listHandle));
            }
            finally
            {
                if (listHandle.IsAllocated)
                    listHandle.Free();
            }
            return result;
        }

        private static string GetWinClass(IntPtr hwnd)
        {
            if (hwnd == IntPtr.Zero)
                return null;
            StringBuilder classname = new StringBuilder(100);
            IntPtr result = GetClassName(hwnd, classname, classname.Capacity);
            if (result != IntPtr.Zero)
                return classname.ToString();
            return null;
        }

        private static IEnumerable<IntPtr> EnumAllVisibleWindows(IntPtr hwnd, string childClassName)
        {
            List<IntPtr> children = GetChildWindows(hwnd);
            if (children == null)
                yield break;
            foreach (IntPtr child in children)
            {
                if (GetWinClass(child) == childClassName && IsWindowVisible(child))
                    yield return child;
                foreach (var childchild in EnumAllVisibleWindows(child, childClassName))
                    yield return childchild;
            }
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsWindowVisible(IntPtr hWnd);

        #endregion
    }
}
