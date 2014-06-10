/***************************************************
//  Copyright (c) Premium Tax Free 2014
/***************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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

                this.InvokeSf(() =>
                {
                    this.imageBox1.Image = imageBox1.Image.DisposeSf();

                    string imageFileName = Convert.ToString(o);
                    var img = Image.FromFile(imageFileName);
                    
                    this.imageBox1.Image = img.CopyFree(new Rectangle(0, 0, img.Width, img.Height));
                    var thumbnail = imageBox1.Image.GetThumbnailImage(60, 60, null, IntPtr.Zero);
                    var pictureBox1 = new PictureBox();
                    pictureBox1.Size = new System.Drawing.Size(60, 60);
                    pictureBox1.Image = thumbnail;
                    pictureBox1.Tag = imageFileName;

                    this.imageBox1.Controls.Add(pictureBox1);
                    pictureBox1.Click += new EventHandler(PictureBox1_Click);
                });

                hWnd.SetTopmost();
            }, fileName);
        }

        public void ResetState()
        {
            Task.Factory.StartNew(() =>
            {
                this.InvokeSf(() =>
                {
                    var list = new List<Control>();
                    foreach (PictureBox box in this.imageBox1.Controls)
                        if (box != null)
                            list.Add(box);
                    foreach (PictureBox box in list)
                        PictureBox1_Click(box, EventArgs.Empty);
                });
            });
        }

        private void PictureBox1_Click(object sender, EventArgs e)
        {
            PictureBox pbox = (PictureBox)sender;
            try
            {
                pbox.Image = pbox.Image.DisposeSf();
                string imageFileName = Convert.ToString(pbox.Tag);
                if (File.Exists(imageFileName))
                    File.Delete(imageFileName);
            }
            catch (Exception ex) { Debug.WriteLine(ex); }
            finally
            {
                pbox.Click -= new EventHandler(PictureBox1_Click);
                this.imageBox1.Controls.Remove(pbox);
            }
        }
    }
}
