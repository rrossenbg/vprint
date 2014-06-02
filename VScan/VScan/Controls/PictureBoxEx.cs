/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

#pragma warning disable 642

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace PremierTaxFree.Controls
{
    public class PictureBoxEx : Control
    {
        public bool ShowCurrent { get; set; }

        private Image m_picture = null;

        /// <summary>
        /// Use this to send new image disposingly the old one
        /// </summary>
        public Image Picture
        {
            get
            {
                return m_picture;
            }
            set
            {
                using (m_picture) { }
                m_picture = value;
            }
        }

        /// <summary>
        /// Call this method to prezerve data undisposed
        /// </summary>
        /// <param name="value"></param>
        public void SetPicture(Image value)
        {
            m_picture = value;
        }

        public PictureBoxEx()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint |
                ControlStyles.OptimizedDoubleBuffer, true);
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            //base.OnPaintBackground(pevent);
        }

        protected override void Dispose(bool disposing)
        {
            using (m_picture) ;
            m_picture = null;
            base.Dispose(disposing);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(Color.White);

            if (m_picture != null)
            {
                double largestRatio = Math.Max((double)m_picture.Width / this.ClientSize.Width, (double)m_picture.Height / this.ClientSize.Height);
                float posX = (float)(this.ClientSize.Width * largestRatio / 2 - m_picture.Width / 2);
                float posY = (float)(this.ClientSize.Height * largestRatio / 2 - m_picture.Height / 2);
                using (Matrix mx = new Matrix(1.0f / (float)largestRatio, 0, 0, 1.0f / (float)largestRatio, 0, 0))
                {
                    mx.Translate(posX, posY);
                    e.Graphics.Transform = mx;
                    e.Graphics.DrawImageUnscaled(m_picture, 0, 0);
                    e.Graphics.ResetTransform();
                }
            }

            if (ShowCurrent)
            {
                using (var p = new Pen(Color.Red, 3))
                    e.Graphics.DrawRectangle(p, this.ClientRectangle);
            }
            base.OnPaint(e);
        }
    }
}
