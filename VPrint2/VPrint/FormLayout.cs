/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using VPrinting.Extentions;

namespace VPrinting
{
    public partial class FormLayout : Form
    {
        protected Point Offset
        {
            get
            {
                return new Point(-this.HorizontalScroll.Value, -this.VerticalScroll.Value);
            }
        }

        protected AllocationDocumentLayout m_DocumentLayout = new AllocationDocumentLayout();
        protected readonly PrintDocument m_PrintDocument = new PrintDocument();

        private IImageObject m_SelectImageObject = null;

        private Point m_CurrentLocation;

        public FormLayout()
        {
            SetStyle(   ControlStyles.UserPaint |
                        ControlStyles.ResizeRedraw | 
                        ControlStyles.OptimizedDoubleBuffer |
                        ControlStyles.AllPaintingInWmPaint, true);

            InitializeComponent();
            m_PrintDocument.PrintPage += new PrintPageEventHandler(PrintDocument_PrintPage);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point p = this.PointToClient(Control.MousePosition);
                p.Offset(Offset);

                foreach (var obj in m_DocumentLayout.MetaObjectsList)
                {
                    obj.Selected = obj.Contains(p);
                    if (obj.Selected)
                        m_SelectImageObject = obj;
                }

                Invalidate();
            }
            base.OnMouseDown(e);
        }

