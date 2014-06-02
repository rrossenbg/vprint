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
    /// Poly pen tool. Draws polio lines over image.
    /// </summary>
    public class PolyPenTool : BaseTool
    {
        private Point m_First, m_Last;

        public PolyPenTool(CanvasControl canvas)
            : base(canvas)
        {
        }

        protected override void MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (m_First == Point.Empty)
                {
                    Point p = e.Location;
                    p.Offset(Offset);
                    m_First = p;
                    m_Canvas.Cursor = Resources.pencil_red16.CreateCursor(3, 16);
                }
                else
                {
                    using (Graphics g = Graphics.FromImage(m_Canvas.BackgroundImage))
                    using (Pen pen = new Pen(m_Canvas.BackColor, m_Canvas.LineSize))
                        g.DrawLine(pen, m_First, m_Last);

                    m_Canvas.Invalidate();
                    m_First = m_Last;
                }
            }
            else
            {
                m_First = Point.Empty;
                m_Last = Point.Empty;
                m_Canvas.Cursor = Cursors.Default;
            }
        }

        protected override void MouseMove(object sender, MouseEventArgs e)
        {
            if (m_First != Point.Empty)
            {
                Point p = e.Location;
                p.Offset(Offset);
                m_Last = p;
            }
        }

        protected override void DoubleClick(object sender, EventArgs e)
        {
            m_First = Point.Empty;
            m_Last = Point.Empty;
            m_Canvas.Cursor = Cursors.Default;
        }

        protected override void MouseLeave(object sender, EventArgs e)
        {
            m_First = Point.Empty;
            m_Last = Point.Empty;
            m_Canvas.Cursor = Cursors.Default;
        }
    }
}
