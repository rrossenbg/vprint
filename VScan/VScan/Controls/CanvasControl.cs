/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using PremierTaxFree.Controls;
using PremierTaxFree.Extensions;
using PremierTaxFree.PTFLib;
using PremierTaxFree.PTFLib.Data;

using PremierTaxFree.Tools;

namespace PremierTaxFree
{
    public class CanvasControl : Panel
    {
        public readonly List<ImageObj> m_ImgList = new List<ImageObj>();
        public readonly HashSet<ImageObj> m_SelectedImgList = new HashSet<ImageObj>();
        public int LineSize { get; set; }
        public int BrushNum { get; set; }
        public Size EraserSize { get; set; }
        public BaseTool Tool { get; set; }

        /// <summary>
        /// Voucher details
        /// </summary>
        private string m_FileName;
        public string FileName
        {
            get { return m_FileName; }
            set
            {
                m_FileName = value;
                this.InvokeSf(() =>
                {
                    Form f = this.FindForm();
                    if (f != null)
                        f.Text = value;
                });
            }
        }

        public Voucher Data = new Voucher();      

        public CanvasControl()
        {
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.Dock = DockStyle.Top;
            this.AllowDrop = true;

            LineSize = 3;
            BrushNum = 2;
            EraserSize = new Size(10, 10);

            Data.CompressionLevel = SettingsTable.Get<long>(Strings.VScan_CompressionLevel, Consts.DEFAULTCOMPRESSIONLEVEL);
        }

        public void ShowInputControl(string message)
        {
            this.Controls.RemoveByKey(ManualInsertDataControl.MANUALINSERT_DATACONTROL_NAME);
            ManualInsertDataControl cnt = new ManualInsertDataControl();
            cnt.Name = ManualInsertDataControl.MANUALINSERT_DATACONTROL_NAME;
            cnt.Message = message;
            cnt.Location = new Point(10, 10);
            cnt.MinimumSize = new Size(134, 65);
            this.Controls.Add(cnt);
        }

        public void Update(Voucher data)
        {
            if (data == null)
                return;
            //Clean
            m_SelectedImgList.Clear();
            m_ImgList.Clear();

            Data = data;

            using (this.BackgroundImage) ;
            this.BackgroundImage = data.VoucherImage;

            ShowInputControl(data.Message);
        }

        public void DeleteSelectedImages()
        {
            foreach (var obj in m_SelectedImgList)
            {
                var found = m_ImgList.Find((o) => { return o.Id == obj.Id; });
                m_ImgList.Remove(found);
                Invalidate(obj.Rect.InflateEx());
            }
            m_SelectedImgList.Clear();
        }

        public void UnselectSelectedImages()
        {
            foreach (var obj in m_SelectedImgList)
                obj.Selection = SelectionType.None;
            m_SelectedImgList.Clear();
            Invalidate();
        }

        #region Overrides

        private const int POINTS_TO_STEP = 5;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var obj in m_ImgList)
                    obj.Dispose();
            }
            base.Dispose(disposing);
        }

        protected override void OnDragEnter(DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(typeof(HashSet<ImageObj>)) ?
                DragDropEffects.All :
                DragDropEffects.None;

            if (e.Effect != DragDropEffects.All)
                return;

            HashSet<ImageObj> objList = (HashSet<ImageObj>)e.Data.GetData(typeof(HashSet<ImageObj>));

            Debug.Assert(!objList.IsEmpty());

            Rectangle area = Rectangle.Empty.Invalid();

            foreach (var obj in objList)
            {
                area = area.Area(obj.Rect);
            }

            using (Bitmap bitmap = new Bitmap(area.Width, area.Height))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    foreach (var obj in objList)
                    {
                        obj.Paint(g);
                    }

                    Cursor.Current.Draw(g, new Rectangle(objList.First().Offset, Cursor.Current.Size));
                }

                Cursor.Current = bitmap.CreateCursor(0, 0);
            }
            base.OnDragEnter(e);
        }

        protected override void OnDragDrop(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(HashSet<ImageObj>)))
            {
                HashSet<ImageObj> objList = (HashSet<ImageObj>)e.Data.GetData(typeof(HashSet<ImageObj>));

                if (!objList.IsEmpty())
                {
                    foreach (var obj in objList)
                    {
                        obj.Resize(new Rectangle(this.PointToClient(Control.MousePosition), obj.Rect.Size));
                        m_ImgList.Add(obj);
                        Invalidate(obj.Rect);
                    }
                    Cursor.SetOffset(objList.First().Offset);
                }
            }
            base.OnDragDrop(e);
        }

        protected override void OnGiveFeedback(GiveFeedbackEventArgs e)
        {
            e.UseDefaultCursors = e.Effect == DragDropEffects.None;
            base.OnGiveFeedback(e);
        }

        protected override void OnScroll(ScrollEventArgs se)
        {
            Invalidate();
            base.OnScroll(se);
        }

        protected override void OnInvalidated(InvalidateEventArgs e)
        {
            if (this.BackgroundImage != null && this.AutoScrollMinSize != this.BackgroundImage.Size)
                this.AutoScrollMinSize = this.BackgroundImage.Size;
            base.OnInvalidated(e);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //Don't uncomment
            //base.OnPaintBackground(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(Color.White);

            if (this.BackgroundImage != null)
            {
                if(Monitor.TryEnter(this.BackgroundImage, TimeSpan.FromMilliseconds(100)))
                {
                    e.Graphics.DrawImage(this.BackgroundImage, this.AutoScrollPosition);
                    Monitor.Exit(this.BackgroundImage);
                }
            }

            foreach (var obj in m_ImgList)
            {
                obj.Paint(e.Graphics);
            }

            base.OnPaint(e);
        }

        #endregion
    }
}