        protected override void OnBackgroundImageChanged(EventArgs e)
        {
            if (this.BackgroundImage != null)
            {
                this.AutoScrollMinSize = this.BackgroundImage.Size;
                GraphicsUnit unit = GraphicsUnit.Point;
                RectangleF rect = this.BackgroundImage.GetBounds(ref unit);
            }

            base.OnBackgroundImageChanged(e);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //base.OnPaintBackground(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            const int MOVE = 20, MOVESLOW = 5;

            if (m_SelectImageObject != null)
            {
                Point p =  m_SelectImageObject.Location;
                switch (e.KeyCode)
                {
                    case Keys.Left:
                        p.Offset(-MOVE, 0);
                        e.Handled = true;
                        break;
                    case Keys.Right:
                        p.Offset(MOVE, 0);
                        e.Handled = true;
                        break;
                    case Keys.Up:
                        p.Offset(0, -MOVE);
                        e.Handled = true;
                        break;
                    case Keys.Down:
                        p.Offset(0, MOVE);
                        e.Handled = true;
                        break;

                    case Keys.NumPad4:
                        p.Offset(-MOVESLOW, 0);
                        e.Handled = true;
                        break;
                    case Keys.NumPad6:
                        p.Offset(MOVESLOW, 0);
                        e.Handled = true;
                        break;
                    case Keys.NumPad8:
                        p.Offset(0, -MOVESLOW);
                        e.Handled = true;
                        break;
                    case Keys.NumPad2:
                        p.Offset(0, MOVESLOW);
                        e.Handled = true;
                        break;

                    default:
                        break;
                }
                m_SelectImageObject.Location = p;
                Invalidate();
            }
            base.OnKeyUp(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(Color.Wheat);

            if (this.BackgroundImage != null)
                e.Graphics.DrawImageUnscaled(this.BackgroundImage, Offset);

            foreach (var obj in m_DocumentLayout.MetaObjectsList)
                obj.Draw(e.Graphics, Offset, DrawingSurface.Screen);

            base.OnPaint(e);
        }

        protected override void OnScroll(ScrollEventArgs e)
        {
            base.OnScroll(e);
            Invalidate();
        }

        private void AddDelete_Click(object sender, EventArgs e)
        {
            if (sender == tsmiAdd)
            {
                IImageObject point = PrintObjectForm.ShowCreate(this);
                if (point != null)
                {
                    point.Location = m_CurrentLocation;
                    m_DocumentLayout.MetaObjectsList.Add(point);
                    Invalidate();
                }
            }
            else if (sender == tsmiDelete)
            {
                if (m_SelectImageObject != null && m_DocumentLayout.MetaObjectsList.Remove(m_SelectImageObject))
                    Invalidate();
                m_SelectImageObject = null;
            }
            else if (sender == tsmiEdit)
            {
                if (m_SelectImageObject != null && PrintObjectForm.ShowEdit(this, m_SelectImageObject))
                    Invalidate();
            }
            else
                throw new NotImplementedException();
        }

        private void LoadImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = Strings.ImageFilter;
                dlg.FilterIndex = 1;
                dlg.CheckFileExists = true;
                dlg.Multiselect = false;

                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        this.BackgroundImage = this.BackgroundImage.DisposeSf();
                        this.BackgroundImage = Image.FromFile(dlg.FileName);
                    }
                    catch (Exception ex)
                    {
                        ex.ShowDialog(this);
                    }
                }
            }
        }

        private void LoadLayout_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = Strings.LayoutFilter;
                dlg.DefaultExt = Strings.LayoutDefaultExt; ;
                dlg.CheckFileExists = true;
                dlg.Multiselect = false;

                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        using (var file = dlg.OpenFile())
                        {
                            m_DocumentLayout = (AllocationDocumentLayout)formatter.Deserialize(file);
                            m_PrintDocument.DefaultPageSettings.PaperSize = new PaperSize("Custom", m_DocumentLayout.PaperSize.Width, m_DocumentLayout.PaperSize.Height);
                            this.BackgroundImage = this.BackgroundImage.DisposeSf();
                            this.BackgroundImage = m_DocumentLayout.DocumentImage;
                        }
                        Invalidate();
                    }
                    catch (Exception ex)
                    {
                        ex.ShowDialog(this);
                    }
                }
            }
        }

        private void SaveLayout_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.Filter = Strings.LayoutFilter;
                dlg.DefaultExt = Strings.LayoutDefaultExt;
                dlg.AddExtension = true;

                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        using (var file = dlg.OpenFile())
                        {
                            m_PrintDocument.DefaultPageSettings.PaperSize = this.BackgroundImage.ToPaperSize("Custom");
                            m_DocumentLayout.DocumentImage = this.BackgroundImage;
                            formatter.Serialize(file, m_DocumentLayout);
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.ShowDialog(this);
                    }
                }
            }
        }

        private void Clear_Click(object sender, EventArgs e)
        {
            m_DocumentLayout.MetaObjectsList.Clear();
            this.BackgroundImage = this.BackgroundImage.DisposeSf();
            Invalidate();
        }

        private void Print_Click(object sender, EventArgs e)
        {
            m_PrintDocument.DefaultPageSettings.PaperSize = this.BackgroundImage.ToPaperSize("Custom");

            using (PrintDialog dlg = new PrintDialog())
            {
                dlg.Document = m_PrintDocument;
                if (dlg.ShowDialog(this) == DialogResult.OK)
                    m_PrintDocument.Print();
            }
        }

        private void PrintPreview_Click(object sender, EventArgs e)
        {
            m_PrintDocument.DefaultPageSettings.PaperSize = this.BackgroundImage.ToPaperSize("Custom");

            using (PrintPreviewDialog dialog = new PrintPreviewDialog())
            {
                dialog.Document = m_PrintDocument;
                dialog.ShowDialog(this);
            }
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ContextMenu_Opening(object sender, CancelEventArgs e)
        {
            m_SelectImageObject = null;
            m_CurrentLocation = this.PointToClient(Control.MousePosition);
            m_CurrentLocation.Offset(Offset);

            foreach (var obj in m_DocumentLayout.MetaObjectsList)
            {
                if (obj.Contains(m_CurrentLocation))
                {
                    m_SelectImageObject = obj;
                    break;
                }
            }
        }

        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            foreach (var obj in m_DocumentLayout.MetaObjectsList)
                obj.Draw(e.Graphics, Point.Empty, DrawingSurface.Printer);
        }
    }
}