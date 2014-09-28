using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;

namespace CPrint2
{
    /// <summary>
    /// http://support.microsoft.com/kb/318804
    /// </summary>
    public class MouseHook : IDisposable
    {
        public delegate int HookProc(int nCode, IntPtr wParam, IntPtr lParam);

        //Declare the hook handle as an int.
        static int hHook = 0;

        //Declare the mouse hook constant.
        //For other hook types, you can obtain these values from Winuser.h in the Microsoft SDK.
        public const int WH_MOUSE = 7;
        private System.Windows.Forms.Button button1;

        //Declare MouseHookProcedure as a HookProc type.
        HookProc MouseHookProcedure;

        //Declare the wrapper managed POINT class.
        [StructLayout(LayoutKind.Sequential)]
        public class POINT
        {
            public int x;
            public int y;
        }

        //Declare the wrapper managed MouseHookStruct class.
        [StructLayout(LayoutKind.Sequential)]
        public class MouseHookStruct
        {
            public POINT pt;
            public int hwnd;
            public int wHitTestCode;
            public int dwExtraInfo;
        }

        //This is the Import for the SetWindowsHookEx function.
        //Use this function to install a thread-specific hook.
        [DllImport("user32.dll", CharSet = CharSet.Auto,
         CallingConvention = CallingConvention.StdCall)]
        public static extern int SetWindowsHookEx(int idHook, HookProc lpfn,
        IntPtr hInstance, int threadId);

        //This is the Import for the UnhookWindowsHookEx function.
        //Call this function to uninstall the hook.
        [DllImport("user32.dll", CharSet = CharSet.Auto,
         CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(int idHook);

        //This is the Import for the CallNextHookEx function.
        //Use this function to pass the hook information to the next hook procedure in chain.
        [DllImport("user32.dll", CharSet = CharSet.Auto,
         CallingConvention = CallingConvention.StdCall)]
        public static extern int CallNextHookEx(int idHook, int nCode,
        IntPtr wParam, IntPtr lParam);

        bool clipped = false;   // Do we need to clip the mouse?

        public void Start()
        {
            if (hHook == 0)
            {
                // Create an instance of HookProc.
                MouseHookProcedure = new HookProc(MouseHookProc);

                hHook = SetWindowsHookEx(WH_MOUSE,
                            MouseHookProcedure,
                            (IntPtr)0,
                            AppDomain.GetCurrentThreadId());
                //If the SetWindowsHookEx function fails.
                if (hHook == 0)
                    throw new ApplicationException("SetWindowsHookEx Failed");
            }
        }

        public void Stop()
        {
            if (hHook != 0)
            {
                bool ret = UnhookWindowsHookEx(hHook);
                //If the UnhookWindowsHookEx function fails.
                if (ret == false)
                    throw new ApplicationException("UnhookWindowsHookEx Failed");
                hHook = 0;
            }
        }

        private static int MouseHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            //Marshall the data from the callback.
            MouseHookStruct MyMouseHookStruct = (MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseHookStruct));
            if (nCode < 0)
            {
                return CallNextHookEx(hHook, nCode, wParam, lParam);
            }
            else
            {
                //Create a string variable that shows the current mouse coordinates.
                String strCaption = "x = " + MyMouseHookStruct.pt.x.ToString("d") + "  y = " + MyMouseHookStruct.pt.y.ToString("d");
                //You must get the active form because it is a static function.
                Form tempForm = Form.ActiveForm;

                //Set the caption of the form.
                tempForm.Text = strCaption;

                if (true)
                {
                    CameraShooter.RECT rr = new Rectangle(0, 0, 10000, 10000);
                    CameraShooter.ClipCursor(ref rr);
                }

                return CallNextHookEx(hHook, nCode, wParam, lParam);
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
