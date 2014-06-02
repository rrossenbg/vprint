/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Drawing;
using System.Windows.Forms;

namespace System.Windows.Forms
{
    public partial class DockableUserControl : UserControl
    {
        public DockableUserControl()
        {
            this.SetStyle(ControlStyles.ResizeRedraw , true);
            InitializeComponent();
        }

        private class Consts
        {
            public const int WM_NCHITTEST = 0x0084;
            public const int OFFSET = 5;
            public const int MOUSEPOINTER_MASK = 0xFFFF;
            public const int FrameHeight = 20;
            public const int ButtonsTop = 6;
            public static readonly Padding Padding_Dockable = new Padding(0, 22, 0, 0);
            public static readonly Padding Padding_Floating = new Padding(0);
            public static readonly Size InnerRectangleInflate = new Size(-3, -3);
            public static readonly Size ButtonSize = new Size(10, 10);
            public static readonly Padding Margin = new Padding(3);
            public static readonly Size MinimumSize = new Size(50, 50);
        }

        [Flags]
        protected enum AllowDirections
        {
            None = 0,
            Left = 1,
            Right = 2,
            Top = 4,
            Bottom = 8,
            LeftRight = Left | Right,
            TopBottom = Top | Bottom,
            All = Left | Right | Top | Bottom,
            Drag = 127,
        }

        protected enum HitTestResult
        {
            HTERROR = -2,
            HTTRANSPARENT = -1,
            HTNOWHERE = 0,
            HTCLIENT = 1,
            HTCAPTION = 2,
            HTSYSMENU = 3,
            HTGROWBOX = 4,
            HTMENU = 5,
            HTHSCROLL = 6,
            HTVSCROLL = 7,
            HTMINBUTTON = 8,
            HTMAXBUTTON = 9,
            HTLEFT = 10,
            HTRIGHT = 11,
            HTTOP = 12,
            HTTOPLEFT = 13,
            HTTOPRIGHT = 14,
            HTBOTTOM = 15,
            HTBOTTOMLEFT = 16,
            HTBOTTOMRIGHT = 17,
            HTBORDER = 18,
            HTOBJECT = 19,
            HTCLOSE = 20,
            HTHELP = 21,
        }

        private bool showPointers = false, canChangeSize = true;

        public bool CanReSize
        {
            set
            {
                this.canChangeSize = value;
            }
        }

        public bool ShowPointers
        {
            get
            {
                return this.showPointers;
            }
            set
            {
                this.showPointers = value;
                Invalidate();
            }
        }

        protected AllowDirections ALLOWRESIZEDIRECTIONS = AllowDirections.All;

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == Consts.WM_NCHITTEST)
            {
                if (this.canChangeSize)
                {
                    Point mousePointer = new Point((int)m.LParam & Consts.MOUSEPOINTER_MASK, (int)m.LParam >> 16 & Consts.MOUSEPOINTER_MASK);
                    mousePointer = PointToClient(mousePointer);

                    #region KnowHow

                    if (mousePointer.X <= Consts.OFFSET && IsDirectionPermited(AllowDirections.Left))
                    {
                        if (mousePointer.Y <= Consts.OFFSET && IsDirectionPermited(AllowDirections.Top))
                        {
                            m.Result = (IntPtr)HitTestResult.HTTOPLEFT;
                            return;
                        }
                        else
                        {
                            if (mousePointer.Y >= ClientSize.Height - Consts.OFFSET && IsDirectionPermited(AllowDirections.Bottom))
                            {
                                m.Result = (IntPtr)HitTestResult.HTBOTTOMLEFT;
                                return;
                            }
                            else
                            {
                                m.Result = (IntPtr)HitTestResult.HTLEFT;
                                return;
                            }
                        }
                    }
                    else
                    {
                        if (mousePointer.X >= ClientSize.Width - Consts.OFFSET && IsDirectionPermited(AllowDirections.Right))
                        {
                            if (mousePointer.Y <= Consts.OFFSET && IsDirectionPermited(AllowDirections.Top))
                            {
                                m.Result = (IntPtr)HitTestResult.HTTOPRIGHT;
                                return;
                            }
                            else
                            {
                                if (mousePointer.Y >= ClientSize.Height - Consts.OFFSET && IsDirectionPermited(AllowDirections.Bottom))
                                {
                                    m.Result = (IntPtr)HitTestResult.HTBOTTOMRIGHT;
                                    return;
                                }
                                else
                                {
                                    m.Result = (IntPtr)HitTestResult.HTRIGHT;
                                    return;
                                }
                            }
                        }
                        else
                        {
                            if (mousePointer.Y <= Consts.OFFSET && IsDirectionPermited(AllowDirections.Top))
                            {
                                m.Result = (IntPtr)HitTestResult.HTTOP;
                                return;
                            }
                            else
                            {
                                if (mousePointer.Y >= ClientSize.Height - Consts.OFFSET && IsDirectionPermited(AllowDirections.Bottom))
                                {
                                    m.Result = (IntPtr)HitTestResult.HTBOTTOM;
                                    return;
                                }
                            }
                        }
                    }

                    if (this.DisplayRectangle.Contains(mousePointer) && IsDirectionPermited(AllowDirections.Drag))
                    {
                        m.Result = (IntPtr)HitTestResult.HTCAPTION;
                        return;
                    }
                }
                    #endregion
            }
        }

        private bool IsDirectionPermited(AllowDirections direction)
        {
            return ((this.ALLOWRESIZEDIRECTIONS & direction) == direction);
        }

        bool m_dragging = false;

        Point m_StartPoint;

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                m_dragging = true;
                Cursor = Cursors.SizeAll;
                m_StartPoint = e.Location;
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (m_dragging)
            {
                Point p = this.Parent.PointToClient(Control.MousePosition);
                p.Offset(-m_StartPoint.X, -m_StartPoint.Y);
                this.Location = p;
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            m_dragging = false;
            Cursor = Cursors.Default;
            base.OnMouseUp(e);
        }

        protected override void OnResize(EventArgs e)
        {
            if (Size.Width < MinimumSize.Width)
                SetBoundsCore(Location.X, Location.Y, MinimumSize.Width, Size.Height, BoundsSpecified.All);

            if (Size.Height < MinimumSize.Height)
                SetBoundsCore(Location.X, Location.Y, Size.Width, MinimumSize.Height, BoundsSpecified.All);

            base.OnResize(e);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            //
            // DockableUserControl
            //
            this.Name = "DockableUserControl";
            this.Size = new System.Drawing.Size(315, 314);
            this.ResumeLayout(false);
        }
    }
}