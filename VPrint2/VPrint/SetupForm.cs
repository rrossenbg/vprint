/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace VPrinting
{
    public partial class SetupForm : Form
    {
        protected static SetupForm ms_First = null;

        private bool m_Dragging = false;
        private Point m_Start;
        private Rectangle m_Selection;

        public Image Img
        {
            get
            {
                return this.BackgroundImage;
            }
            set
            {
                this.BackgroundImage = value;

                if (value != null)
                {
                    this.SetClientSizeCore(value.Width, value.Height);
                    this.AutoScrollMinSize = value.Size;
                }
            }
        }

        public Rectangle Selection
        {
            get { return m_Selection; }
            set { m_Selection = value; }
        }

        public bool IsFirst
        {
            get { return ms_First == null; }
        }

        public SetupForm()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            InitializeComponent();
            this.BackgroundImageLayout = ImageLayout.None;
            btnOK.Click += (o, e) => this.Close();
            btnCancel.Click += (o, e) => this.Close();
        }

        private void Delete_Click(object sender, EventArgs e)
        {
            if (this.ShowQuestion("You are to delete cover area?\r\nAre you sure?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Selection = Rectangle.Empty;
                Invalidate();
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            if (ms_First != null)
            {
                MessageBox.Show(this, "This form is already open.\r\nClose it first.", 
                    Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Close();
            }
            else
            {
                ms_First = this;
                base.OnLoad(e);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            ms_First = null;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            m_Dragging = true;
            m_Start = e.Location;
            m_Start.Offset(this.AutoScrollPosition.Invert());
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (m_Dragging)
            {
                var current = e.Location;
                current.Offset(this.AutoScrollPosition.Invert());
                m_Selection = Rectangle.FromLTRB(m_Start.X, m_Start.Y, current.X, current.Y);
                Invalidate();
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (m_Dragging)
                m_Dragging = false;
            base.OnMouseUp(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            Invalidate();
            base.OnMouseWheel(e);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            e.Graphics.Clear(Color.White);
            if (this.BackgroundImage != null)
                e.Graphics.DrawImage(this.BackgroundImage, this.AutoScrollPosition);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (!m_Selection.IsEmpty)
            {
                var p = m_Selection;
                p.Offset(this.AutoScrollPosition);

                using (var b = new HatchBrush(HatchStyle.DiagonalCross, Color.Blue, Color.Transparent))
                    e.Graphics.FillRectangle(b, p);

                e.Graphics.DrawRectangle(Pens.Black, p);
            }

            base.OnPaint(e);
        }
    }
}
