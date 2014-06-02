using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    /// 
    /// </summary>
    /// <see cref="http://www.codeproject.com/KB/dialog/FormEx.aspx"/>
    public partial class WinFormEx : Form
    {
        //Member variables
        private bool m_EnableCloseButton;
        private bool m_FullScreen;
        private bool m_Sizable;
        private Boolean m_Movable;
        private bool m_DesktopAttached;
        private IntPtr m_PreviousParent;

        /*
         * Constants
         */
        //Paramaters to EnableMenuItem Win32 function
        private const int SC_CLOSE = 0xF060; //The Close Box identifier
        private const int MF_ENABLED = 0x0;  //Enabled Value
        private const int MF_DISABLED = 0x2; //Disabled Value
        
        //Windows Messages
        private const int WM_NCPAINT = 0x85;//Paint non client area message
        private const int WM_PAINT = 0xF;//Paint client area message
        private const int WM_SIZE = 0x5;//Resize the form message
        private const int WM_IME_NOTIFY = 0x282;//Notify IME Window message
        private const int WM_SETFOCUS = 0x0007;//Form.Activate message
        private const int WM_SYSCOMMAND = 0x112; //SysCommand message
        private const int WM_SIZING = 0x214; //Resize Message
        private const int WM_NCLBUTTONDOWN = 0xA1; //Left Mouse Button on Non-Client Area is Down
        private const int WM_NCACTIVATE = 0x86; //Message sent to the window when it's activated or deactivated

        //WM_SIZING WParams that stands for Hit Tests in the direction the form is resizing
        private const int HHT_ONHEADER = 0x0002; 
        private const int HT_TOPLEFT = 0XD; 
        private const int HT_TOP = 0XC; 
        private const int HT_TOPRIGHT = 0XE;
        private const int HT_RIGHT = 0XB;
        private const int HT_BOTTOMRIGHT = 0X11;
        private const int HT_BOTTOM = 0XF;
        private const int HT_BOTTOMLEFT = 0X10;
        private const int HT_LEFT = 0XA;

        //WM_SYSCOMMAND WParams that stands for which operation is beeing done
        private const int SC_DRAGMOVE = 0xF012; //SysCommand Dragmove parameter
        private const int SC_MOVE = 0xF010; //SysCommand Move with keyboard command

        //Remember Window State before beeing set to FullScreen
        private FormWindowState m_fws;
        private Point m_Location;
        private bool m_TopMost;
        private Size m_Size;
        private bool m_MaxBox;
        private Graphics m_GraphicsFrameArea = null;

        /// <summary>
        /// Occurs when the frame area (including Title Bar, excluding the client area) is redrawn."
        /// </summary>
        [Description("Occurs when The frame area (including Title Bar, excluding the client area) needs repainting."), Category("Appearance")]
        public event PaintEventHandler PaintFrameArea;

        //GetSystemMenu Win32 API Declaration (The Window Title bar a is SystemMenu)
        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        //EnableMenuItem Win32 API Declaration - Set the enabled values of the title bar (from GetSystemMenu) items
        [DllImport("user32.dll")]
        private static extern int EnableMenuItem(IntPtr hMenu, int wIDEnable, int wValue);

        //Get Desktop Window Handle
        [DllImport("user32.dll")]
        private static extern IntPtr GetDesktopWindow();

        //Set Parent Window - We will use this to set ProgMan as the Window Parent
        [DllImport("user32.dll")]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        //Find any Window in the OS. We will look for the parent of where the desktop is (ProgMan)
        [DllImport("User32.dll")]
        public static extern IntPtr FindWindow(String lpClassName, String lpWindowName);

        //Get the device component of the window to allow drawing on the title bar and frame
        [DllImport("User32.dll")]
        public static extern IntPtr GetWindowDC(IntPtr hWnd);

        //Releases the Device Component after it's been used
        [DllImport("User32.dll")]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        public WinFormEx()
        {
            m_Movable = true;
            m_FullScreen = false;
            m_EnableCloseButton = true;
            m_Sizable = true;
            m_DesktopAttached = false;
            InitializeComponent();
            SaveFormState();
        }

        private void SaveFormState()
        {
            m_fws = this.WindowState;
            m_Location = this.Location;
            m_TopMost = this.TopMost;
            m_Size = this.Size;
            m_MaxBox = this.MaximizeBox;
        }

        private void RecallFormState()
        {
            this.WindowState = this.m_fws;
            this.TopMost = this.m_TopMost;
            this.Size = this.m_Size;
            this.MaximizeBox = this.m_MaxBox;
            this.Location = this.m_Location;
        }

        /// <summary>
        /// Gets or Sets the value indicating wether the Close Button on the window title bar is enabled.
        /// </summary>
        [Browsable(true), DefaultValue(true), Category("Window Style")]
        [Description("Determines wether the Close Button on the window title bar is enabled.")]
        public bool CloseButton
        {
            get
            {
                return m_EnableCloseButton;
            }
            set
            {
                m_EnableCloseButton = value;
                CloseBoxEnable(m_EnableCloseButton);
            }
        }

        /// <summary>
        /// Gets or Sets the value indicating wether the Form is in Full Screen mode
        /// </summary>
        [Browsable(true), DefaultValue(false), Category("Layout")]
        [Description("Determines wether the the Form is in Full Screen mode.")]
        public bool FullScreen
        {
            get
            {
                return m_FullScreen;
            }
            set
            {
                if (!DesignMode)
                {
                    if (value && m_FullScreen != value)
                        SetFullScreen(true);

                    if (!value && m_FullScreen != value)
                        SetFullScreen(false);
                }
                else
                    m_FullScreen = value;
            }
        }

        /// <summary>
        /// Gets or Sets the value indicating wether the Form is Movable.
        /// </summary>
        [Browsable(true), DefaultValue(true), Category("Layout")]
        [Description("Determines wether the Form is movable.")]
        public bool Movable
        {
            get
            {
                return m_Movable;
            }
            set
            {
                m_Movable = value;
            }
        }

        /// <summary>
        /// Gets or Sets the value indicating wether the Form is Sizable.
        /// </summary>
        [Browsable(true), DefaultValue(true), Category("Layout")]
        [Description("Determines wether the Form is sizable.")]
        public bool Sizable
        {
            get
            {
                return m_Sizable;
            }
            set
            {
                m_Sizable = value;
            }
        }

        /// <summary>
        /// Gets or Sets the value indicating wether the Form is attached to the Desktop.
        /// </summary>
        [Browsable(true), DefaultValue(false), Category("Layout")]
        [Description("Determines wether the Form is attached to the Desktop.")]
        public bool DesktopAttached
        {
            get
            {
                return m_DesktopAttached;
            }
            set
            {
                m_DesktopAttached = value;
                this.MinimizeBox = !value;

                if (value)
                    m_PreviousParent = SetParent(this.Handle, FindWindow("Progman", null));
                else
                    SetParent(this.Handle, m_PreviousParent);
            }
        }

        private void SetFullScreen(bool fullscreen)
        {
            m_FullScreen = fullscreen;

            if (fullscreen)
            {
                SaveFormState();
                this.MaximizeBox = false;
                this.WindowState = FormWindowState.Normal;
                this.Location = new Point(0, 0);                
                this.TopMost = true;
                Screen currentScreen = Screen.FromHandle(this.Handle);
                this.Size = new System.Drawing.Size(currentScreen.Bounds.Width, currentScreen.Bounds.Height);
            }
            else
                RecallFormState();
        }

        protected override void WndProc(ref Message m)
        {
            // Prevents moving or resizing through the task bar
            if ((m.Msg == WM_SYSCOMMAND && (m.WParam == new IntPtr(SC_DRAGMOVE) || m.WParam == new IntPtr(SC_MOVE))))
            {
                if (m_FullScreen || !m_Movable)
                    return;
            }

            // Preventes Resizing from dragging the borders
            if (m.Msg == WM_SIZING || (m.Msg == WM_NCLBUTTONDOWN && (m.WParam == new IntPtr(HT_TOPLEFT) || m.WParam == new IntPtr(HT_TOP)
                || m.WParam == new IntPtr(HT_TOPRIGHT) || m.WParam == new IntPtr(HT_RIGHT) || m.WParam == new IntPtr(HT_BOTTOMRIGHT)
                || m.WParam == new IntPtr(HT_BOTTOM) || m.WParam == new IntPtr(HT_BOTTOMLEFT) || m.WParam == new IntPtr(HT_LEFT))))
            {
                if (m_FullScreen || !m_Sizable || !m_Movable)
                    return;
            }

            base.WndProc(ref m);

            // Handles painting of the Non Client Area
            if (m.Msg == WM_NCPAINT || m.Msg == WM_IME_NOTIFY || m.Msg == WM_SIZE || m.Msg == WM_NCACTIVATE)
            {
                // To avoid unnecessary graphics recreation and thus improving performance
                if (m_GraphicsFrameArea == null || m.Msg == WM_SIZE)
                {
                    ReleaseDC(this.Handle, m_WndHdc);
                    m_WndHdc = GetWindowDC(this.Handle);
                    m_GraphicsFrameArea = Graphics.FromHdc(m_WndHdc);

                    Rectangle clientRecToScreen = new Rectangle(this.PointToScreen(new Point(this.ClientRectangle.X, this.ClientRectangle.Y)), new System.Drawing.Size(this.ClientRectangle.Width, this.ClientRectangle.Height));
                    Rectangle clientRectangle = new Rectangle(clientRecToScreen.X - this.Location.X, clientRecToScreen.Y - this.Location.Y, clientRecToScreen.Width, clientRecToScreen.Height);

                    m_GraphicsFrameArea.ExcludeClip(clientRectangle);
                }

                RectangleF recF = m_GraphicsFrameArea.VisibleClipBounds;

                PaintEventArgs pea = new PaintEventArgs(m_GraphicsFrameArea, new Rectangle((int)recF.X, (int)recF.Y, (int)recF.Width, (int)recF.Height));
                OnPaintFrameArea(pea);
                CloseBoxEnable(m_EnableCloseButton);
                this.Refresh();
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnPaintFrameArea(PaintEventArgs e)
        {
            PaintEventHandler handler = PaintFrameArea;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private IntPtr m_WndHdc;
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            PaintFrameArea = null;
            ReleaseDC(this.Handle, m_WndHdc);
        }

        //Overrides attempts to move the form by code when in Full Screen mode.
        protected override void OnLocationChanged(EventArgs e)
        {
            if (m_FullScreen)
            {
                this.Location = new Point(0, 0);
                return;
            }

            base.OnLocationChanged(e);
        }

        //Allows changes using the desginer to be reflected immediately
        protected override void OnChangeUICues(UICuesEventArgs e)
        {
            base.OnChangeUICues(e);
            CloseBoxEnable(m_EnableCloseButton);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (this.CloseButton || e.CloseReason == CloseReason.TaskManagerClosing || e.CloseReason == CloseReason.FormOwnerClosing || e.CloseReason == CloseReason.ApplicationExitCall || e.CloseReason == CloseReason.WindowsShutDown)
                base.OnFormClosing(e);
            else
                e.Cancel = true;
        }

        //Calls Win32 API to disable or enable the close Box.
        private void CloseBoxEnable(bool enabled)
        {
            IntPtr thisSystemMenu = GetSystemMenu(this.Handle, false);

            if (enabled == true)
                EnableMenuItem(thisSystemMenu, SC_CLOSE, MF_ENABLED);
            else
                EnableMenuItem(thisSystemMenu, SC_CLOSE, MF_DISABLED);
        }

        /// <summary>
        /// Get if Key is Up or Down
        /// </summary>
        /// <param name="key">The keyboard key to evaluate</param>
        /// <returns>If the key is Up or Down</returns>
        public KeyState GetKeyState(Keys key)
        {
            return KeyStateCheck.GetKeyState(key);
        }

        /// <summary>
        /// // Get if key is toggled or untgled (useful to detect if capslock or nunlock is on)
        /// </summary>
        /// <param name="key">The keyboard key to evaluate</param>
        /// <returns>If the key is toggled or untoggled</returns>
        public KeyValue GetKeyValue(Keys key)
        {
            return KeyStateCheck.GetToggled(key);
        }
    }

    public enum KeyState { Up = 0, Down = 1 }
    public enum KeyValue { Untoggled = 0, Toggled = 1 }

    internal static class KeyStateCheck
    {
        //GetKeyState Win32 API declaration
        [DllImport("user32.dll")]
        private static extern short GetKeyState(int nVirtKey);

        private static short m_KeyDown = Convert.ToInt16("1000000000000000", 2); //High-Order bit set (KeyDown)
        private static short m_KeyUp = Convert.ToInt16("0000000000000000", 2); //High-Order bit not set (Key Up)
        private static short m_KeyToggled = Convert.ToInt16("0000000000000001", 2); //Low-Order bit set (Key Toggled)
        private static short m_KeyUnToggled = Convert.ToInt16("0000000000000000", 2); // Low-Order bit not set (Key Untoggled)

        // Get if Key is Up or Down
        public static KeyState GetKeyState(Keys virtualKey)
        {
            short keyState = GetKeyState((int)virtualKey);
            KeyState state;

            // Bitwise AND to get wether the key is Down
            if ((keyState & m_KeyDown) == m_KeyDown)
                state = KeyState.Down;
            else
                state = KeyState.Up;

            return state;
        }

        // Get if key is toggled or untgled (useful to detect if capslock or nunlock is on)
        public static KeyValue GetToggled(Keys virtualKey)
        {
            short keyState = GetKeyState((int)virtualKey);
            KeyValue value;

            // Bitwise AND to get wether the key is toggled
            if ((keyState & m_KeyToggled) == m_KeyToggled)
                value = KeyValue.Toggled;
            else
                value = KeyValue.Untoggled;

            return value;
        }
    }
}
