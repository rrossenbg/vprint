/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Drawing;
using System.Windows.Forms;
using PremierTaxFree.Properties;
using PremierTaxFree.PTFLib;

namespace PremierTaxFree.Tools
{
    /// <summary>
    /// Pen tool class. Draws dots on image
    /// </summary>
    public class PenTool : BaseTool
    {
        protected Point m_Last;

        public PenTool(CanvasControl cnt)
            : base(cnt)
        {
        }

        protected override void MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point point = e.Location;
                point.Offset(Offset);
                m_Last = point;
                m_Canvas.Cursor = Resources.pencil16.CreateCursor(3, 16);
            }
            else
            {
                m_Last = Point.Empty;
                m_Canvas.Cursor = Cursors.Default;
            }
            base.MouseDown(sender, e);
        }

        protected override void MouseMove(object sender, MouseEventArgs e)
        {
            if (m_Last != Point.Empty)
            {
                Point point = e.Location;
                point.Offset(Offset);

                using (Graphics g = Graphics.FromImage(this.m_Canvas.BackgroundImage))
                using (Pen pen = new Pen(this.m_Canvas.BackColor, this.m_Canvas.LineSize))
                    g.DrawLine(pen, m_Last, point);

                m_Last = point;

                this.m_Canvas.Invalidate();
            }

            base.MouseMove(sender, e);
        }

        protected override void MouseUp(object sender, MouseEventArgs e)
        {
            m_Canvas.Cursor = Cursors.Default;
            m_Last = Point.Empty;
            base.MouseUp(sender, e);
        }

        protected override void MouseLeave(object sender, EventArgs e)
        {
            m_Canvas.Cursor = Cursors.Default;
            m_Last = Point.Empty;
            base.MouseLeave(sender, e);
        }
    }
}
