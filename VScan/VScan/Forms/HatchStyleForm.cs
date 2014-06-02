/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace PremierTaxFree.Forms
{
    public partial class HatchStyleForm : Form
    {
        public Color Back_Color
        {
            get
            {
                return backPanel.Color;
            }
            set
            {
                hatchStyleComboBox.Back_Color = backPanel.Color = value;
                Invalidate();
            }
        }

        public Color Fore_Color
        {
            get
            {
                return forePanel.Color;
            }
            set
            {
                hatchStyleComboBox.Fore_Color = forePanel.Color = value;
                Invalidate();
            }
        }

        public HatchStyle Style
        {
            get
            {
                var result = (HatchStyle)hatchStyleComboBox.SelectedItem;
                return result;
            }
            set
            {
                hatchStyleComboBox.SelectedItem = value;
                Invalidate();
            }
        }

        public HatchStyleForm()
        {
            InitializeComponent();
        }

        private void ColorPanel_ColorChanged(object sender, EventArgs e)
        {
            if (sender == forePanel)
                hatchStyleComboBox.Fore_Color = forePanel.Color;
            else if (sender == backPanel)
                hatchStyleComboBox.Back_Color = backPanel.Color;
            hatchStyleComboBox.Invalidate();
        }
    }

    public class HatchStyleComboBox : ComboBox
    {
        public Color Fore_Color { get; set; }

        public Color Back_Color { get; set; }

        protected new object DataSource { get; set; }

        public HatchStyleComboBox()
        {
            this.Fore_Color = Color.Black;
            this.Back_Color = Color.White;

            this.DrawMode = DrawMode.OwnerDrawVariable;
            this.Size = new Size(100, 120);
            this.DropDownStyle = ComboBoxStyle.DropDownList;
            foreach (var e in Enum.GetValues(typeof(HatchStyle)))
                this.Items.Add(e);
            this.SelectedItem = HatchStyle.Cross;
        }

        protected override void OnMeasureItem(MeasureItemEventArgs e)
        {
            e.ItemHeight = 35;
            e.ItemWidth = 35;
            base.OnMeasureItem(e);
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            e.DrawBackground();

            if (e.Index != -1)
            {
                var style = (HatchStyle)Items[e.Index];
                var rect = new Rectangle(2, e.Bounds.Top + 2, e.Bounds.Height, e.Bounds.Height - 4);
                using (var brush = new HatchBrush(style, Fore_Color, Back_Color))
                    e.Graphics.FillRectangle(brush, rect);
            }

            e.DrawFocusRectangle();

            base.OnDrawItem(e);
        }
    }

    public class ColorPanel : Panel
    {
        public Color Color { get; set; }

        public event EventHandler ColorChanged;

        public ColorPanel()
        {
            Color = SystemColors.Control;
            BorderStyle = BorderStyle.FixedSingle;
        }

        protected override void OnClick(EventArgs e)
        {
            using (var f = new ColorDialog())
            {
                if (f.ShowDialog(this.Parent) == DialogResult.OK)
                {
                    Color = f.Color;
                    Invalidate();
                    if (ColorChanged != null)
                        ColorChanged(this, EventArgs.Empty);
                }
            }
            base.OnClick(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(Color);
            base.OnPaint(e);
        }
    }
}
