/***************************************************
//  Copyright (c) Premium Tax Free 2011
***************************************************/

using System.Drawing;
using System.Windows.Forms;

namespace PremierTaxFree.Controls
{
    public class TTabControl : TabControl
    {
        public TTabControl()
        {
            InitializeComponent();
            this.DrawMode = TabDrawMode.OwnerDrawFixed;
            this.BackColor = Color.Transparent;             
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            Font font = null;

            using (Brush backBrush = new SolidBrush(e.BackColor))
            {
                Brush foreBrush = null;

                if (e.Index == this.SelectedIndex)
                {
                    font = new Font(e.Font, FontStyle.Bold);
                    foreBrush = new SolidBrush(Color.White);
                }
                else
                {
                    font = new Font(e.Font, FontStyle.Regular);
                    foreBrush = new SolidBrush(e.ForeColor);
                }

                string tabName = this.TabPages[e.Index].Text;

                using (StringFormat sf = new StringFormat())
                {
                    sf.Alignment = StringAlignment.Center;
                    e.Graphics.FillRectangle(backBrush, e.Bounds);
                    Rectangle r = new Rectangle(e.Bounds.X, e.Bounds.Y + 3, e.Bounds.Width, e.Bounds.Height - 3);
                    e.Graphics.DrawString(tabName, font, foreBrush, r, sf);
                }

                using (font)
                using (foreBrush)
                {
                }
            }

            base.OnDrawItem(e);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ResumeLayout(false);
        }
    }
}
