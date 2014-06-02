/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DTKBarReader;
using PremierTaxFree.Data;
using PremierTaxFree.Extensions;
using PremierTaxFree.PTFLib;
using PremierTaxFree.PTFLib.Data;

using PremierTaxFree.PTFLib.Security;

namespace PremierTaxFree.Forms
{
    public partial class CardCodeRegionsForm : Form 
    {
        private enum SelectionMode
        {
            CardCodeArea,
            PrintArea
        }

        private Rectangle m_BarcodeArea;
        private Rectangle m_AreaToHide, m_PrintArea;
        private SelectionMode m_SelectionMode = SelectionMode.CardCodeArea; 
        private bool m_dirty = false;

        private bool m_dragging;
        private Point m_Start, m_Prev;
        private string m_FileName;

        protected Point Offset
        {
            get
            {
                return new Point(-this.HorizontalScroll.Value, -this.VerticalScroll.Value);
            }
        }

        public CardCodeRegionsForm()
        {
            SetStyle(ControlStyles.UserPaint |
                ControlStyles.ResizeRedraw |
                ControlStyles.AllPaintingInWmPaint, true);
            InitializeComponent();
        }

        private void Refresh_Click(object sender, EventArgs e)
        {
            RefreshAsync();
        }

        private void RefreshAsync()
        {
            m_dirty = false;
            new MethodInvoker(() =>
            {
                var layout = SettingsTable.Get<VoucherLayout>(Strings.VScan_VoucherLayout);
                if (layout != null)
                {
                    this.InvokeSafe(new Action<VoucherLayout>((l) =>
                    {
                        this.BackgroundImage = l.Background;
                        this.m_BarcodeArea = l.BarcodeArea;
                        this.m_AreaToHide = l.CardcodeArea;
                        this.m_PrintArea = l.PrintArea;
                    }), layout);
                }
                else
                {
                    var file = ClientDataAccess.SelectFileMax();
                    if (file != null)
                    {
                        this.InvokeSafe(new MethodInvoker(() =>
                        {
                            Image image = file.VoucherImage.ToImage();
                            this.BackgroundImage = image;
                            this.AutoScrollMinSize = image.Size;
                        }));
                    }
                    else
                    {
                        //Do not change this line
                        MessageBox.Show(
                            "Could not find any voucher into the database.\r\n" +
                            "Use 'Foad file' and load a sample image.", Application.ProductName, 
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
            }
                ).FireAndForget();
        }

        private void ReadBarCode_Click(object sender, EventArgs e)
        {
            if (this.BackgroundImage == null)
                return;

            var bmp = new Bitmap(this.BackgroundImage);
            new MethodInvoker(() =>
            {
                Barcode[] readBarCodes = null;
                Voucher data = new Voucher();

                lock (typeof(BarcodeReader))
                {
                    BarcodeReader reader = new BarcodeReader(Strings.VScan_BarcodeReaderSDKDeveloperLicenseKey);
                    reader.LicenseManager.AddLicenseKey(Strings.VScan_BarcodeReaderSDKUnlimitedRuntimeLicenseKey);
                    reader.BarcodesToRead = 1;
                    reader.BarcodeTypes = BarcodeTypeEnum.BT_Inter2of5;
                    readBarCodes = reader.ReadFromBitmapRotateAll(ref bmp, data);
                }

                bmp.DisposeSf();

                if (readBarCodes.Length == 0)
                    throw new AppExclamationException("Can not find barcode");
                else if (readBarCodes.Length > 1)
                    throw new AppExclamationException("Too many barcodes");
                m_BarcodeArea = data.BarCodeArea;
                data.DisposeSf();
                Invalidate();

            }).FireAndForget();
        }

        private void ContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            tsmiSave.Enabled = (m_AreaToHide != Rectangle.Empty && m_BarcodeArea != Rectangle.Empty && m_PrintArea != Rectangle.Empty);
            tsmiAreaSelectionMode.Text = m_SelectionMode != SelectionMode.PrintArea ? "&Print area" : "&Cardcode area";
        }

        private void AreaSelectionMode_Click(object sender, EventArgs e)
        {
            m_SelectionMode = m_SelectionMode.Invert(SelectionMode.CardCodeArea, SelectionMode.PrintArea);
        }

        private void DrawFrame()
        {
            if (m_Prev != Point.Empty)
            {
                Rectangle rec = Rectangle.FromLTRB(m_Start.X, m_Start.Y, m_Prev.X, m_Prev.Y);
                ControlPaint.DrawReversibleFrame(rec, Color.Green, FrameStyle.Dashed);
            }
        }

        private void Load_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.FileName = m_FileName;
                dlg.Multiselect = false;
                dlg.Filter = Strings.VScan_ImageFilter;
                dlg.CheckFileExists = true;

                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        Image image = Image.FromFile(dlg.FileName);
                        m_FileName = dlg.FileName;
                        this.BackgroundImage.DisposeSf();
                        this.BackgroundImage = image;
                        this.AutoScrollMinSize = image.Size;
                    }
                    catch (Exception ex)
                    {
                        ex.ShowDialog(this);
                    }
                }
            }
        }

        private void Save_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.FileName = m_FileName;
                dlg.Filter = Strings.VScan_ImageFilter;
                dlg.FilterIndex = 1;
                dlg.CheckPathExists = true;
                dlg.AddExtension = true;

                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    try
                    {
                        File.WriteAllBytes(dlg.FileName, this.BackgroundImage.ToArray(50L));
                    }
                    catch (Exception ex)
                    {
                        ex.ShowDialog(this);
                    }
                }
            }
        }

        private void SaveResults_Click(object sender, EventArgs e)
        {
            if (m_BarcodeArea != Rectangle.Empty)
            {
                var bottomLeft = new Point(m_BarcodeArea.Left, m_BarcodeArea.Bottom);
                var distanceToHiddenArea = Point.Subtract(m_AreaToHide.Location, new Size(bottomLeft));

                SettingsTable.Set(Strings.VScan_DistanceFromBarcodeBottomLeftToHiddenArea, distanceToHiddenArea);
                SettingsTable.Set(Strings.VScan_HiddenAreaSize, m_AreaToHide.Size);
                SettingsTable.Set(Strings.VScan_PrintAreaLocation, m_PrintArea.Location);
                SettingsTable.Set(Strings.VScan_VoucherLayout, new VoucherLayout()
                {
                    Background = this.BackgroundImage,
                    BarcodeArea = this.m_BarcodeArea,
                    CardcodeArea = this.m_AreaToHide,
                    PrintArea = this.m_PrintArea,
                });

                m_dirty = false;
            }
        }

        private void Close_Click(object sender, EventArgs e)
        {
            Close();
        }

        protected override void OnLoad(EventArgs e)
        {
            RefreshAsync();
            base.OnLoad(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                m_dirty = true;
                m_dragging = true;
                m_Start = Control.MousePosition;
            }
            else
            {
                m_dragging = false;
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (m_dragging)
            {
                DrawFrame();
                m_Prev = Control.MousePosition;
                DrawFrame();
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            DrawFrame();
            m_dragging = false;
            m_Prev = Point.Empty;
            base.OnMouseLeave(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            DrawFrame();

            if (m_dragging)
            {
                Point m = Control.MousePosition;
                var r1 = Rectangle.FromLTRB(m_Start.X, m_Start.Y, m.X, m.Y);
                var r2 = this.RectangleToClient(r1);
                r2.Offset(Offset.Invert());

                if (m_SelectionMode == SelectionMode.CardCodeArea)
                    m_AreaToHide = r2;
                else
                    m_PrintArea = r2;

                Invalidate();
            }

            m_dragging = false;
            m_Prev = Point.Empty;

            base.OnMouseUp(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (m_dirty)
            {
                var result = MessageBox.Show(this, "Results are not saved.\r\nWould you like to save the results?",
                    Application.ProductName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                    tsmiSave.PerformClick();

                e.Cancel = result == DialogResult.Cancel;
            }
            base.OnClosing(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(Color.Wheat);

            base.OnPaint(e);

            if (this.BackgroundImage != null)
                e.Graphics.DrawImageUnscaled(this.BackgroundImage, this.Offset);

            if (m_AreaToHide != Rectangle.Empty)
            {
                var r = m_AreaToHide;
                r.Offset(Offset);
                e.Graphics.FillRectangleHatch(r, Color.Green, Color.Transparent);
                e.Graphics.DrawStringInRectangle(r, "HIDE AREA", SystemFonts.CaptionFont, Brushes.Red);
            }

            if (m_PrintArea != Rectangle.Empty)
            {
                var r = m_PrintArea;
                r.Offset(Offset);
                e.Graphics.FillRectangleHatch(r, Color.Blue, Color.Transparent);
                e.Graphics.DrawStringInRectangle(r, "PRINT AREA", SystemFonts.CaptionFont, Brushes.Green);
            }

            if (m_BarcodeArea != Rectangle.Empty)
            {
                var r = m_BarcodeArea;
                r.Offset(Offset);
                e.Graphics.FillRectangleHatch(r, Color.Red, Color.Transparent);
                e.Graphics.DrawStringInRectangle(r, "BARCODE AREA", SystemFonts.CaptionFont, Brushes.Blue);
            }

            if (m_AreaToHide != Rectangle.Empty && m_BarcodeArea != Rectangle.Empty)
            {
                var r1 = m_AreaToHide;
                r1.Offset(Offset);
                var r2 = m_BarcodeArea;
                r2.Offset(Offset);
                r2.Offset(0, m_BarcodeArea.Height);
                using (var p = new Pen(Color.Yellow, 3))
                {
                    e.Graphics.DrawLine(p, r1.Location, r2.Location);
                }
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //
        }

        protected override void OnScroll(ScrollEventArgs e)
        {
            base.OnScroll(e);
            Invalidate();
        }
    }
}
