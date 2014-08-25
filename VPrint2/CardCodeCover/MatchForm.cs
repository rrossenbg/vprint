/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CardCodeCover.Common;
using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.VisualBasic;

namespace CardCodeCover
{
    public partial class MatchForm : Form
    {
        public static ThreadExceptionEventHandler Error;

        private TemplateInfo m_MatchTemplate;
        private bool m_bDrag, m_bAddToHiddenArea, m_bCreateTemplate;
        private Point m_StartScreen, m_Start;
        private Rectangle m_OldFrameScreen;

        public TemplateInfo MatchTemplate
        {
            get
            {
                return m_MatchTemplate;
            }
            set
            {
                using (m_MatchTemplate) ;
                m_MatchTemplate = value;
            }
        }

        public string MatchName
        {
            get
            {
                return MatchTemplate.Name;
            }
            set
            {
                MatchTemplate.Name = value;
                Invalidate();
            }
        }

        public Image<Bgr, byte> Template
        {
            get
            {
                return MatchTemplate.Template;
            }
            set
            {
                Debug.Assert(value != null);
                MatchTemplate.Template = value;
                Invalidate();
            }
        }

        public Image<Bgr, byte> Image
        {
            get
            {
                return MatchTemplate.Image;
            }
            set
            {
                Debug.Assert(value != null);
                MatchTemplate.Image = value;
                Invalidate();
            }
        }

        public CoverInfo CoverDetails
        {
            get
            {
                return MatchTemplate.CoverDetails;
            }
        }

        public float ScrollOffset
        {
            get
            {
                return this.Image != null ? this.Image.Height * (this.vScroll.Value + 0.01f) / 100f : 0;
            }
        }

        public MatchForm()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            MatchTemplate = TemplateInfo.Instance;
            this.MouseWheel += new MouseEventHandler(MatchForm_MouseWheel);
        }

        private void AddHiddenAreaMenuItem_Click(object sender, EventArgs e)
        {
            m_bAddToHiddenArea = true;
        }

        private void CreateTemplateMenuItem_Click(object sender, EventArgs e)
        {
            //throw new NotImplementedException("This method is not implemented yet. It will be completed later.");
            m_bCreateTemplate = true;
        }

        private void MatchForm_MouseWheel(object sender, MouseEventArgs e)
        {
            this.vScroll.Value = e.Delta < 0 ? 
                Math.Min(this.vScroll.Value + 10, 100) : 
                Math.Max(this.vScroll.Value - 10, 0);
            Invalidate();
        }

