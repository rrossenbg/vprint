/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace PremierTaxFree.Controls
{
    public class MultyColorComboBox : ComboBox
    {
        private readonly Hashtable m_ItemForeColors = new Hashtable();

        public Color DefaultItemForeColor { get; set; }

        public MultyColorComboBox()
        {
            DefaultItemForeColor = Color.Black;
            this.DrawMode = DrawMode.OwnerDrawFixed;
        }

        public void SetItemColor(int itemIndex, Color color)
        {
            if (0 <= itemIndex && itemIndex < Items.Count)
            {
                object item = Items[itemIndex];
                m_ItemForeColors[item] = color;
            }
        }

        public void Clear()
        {
            Items.Clear();
            m_ItemForeColors.Clear();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            e.DrawBackground();

            if (0 <= e.Index && e.Index < Items.Count)
            {
                object item = Items[e.Index];
                string text = item.ToString();

                Color color = (Color)(m_ItemForeColors[item] ?? DefaultItemForeColor);

                using (Brush brush = new SolidBrush(color))
                    e.Graphics.DrawString(text, Font, brush, e.Bounds.X, e.Bounds.Y);
            }
            base.OnDrawItem(e);
        }
    }
}
