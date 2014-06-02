/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using PremierTaxFree.Properties;
using PremierTaxFree.PTFLib;


namespace PremierTaxFree.Tools
{
    /// <summary>
    /// Text tool. Draws text over the image.
    /// </summary>
    public class TextTool : BaseTool
    {
        protected Point m_Last;
        private StringBuilder m_Text = new StringBuilder();

        public TextTool(CanvasControl canvas)
            : base(canvas)
        {
        }

        protected override void MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point point = e.Location;
                point.Offset(Offset);
                m_Last = point;
                m_Text = new StringBuilder();
                m_Canvas.Cursor = Resources.letter_t16.CreateCursor(3, 16);
            }
            else
            {
                m_Last = Point.Empty;
                m_Canvas.Cursor = Cursors.Default;
            }
            base.MouseClick(sender, e);
        }

        protected override void KeyPress(object sender, KeyPressEventArgs e)
        {
            if (m_Last != Point.Empty)
            {
                m_Text.Append(e.KeyChar);

                using (Graphics g = Graphics.FromImage(this.m_Canvas.BackgroundImage))
                using (Brush br = new SolidBrush(m_Canvas.ForeColor))
                {
                    g.DrawString(m_Text.ToString(), m_Canvas.Font, br, m_Last);
                }

                m_Canvas.Invalidate();
            }
            base.KeyPress(sender, e);
        }

        protected override void MouseLeave(object sender, EventArgs e)
        {
            m_Last = Point.Empty;
            m_Canvas.Cursor = Cursors.Default;
        }
    }
}
