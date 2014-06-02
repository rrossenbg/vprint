﻿/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CPrint2.Data;

namespace CPrint2
{
    public partial class MainForm : Form
    {
        public static MainForm Default { get; set; }

        protected IntPtr hWnd;

        public MainForm()
        {
            InitializeComponent();
            Default = this;
            menuStrip1.MenuActivate += new EventHandler(MenuStrip1_Open);
            menuStrip1.MenuDeactivate += new EventHandler(MenuStrip1_Close);
            imageBox1.ZoomChanged += new EventHandler(imageBox1_ZoomChanged);
            imageBox1.Resize += new EventHandler(imageBox1_ZoomChanged);
            hWnd = this.Handle;
        }

        private void imageBox1_ZoomChanged(object sender, EventArgs e)
        {
            tsPosLabel.Text = imageBox1.AutoScrollPosition.ToString();
            tsSizeLabel.Text = imageBox1.GetImageViewPort().ToString();
            tsZoomLabel.Text = string.Format("{0}%", imageBox1.Zoom);
        }

        protected override void OnLoad(EventArgs e)
        {
            this.Location = StateSaver.Default.Get<Point>("CPrint2.MainForm.Location", this.Location);
            this.Size = StateSaver.Default.Get<Size>("CPrint2.MainForm.Size", this.Size);
            base.OnLoad(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            StateSaver.Default.Set("CPrint2.MainForm.Location", this.Location);
            StateSaver.Default.Set("CPrint2.MainForm.Size", this.Size);

            e.Cancel = true;
            Hide();
            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            this.SetTopmost();
        }

        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            AppContext.Default.Exit();
        }

        private void MenuStrip1_Open(object sender, EventArgs e)
        {
            startStopMenuItem.Enabled = Program.currentUser != null;
            if (AppContext.Default.IsStarted)
            {
                startStopMenuItem.Text = "&Stop";
            }
            else
            {
                startStopMenuItem.Text = (Program.currentUser != null) ? "&Start" : "&Login";
            }
        }

        private void StartStopMenuItem_Click(object sender, EventArgs e)
        {
            if (AppContext.Default.IsStarted)
            {
                AppContext.Default.Stop();
            }
            else
            {
                AppContext.Default.Start();
            }
        }

        public void MenuStrip1_Close(object sender, EventArgs e)
        {
        }

        private void LoadFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                {
                    imageBox1.Image.DisposeSf();
                    imageBox1.Image = Image.FromFile(dlg.FileName);
                }
            }
        }

        public void ShowImageAsynch(string fileName)
        {
            Task.Factory.StartNew((o) =>
            {
                Thread.Sleep(Config.ImagePickupDelay);
                imageBox1.Image.DisposeSf();
                imageBox1.Image = Image.FromFile(Convert.ToString(o));
                hWnd.SetTopmost();
            }, fileName);
        }
    }
}
