/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System.Drawing;
using System.Windows.Forms;

namespace PremierTaxFree.Tools
{
    /// <summary>
    /// Eraser tool class
    /// </summary>
    public class EraserTool : BaseTool
    {
        public EraserTool(CanvasControl canvas)
            : base(canvas)
        {

        }

        protected override void MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                Draw(e);
        }

        protected override void MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                Draw(e);
        }

        private void Draw(MouseEventArgs e)
        {
            Point p = e.Location;
            p.Offset(Offset);

            using (Graphics g = Graphics.FromImage(m_Canvas.BackgroundImage))
            using (Brush brush = new SolidBrush(m_Canvas.BackColor))
            {
                g.FillRectangle(brush, p.X, p.Y, m_Canvas.LineSize, m_Canvas.LineSize);
            }

            m_Canvas.Invalidate();
        }
    }
}
