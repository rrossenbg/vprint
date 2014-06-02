/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/
#pragma warning disable 642

using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.FancyNetForms;
using PremierTaxFree.PTFLib;
using PremierTaxFree.PTFLib.Data;
using PremierTaxFree.PTFLib.Data.Objects;

using PremierTaxFree.PTFLib.Native;
using PremierTaxFree.PTFLib.Sys;
using WinFormsResources = System.Windows.Forms.Properties.Resources;

namespace PremierTaxFree.Forms
{
    public partial class DbObserverForm : XCoolForm
    {
        const string strSQL = "SELECT FileID FROM dbo.FILES WHERE DateExported IS NULL;";

        private class VoucherInfo : IDisposable
        {
            public string FileID { get; set; }
            public Image BarCodeImage { get; set; }
            public Image VoucherImage { get; set; }
            public string FileName { get; set; }
            public string BarCodeString { get; set; }
            public string SiteCode { get; set; }
            /// <summary>
            /// RelatedID
            /// </summary>
            public int RelatedID { get; set; }

            public VoucherInfo()
            {
            }
            ~VoucherInfo()
            {
                using (BarCodeImage) { }
                using (VoucherImage) { }
            }
            public void Dispose()
            {
                using (BarCodeImage) ;
                using (VoucherImage) ;
                GC.SuppressFinalize(this);
            }
        }

        public DbObserverForm()
        {
            InitializeComponent();
            this.TitleBar.TitleBarCaption = "Data observer";
            timer1.Interval = TimeSpan.FromSeconds(10).TotalMilliseconds.Cast<int>();
            timer1.Stop();

            dgvData.CellToolTipTextNeeded += (o, e) =>
            {
                var row = dgvData[e.ColumnIndex, e.RowIndex];
                var tag = (VoucherInfo)row.Tag;
                e.ToolTipText = tag.BarCodeString;
            };
        }

        protected static void InsertRecordAsync(DbObserverForm form, DbClientFileInfo info)
        {
            if (form.IsDisposed)
                return;

            form.InvokeSafe(new Action<DbClientFileInfo>((i) =>
            {
                Image voucher = i.VoucherImage.ToImage();
                Image barcode = i.BarCodeImage.ToImage();

                form.dgvData.Rows.Insert(0, i.RetailerID, i.RetailerID, i.VoucherID);
                form.dgvData.Rows[0].Tag = i;
                form.SetCurrent(0);
            }), info);
        }

        private void SetCurrent(int index)
        {
            if (0 > index || index >= dgvData.Rows.Count)
                return;

            DbClientFileInfo info = (DbClientFileInfo)dgvData.Rows[index].Tag;
            if (info == null)
                return;

            pbBarCode.Picture = (Image)info.BarCodeImage.Clone();
            pbVoucher.Picture = (Image)info.VoucherImage.Clone();
            dgvData.Rows[index].Selected = true;
            Invalidate(true);
        }

        #region PRIVATE_HANDLERS

        private void Button_Click(object sender, EventArgs e)
        {
            if (dgvData.SelectedRows.Count != 1)
                return;

            if (sender == btnUp)
            {
                if (dgvData.SelectedRows[0].Index > 0)
                    SetCurrent(dgvData.SelectedRows[0].Index - 1);
            }
            else if (sender == btnDown)
            {
                if (dgvData.SelectedRows[0].Index < dgvData.Rows.Count - 1)
                    SetCurrent(dgvData.SelectedRows[0].Index + 1);
            }
            else
                throw new NotImplementedException();
        }

        private void Button_DoubleClick(object sender, EventArgs e)
        {
            if (dgvData.Rows.Count == 0)
                return;

            if (sender == btnUp)
            {
                SetCurrent(0);
            }
            else if (sender == btnDown)
            {
                SetCurrent(dgvData.Rows.Count - 1);
            }
            else
                throw new NotImplementedException();
        }

        private void DataView_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvData.SelectedRows.Count == 1)
                SetCurrent(dgvData.SelectedRows[0].Index);
        }

        private void Refresh_Click(object sender, EventArgs e)
        {
            timer1.Enabled = !timer1.Enabled;
        }

        private void Menu_Opening(object sender, CancelEventArgs e)
        {
            tsmiRefresh.Checked = timer1.Enabled;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            dgvData.Rows.ForEach<DataGridViewRow>((r) => (r.Tag as IDisposable).DisposeSf());
            dgvData.Rows.Clear();
            foreach (var info in ClientDataAccess.SelectAllFiles())
                new Action<DbObserverForm, DbClientFileInfo>((f, i) => InsertRecordAsync(f, i)).FireAndForget(this, info);
        }

        #endregion

        #region FORM_START_STOP

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
                DbObserverForm form = new DbObserverForm();
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

        #endregion

        #region OVERRIDES

        protected override void OnLoad(EventArgs e)
        {
            XmlThemeLoader xtl = new XmlThemeLoader();
            xtl.ApplyTheme(this, WinFormsResources.BlueWinterTheme);
            Timer_Tick(null, null);
            base.OnLoad(e);
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
                    }
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        #endregion
    }
}