        private void ScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            m_bDrag = true;
            m_Start = e.Location;
            m_StartScreen = this.PointToScreen(e.Location);
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (m_bDrag)
            {
                var loc = this.PointToScreen(e.Location);
                var rec = Rectangle.FromLTRB(m_StartScreen.X, m_StartScreen.Y, loc.X, loc.Y);
                ControlPaint.DrawReversibleFrame(m_OldFrameScreen, Color.PowderBlue, FrameStyle.Dashed);
                ControlPaint.DrawReversibleFrame(rec, Color.PowderBlue, FrameStyle.Dashed);
                m_OldFrameScreen = rec;
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (m_bDrag)
            {
                float offset = ScrollOffset;
                ControlPaint.DrawReversibleFrame(m_OldFrameScreen, Color.PowderBlue, FrameStyle.Dashed);
                if (m_bAddToHiddenArea)
                {
                    var cover = new Cover();
                    var rec = Rectangle.FromLTRB(m_Start.X, m_Start.Y, e.Location.X, e.Location.Y);
                    cover.Rectangle = rec.ScrollOffsetY(-offset);
                    CoverDetails.HiddenAreas.Add(cover);
                }
                else if (m_bCreateTemplate)
                {
                    var rec = Rectangle.FromLTRB(m_Start.X, m_Start.Y, e.Location.X, e.Location.Y);
                    var rec2 = rec.ScrollOffsetY(-offset);
                    Template = Image.Copy(rec2);
                }

                m_bAddToHiddenArea = m_bCreateTemplate = m_bDrag = false;
                m_OldFrameScreen = Rectangle.Empty;
                Invalidate();
            }
            base.OnMouseUp(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            float offset = ScrollOffset;

            if (Image != null)
            {
                if (Monitor.TryEnter(Image, 100))
                {
                    try
                    {
                        e.Graphics.DrawImageUnscaled(Image.Bitmap, new Point(0, -(int)offset));

                        using (var brush = new HatchBrush(HatchStyle.DiagonalCross, Color.Blue, Color.Transparent))
                        {
                            foreach (var cover in CoverDetails.HiddenAreas)
                                e.Graphics.FillRectangle(brush, cover.Rectangle.ScrollOffsetY(offset));

                            if (CoverDetails.HasMatch)
                            {
                                e.Graphics.DrawRectangle(Pens.Red, CoverDetails.Match.ScrollOffsetY(offset));

                                using (var font = new Font("Arial", 9f, FontStyle.Bold))
                                using (var pen = new Pen(Color.BlueViolet, 2f))
                                {
                                    var center = CoverDetails.Match.ScrollOffsetY(offset).Location;

                                    foreach (var cover in CoverDetails.HiddenAreas)
                                    {
                                        var point = cover.Rectangle.ScrollOffsetY(offset).Location;
                                        e.Graphics.DrawLine(pen, center, point);
                                        e.Graphics.DrawString(cover.Distance.ToString("0.00"), font, 
                                            Brushes.BlueViolet, new PointF(point.X + 10, point.Y - 20));
                                    }
                                }
                            }
                        }
                    }
                    finally
                    {
                        Monitor.Exit(Image);
                    }
                }
            }

            if (Template != null)
            {
                if (Monitor.TryEnter(Template, 100))
                {
                    try
                    {
                        e.Graphics.FillRectangle(Brushes.Chocolate, Rectangle.FromLTRB(0, 0,
                            Template.Size.Width + 20,
                            Template.Size.Height + 20));
                        e.Graphics.DrawImageUnscaled(Template.Bitmap, new Point(10, 10));

                        if (!string.IsNullOrWhiteSpace(MatchName))
                            using (var font = new Font("Arial", 15f, FontStyle.Bold))
                                e.Graphics.DrawString(MatchName, font, Brushes.Blue,
                                    new Point(10, Template.Height + 30));
                    }
                    finally
                    {
                        Monitor.Exit(Template);
                    }
                }
            }

            base.OnPaint(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            using (MatchTemplate) ;
            base.OnClosed(e);
        }

        private void ClearMenuItem_Click(object sender, EventArgs e)
        {
            CoverDetails.Clear();
            Invalidate();
        }

        private void ClearMatchMenuItem_Click(object sender, EventArgs e)
        {
            MatchTemplate = TemplateInfo.Instance;
            Invalidate();
        }

        private void LoadImage_MenuItem_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog())
                if (dlg.ShowDialog(this) == DialogResult.OK)
                    this.Image = new Image<Bgr, byte>(dlg.FileName);
        }

        private void LoadTemplate_MenuItem_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog())
                if (dlg.ShowDialog(this) == DialogResult.OK)
                    this.Template = new Image<Bgr, byte>(dlg.FileName);
        }        

        private void SaveMatchMenuItem_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew((o) =>
            {
                try
                {
                    var dbinfo = (DataAccess.TemplateInfoDb)((TemplateInfo)o).Commit();
                    DataAccess.UpdateTemplate(dbinfo);
                    MessageBox.Show("Match saved", Application.ProductName);
                }
                catch (Exception ex)
                {
                    if (Error != null)
                        Error(null, new ThreadExceptionEventArgs(ex));
                }
            }, MatchTemplate);
        }

        private void Match_MenuItem_Click(object sender, EventArgs e)
        {
            if (this.Image == null)
                MessageBox.Show("Image is null");
            else if (this.Template == null)
                MessageBox.Show("Template is null");
            else
            {
                if (string.IsNullOrWhiteSpace(MatchName))
                    MatchName = Interaction.InputBox("Match name", Application.ProductName, DateTime.Now.ToShortDateString());

                Rectangle match = Rectangle.Empty;

                if (!ImageToolsCV.MatchTemplate(MatchTemplate.Image, MatchTemplate.Template, ref match))
                {
                    MessageBox.Show("Cannot match Template");
                }
                else
                {
                    float offset = ScrollOffset;

                    CoverDetails.Match = match;

                    foreach (var cover in CoverDetails.HiddenAreas)
                        cover.Distance = match.Distance(cover.Rectangle);
                }
            }
        }

        private void CloseMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
