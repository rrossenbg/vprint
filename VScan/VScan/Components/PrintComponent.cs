using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.Drawing;

namespace PremierTaxFree
{
    public partial class PrintComponent : Component
    {
        private Control m_Control;
        private readonly PrintDocument m_Document = new PrintDocument();
        private readonly PrintPreviewDialog m_PrintPreview = new PrintPreviewDialog();

        public PrintComponent()
        {
            InitializeComponent();
            m_Document.PrintPage += new PrintPageEventHandler(OnPrintImage);
        }

        public PrintComponent(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
            m_Document.PrintPage += new PrintPageEventHandler(OnPrintImage);
        }

        public void Print(Control control)
        {
            Debug.Assert(control != null);

            m_Control = control;
            m_Document.Print();
        }

        public void PrintPreview(Control control)
        {
            m_Control = control;
            m_PrintPreview.Document = m_Document;
            m_PrintPreview.ShowDialog();
        }

        public void Setup()
        {
            using (PageSetupDialog dialog = new PageSetupDialog())
            {
                dialog.Document = m_Document;
                dialog.ShowDialog();
            }
        }

        private void OnPrintImage(object o, PrintPageEventArgs e)
        {
            int x = SystemInformation.WorkingArea.X;
            int y = SystemInformation.WorkingArea.Y;
            int width = m_Control.Width;
            int height = m_Control.Height;

            Rectangle bounds = new Rectangle(x, y, width, height);

            using (Bitmap img = new Bitmap(width, height))
            {
                m_Control.DrawToBitmap(img, bounds);
                e.Graphics.DrawImage(img, Point.Empty);
            }
        }
    }
}
