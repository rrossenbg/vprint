/***************************************************
//  Copyright (c) Premium Tax Free 2013
/***************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using VPrint.Common;
using VPrinting.Common;
using VPrinting.Controls;
using VPrinting.Data;
using VPrinting.Extentions;
using VPrinting.ScanServiceRef;

namespace VPrinting
{
    partial class MainForm
    {
        #region CONSTANTS
        /// <summary>
        /// png
        /// </summary>
        private const string PNG = "png";
        /// <summary>
        /// -7
        /// </summary>
        private const int DAYS_BACK = -7;

        private readonly string[] SUPPORTED_FILE_EXTENTIONS = new string[] { "*.jpg", "*.jpeg", "*.tif", "*.pdf" };

        #endregion

        #region MEMBERS

        private readonly List<FileSystemWatcher> m_FileSysWatchers = new List<FileSystemWatcher>();
        //private readonly SortedIndexList<int, ItemControl> m_ControlIndexes = new SortedIndexList<int, ItemControl>();

        public readonly StateManager m_StateManager = new StateManager();

        private readonly StringTaskOrganizer m_ScanFileOrganizer;

        private readonly StateManagerItemOrganizer m_SendFileOrganizer;

        private readonly StateManagerItemOrganizer m_DownloadFileOrganizer;

        #endregion

        private void InitializeComponentScanning()
        {
            m_StateManager.NewItemAdded += new EventHandler<ItemEventArgs>(StateManager_NewItemAdd);
            m_StateManager.ItemRemoved += new EventHandler<ItemEventArgs>(StateManager_ItemRemoved);
            m_StateManager.ItemsCleared += new EventHandler(StateManager_Cleared);

            m_StateManager.CurrentItemCompleted += new EventHandler<CurrentItemEventArgs>(StateManager_ItemCompleted);
            m_StateManager.CurrentItemSelected += new EventHandler<CurrentItemEventArgs>(StateManager_ItemSelected);
            m_StateManager.LastItemProcessing += new EventHandler<CurrentItemEventArgs>(StateManager_LastItemProcessing);

            m_StateManager.NextItemExpected += new EventHandler<ItemEventArgs>(StateManager_NextItemExpected);

            m_StateManager.Mode = StateManager.eMode.Barcode;
            m_StateManager.Load(tbTransferFile.Text);

            btnBrowseForScanDir.Tag = tbScanDirectory;
            btnBrowseForExprFile.Tag = tbTransferFile;

            btnBrowseForScanDir.Click += new EventHandler(BrowseForFolder_Click);
            btnBrowseForExprFile.Click += new EventHandler(BrowseForExprFile_Click);

            toggleButtonControl1.ActiveChanged += new EventHandler<ValueEventArgs<int>>(toggleButtonControl1_ActiveChanged);
            toggleButtonControl1.Padding = new Padding(0);
            toggleButtonControl1.RefreshControl();

            var list = StateSaver.Default.Get<List<BarcodeConfig>>(Strings.LIST_OF_BARCODECONFIGS);
        }

        #region StateManager & ImageIconControl HANDLERS        

        const int TIMEOUT = 1000;

        private void StateManager_NewItemAdd(object sender, ItemEventArgs e)
        {
            this.InvokeSafe(new System.Action<ItemEventArgs>((args) =>
            {
                if(Monitor.TryEnter(lpScannedFiles, TIMEOUT))
                {
                    try
                    {
                        var cnt = ItemControl.GetInstance();
                        ToolTip1.SetToolTip(cnt, args.Item.SessionID.ToString());
                        cnt.Item = args.Item;
                        cnt.Click += new EventHandler(ImageIconControl_Click);
                        cnt.Updated += new EventHandler(ImageIconControl_Updated);
                        cnt.ContextMenuStrip = scanContextMenuStrip;
                        lpScannedFiles.Controls.Add(cnt);
                        lblMessage.Text = string.Concat("Vouchers in folder: ", lpScannedFiles.Controls.Count, "     Starting from: ", m_CurrentVoucher);
                    }
                    finally
                    {
                        Monitor.Exit(lpScannedFiles);
                    }
                }
            }), e);
        }

        private void StateManager_ItemRemoved(object sender, ItemEventArgs e)
        {
            this.InvokeSafe(new System.Action<ItemEventArgs>((args) =>
            {
                if(Monitor.TryEnter(lpScannedFiles,TIMEOUT))
                {
                    try
                    {
                        foreach (Control cnt in lpScannedFiles.Controls)
                        {
                            var icnt = (ItemControl)cnt;
                            if (icnt != null && icnt.Item != null && icnt.Item.Equals(args.Item))
                            {
                                lpScannedFiles.Controls.Remove(icnt);
                                //icnt.DisposeSf();
                                ItemControl.SetInstance(icnt);
                                break;
                            }
                        }
                    }
                    finally
                    {
                        Monitor.Exit(lpScannedFiles);
                    }
                }
            }), e);
        }

        private void StateManager_Cleared(object sender, EventArgs e)
        {
            this.InvokeSafe(new System.Action(() =>
            {
                if(Monitor.TryEnter(lpScannedFiles, TIMEOUT))
                {
                    try
                    {
                        foreach (ItemControl cnt in lpScannedFiles.Controls)
                        {
                            cnt.Click -= new EventHandler(ImageIconControl_Click);
                            cnt.Updated -= new EventHandler(ImageIconControl_Updated);
                            //cnt.DisposeSf();
                            ItemControl.SetInstance(cnt);
                        }

                        ToolTip1.RemoveAll();
                        lpScannedFiles.Controls.Clear();
                        tbTransferFile.Clear();

                        m_ScanFileOrganizer.Clear();
                        m_SendFileOrganizer.Clear();

                        lblItemsWithErr.Text = null;
                        lblMessage.Text = string.Empty;
                    }
                    finally
                    {
                        Monitor.Exit(lpScannedFiles);
                    }
                }
                //m_ControlIndexes.Clear();
            }));
        }

        private void StateManager_ItemCompleted(object sender, CurrentItemEventArgs e)
        {
            var mode = m_StateManager.Mode;
            SendToServerAsync(e.CurrentItem);
        }

        private void StateManager_ItemSelected(object sender, CurrentItemEventArgs e)
        {
            SendToServerAsync(e.PrevItem);
        }

        private void StateManager_LastItemProcessing(object sender, CurrentItemEventArgs e)
        {
            lblWarningMessage.Start();
        }

        private void StateManager_NextItemExpected(object sender, ItemEventArgs e)
        {
            StateManager.VoucherItem vi = e.Item as StateManager.VoucherItem;
            if (vi != null)
            {
                MonitorForm.Message =
                    string.Format("Expected voucher: \r\n ISO: {0}\r\nRetailer ID: {1}\r\nVoucher ID: {2}\r\nSite code: {3}",
                                                                        vi.CountryID,
                                                                        vi.RetailerID.ToString().SplitJoinSafe(3, " "),
                                                                        vi.VoucherID.ToString().SplitJoinSafe(3, " "),
                                                                        vi.SiteCode.SplitJoinSafe(3, " "));
            }
        }

        private void ImageIconControl_Click(object sender, EventArgs e)
        {
            ItemControl selectedItemCnt = (ItemControl)sender;
            selectedItemCnt.PlayEffect();

            StateManager.VoucherItem vi = selectedItemCnt.Item as StateManager.VoucherItem;
            if (vi != null)
            {
                MonitorForm.Message =
                    string.Format("Selected voucher: \r\n ISO: {0}\r\nRetailer ID: {1}\r\nVoucher ID: {2}\r\nSite code: {3}",
                                                                        vi.CountryID,
                                                                        vi.RetailerID.ToString().SplitJoinSafe(3, " "),
                                                                        vi.VoucherID.ToString().SplitJoinSafe(3, " "),
                                                                        vi.SiteCode.SplitJoinSafe(3, " "));
            }
        }

        private void ImageIconControl_Updated(object sender, EventArgs e)
        {
            ItemControl cnt = (ItemControl)sender;
            var item = cnt.Item;
            if (item == null)
                return;

            this.InvokeSafe(new System.Action(() =>
            {
                ToolTip1.SetToolTip(cnt, (cnt.Item.State == StateManager.eState.Err ? cnt.Item.Message : cnt.Item.ToString()));
            }));
        }

        #endregion

        #region Organizer CALLBACKS

        private void DownloadOrganizer_Completed(object sender, TaskProcessOrganizer<StateManager.Item>.CompletedEventArgs e)
        {
            string fullFileName = m_DownloadFileOrganizer.Data.GetValueAndRemove<StateManager.Item, string>(e.Value);
            if (!fullFileName.IsNullOrEmpty())
            {
                var temp = new DirectoryInfo(Path.GetTempPath());
                var gdir = temp.Combine(e.Value.SessionID.ToString());
                gdir.EnsureDirectory();
                e.Value.FileInfoList.AddRange(ZipFileAccess.Instance.ExtractFileZip(fullFileName, gdir.FullName));

                var cover = ServiceDataAccess.Instance.ReadCoverInfo(e.Value.Id);
                if (!string.IsNullOrWhiteSpace(cover))
                {
                    var covers = cover.ToObject<List<Rectangle>>();
                    var voucherFile = e.Value.FileInfoList.Max((i1, i2) => i1.Length > i2.Length);
                    using (var bmp = (Bitmap)Bitmap.FromFile(voucherFile.FullName))
                    {
                        using (Graphics g = Graphics.FromImage(bmp))
                        {
                            foreach (var area in covers)
                            {
                                bmp.Pixellate(area);
                                g.DrawString(string.Format("{0}", area), this.Font, Brushes.BlueViolet, area.Left, area.Top);
                            }
                        }

                        var newvoucherfile = voucherFile.Rename((i) => i.GetFileNameWithoutExtension() + "_new");
                        e.Value.FileInfoList.Remove(voucherFile);
                        e.Value.FileInfoList.Add(newvoucherfile);
                        bmp.Save(newvoucherfile.FullName, ImageFormat.Jpeg);
                    }
                    voucherFile.DeleteSafe();
                }

                var sgn = e.Value.FileInfoList.FirstOrDefault(f => f.Extension == ".sgn");
                if (sgn != null)
                {
                    var sec = new CertificateSecurity(X509FindType.FindBySerialNumber, Strings.CERTNUMBER, StoreLocation.LocalMachine);
                    if (sec.Loaded)
                    {
                        FileInfo fi = null;
                        long max = 0;
                        foreach (var f in e.Value.FileInfoList)
                        {
                            if (f.Length > max)
                            {
                                max = f.Length;
                                fi = f;
                            }
                        }
                        byte[] voucher = fi.ReadAllBytes();
                        byte[] signature = e.Value.Signature = sgn.ReadAllBytes();
                        e.Value.IsSignatureValid = sec.Verify(voucher, signature);
                    }
                }

                var vi = e.Value as StateManager.VoucherItem;
                if (vi != null)
                {
                    ServiceDataAccess.Instance.SaveHistory(OperationHistory.FileDownloaded, e.Value.CountryID, vi.RetailerID, vi.VoucherID, 0, 1,
                        e.Value.ToString());
                }

                if (m_DownloadFileOrganizer.Data.ContainsKey(e.Value.SessionID))
                {
                    ItemControl selectedItemCnt = m_DownloadFileOrganizer.Data.GetValueAndRemove<Guid, ItemControl>(e.Value.SessionID);
                    this.InvokeSf(() => { selectedItemCnt.UnLock(); });
                }
                this.InvokeSf(() => SelectFilesForm.ShowFiles(this, e.Value.FileInfoList));
            }
        }

        #endregion

        #region EVENT HANDLERS

        // Changed, renamed
        //this.m_FileSysWatcher.Changed += new System.IO.FileSystemEventHandler(this.FileSysWatcher_Event);
        //this.m_FileSysWatcher.Renamed += new System.IO.RenamedEventHandler(this.FileSysWatcher_Event);
        private void FileSysWatcher_Event(object sender, FileSystemEventArgs e)
        {
            ScanFileAsync(e.FullPath);
        }

        private void FileSysWatcher_Event(object sender, RenamedEventArgs e)
        {
            ScanFileAsync(e.FullPath);
        }

        #endregion

        #region PRIVATE METHODS

        private void ClearScanDirectory()
        {
            //CLEAR SCAN DIRECTORY
            var t0 = Task.Factory.StartNew((o) =>
            {
                var dir = new DirectoryInfo(Convert.ToString(o));
                var dirs = dir.GetDirectories();
                foreach (var di in dirs)
                {
                    new Action<DirectoryInfo>((d) =>
                    {
                        if (DateTime.Now.AddDays(DAYS_BACK) < d.CreationTime)
                            d.Delete(true);
                    }).RunSafe(di);
                }

                var files = dir.GetFiles();
                foreach (var fi in files)
                {
                    new Action<FileInfo>((f) =>
                    {
                        if (DateTime.Now.AddDays(DAYS_BACK) < f.CreationTime)
                            f.Delete();
                    }).RunSafe(fi);
                }
            }, tbScanDirectory.Text);
        }

        #endregion

        #region OVERRIDES

        protected override void OnClosing(CancelEventArgs e)
        {
            if (m_ScanFileOrganizer.HasItems())
                e.Cancel = ((this.ShowQuestion(Messages.UnprocessedItemsCloseAnyway, MessageBoxButtons.YesNo) != DialogResult.Yes));
            if (!e.Cancel && m_SendFileOrganizer.HasItems())
                e.Cancel = ((this.ShowQuestion(Messages.UnsentItemsCloseAnyway, MessageBoxButtons.YesNo) != DialogResult.Yes));
            base.OnClosing(e);
        }

        #endregion
    }
}