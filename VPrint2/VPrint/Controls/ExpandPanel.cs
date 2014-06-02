using System;
using System.Windows.Forms;
using System.Drawing;

namespace VPrinting.Controls
{
    public class ExpandPanel : Panel
    {
        private int m_Height;

        protected Rectangle m_Rectangle;

        public ExpandPanel()
        {
            m_Rectangle = new Rectangle(0, 0, 20, 20);
        }

        protected override void OnParentChanged(EventArgs e)
        {
            if (this.Parent != null)
                m_Height = this.Height;
            base.OnParentChanged(e);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (m_Rectangle.Contains(e.Location))
            {
                if (this.Height == m_Rectangle.Height)
                    this.Height = m_Height;
                else
                    this.Height = m_Rectangle.Height;
            }

            base.OnMouseClick(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            ControlPaint.DrawButton(e.Graphics, m_Rectangle, ButtonState.Normal);
            base.OnPaint(e);
        }
    }
}
