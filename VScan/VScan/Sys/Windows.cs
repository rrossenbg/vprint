using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections.Generic;

namespace PremierTaxFree.Sys
{
    /// <summary>
    /// Window class
    /// </summary>
    public class Window
    {
        /// <summary>
        /// Win32 API Imports
        /// </summary>
        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern bool IsZoomed(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);
        [DllImport("user32.dll")]
        private static extern IntPtr AttachThreadInput(IntPtr idAttach, IntPtr idAttachTo, int fAttach);
        [DllImport("user32.dll")]
        private static extern bool CloseWindow(IntPtr hWnd);
        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DestroyWindow(IntPtr hwnd);
        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

        private const int WM_SYSCOMMAND = 0x0112;
        private const int SC_CLOSE = 0xF060;
        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder title, int size);
        [DllImport("user32.dll")]
        private static extern int GetWindowModuleFileName(IntPtr hWnd, StringBuilder title, int size);
        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr parameter);
        private delegate bool Win32Callback(IntPtr hwnd, IntPtr lParam);
        [DllImport("user32.Dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumChildWindows(IntPtr parentHandle, Win32Callback callback, IntPtr lParam);

        /// <summary>
        /// Win32 API Constants for ShowWindowAsync()
        /// </summary>
        private const int SW_HIDE = 0;
        private const int SW_SHOWNORMAL = 1;
        private const int SW_SHOWMINIMIZED = 2;
        private const int SW_SHOWMAXIMIZED = 3;
        private const int SW_SHOWNOACTIVATE = 4;
        private const int SW_RESTORE = 9;
        private const int SW_SHOWDEFAULT = 10;

        private const int WM_LBUTTONDOWN = 0x0201;
        private const int WM_LBUTTONUP = 0x0202;

        /// <summary>
        /// Private Fields
        /// </summary>
        private IntPtr m_hWnd;
        private string m_Title;
        private bool m_Visible = true;
        private string m_Process;
        private bool m_WasMax = false;

        /// <summary>
        /// Window Object's Public Properties
        /// </summary>
        public IntPtr hWnd
        {
            get { return m_hWnd; }
        }
        public string Title
        {
            get { return m_Title; }
        }
        public string Process
        {
            get { return m_Process; }
        }

        /// <summary>
        /// Sets this Window Object's visibility
        /// </summary>
        public bool Visible
        {
            get { return m_Visible; }
            set
            {
                //show the window
                if (value == true)
                {
                    if (m_WasMax)
                    {
                        if (ShowWindowAsync(m_hWnd, SW_SHOWMAXIMIZED))
                            m_Visible = true;
                    }
                    else
                    {
                        if (ShowWindowAsync(m_hWnd, SW_SHOWNORMAL))
                            m_Visible = true;
                    }
                }
                //hide the window
                if (value == false)
                {
                    m_WasMax = IsZoomed(m_hWnd);
                    if (ShowWindowAsync(m_hWnd, SW_HIDE))
                        m_Visible = false;
                }
            }
        }

        /// <summary>
        /// Returns a list of child windows
        /// </summary>
        /// <param name="parent">Parent of the windows to return</param>
        /// <returns>List of child windows</returns>
        public List<Window> GetChildWindows()
        {
            List<Window> result = new List<Window>();
            GCHandle listHandle = GCHandle.Alloc(result);
            try
            {
                Win32Callback childProc = new Win32Callback(EnumWindow);
                EnumChildWindows(m_hWnd, childProc, GCHandle.ToIntPtr(listHandle));
            }
            finally
            {
                if (listHandle.IsAllocated)
                    listHandle.Free();
            }
            return result;
        }

        public void Click()
        {
            SendMessage(hWnd, WM_LBUTTONDOWN, IntPtr.Zero, IntPtr.Zero);
            SendMessage(hWnd, WM_LBUTTONUP, IntPtr.Zero, IntPtr.Zero);
        }

        /// <summary>
        /// Callback method to be used when enumerating windows.
        /// </summary>
        /// <param name="handle">Handle of the next window</param>
        /// <param name="pointer">Pointer to a GCHandle that holds a reference to the list to fill</param>
        /// <returns>True to continue the enumeration, false to bail</returns>
        private static bool EnumWindow(IntPtr hWnd, IntPtr pointer)
        {
            GCHandle gch = GCHandle.FromIntPtr(pointer);
            List<Window> list = gch.Target as List<Window>;
            if (list == null)
                throw new InvalidCastException("GCHandle Target could not be cast as List<IntPtr>");

            StringBuilder title = new StringBuilder(256);
            StringBuilder module = new StringBuilder(256);

            GetWindowModuleFileName(hWnd, module, 256);
            GetWindowText(hWnd, title, 256);

            list.Add(new Window(title.ToString(), hWnd, module.ToString()));

            return (true);
        }

        /// <summary>
        /// Constructs a Window Object
        /// </summary>
        /// <param name="Title">Title Caption</param>
        /// <param name="hWnd">Handle</param>
        /// <param name="Process">Owning Process</param>
        public Window(string Title, IntPtr hWnd, string Process)
        {
            m_Title = Title;
            m_hWnd = hWnd;
            m_Process = Process;
        }

        public void Close()
        {
            SendMessage(hWnd, WM_SYSCOMMAND, SC_CLOSE, 0);
        }

        //Override ToString()
        public override string ToString()
        {
            //return the title if it has one, if not return the process name
            if (m_Title.Length > 0)
            {
                return m_Title;
            }
            else
            {
                return m_Process;
            }
        }

        /// <summary>
        /// Sets focus to this Window Object
        /// </summary>
        public void Activate()
        {
            if (m_hWnd == GetForegroundWindow())
                return;

            IntPtr ThreadID1 = GetWindowThreadProcessId(GetForegroundWindow(), IntPtr.Zero);
            IntPtr ThreadID2 = GetWindowThreadProcessId(m_hWnd, IntPtr.Zero);

            if (ThreadID1 != ThreadID2)
            {
                AttachThreadInput(ThreadID1, ThreadID2, 1);
                SetForegroundWindow(m_hWnd);
                AttachThreadInput(ThreadID1, ThreadID2, 0);
            }
            else
            {
                SetForegroundWindow(m_hWnd);
            }

            if (IsIconic(m_hWnd))
            {
                ShowWindowAsync(m_hWnd, SW_RESTORE);
            }
            else
            {
                ShowWindowAsync(m_hWnd, SW_SHOWNORMAL);
            }
        }
    }

    /// <summary>
    /// Collection used to enumerate Window Objects
    /// </summary>
    public class Windows : IEnumerable, IEnumerator
    {
        /// <summary>
        /// Win32 API Imports
        /// </summary>
        [DllImport("user32.dll")]
        private static extern int GetWindowText(int hWnd, StringBuilder title, int size);
        [DllImport("user32.dll")]
        private static extern int GetWindowModuleFileName(int hWnd, StringBuilder title, int size);
        [DllImport("user32.dll")]
        private static extern int EnumWindows(EnumWindowsProc ewp, int lParam);
        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(int hWnd);

        //delegate used for EnumWindows() callback function
        public delegate bool EnumWindowsProc(int hWnd, int lParam);

        private int m_Position = -1; //holds current index of wndArray, necessary for IEnumerable

        ArrayList wndArray = new ArrayList(); //array of windows

        //Object's private fields
        private bool m_invisible = false;
        private bool m_notitle = false;

        /// <summary>
        /// Collection Constructor with additional options
        /// </summary>
        /// <param name="Invisible">Include invisible Windows</param>
        /// <param name="Untitled">Include untitled Windows</param>
        public Windows(bool Invisible, bool Untitled)
        {
            m_invisible = Invisible;
            m_notitle = Untitled;

            //Declare a callback delegate for EnumWindows() API call
            EnumWindowsProc ewp = new EnumWindowsProc(EvalWindow);
            //Enumerate all Windows
            EnumWindows(ewp, 0);
        }
        /// <summary>
        /// Collection Constructor
        /// </summary>
        public Windows()
        {
            //Declare a callback delegate for EnumWindows() API call
            EnumWindowsProc ewp = new EnumWindowsProc(EvalWindow);
            //Enumerate all Windows
            EnumWindows(ewp, 0);
        }
        //EnumWindows CALLBACK function
        private bool EvalWindow(int hWnd, int lParam)
        {
            if (m_invisible == false && !IsWindowVisible(hWnd))
                return (true);

            StringBuilder title = new StringBuilder(256);
            StringBuilder module = new StringBuilder(256);

            GetWindowModuleFileName(hWnd, module, 256);
            GetWindowText(hWnd, title, 256);

            if (m_notitle == false && title.Length == 0)
                return (true);

            wndArray.Add(new Window(title.ToString(), (IntPtr)hWnd, module.ToString()));

            return (true);
        }

        //implement IEnumerable
        public IEnumerator GetEnumerator()
        {
            return (IEnumerator)this;
        }
        //implement IEnumerator
        public bool MoveNext()
        {
            m_Position++;
            if (m_Position < wndArray.Count)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void Reset()
        {
            m_Position = -1;
        }
        public object Current
        {
            get
            {
                return wndArray[m_Position];
            }
        }
    }
}