/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.VisualBasic;
using VCover.Common;

namespace VCover
{
    public partial class MatchForm : Form
    {
        public static ThreadExceptionEventHandler Error;

        private static volatile bool m_Result = false;
        private MatchTemplate m_MatchTemplate;
        private bool m_bDrag, m_bAddToHiddenArea, m_bCreateTemplate;
        private Point m_StartScreen, m_Start;
        private Rectangle m_OldFrameScreen;

        public MatchTemplate MTemplate
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
                return MTemplate.Name;
            }
            set
            {
                MTemplate.Name = value;
                Invalidate();
            }
        }

        public Image<Bgr, byte> Template
        {
            get
            {
                return MTemplate.Cover.Template;
            }
            set
            {
                Debug.Assert(value != null);
                MTemplate.Cover.Template = value;
                Invalidate();
            }
        }

        public Image<Bgr, byte> Image 
        {
            get
            {
                return MTemplate.Image;
            }
            set
            {
                Debug.Assert(value != null);
                MTemplate.Image = value;
                Invalidate();
            }
        }

        public TemplateInfo CoverDetails
        {
            get
            {
                return MTemplate.Cover;
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
            MTemplate = new MatchTemplate();
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            this.MouseWheel += new MouseEventHandler(MatchForm_MouseWheel);
        }

        public static bool Run(string imageFullFileName)
        {
            using (var done = new ManualResetEventSlim(false))
            {
                Task.Factory.StartNew((o) =>
                {
                    Tuple<ManualResetEventSlim, string> ev = (Tuple<ManualResetEventSlim, string>)o;
                    try
                    {
                        var form = new MatchForm();
                        form.Image = new Image<Bgr, byte>(ev.Item2);
                        Application.Run(form);
                    }
                    finally
                    {
                        ev.Item1.Set();
                    }
                }, new Tuple<ManualResetEventSlim, string>(done, imageFullFileName), TaskCreationOptions.LongRunning);

                done.Wait();
            }
            return m_Result;
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

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                m_Result = false;
                this.Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
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
                    var cover = new HiddenAreaInfo();
                    var rec = Rectangle.FromLTRB(m_Start.X, m_Start.Y, e.Location.X, e.Location.Y);
                    cover.Rectangle = rec.ScrollOffsetY(-offset);
                    MTemplate.HiddenAreas.Add(cover);
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
                            foreach (var cover in MTemplate.HiddenAreas)
                                e.Graphics.FillRectangle(brush, cover.Rectangle.ScrollOffsetY(offset));

                            if (MTemplate.HasAreas)
                            {
                                using (var font = new Font("Arial", 12f, FontStyle.Bold))
                                using (var pen = new Pen(Color.BlueViolet, 2f))
                                {
                                    var match = CoverDetails.Match.ScrollOffsetY(offset);

                                    var matchLocation = CoverDetails.Match.ScrollOffsetY(offset).Location;

                                    e.Graphics.DrawRectangle(Pens.Red, match);

                                    e.Graphics.DrawString(string.Format("Rectangle : {0}", matchLocation), font,
                                            Brushes.BlueViolet, new PointF(matchLocation.X + 10, matchLocation.Y - 20));

                                    foreach (var cover in MTemplate.HiddenAreas)
                                    {
                                        var point = cover.Rectangle.ScrollOffsetY(offset).Location;

                                        e.Graphics.DrawLine(pen, matchLocation, point);

                                        e.Graphics.DrawString(string.Format("Distance: {0:0.00}", cover.Distance), font,
                                            Brushes.BlueViolet, new PointF(point.X + 10, point.Y - 60));

                                        e.Graphics.DrawString(string.Format("Offset: {0}", cover.Offset), font,
                                            Brushes.BlueViolet, new PointF(point.X + 10, point.Y - 40));

                                        e.Graphics.DrawString(string.Format("Rectangle : {0}", cover.Rectangle), font,
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
            base.OnClosed(e);
        }

        private void ClearMenuItem_Click(object sender, EventArgs e)
        {
            CoverDetails.Clear();
            Invalidate();
        }

        private void SaveMatchMenuItem_Click(object sender, EventArgs e)
        {
            m_Result = true;
            TemplateMatcher.AddTemplate(MTemplate);
            //var xml = temp.FromObjectXml();
            //File.WriteAllText("C:\\test.xml", xml);
            MessageBox.Show("Match saved", Application.ProductName);
            Close();
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

                if (!TemplateMatcher.MatchTemplate(MTemplate.Image, MTemplate.Cover.Template, ref match))
                {
                    MessageBox.Show("Cannot match Template");
                }
                else
                {
                    float offset = ScrollOffset;

                    CoverDetails.Match = match;

                    foreach (var hidden in MTemplate.HiddenAreas)
                    {
                        hidden.Offset = hidden.Rectangle.Offset(match);
                        hidden.Distance = hidden.Rectangle.Distance(match);
                    }
                }
            }
        }

        private void CloseMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        //private void ClearMatchMenuItem_Click(object sender, EventArgs e)
        //{
        //    var img = MTemplate.Image.Copy();
        //    MTemplate = new MatchTemplate();
        //    MTemplate.Image = img;
        //    Invalidate();
        //}

        //private void LoadImage_MenuItem_Click(object sender, EventArgs e)
        //{
        //    using (var dlg = new OpenFileDialog())
        //        if (dlg.ShowDialog(this) == DialogResult.OK)
        //            this.Image = new Image<Bgr, byte>(dlg.FileName);
        //}

        //private void LoadTemplate_MenuItem_Click(object sender, EventArgs e)
        //{
        //    using (var dlg = new OpenFileDialog())
        //        if (dlg.ShowDialog(this) == DialogResult.OK)
        //            this.Template = new Image<Bgr, byte>(dlg.FileName);
        //}        

        //private void TestMenuItem_Click(object sender, EventArgs e)
        //{
        //    int id = 0;

        //    var strid = Interaction.InputBox("Db Id", Application.ProductName);

        //    if (int.TryParse(strid, out id))
        //    {
        //        using (MatchForm form = new MatchForm())
        //        {
        //            form.MTemplate = this.MTemplate;
        //            form.Show();
        //        }
        //    }
        //}
    }
}
