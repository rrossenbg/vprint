/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace CPrint2
{
    public class CameraShooter
    {
        const string IPEVO_PresenterProcess = "Presenter";
        const string IPEVO_PresenterCaption = "IPEVO Presenter";

        public void TryStartPresenter(string path)
        {
            if (!File.Exists(path))
                throw new ApplicationException("Can not find Presenter executable.\r\nPlease make sure it's installed and path is correct.");

            IntPtr hWnd = FindWindow(null, IPEVO_PresenterCaption);
            if (hWnd == IntPtr.Zero)
            {
                Process.Start(new ProcessStartInfo(path));
                Thread.Sleep(500);
                hWnd = FindWindow(null, IPEVO_PresenterCaption);
            }
            SetWindowPos(hWnd, IntPtr.Zero, -1000, -1000, 100, 100, 0);
        }

        public void TryStopPresenter()
        {
            var ps = Process.GetProcessesByName(IPEVO_PresenterProcess);
            if (ps.Length > 0)
            {
                foreach (var p1 in ps)
                    p1.Kill();
            }
        }

        public void ClickPresenterShootButton()
        {
            IntPtr hWnd = FindWindow(null, IPEVO_PresenterCaption);
            SetWindowPos(hWnd, IntPtr.Zero, -1000, -1000, 100, 100, 0);

            RECT r = new RECT()
            {
                Bottom = -452,
                Left = -486,
                Right = -320,
                Top = -484
            };

            IntPtr btnhWnd = GetChildWindowHandleTextAndRect("", hWnd, r);
            SendMessage(btnhWnd, WM_LBUTTONDOWN, IntPtr.Zero, IntPtr.Zero);
            SendMessage(btnhWnd, WM_LBUTTONUP, IntPtr.Zero, IntPtr.Zero);
        }

        const string LogitechProcess = "Launcher_Main";
        const string LogitechCaption = "Logitech® Webcam Software";
        const string LogitechSettingsCaption = "Logitech Webcam Software Settings";

        public void TryStartLogitech(string path)
        {
            if (!File.Exists(path))
                throw new ApplicationException("Can not find Logitech executable.\r\nPlease make sure it's installed and path is correct.");

            IntPtr hWnd = FindWindow(null, LogitechCaption);
            if (hWnd == IntPtr.Zero)
            {
                Process.Start(new ProcessStartInfo(path));
                Thread.Sleep(500);
                hWnd = FindWindow(null, LogitechCaption);
            }
            SetWindowPos(hWnd, IntPtr.Zero, -1000, -1000, 100, 100, 0);
        }

        public void TryStopLogitech()
        {
            var ps = Process.GetProcessesByName(LogitechProcess);
            if (ps.Length > 0)
            {
                foreach (var p1 in ps)
                    p1.Kill();
            }
        }

        //http://stackoverflow.com/questions/12015200/clipcursor-succeeds-but-effectively-does-nothing
        //http://support.microsoft.com/kb/318804
        public void ClickLogitechShootButton()
        {
            IntPtr hWnd = FindWindow(null, LogitechCaption);

            //hWnd.MoveWinPos(10000, 10000);

            IntPtr pushButtonTakePicture, controlAreaWidget, PicVidWidgetClass;

            PicVidWidgetClass = FindFirstVisibleChildByClassAndSize(hWnd, "QWidget", new Size(912, 629));//controlAreaWidget
            if (PicVidWidgetClass.IsZero())
                throw new ApplicationException("PicVidWidgetClass is ZERO");

            controlAreaWidget = FindFirstVisibleChildByClassAndSize(PicVidWidgetClass, "QWidget", new Size(640, 65));//controlAreaWidget
            if (controlAreaWidget.IsZero())
                throw new ApplicationException("controlAreaWidget is ZERO");

            pushButtonTakePicture = FindFirstVisibleChildByClassAndSize(controlAreaWidget, "QWidget", new Size(80, 42), 1);//pushButtonTakePicture
            if (pushButtonTakePicture.IsZero())
                throw new ApplicationException("pushButtonTakePicture is ZERO");

            LeftMouseClick(pushButtonTakePicture);
        }        

        private IntPtr GetChildWindowHandleTextAndRect(string windowText, IntPtr parentHandle, RECT rect)
        {
            var searchData = new SearchData { ParentHandle = parentHandle, Text = windowText, Rect = rect };

            EnumWindows(CallbackTextAndRect, ref searchData);

            return searchData.ResultHandle;
        }

        private IntPtr GetChildWindowHandleClassAndRect(string className, IntPtr parentHandle, RECT rect)
        {
            var searchData = new SearchData { ParentHandle = parentHandle, Text = className, Rect = rect };

            EnumWindows(CallbackClassAndRect, ref searchData);

            return searchData.ResultHandle;
        }

        private IntPtr GetChildWindowHandleRectSize(IntPtr parentHandle, Size size)
        {
            var searchData = new SearchData { ParentHandle = parentHandle, Size = size };

            EnumWindows(CallbackRectSize, ref searchData);

            return searchData.ResultHandle;
        }

        private static bool CallbackTextAndRect(IntPtr currentWindowHandle, ref SearchData searchData)
        {
            IntPtr currentWindowParentHandle = GetParent(currentWindowHandle);

            if (currentWindowParentHandle == searchData.ParentHandle)
            {
                var text = new StringBuilder(1024);

                GetWindowText(currentWindowHandle, text, text.Capacity);

                if (text.ToString() == searchData.Text)
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

        private static bool CallbackClassAndRect(IntPtr currentWindowHandle, ref SearchData searchData)
        {
            IntPtr currentWindowParentHandle = GetParent(currentWindowHandle);

            if (currentWindowParentHandle == searchData.ParentHandle)
            {
                var text = new StringBuilder(1024);

                GetClassName(currentWindowHandle, text, text.Capacity);

                if (text.ToString() == searchData.Text)
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

        private static bool CallbackRectSize(IntPtr currentWindowHandle, ref SearchData searchData)
        {
            Debug.WriteLine(currentWindowHandle.ToString("X"));

            IntPtr currentWindowParentHandle = GetParent(currentWindowHandle);

            if (currentWindowParentHandle == searchData.ParentHandle)
            {
                RECT lpRect;

                if (GetWindowRect(currentWindowHandle, out lpRect))
                {
                    Debug.WriteLine(lpRect);

                    if ((Size)lpRect == searchData.Size)
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
            public string Text;
            public IntPtr ParentHandle;
            public IntPtr ResultHandle;
            public RECT Rect;
            public Size Size;
        }

        delegate bool EnumWindowsCallback(IntPtr currentWindowHandle, ref SearchData searchData);

        [DllImport("user32.dll")]
        static extern bool EnumWindows(EnumWindowsCallback callback, ref SearchData searchData);

        [DllImport("user32.dll")]
        static extern IntPtr GetParent(IntPtr childHandle);

        [DllImport("user32.dll")] //GetWindowCaption
        static extern void GetWindowText(IntPtr handle, StringBuilder resultWindowText, int maxTextCapacity);

        /*************************************************************************************/

        [DllImport("user32.dll")]
        public static extern int SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool ClipCursor(ref RECT rcClip);

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

            public static implicit operator RECT(Rectangle rect)
            {
                return new RECT(rect.Left, rect.Top, rect.Right, rect.Bottom);
            }

            public static explicit operator Size(RECT r1)
            {
                return new Size(r1.Right - r1.Left, r1.Bottom - r1.Top);
            }

            public Point LeftTop()
            {
                return new Point(Left, Top);
            }

            public Point Middle()
            {
                return new Point(Left + (Right - Left) / 2, Top + (Bottom - Top) / 2);
            }

            public RECT(int left, int top, int right, int bottom)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }
        }

        /*************************************************************************************/

        public const short SWP_NOMOVE = 0X2;
        public const short SWP_NOSIZE = 0X1;
        public const short SWP_NOZORDER = 0X4;
        public const int SWP_SHOWWINDOW = 0x0040;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

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

        public static void LeftMouseClick(IntPtr btnhWnd)
        {
            SendMessage(btnhWnd, WM_LBUTTONDOWN, IntPtr.Zero, IntPtr.Zero);
            SendMessage(btnhWnd, WM_LBUTTONUP, IntPtr.Zero, IntPtr.Zero);
        }

        /*************************************************************************************/

        public delegate bool Win32Callback(IntPtr hwnd, IntPtr lParam);

        [DllImport("user32.Dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumChildWindows(IntPtr parentHandle, Win32Callback callback, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static public extern IntPtr GetClassName(IntPtr hWnd, System.Text.StringBuilder lpClassName, int nMaxCount);

        private static bool EnumWindowCallback(IntPtr handle, IntPtr pointer)
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
                Win32Callback childProc = new Win32Callback(EnumWindowCallback);
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

        private static IEnumerable<IntPtr> EnumAllVisibleWindows(IntPtr parenthWnd, string childClassName)
        {
            List<IntPtr> children = GetChildWindows(parenthWnd);
            if (children == null)
                yield break;
            foreach (IntPtr child in children)
            {
                if (GetWinClass(child) == childClassName)//&& IsWindowVisible(child)
                    yield return child;
                foreach (var childchild in EnumAllVisibleWindows(child, childClassName))
                    yield return childchild;
            }
        }

        private static IntPtr FindFirstVisibleChildByClassAndSize(IntPtr hWnd, string className, Size size, int number = 0)
        {
            int count = 0;
            foreach (var child in EnumAllVisibleWindows(hWnd, className))
            {
                RECT lpRect;
                if (GetWindowRect(child, out lpRect) && (Size)lpRect == size && count++ == number)
                    return child;
            }
            return IntPtr.Zero;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsWindowVisible(IntPtr hWnd);

        #endregion
    }

    [Flags]
    public enum MouseEventFlags
    {
        LeftDown = 0x00000002,
        LeftUp = 0x00000004,
        MiddleDown = 0x00000020,
        MiddleUp = 0x00000040,
        Move = 0x00000001,
        Absolute = 0x00008000,
        RightDown = 0x00000008,
        RightUp = 0x00000010,
        LeftDownUp = LeftDown | LeftUp,
        RightDownUp = RightDown | RightUp,
    }

    public static class ClassEx
    {
        public static void MoveWinPos(this IntPtr hWnd, int x, int y)
        {
            CameraShooter.SetWindowPos(hWnd, IntPtr.Zero, x, y, 0, 0, 
                CameraShooter.SWP_NOZORDER | CameraShooter.SWP_NOSIZE | CameraShooter.SWP_SHOWWINDOW);
        }

        public static void ClickOffset(this IntPtr control, Point offset)
        {
            CPrint2.CameraShooter.RECT r2;
            CameraShooter.GetWindowRect(control, out r2);
            var p2 = r2.LeftTop();
            p2.Offset(offset);
            CameraShooter.SetForegroundWindow(control);
            MouseOperations.MouseEventXY(p2.X, p2.Y, MouseEventFlags.LeftDownUp);
        }
    }

    public class MouseOperations
    {
        [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out MousePoint lpMousePoint);

        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        public static MousePoint SetCursorPosition(int X, int Y)
        {
            SetCursorPos(X, Y);
            return new MousePoint(X, Y);
        }

        public static void SetCursorPosition(MousePoint point)
        {
            SetCursorPos(point.X, point.Y);
        }

        public static MousePoint GetCursorPosition()
        {
            MousePoint currentMousePoint;
            var gotPoint = GetCursorPos(out currentMousePoint);
            if (!gotPoint) { currentMousePoint = new MousePoint(0, 0); }
            return currentMousePoint;
        }

        public static void MouseEvent(MouseEventFlags value)
        {
            MousePoint position = GetCursorPosition();

            mouse_event
                ((int)value,
                 position.X,
                 position.Y,
                 0,
                 0)
                ;
        }

        public static void MouseEventXY(int x, int y, MouseEventFlags value)
        {
            MousePoint position = SetCursorPosition(x, y);

            mouse_event
                ((int)value,
                 position.X,
                 position.Y,
                 0,
                 0)
                ;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MousePoint
        {
            public int X;
            public int Y;

            public MousePoint(int x, int y)
            {
                X = x;
                Y = y;
            }
        }
    }

    public static class VirtualMouse
    {
        [DllImport("user32.dll")]
        static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        private const int MOUSEEVENTF_MOVE = 0x0001;
        private const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const int MOUSEEVENTF_LEFTUP = 0x0004;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
        private const int MOUSEEVENTF_RIGHTUP = 0x0010;
        private const int MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        private const int MOUSEEVENTF_MIDDLEUP = 0x0040;
        private const int MOUSEEVENTF_ABSOLUTE = 0x8000;
        public static void Move(int xDelta, int yDelta)
        {
            mouse_event(MOUSEEVENTF_MOVE, xDelta, yDelta, 0, 0);
        }
        public static void MoveTo(int x, int y)
        {
            mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE, x, y, 0, 0);
        }
        public static void LeftClick()
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN, System.Windows.Forms.Control.MousePosition.X, System.Windows.Forms.Control.MousePosition.Y, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, System.Windows.Forms.Control.MousePosition.X, System.Windows.Forms.Control.MousePosition.Y, 0, 0);
        }

        public static void LeftDown()
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN, System.Windows.Forms.Control.MousePosition.X, System.Windows.Forms.Control.MousePosition.Y, 0, 0);
        }

        public static void LeftUp()
        {
            mouse_event(MOUSEEVENTF_LEFTUP, System.Windows.Forms.Control.MousePosition.X, System.Windows.Forms.Control.MousePosition.Y, 0, 0);
        }

        public static void RightClick()
        {
            mouse_event(MOUSEEVENTF_RIGHTDOWN, System.Windows.Forms.Control.MousePosition.X, System.Windows.Forms.Control.MousePosition.Y, 0, 0);
            mouse_event(MOUSEEVENTF_RIGHTUP, System.Windows.Forms.Control.MousePosition.X, System.Windows.Forms.Control.MousePosition.Y, 0, 0);
        }

        public static void RightDown()
        {
            mouse_event(MOUSEEVENTF_RIGHTDOWN, System.Windows.Forms.Control.MousePosition.X, System.Windows.Forms.Control.MousePosition.Y, 0, 0);
        }

        public static void RightUp()
        {
            mouse_event(MOUSEEVENTF_RIGHTUP, System.Windows.Forms.Control.MousePosition.X, System.Windows.Forms.Control.MousePosition.Y, 0, 0);
        }
    }
}
