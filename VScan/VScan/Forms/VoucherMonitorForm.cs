/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.FancyNetForms;
using PremierTaxFree.Controls;
using PremierTaxFree.PTFLib;
using PremierTaxFree.PTFLib.Data;

using PremierTaxFree.PTFLib.Native;
using PremierTaxFree.PTFLib.Sys;
using PremierTaxFree.PTFLib.Threading;
using WinFormsResources = System.Windows.Forms.Properties.Resources;

namespace PremierTaxFree
{
    public partial class VoucherMonitorForm : XCoolForm
    {
        private int m_Current = 0;
        private readonly PictureBoxEx[] ImageBoxes;

        public VoucherMonitorForm()
        {
            InitializeComponent();
            this.TitleBar.TitleBarCaption = "Voucher monitor ";
            ImageBoxes = new PictureBoxEx[] { 
                pbImage0, pbImage1, 
                pbImage2, pbImage3, 
                pbImage4, pbImage5, 
                pbImage6, pbImage7 };

            for (int i = 0; i < ImageBoxes.Length; i++)
                ImageBoxes[i].Tag = i;
        }

        public void MoveImage(Image img)
        {
            pbImage7.Picture = null;

            for (int i = 7; i > 0; i--)
                ImageBoxes[i].SetPicture(ImageBoxes[i - 1].Picture);

            pbMainImage.SetPicture(img);
            pbImage0.SetPicture(img);

            Invalidate(true);
        }

        protected override void OnLoad(EventArgs e)
        {
            XmlThemeLoader xtl = new XmlThemeLoader();
            xtl.ApplyTheme(this, WinFormsResources.BlueWinterTheme);

            base.OnLoad(e);
        }

        private static Thread ms_Thread = null;
        private static IntPtr ms_This = IntPtr.Zero;

        public static bool IsStarted
        {
            get
            {
                return ms_Thread != null;
            }
        }

        public static void Start()
        {
            //Start once
            if (ms_Thread != null)
                return;

            ms_Thread = new Thread(() =>
            {
                VoucherMonitorForm form = new VoucherMonitorForm();
                ms_This = form.Handle;
                Application.Run(form);
                ms_Thread = null;
                ms_This = IntPtr.Zero;
            }) { IsBackground = true };
            ms_Thread.Start();
        }

        public static void Stop()
        {
            if (ms_This != IntPtr.Zero)
                WinMsg.SendText(ms_This, Strings.VScan_SinglentonFormClose);
        }

        /// <summary>
        /// Shows image on monitor if its open
        /// </summary>
        /// <param name="name"></param>
        public static void ShowImage(string name)
        {
            if (ms_This != IntPtr.Zero)
                WinMsg.SendText(ms_This, name);
        }

        private void ButtonPverNext_Click(object sender, EventArgs e)
        {
            if (sender == btnNext)
                m_Current = m_Current.AddSf(0, ImageBoxes.Length);
            else if (sender == btnPrev)
                m_Current = m_Current.SubSf(0, ImageBoxes.Length);
            else 
                throw new NotImplementedException();
            Picture_Click(ImageBoxes[m_Current], EventArgs.Empty);
        }

        private void Picture_Click(object sender, EventArgs e)
        {
            PictureBoxEx p = (PictureBoxEx)sender;
            ImageBoxes.ForEach((b) => { b.ShowCurrent = b == sender; b.Invalidate(); });
            m_Current = p.Tag.Cast<int>();
            pbMainImage.SetPicture(p.Picture);
            pbMainImage.Invalidate();
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case user32.WM_COPYDATA:
                    {
                        string text = WinMsg.ReceiveText(m.LParam);
                        if (text.CompareNoCase(Strings.VScan_SinglentonFormClose))
                        {
                            DialogResult = DialogResult.OK;
                            OnClosed(EventArgs.Empty);
                            Application.ExitThread();
                        }
                        else if (text.StartsWith(Strings.VScan_EditItem))
                        {
                            var data = DataSlot.Get<Voucher>(text);
                            Debug.Assert(data != null);
                            MoveImage((Bitmap)data.VoucherImage.Clone());
                            DataSlot.Free(text);
                        }
                        else if (text.StartsWith(Strings.VScan_ItemSaved))
                        {
                            var data = DataSlot.Get<Voucher>(text);
                            Debug.Assert(data != null);
                            MoveImage((Bitmap)data.VoucherImage.Clone());
                            DataSlot.Free(text);
                        }

                        if (tsmiShowNumber.Checked)
                            this.TitleBar.TitleBarCaption = "Voucher monitor ".concat(SettingsTable.Get<int>(Strings.VScan_ScanCount, 0));
                    }
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }
    }
}
