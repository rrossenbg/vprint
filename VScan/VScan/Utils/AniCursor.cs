using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace vbAccelerator.Components.Win32
{
    /// <summary>
    /// Allows animated cursors to be loaded and displayed
    /// </summary>
    /// <remarks>Note we cannot extend Cursor since it 
    /// is sealed, which is a shame.</remarks>
    /// <see cref="http://www.vbaccelerator.com/home/net/code/libraries/graphics/Animated_Cursors_in__NET/article.asp"/>
    public class AniCursor : IDisposable, ICloneable
    {
        #region Unmanaged Code
        /// <summary>
        /// Draws an icon or cursor.
        /// </summary>
        /// <remarks>Be careful when using this function - I have successfully
        /// blue-screened my system by settting an incorrect (negative)
        /// iStepIfAniCur value.  This may be because it is implemented
        /// in the graphics driver.</remarks>
        [DllImport("user32")]
        private extern static int DrawIconEx(IntPtr hDC, int xLeft, int yTop, IntPtr hIcon, int cxWidth, int cyWidth, int istepIfAniCur, IntPtr hbrFlickerFreeDraw, int diFlags);

        [DllImport("user32", CharSet = CharSet.Auto)]
        private extern static IntPtr LoadImage(IntPtr hInst, string lpsz, int uType, int cx, int cy, int uFlags);

        [DllImport("user32", CharSet = CharSet.Auto)]
        private extern static IntPtr LoadImage(IntPtr hInst, int lpsz, int uType, int cx, int cy, int uFlags);

        [DllImport("user32")]
        private extern static IntPtr CopyImage(IntPtr handle, int uType, int cxDesired, int cyDesired, int uFlags);

        [DllImport("user32")]
        private extern static int DestroyCursor(IntPtr hCursor);

        private const int IMAGE_BITMAP = 0x0;
        private const int IMAGE_ICON = 0x1;
        private const int IMAGE_CURSOR = 0x2;

        private const int LR_DEFAULTCOLOR = 0x0000;
        private const int LR_MONOCHROME = 0x0001;
        private const int LR_COLOR = 0x0002;
        private const int LR_COPYRETURNORG = 0x0004;
        private const int LR_COPYDELETEORG = 0x0008;
        private const int LR_LOADFROMFILE = 0x10;
        private const int LR_LOADTRANSPARENT = 0x20;
        private const int LR_DEFAULTSIZE = 0x0040;
        private const int LR_LOADMAP3DCOLORS = 0x1000;
        private const int LR_CREATEDIBSECTION = 0x2000;
        private const int LR_COPYFROMRESOURCE = 0x4000;

        private const int DI_MASK = 0x1;
        private const int DI_IMAGE = 0x2;
        private const int DI_NORMAL = 0x3;
        private const int DI_COMPAT = 0x4;
        private const int DI_DEFAULTSIZE = 0x8;
        #endregion

        #region Member Variables
        private Cursor cursor = null;
        private int frameCount = 0;
        private int frame = 0;
        private IntPtr hCur = IntPtr.Zero;
        #endregion

        /// <summary>
        /// Makes an independent copy of this cursor
        /// </summary>
        /// <returns>A copy of this object</returns>
        public object Clone()
        {
            AniCursor clone;
            if (hCur != IntPtr.Zero)
            {
                IntPtr hCurClone = CopyImage(hCur, IMAGE_CURSOR, 0, 0, 0);
                clone = new AniCursor(hCurClone);
            }
            else
            {
                clone = new AniCursor();
            }
            return clone;
        }

        /// <summary>
        /// Gets the handle of the cursor
        /// </summary>
        public IntPtr Handle
        {
            get
            {
                return this.hCur;
            }
        }

        /// <summary>
        /// Gets a Cursor containing the specified animated cursor
        /// </summary>
        public Cursor Cursor
        {
            get
            {
                return this.cursor;
            }
        }

        /// <summary>
        /// Returns the number of frames in this animated cursor
        /// </summary>
        public int FrameCount
        {
            get
            {
                return this.frameCount;
            }
        }

        /// <summary>
        /// Steps to the next frame in the animation
        /// </summary>
        public void Step()
        {
            this.frame++;
            if (this.frame >= this.frameCount)
            {
                this.frame = 0;
            }
        }

        /// <summary>
        /// Gets/sets the current cursor frame
        /// </summary>
        public int Frame
        {
            get
            {
                return this.frame;
            }
            set
            {
                if ((value < 0) || (value > this.frameCount))
                    throw new ArgumentException("Frame must be between 0 and FrameCount-1", "Frame");
                this.frame = value;
            }
        }

        /// <summary>
        /// Draws the cursor at the specified position
        /// </summary>
        /// <param name="gfx">Graphics object to draw on</param>
        /// <param name="x">X position to draw at</param>
        /// <param name="y">Y position to draw at</param>
        public void Draw(Graphics gfx, int x, int y)
        {
            Draw(gfx, x, y, 0, 0, false);
        }

        /// <summary>
        /// Draws the cursor at the specified position
        /// </summary>
        /// <param name="gfx">Graphics object to draw on</param>
        /// <param name="x">X Position to draw at</param>
        /// <param name="y">Y Position to draw at</param>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        public void Draw(Graphics gfx, int x, int y, int width, int height, bool stretch)
        {
            if (!stretch)
            {
                width = 0;
                height = 0;
            }

            IntPtr hdc = gfx.GetHdc();
            DrawIconEx(hdc, x, y, this.hCur, 0, 0, this.frame, IntPtr.Zero, DI_NORMAL);
            gfx.ReleaseHdc(hdc);
        }

        /// <summary>
        /// Draws the cursor in the specified rectangle
        /// </summary>
        /// <param name="gfx">Graphics object to draw on</param>
        /// <param name="rect">Rectangle to draw at</param>
        public void Draw(Graphics gfx, Rectangle rect, bool stretch)
        {
            Draw(gfx, rect.X, rect.Y, rect.Width, rect.Height, stretch);
        }
        /// <summary>
        /// Loads an animated cursor from a file
        /// </summary>
        /// <param name="fileName">Filename to load from</param>
        public void Load(string fileName)
        {
            clearCursor();
            hCur = LoadImage(Marshal.GetHINSTANCE(this.GetType().Module), fileName, IMAGE_CURSOR, 0, 0, LR_LOADFROMFILE);
            if (hCur != IntPtr.Zero)
            {
                evaluateFrames();
                createCursor();
            }
        }

        private void evaluateFrames()
        {
            // the only way to get the frames appears to
            // be by trying to draw a frame... ugly
            this.frameCount = 0;
            this.frame = 0;
            Bitmap bm = new Bitmap(128, 128);
            Graphics gfx = Graphics.FromImage(bm);
            IntPtr hdc = gfx.GetHdc();
            int success = 1;
            while (success != 0)
            {
                success = DrawIconEx(hdc, 0, 0, hCur, 0, 0, this.frameCount, IntPtr.Zero, DI_NORMAL);
                if (success != 0)
                {
                    this.frameCount++;
                }
            }
            gfx.ReleaseHdc(hdc);
        }

        private void createCursor()
        {
            this.cursor = new Cursor(this.hCur);
        }

        private void clearCursor()
        {
            if (this.hCur != IntPtr.Zero)
            {
                DestroyCursor(this.hCur);
                this.hCur = IntPtr.Zero;
            }

            if (this.cursor != null)
            {
                this.cursor.Dispose();
            }
        }

        /// <summary>
        /// Constructs a blank instance of the AniCursor
        /// class
        /// </summary>
        public AniCursor()
        {
        }

        /// <summary>
        /// Constructs a new instance of the class
        /// and loads an animated cursor from the 
        /// specified file.
        /// </summary>
        /// <param name="fileName">File containing animated cursor</param>
        public AniCursor(string fileName)
        {
            Load(fileName);
        }

        public AniCursor(IntPtr hInstance, int resourceId)
        {
            LoadImage(hInstance, resourceId, IMAGE_CURSOR, 0, 0, 0);
        }

        public AniCursor(IntPtr hInstance, string resourceName)
        {
            LoadImage(hInstance, resourceName, IMAGE_CURSOR, 0, 0, 0);
        }

        /// <summary>
        /// Constructs a new instance of the class from the
        /// specified cursor handle.
        /// </summary>
        /// <param name="handle"></param>
        public AniCursor(IntPtr handle)
        {
            this.hCur = handle;
            evaluateFrames();
            createCursor();
        }

        /// <summary>
        /// Clears up any resources associated with the animated cursor
        /// </summary>
        public void Dispose()
        {
            clearCursor();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <example>
    ///    private ThreadedAniCursor m_ThreadCursor = null;
    ///
    ///    public frmAnimatedCursor()
    ///    {
    ///        InitializeComponent();
    ///        // AniCursor class:
    ///        string file = Path.Combine(System.Environment.CurrentDirectory, "globe.ani");
    ///        // ThreadedAniCursor class:
    ///        m_ThreadCursor = new ThreadedAniCursor(this);
    ///        m_ThreadCursor.AniCursor = new AniCursor(file);
    ///        m_ThreadCursor.Location = new Point(32, 148);
    ///        m_ThreadCursor.Size = new Size(32, 32);
    ///        this.Load += Form_Load;
    ///        this.Paint += Form_Paint;
    ///        this.Closing += Form_Closing;
    ///    }
    ///    private void Form_Load(object sender, EventArgs e)
    ///    {
    ///        m_ThreadCursor.Handle = this.Handle;
    ///    }
    ///    private void Form_Closing(object sender, CancelEventArgs e)
    ///    {
    ///        if (!m_ThreadCursor.IsDone)
    ///            m_ThreadCursor.CancelAndWait();
    ///    }
    ///    private void cbStartStopCursor_CheckedChanged(object sender, EventArgs e)
    ///    {
    ///        if (chkSetCursor.Checked)
    ///            this.Cursor = m_ThreadCursor.AniCursor.Cursor;
    ///        else
    ///            this.Cursor = null;
    ///    }
    ///    private void cbUseThreadedCursor_CheckedChanged(object sender, EventArgs e)
    ///    {
    ///        if (chkDrawThreaded.Checked)
    ///            m_ThreadCursor.Start();
    ///        else
    ///            m_ThreadCursor.Cancel();
    ///    }
    ///    private void Form_Paint(object sender, PaintEventArgs p)
    ///    {
    ///        p.Graphics.FillRectangle(SystemBrushes.Control, 32, 148, 32, 32);
    ///        m_ThreadCursor.AniCursor.Draw(p.Graphics, 32, 148);
    ///    }
    /// </example>
    public class ThreadedAniCursor : Microsoft.MSDNMag.AsyncUtils.AsyncOperation
    {
        private IntPtr handle = IntPtr.Zero;
        private AniCursor aniCursor;
        private Size size = new Size(32, 32);
        private Point pos = new Point(0, 0);
        private int interval = 100;

        protected override void DoWork()
        {
            if (this.handle == IntPtr.Zero)
            {
                throw new InvalidOperationException("Handle property must be assigned");
            }

            if (this.aniCursor == null)
            {
                throw new InvalidOperationException("AniCursor object must be assigned");
            }

            while (!CancelRequested)
            {
                OnCursor();
                Thread.Sleep(this.interval);
            }

            if (CancelRequested)
            {
                AcknowledgeCancel();
            }
        }

        protected virtual void OnCursor()
        {
            lock (this)
            {
                Graphics gfx = Graphics.FromHwnd(this.handle);
                aniCursor.Draw(gfx, pos.X, pos.Y, size.Width, size.Height, false);
                gfx.Dispose();
                aniCursor.Step();
            }
        }

        /// <summary>
        /// Gets/sets the interval between frames of the cursor.
        /// </summary>
        public int Interval
        {
            get
            {
                lock (this)
                {
                    return this.interval;
                }
            }
            set
            {
                lock (this)
                {
                    this.interval = value;
                }
            }
        }

        /// <summary>
        /// Gets/sets the size to draw the cursor.
        /// </summary>
        public Size Size
        {
            get
            {
                lock (this)
                {
                    return this.size;
                }
            }
            set
            {
                lock (this)
                {
                    this.size = value;
                }
            }
        }

        /// <summary>
        /// Gets/sets the location in the window to draw
        /// the cursor
        /// </summary>
        public Point Location
        {
            get
            {
                lock (this)
                {
                    return this.pos;
                }
            }
            set
            {
                lock (this)
                {
                    this.pos = value;
                }
            }
        }

        /// <summary>
        /// Gets/sets the window handle to draw onto
        /// </summary>
        public IntPtr Handle
        {
            get
            {
                lock (this)
                {
                    return this.handle;
                }
            }
            set
            {
                lock (this)
                {
                    this.handle = value;
                }
            }
        }

        /// <summary>
        /// Assigns the animated cursor
        /// </summary>
        public AniCursor AniCursor
        {
            get
            {
                return this.aniCursor;
            }
            set
            {
                this.aniCursor = value;
            }
        }

        /// <summary>
        /// Creates a new instance of the Threaded AniCursor class
        /// </summary>
        /// <param name="isi">The object to receive notifications.
        /// Note that only the Cancelled and Failed events will
        /// be fired - this object never completes.</param>
        /// <remarks>Start the cursor running with Start and 
        /// stop it running using Cancel or CancelAndWait.
        /// CancelAndWait ensures that the class has completed
        /// before it returns</remarks>
        public ThreadedAniCursor(ISynchronizeInvoke isi)
            : base(isi)
        {

        }
    }
}