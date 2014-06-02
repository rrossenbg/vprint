/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Drawing;
using System.Windows.Forms;
using VPrinting.Common;
using VPrinting.Extentions;
using VPrinting.Tools;

namespace VPrinting
{
    public partial class MonitorForm : Form
    {
        #region MEMBERS

        private static MonitorForm ms_Form;

        private static volatile string ms_Message;

        private FontWrapper m_Font;

        #endregion

        public static string Message
        {
            set
            {
                ms_Message = Speeker.SpeakAsynchSf(value);

                if (ms_Form != null)
                    ms_Form.Invalidate();
            }
        }

        public static void show()
        {
            MainForm.Default.InvokeSf(new MethodInvoker(() =>
            {
                if (ms_Form == null)
                {
                    var form = new MonitorForm();
                    form.Show();
                }

                ms_Form.Focus();
            }));
        }

        public MonitorForm()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            InitializeComponent();
            ms_Form = this;
        }

        #region HANDLERS

        private void Mute_Click(object sender, EventArgs e)
        {
            Speeker.Enabled = !Speeker.Enabled;
            btnMute.Text = Speeker.Enabled ? "Mute" : "Speak";
        }

        private void Font_Click(object sender, EventArgs e)
        {
            using (FontDialog dlg = new FontDialog())
            {
                dlg.Font = m_Font.Value;
                if (dlg.ShowDialog(this) == DialogResult.OK)
                    m_Font.Value = dlg.Font;
            }
        }

        private void Close_Click(object sender, EventArgs e)
        {
            Close();
            ms_Form = null;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Invalidate();
        }

        #endregion

        #region OVERRIDES

        protected override void OnLoad(EventArgs e)
        {
            m_Font = StateSaver.Default.Get<FontWrapper>("MonitorForm.m_Font", new FontWrapper(new Font("Arial", 16, FontStyle.Regular)));
            this.Size = StateSaver.Default.Get<Size>("MonitorForm.Size", this.Size);
            this.Location = StateSaver.Default.Get<Point>("MonitorForm.Location", this.Location);
            base.OnLoad(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            StateSaver.Default.Set("MonitorForm.m_Font", m_Font);
            StateSaver.Default.Set("MonitorForm.Size", this.Size);
            StateSaver.Default.Set("MonitorForm.Location", this.Location);
            ms_Form = null;
            base.OnClosed(e);
        }

        protected override void OnShown(EventArgs e)
        {
            timer1.Enabled = true;
            base.OnShown(e);
        }

        protected override void OnDeactivate(EventArgs e)
        {
            timer1.Enabled = false;
            base.OnDeactivate(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (!ms_Message.IsNullOrEmpty())
            {
                using (var brush = new SolidBrush(Color.Black))
                {
                    var lines = ms_Message.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    Size size;
                    for (int i = 0, x = 100, y = 100; i < lines.Length; i++, y += size.Height)
                    {
                        e.Graphics.DrawString(lines[i], m_Font.Value, brush, x, y);
                        size = Size.Round(e.Graphics.MeasureString(lines[i], m_Font.Value));
                    }
                }
            }

            base.OnPaint(e);
        }

        #endregion //OVERRIDES
    }
}