/***************************************************
//  Copyright (c) Premium Tax Free 2014
***************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CPrint2.Common;
using CPrint2.Controls;
using CPrint2.Data;
using CPrint2.ScanServiceRef;

namespace CPrint2
{
    /// <summary>
    /// http://www.codeproject.com/Articles/257502/Creating-Your-First-EMGU-Image-Processing-Project
    /// http://stackoverflow.com/questions/6174527/emgu-cv-blob-detection
    /// </summary>
    public partial class MultyCamForm : Form
    {
        public static event ThreadExceptionEventHandler Error;

        private readonly Queue m_Queue = Queue.Synchronized(new Queue());
        private readonly Hashtable m_Image_Infos = Hashtable.Synchronized(new Hashtable());

        public static MultyCamForm Default { get; set; }

        private List<CameraControl> CameraControls { get; set; }

        public MultyCamForm()
        {
            InitializeComponent();

            CameraControls = new List<CameraControl>();

            for (int index = 0; index < Config.CAMERAS; index++)
            {
                CameraControl cnt = new CameraControl(index);
                cnt.Dock = DockStyle.Fill;
                cnt.BorderStyle = BorderStyle.FixedSingle;
                cnt.ImageTaken += new CameraControl.ImageTakenDelegate(cnt_ImageTaken);
                CameraControls.Add(cnt);
            }

            tablePanelControl1.AddRows(CameraControls.ConvertAll<Control>(cc => cc));

            foreach (var cnt in this.CameraControls)
                cnt.Start();

            Default = this;
        }

        protected override void OnLoad(EventArgs e)
        {
            this.Location = StateSaver.Default.Get<Point>("CPrint2.MainForm.Location", this.Location);
            this.Size = StateSaver.Default.Get<Size>("CPrint2.MainForm.Size", this.Size);

            int highS = 0;
            int minWidth = 0;
            int minHeigth = 0;

            VCamLib.ReadSettings(ref highS, ref minWidth, ref minHeigth);

            highS = StateSaver.Default.Get<int>("highS", highS);
            minWidth = StateSaver.Default.Get<int>("minWidth", minWidth);
            minHeigth = StateSaver.Default.Get<int>("minHeigth", minHeigth);
            CameraControl.Mode = StateSaver.Default.Get<CameraControl.ShowMode>("CameraControl.Mode", CameraControl.ShowMode.Normal);

            VCamLib.SaveSettings(highS, minWidth, minHeigth);

            this.highS.Value = highS;
            base.OnLoad(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            int highS = 0;
            int minWidth = 0;
            int minHeigth = 0;

            VCamLib.ReadSettings(ref highS, ref minWidth, ref minHeigth);

            StateSaver.Default.Set("CPrint2.MainForm.Location", this.Location);
            StateSaver.Default.Set("CPrint2.MainForm.Size", this.Size);

            StateSaver.Default.Set("highS", highS);
            StateSaver.Default.Set("minWidth", minWidth);
            StateSaver.Default.Set("minHeigth", minHeigth);
            StateSaver.Default.Set("CameraControl.Mode", CameraControl.Mode);

            e.Cancel = true;
            this.Hide();
            base.OnClosing(e);
        }

        public static void Stop()
        {
            foreach (var cnt in Default.CameraControls)
                cnt.Stop();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            this.SetTopmost();
        }

        private void RunMenuItem_Click(object sender, EventArgs e)
        {
            m_Image_Infos.Clear();
            foreach (var cnt in this.CameraControls)
                cnt.Run();
        }

        public void ProcessCommand(DataObj2 data = null)
        {
            if (data != null)
            {
                m_Queue.Enqueue(data);
                foreach (var cnt in this.CameraControls)
                    cnt.Run();
            }
        }

        internal void ResetState()
        {
        }

        private void TestMenuItem_Click(object sender, EventArgs e)
        {
            ProcessCommand(DataObj2.Test());
        }

        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void cnt_ImageTaken(object sender, EventArgs e)
        {
            CameraControl ccnt = (CameraControl)sender;
            m_Image_Infos[ccnt.CameraIndex] = ccnt.Cap_Fore_Info;

            if (m_Image_Infos.Count == Config.CAMERAS)
            {
                Task.Factory.StartNew(() =>
                {
                    List<byte[]> list = new List<byte[]>();

                    for (int i = 0; i < Config.CAMERAS; i++)
                        list.Add(((FileInfo)m_Image_Infos[i]).ToArray());

                    var file2 = ((FileInfo)null).Temp(".tif").IfDebug("C:\\test_result.tif");
                    try
                    {
                        var obj = (DataObj2)m_Queue.Dequeue();

                        TiffConverter converter = new TiffConverter();
                        var buffer = converter.WrapJpegs(list.ToArray());
                        file2.WriteAllBytes(buffer);

                        //copy voucher
                        var keys = Security.CreateInstance().GenerateSecurityKeys();
                        var serverSessionId = obj.Id;
                        var sserverSessionId = obj.Id.ToString();

                        var srv = ServiceDataAccess.Instance;

                        srv.SendFile(file2, sserverSessionId, keys);

                        srv.CommitVoucherChanges(sserverSessionId, 0, obj.Iso, obj.RetailerId, obj.VoucherId,
                            Global.FolderID.HasValue ? Global.FolderID.Value : (int?)null, "", "", keys);

                        srv.SaveHistory(OperationHistory.Scan, serverSessionId, obj.Iso, obj.RetailerId, obj.VoucherId, 0, 0, "", keys);
                    }
                    catch (Exception ex)
                    {
                        if (Error != null)
                            Error(this, new ThreadExceptionEventArgs(ex));
                    }
                    finally
                    {
#if DEBUG
                        new Action(() => Process.Start(file2.FullName)).RunSafe();
#endif
#if! DEBUG
                        file2.DeleteSafe();
#endif
                        for (int i = 0; i < Config.CAMERAS; i++)
                            ((FileInfo)m_Image_Infos[i]).DeleteSafe2();
                        m_Image_Infos.Clear();
                    }
                });
            }
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingsForm form = new SettingsForm();
            form.Show();
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            SettingsForm form = new SettingsForm();
            form.Show();
        }

        private void testRunToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProcessCommand(DataObj2.Test());
        }

        private void highS_ValueChanged(object sender, EventArgs e)
        {
            VCamLib.SaveSettings(highS.Value, -1, -1);
            TrackBar tb = (TrackBar)sender;
            string text = string.Format("{0} .value = {1}", tb.Name, tb.Value);
            Label lbl = (Label)tb.Tag;
            lbl.Text = text;
            toolTip1.SetToolTip(tb, text);
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
