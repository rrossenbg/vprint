/***************************************************
//  Copyright (c) Premium Tax Free 2011
/***************************************************/

//#define HARD_CODE

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using GaryPerkin.UserControls.Buttons;
using VPrinting;
using VPrinting.Common;
using VPrinting.Controls;
using VPrinting.Controls.ArrowButton;
using VPrinting.Data;
using VPrinting.Documents;
using VPrinting.Extentions;
using VPrinting.Forms.Explorer;
using VPrinting.PartyManagement;
using VPrinting.ScanServiceRef;
using VPrinting.VoucherNumberingAllocationPrinting;

namespace VPrinting
{
    public partial class MainForm : Form
    {
        #region CONFIG PROPERTIES

        public static int CurrentCountryId
        {
            get
            {
                int countryId = Convert.ToInt32(ConfigurationManager.AppSettings["CountryId"]);
                return countryId;
            }
        }

        public static string PrintClassName
        {
            get
            {
                return Convert.ToString(ConfigurationManager.AppSettings["PrintClassName"]);
            }
        }

        #endregion

        public static DirectoryInfo GetAppSubFolder(string name)
        {
            var appDir = new FileInfo(Application.ExecutablePath).Directory;
            var resultDir = appDir.Combine(name);
            resultDir.EnsureDirectory();
            return resultDir;
        }

        #region CONSTS

        /// <summary>
        /// 10
        /// </summary>
        private const int TRIES = 10;

        /// <summary>
        /// 10
        /// </summary>
        private const int MAX_ITEMS_TOPROCESS_ON_ONE_GO = 10;

        /// <summary>
        /// 10
        /// </summary>
        private const int MAX_ITEMS_TOSEND_ON_ONE_GO = 10;

        /// <summary>
        /// 5
        /// </summary>
        private const int MAX_ITEMS_TODOWNLOAD_ON_ONE_GO = 5;

        private const int PRINT_TAB0 = 0;
        private const int SCAN_TAB1 = 1;
        private const int HISTORY_TAB2 = 2;

        private readonly Color OffColor = Color.White;
        private readonly Color OnColor = Color.Gray;

        #endregion

        #region DELEGATES

        private delegate void RemoveRowFromViewDelegate(DataGridView view, DataGridViewRow row);

        #endregion

        #region PRIVATE FIELDS

        private int m_HeadOfficeId;
        private int m_RetailerId;

        //The 3 next ints are used to keep tabs on the amount of different type of allocations that are selected
        //The next two ints are used to keep tabs on POS and Shop vouchers selected when attempting to re-print.
        //This string is used to keep the Allocation IDs for the selected pending allocations
        private readonly List<int> m_PendingVouchersList = new List<int>();
        private readonly List<int> m_PrintedVouchersList = new List<int>();
        private readonly List<int> m_StoplistedVouchersList = new List<int>();

        private int m_AllocationPrintings = 0;

        #endregion //PRIVATE FIELDS

        #region CONTROLS

        private GroupBox groupBox3;

        private RemoveRowFromViewDelegate RemoveRowDelegate;
        private readonly RemoveRowFromViewDelegate REMOVEROW = new RemoveRowFromViewDelegate((v, r) => { lock (((ICollection)v.Rows).SyncRoot)v.Rows.Remove(r); });
        private ToolStripSeparator toolStripMenuItem2;
        private Button btnReprintFromCache;
        private ToolStripSeparator toolStripMenuItem3;
        private ToolStripMenuItem tsmiEmptyCache;
        private ToolStripMenuItem assignFormatToolStripMenuItem;
        private ToolStripMenuItem mapPrinterToolStripMenuItem;
        private GroupBox groupBox4;
        private GroupBox groupBox5;
        private TabControl tabControl1;
        private TabPage tpPrint;
        private TabPage tpScan;
        private GroupBox gbScanHeader;
        private Panel ScanningSettingPanel;
        private Button btnBrowseForScanDir;
        private TextBox tbScanDirectory;
        private TextBox textBox1;
        private Button btnRun;
        private ToolStripMenuItem saveToolStripMenuItem;
        private ImageList imageList1;
        private Button btnClear;

        private TextBox tbTransferFile;
        private Button btnBrowseForExprFile;

        private FlowLayoutPanel lpScannedFiles;
        private TextBox textBox3;
        private ToolTip ToolTip1;
        private StatusStrip ssScaningStatus;
        private ToolStripStatusLabel lblMessage;
        private ToolStripProgressBar pbScanProgress;
        private ToolStripStatusLabel lblScanned;
        private RoundButton btnScan;
        private ArrowButton btnShowHide;
        private Controls.BliningLabel lblWarningMessage;
        private ToolStripStatusLabel lblSent;
        private LinkLabel llblOpenFolder;
        private ContextMenuStrip scanContextMenuStrip;
        private ToolStripMenuItem tsmiSend;
        private ToolStripSeparator toolStripMenuItem4;
        private ToolStripMenuItem tsmiExit;

        private TabPage tbSearch;
        private GroupBox groupBox6;
        private Panel pnlDataGrid;
        private DataGridView dgvSearchData;
        private Button btnShowHistory;
        private DateTimePicker historyToTime;
        private DateTimePicker historyFromTime;
        private Label label6;
        private Label label5;
        private ToolStripMenuItem simulatePrintToolStripMenuItem;
        private ComboBox cbHistoryType;
        private Button btnCover;
        private SplitContainer splitContainer1;
        private TreeView tvFolders;
        private ContextMenuStrip folderMenuStrip;
        private ToolStripMenuItem tbAddNew;
        private ToolStripMenuItem tbDelete;
        private ImageList imageList2;

        private ToolStripSeparator toolStripMenuItem5;
        private ToolStripMenuItem tbRename;
        private ToolStripMenuItem tsmiScaningDelete;

        private ContextMenuStrip historyMenuStrip;
        private ToolStripMenuItem tsmiSort;
        private SplitContainer splitContainer2;
        private TabControl tabControl2;
        private TabPage tabVoucher;
        private TabPage tabHistory;
        private Panel panel1;
        private DateTimePicker2 dtTo;
        private DateTimePicker2 dtFrom;
        private Label label8;
        private TextBox tbVoucherId;
        private Label label7;
        private ComboBox cbCountryId;
        private TextBox tbBrId;
        private Label label3;
        private Button btSearch;

        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private ToolStripStatusLabel tslItemsCount;

        private Button btnShowMonitor;
        public CheckBox cbCoversheet;
        private ToolStripMenuItem deleteAllToolStripMenuItem;
        private ToolStripMenuItem tsmiScaningDeleteAll;
        private ToolStripSeparator toolStripMenuItem6;
        private ToolStripMenuItem updateVersionToolStripMenuItem;
        private ToolStripMenuItem tsmiScanningClear;
        private ToolStripSeparator toolStripMenuItem7;

        private ToolStripMenuItem tsmiScanningForce;
        private ToolStripMenuItem tsmiScanningIgnore;
        private ToolStripMenuItem tsmiShow;
        private BliningLabel bliningLabel1;
        private BliningLabel bliningLabel2;
        private ToolStripMenuItem tsmiScanningForceAll;
        private ToolStripMenuItem tsmiScanningDetailes;
        private ToolStripMenuItem tsmiScanningShow;
        private ToolStripSeparator toolStripMenuItem8;
        private ToolStripSeparator toolStripMenuItem9;
        private ToolStripMenuItem tsmiScanningAddToTran;
        private ToolStripMenuItem exitToolStripMenuItem1;

        #endregion

        private CheckBox cbVoucherMustExist;
        private ToolStripMenuItem nameToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem10;
        private ToolStripMenuItem showSignedToolStripMenuItem;
        private Button btnFileBrowser;
        private ToolStripStatusLabel lblItemsWithErr;
        private LinkLabel lblNext;
        private LinkLabel lblPrev;
        private ToolStripMenuItem scheduleStartToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem11;
        private ToggleButtonControl toggleButtonControl1;
        private ToggleButtonControl toggleButtonControl2;
        private Button btnCCCover;

        #region PUBLIC FIELDS

        public static MainForm Default { get; set; }

        public readonly SynchronizationContext m_MainContext;

        #endregion

        #region CONSTRUCTOR

        public MainForm()
        {
            InitializeComponent();
            InitializeComponentScanning();
            InitializeSearch();

            m_ScanFileOrganizer = new StringTaskOrganizer(MAX_ITEMS_TOPROCESS_ON_ONE_GO);
            m_SendFileOrganizer = new StateManagerItemOrganizer(MAX_ITEMS_TOSEND_ON_ONE_GO);
            m_DownloadFileOrganizer = new StateManagerItemOrganizer(MAX_ITEMS_TODOWNLOAD_ON_ONE_GO);

            m_DownloadFileOrganizer.Completed += new EventHandler<TaskProcessOrganizer<StateManager.Item>.CompletedEventArgs>(DownloadOrganizer_Completed);

            foreach (var ex in SUPPORTED_FILE_EXTENTIONS)
                this.m_FileSysWatchers.Add(new FileSystemWatcher());

            this.m_FileSysWatchers.ForEach(f => f.Changed += new System.IO.FileSystemEventHandler(this.FileSysWatcher_Event));
            this.m_FileSysWatchers.ForEach(f => f.Renamed += new System.IO.RenamedEventHandler(this.FileSysWatcher_Event));
            this.m_FileSysWatchers.ForEach(f => f.EnableRaisingEvents = false);
            this.m_FileSysWatchers.ForEach(f => f.SynchronizingObject = this);

            this.Text = "Voucher Printing - (Current User : ".concat(Program.currentUser.Username, ")",
                "  (Connected to: ", Program.LIVE_IP, ")");
            //printdemoToolStripMenuItem.Visible = Program.IsDebug;
            m_HeadOfficeId = Convert.ToInt32(ConfigurationManager.AppSettings["HeadOfficeId"]);
            dgvAllocations.AutoGenerateColumns = false;
            dgvAllocations.Click += new EventHandler(DataGrid_Click);
            dgvAllocations.DataError += new DataGridViewDataErrorEventHandler(Allocations_DataError);
            m_MainContext = SynchronizationContext.Current;

            ms_MiltuPagePrint = Convert.ToBoolean(ConfigurationManager.AppSettings["MILTUPAGEPRINT"]);

            VoucherPrinter.Repeat.Load(ConfigurationManager.AppSettings["REPEAT"]);

            Default = this;

            PrinterQueue.CacheDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Application.ProductName);

            if (ConfigurationManager.AppSettings["PRINTINGONLY"].Cast<bool>())
            {
                tabControl1.TabPages.RemoveAt(1);
                tabControl1.TabPages.RemoveAt(1);
            }
            else if (ConfigurationManager.AppSettings["SCANNINGONLY"].Cast<bool>())
            {
                tabControl1.TabPages.RemoveAt(0);
            }

            if (!ConfigurationManager.AppSettings["USE_SCAN_SERVER"].Cast<bool>())
                this.ScanningSettingPanel.Controls.Remove(this.btnScan);

            ToolTip1.SetToolTip(btnShowHide, "Click to hide/show");
        }

        #endregion //CONSTRUCTOR

        #region OVERRIDES

        protected override void OnLoad(EventArgs e)
        {
            InitializeLayout();

            new MethodInvoker(() =>
            {
                // Get the retailer specific setting s from the .config file
                var partyManagement = new PartyManagement.PartyManagement();
                //It takes too much to execute this call on live db
                var headofficeList = partyManagement.RetrieveHeadOfficeList(Program.currentUser.CountryID);

                this.InvokeSf(new MethodInvoker(() =>
                {
                    VoucherDS.HeadOfficeDataTable table = new VoucherDS.HeadOfficeDataTable();

                    table.AddHeadOfficeRow("Please Select", null);

                    foreach (var headoffice in headofficeList)
                        table.AddHeadOfficeRow(headoffice.Name, headoffice.Id.ToString());

                    cmbHeadoffice.DisplayMember = "Name";
                    cmbHeadoffice.ValueMember = "Id";
                    cmbHeadoffice.DataSource = table;

                    lblStatus.Text = "";

                    //this is added by Ravi on 18th Dec 2008, to default the double/single sale radio button based on voucher types
                    string vouchertype = ConfigurationManager.AppSettings["VoucherType"];
                    rdoSingleSale.Checked = (string.Compare(vouchertype, "S", true) == 0);
                    rdoDoubleSale.Checked = !rdoSingleSale.Checked;

                    bool allowReprint = Convert.ToBoolean(ConfigurationManager.AppSettings["AllowReprint"]);
                    btnRePrint.Visible = allowReprint;
                    if (!allowReprint)
                    {
                        groupBox1.Width = 256;
                        groupBox2.Left = 636;
                    }

                    this.tbScanDirectory.Text = StateSaver.Default.Get<string>(Strings.tbScanDirectory, "C:\\");

                    this.toggleButtonControl1.PerformClick(StateSaver.Default.Get<int>("toggleButtonControl1.GetClicked"));
                    this.toggleButtonControl2.PerformClick(StateSaver.Default.Get<int>("toggleButtonControl2.GetClicked"));

                    Global.Instance.LoadCompleted.Set();

                    cbHistoryType.Items.Clear();

                    const int ADMIN_OPERS = 100;
                    foreach (OperationHistory a in OperationHistory.Scan.GetValues<OperationHistory>())
                        if ((int)a < ADMIN_OPERS || Program.IsAdmin)
                            cbHistoryType.Items.Add(a);
                    cbHistoryType.SelectedItem = OperationHistory.Scan;
                }));
            }).FireAndForget();

            base.OnLoad(e);

            PrinterQueue.EmptyCache();
        }

        protected override void OnClosed(EventArgs e)
        {
            StateSaver.Default.Set(Strings.tbScanDirectory, this.tbScanDirectory.Text);
            StateSaver.Default.Set("toggleButtonControl1.GetClicked", this.toggleButtonControl1.GetClicked());
            StateSaver.Default.Set("toggleButtonControl2.GetClicked", this.toggleButtonControl2.GetClicked());

            m_StateManager.Clear();
            new Action(() => ServiceDataAccess.Instance.LogOperation(OperationHistory.Logout, Program.SessionId, 0, 0, 0, 0, 0, "")).RunSafe();
            Global.Instance.ExitSignal = true;
            m_BuildFilesExit.DisposeSf();
            base.OnClosed(e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #endregion //OVERRIDES

        #region HANDLERS PRINTING

        private void DataGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvAllocations.DataSource == null)
                return;

            var current = dgvAllocations.CurrentCell;

            if (current == null)
                return;

            PrepareDataLists();
        }

        private void DataGrid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvAllocations.DataSource == null)
                return;

            var table = (dgvAllocations.DataSource as DataView).Table;

            var current = dgvAllocations.CurrentCell;

            if (current == null)
                return;

            var row = dgvAllocations.CurrentRow;

            switch (current.ColumnIndex)
            {
                case 12: //Dispatch
                    {
                        string status = row.Cells["Status"].Value.Cast<string>();
                        if (status == "Printed")
                        {
                            int id = row.Cells["Id"].Value.Cast<int>();
                            DateTime date = row.Cells["Dispatch Date"].Value.Cast<DateTime>();
                            string method = row.Cells["Dispatch Method"].Value.Cast<string>();
                            bool priority = row.Cells["Priority"].Value.Cast<bool>();
                            DispatchVoucher(id, method, date, priority);
                            this.ShowInfo("Voucher Dispatched by ".concat(method));
                        }
                        else
                            this.ShowExclamation("You Cannot Dispatch Vouchers that have not been printed or stoplisted.");
                        //Is the user selecting pos vouchers and may try the re-print button.
                        //row["Dispatch Method"] == "Shop"
                        //row["Dispatch Method"] == "POS"
                    }
                    break;
            }
        }

        private void DataGrid_Click(object sender, EventArgs e)
        {
            if (dgvAllocations.DataSource == null)
                return;

            var table = (dgvAllocations.DataSource as DataView).Table;

            var current = dgvAllocations.CurrentCell;

            if (current == null)
                return;

            var row = dgvAllocations.CurrentRow;

            switch (current.ColumnIndex)
            {
                case 14: //Priority
                    current.Value = !current.Value.Cast<bool>();
                    break;
            }
        }

        private void Refresh_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                lblStatus.Text = "Please wait generating output...";
                m_RetailerId = cmbRetailer.SelectedValue.Cast<int>();
                ShowAllocationList(cmbHeadoffice.SelectedValue.Cast<int>(), m_RetailerId, char.MinValue, true);
            }
            finally
            {
                lblStatus.Text = "";
                Cursor.Current = Cursors.Default;
            }
        }

        private void Block_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to block the selected voucher range?", "Block Voucher Range",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                return;

            try
            {
                Cursor.Current = Cursors.WaitCursor;

                var voucherNumberAllocation = new VoucherNumberingAllocationPrinting.VoucherNumberingAllocationPrinting();

                var list = new List<int>(m_PendingVouchersList);
                list.AddRange(m_PrintedVouchersList);

                foreach (int id in list)
                {
                    voucherNumberAllocation.BlacklistAllocationAsync(Program.currentUser.CountryID, m_HeadOfficeId, m_RetailerId, id);
                    var r = dgvAllocations.Rows.Find<int>("Id", new Predicate<int>((i) => i == id)).FirstOrDefault();
                    if (r != null)
                        r.Cells["Status"].Value = "Stoplisted";
                }

                ClearDataGridViewItems(m_PendingVouchersList);
                ClearDataGridViewItems(m_PrintedVouchersList);
            }
            catch (Exception ex)	//catch and display the exception
            {
                lblStatus.Text = ex.Message;
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void EditLayout_Click(object sender, EventArgs e)
        {
            FormLayout form = new FormLayout();
            form.Show();
        }

        private void ReprintFromCache_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Title = Messages.LoadVoucherFromCache;
                dlg.Multiselect = true;
                dlg.InitialDirectory = PrinterQueue.CacheDirectory;
                dlg.DefaultExt = "*.vol";
                dlg.Filter = "VOL|*.vol";
                dlg.ReadOnlyChecked = true;

                if (dlg.ShowDialog(this) == DialogResult.OK)
                    foreach (var file in dlg.FileNames)
                        PrinterQueue.AddFromFile(file);
            }
        }

        private void Retailer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (string.IsNullOrEmpty(this.cmbRetailer.Text))
                    return;

                new Action<string>((txt) =>
                {
                    var partyManagement = new PartyManagement.PartyManagement();
                    var headofficeList = partyManagement.RetrieveHeadOfficeList(Program.currentUser.CountryID);

                    ListRetailers(new Func<Retailer, bool>((r) => r.Name.StartsWith(txt, StringComparison.CurrentCultureIgnoreCase)),
                        headofficeList.Convert<HeadOffice, int>((h) => h.Id));

                }).FireAndForgetSafe(new string(this.cmbRetailer.Text.ToCharArray()));
            }
        }

        private void PrintDemo_Click(object sender, EventArgs e)
        {
            using (var printing = new VoucherPrinter())
                printing.PrintAllocation(0, true);
        }

        private void Print_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to print the selected voucher ranges?", "Print Voucher Range",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
                return;

            Cursor.Current = Cursors.WaitCursor;

            lblStatus.Text = "Please wait, printing vouchers...";

            new MethodInvoker(() =>
            {
                try
                {
                    dgvAllocations.InvokeSf(new MethodInvoker(() => dgvAllocations.Enabled = false));

                    for (int i = 0; i < m_PendingVouchersList.Count; i++)
                    {
                        using (var printing = new VoucherPrinter())
                        {
                            printing.SimulatePrint = MainForm.ms_SimulatePrint;
                            printing.MultyPagePrint = MainForm.ms_MiltuPagePrint;

                            printing.Done += OnAllocationRowIsPrinted;
                            Interlocked.Increment(ref m_AllocationPrintings);
                            printing.PrintAllocation(m_PendingVouchersList[i], false);
                            ServiceDataAccess.Instance.LogOperation(OperationHistory.Print, printing.SessionId, Program.currentUser.CountryID, printing.Retailer.Id,
                                printing.RangeFrom, printing.RangeTo, 0, "");
                        }
                    }

                    ClearDataGridViewItems(m_PendingVouchersList);
                }
                catch (Exception ex)
                {
                    lblStatus.InvokeSf(new MethodInvoker(() => lblStatus.Text = ex.Message));
                }
                finally
                {
                    dgvAllocations.InvokeSf(new MethodInvoker(() => dgvAllocations.Enabled = true));
                    Cursor.Current = Cursors.Default;
                }
            }).FireAndForget();
        }

        private void OnAllocationRowIsPrinted(object sender, EventArgs args)
        {
            var printing = (VoucherPrinter)sender;
            printing.Done -= OnAllocationRowIsPrinted;

            new MethodInvoker(() =>
            {
                this.Invoke(new EventHandler((s, e) =>
                {
                    var r = dgvAllocations.Rows.Find<int>("Id", new Predicate<int>((i) => i == printing.AllocationId)).FirstOrDefault();
                    if (r != null)
                    {
                        if (RemoveRowDelegate != null)
                            RemoveRowDelegate(dgvAllocations, r);
                        else
                            r.Cells["Status"].Value = "Printed";
                    }

                    if (Interlocked.Decrement(ref m_AllocationPrintings) == 0)
                        lblStatus.Text = "";

                    printing.DisposeSf();

                }), sender, args);
            }).FireAndForget();
        }

        private void ShowRetailer_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            lblStatus.Text = Messages.PleaseWaitOutput;

            if (cmbHeadoffice.SelectedIndex != 0)
            {
                m_HeadOfficeId = cmbHeadoffice.SelectedValue.Cast<int>();
                //cmbRetailer.Enabled = true;
                ListRetailers(null, new int[] { m_HeadOfficeId });
            }
            else
            {
                this.ShowExclamation("Please select a head office!");
            }
            lblStatus.Text = "";
            Cursor.Current = Cursors.Default;
            RemoveRowDelegate = null;
        }

        private void Headoffice_SelectedIndexChanged(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            //cmbRetailer.Enabled = false;
            dgvAllocations.DataSource = null;
            Cursor.Current = Cursors.Default;
            RemoveRowDelegate = null;
        }

        private void ShowUnprintedAllocatedVouchers_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                lblStatus.Text = Messages.PleaseWaitOutput;
                m_RetailerId = cmbRetailer.SelectedValue.Cast<int>();
                ShowAllocationList(cmbHeadoffice.SelectedValue.Cast<int>(), m_RetailerId, 'N', true);
            }
            finally
            {
                lblStatus.Text = "";
                Cursor.Current = Cursors.Default;
                RemoveRowDelegate = REMOVEROW;
            }
        }

        private void NotDispatched_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                lblStatus.Text = Messages.PleaseWaitOutput;
                m_RetailerId = cmbRetailer.SelectedValue.Cast<int>();
                ShowAllocationList(cmbHeadoffice.SelectedValue.Cast<int>(), m_RetailerId, 'Y', false);
            }
            finally
            {
                lblStatus.Text = "";
                Cursor.Current = Cursors.Default;
                RemoveRowDelegate = null;
            }
        }

        private void SelectAll_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            foreach (DataGridViewRow row in dgvAllocations.Rows)
                row.Cells["Select"].Value = cboSelectAllNone.Checked;

            PrepareDataLists();

            Cursor.Current = Cursors.Default;
        }

        private void RePrint_Click(object sender, EventArgs e)
        {
            //Set the wait cursor
            Cursor.Current = Cursors.WaitCursor;

            lblStatus.Text = "Please wait, preparing for reprint...";

            new MethodInvoker(() =>
            {
                try
                {
                    foreach (var id in m_PrintedVouchersList)
                    {
                        this.InvokeSf(() =>
                        {
                            using (FormPrint frmPrint = new FormPrint(id))
                            {
                                if (frmPrint.ShowDialog(this) == DialogResult.OK)
                                {
                                    using (var printing = new VoucherPrinter())
                                    {
                                        printing.SimulatePrint = MainForm.ms_SimulatePrint;
                                        printing.MultyPagePrint = MainForm.ms_MiltuPagePrint;

                                        printing.Done += OnAllocationRowIsPrinted;
                                        Interlocked.Increment(ref m_AllocationPrintings);
                                        printing.PrintAllocation(id, frmPrint.ReprintVouchers);

                                        ServiceDataAccess.Instance.LogOperation(
                                            OperationHistory.RePrint, printing.SessionId,
                                            Program.currentUser.CountryID, printing.Retailer.Id,
                                            printing.RangeFrom, printing.RangeTo, 0, "");
                                    }
                                }
                            }
                        });
                    }
                    lblStatus.InvokeSf(new MethodInvoker(() => lblStatus.Text = ""));

                    ClearDataGridViewItems(m_PrintedVouchersList);
                }
                catch (Exception ex)
                {
                    lblStatus.InvokeSf(new MethodInvoker(() => lblStatus.Text = ex.Message));
                }
                finally
                {
                    Cursor.Current = Cursors.Default;
                }
            }).FireAndForget();
        }

        private void Allocations_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
#if DEBUGGER
            Debug.WriteLine(e);
#endif
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void EmptyCache_Click(object sender, EventArgs e)
        {
            if (this.ShowQuestion("Delete cache?\r\nAre you sure?",
                MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                CacheManager.Instance.Clear();
                PrinterQueue.EmptyCache();
            }
        }

        #endregion //HANDLERS

        #region PRIVATE METHODS PRINTING

        private void BuildDataGridView(VoucherAllocation[] allocationArray)
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                var partyManagement = new PartyManagement.PartyManagement();
                Debug.Assert(partyManagement != null, "partyManagement");

                var dt = new VoucherDS.AllocationsDataTable();
                dt.TableName = "allocation";

                foreach (var a in allocationArray)
                {
                    int activityCode = a.HeadOfficeVoucherType;

                    DateTime DispatchDate;
                    if (a.DispatchDate == new DateTime(1753, 1, 1))
                        DispatchDate = DateTime.MinValue;
                    else
                        DispatchDate = a.DispatchDate;

                    string method = null;
                    if (a.Pos)
                        method = "POS";
                    else if (a.Shop)
                        method = "Shop";
                    else if (a.Printer)
                        method = "Printer";
                    else
                        throw new NotImplementedException();

                    string status = null;
                    if (a.Printed)
                        status = (a.BlacklistId != 0) ? "Stoplisted" : "Printed";
                    else
                        status = (a.BlacklistId != 0) ? "Stoplisted" : "Pending";

                    dt.AddAllocationsRow(false,
                        a.Id, a.AllocationDate,
                        a.HeadOfficeId.ToString(), a.RetailerName, a.RetailerId.ToString(),
                        a.RetailerAddress, activityCode, a.OrderVolume,
                        a.RangeFrom.ToString("00000000#"), a.RangeTo.ToString("00000000#"), status,
                        method, DispatchDate, a.Priority,
                        a.SubRangeFrom, a.SubRangeTo);
                }

                // Create a table style that will hold the new column style 
                // that we set and also tie it to our customer's table from our DB
                //since the dataset has things like field name and number of columns,
                //we will use those to create new columnstyles for the columns in our DB table

                string dispatchMethods = ConfigurationManager.AppSettings["DispatchMethods"];
                Debug.Assert(dispatchMethods != null, "DispatchMethods");
                string[] strMethods = dispatchMethods.Split(new char[] { ';' });

                lock (((ICollection)dgvAllocations.Rows).SyncRoot)
                {

                    dgvAllocations.Columns.Clear();

                    for (int i = 0; i < dt.Columns.Count; ++i)
                    {
                        DataGridViewColumn col;
                        if (dt.Columns[i].DataType.Equals(typeof(Boolean)))
                        {
                            col = new DataGridViewCheckBoxColumn();
                            col.HeaderText = dt.Columns[i].ColumnName;
                            col.DataPropertyName = dt.Columns[i].ColumnName;
                            col.ReadOnly = (dt.Columns[i].ColumnName == "Priority");
                        }
                        else if (dt.Columns[i].ColumnName == "Dispatch Method")
                        {
                            col = new DataGridViewComboBoxColumn();
                            col.HeaderText = dt.Columns[i].ColumnName;
                            col.DataPropertyName = dt.Columns[i].ColumnName;
                            (col as DataGridViewComboBoxColumn).DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton;
                            (col as DataGridViewComboBoxColumn).ValueType = typeof(string);
                            (col as DataGridViewComboBoxColumn).DataSource = strMethods;
                        }
                        else if (dt.Columns[i].ColumnName == "Dispatch Date")
                        {
                            col = new DataGridViewTextBoxColumn();
                            col.HeaderText = dt.Columns[i].ColumnName;
                            col.DataPropertyName = dt.Columns[i].ColumnName;
                            col.ValueType = typeof(DateTime);
                        }
                        else
                        {
                            col = new DataGridViewTextBoxColumn();
                            col.ReadOnly = true;
                            col.HeaderText = dt.Columns[i].ColumnName;
                            col.DataPropertyName = dt.Columns[i].ColumnName;
                        }

                        col.Name = dt.Columns[i].ColumnName;
                        col.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                        dgvAllocations.Columns.Add(col);
                    }

                    if (dgvAllocations.Columns[dgvAllocations.ColumnCount - 1] != null)
                        dgvAllocations.Columns[dgvAllocations.ColumnCount - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                    Debug.Assert(dt != null, "dt");
                    DataView dv = new DataView(dt);
                    dv.AllowNew = false;
                    dv.Sort = "Priority desc";
                    dgvAllocations.DataSource = dv;
                }
            }
            finally
            {
                Cursor.Current = Cursors.Default;
                cboSelectAllNone.Checked = false;
            }
        }

        private void PrepareDataLists()
        {
            if (dgvAllocations.DataSource == null)
                return;

            var current = dgvAllocations.CurrentCell;

            if (current == null)
                return;

            var table = (dgvAllocations.DataSource as DataView).Table;

            if (table == null)
                return;

            //Select
            //Is the user selecting shop vouchers and may try the re-print button.
            //"You Cannot Dispatch Vouchers that have not been printed."

            m_PendingVouchersList.Clear();
            m_PrintedVouchersList.Clear();
            m_StoplistedVouchersList.Clear();

            try
            {

                foreach (DataGridViewRow row in dgvAllocations.Rows.Find<bool>("Select", new Predicate<bool>((v) => v)))
                {
                    var id = row.Cells["Id"].Value.Cast<int>();

                    switch (row.Cells["Status"].Value.Cast<string>())
                    {
                        case "Pending":
                            m_PendingVouchersList.Add(id);
                            break;
                        case "Printed":
                            m_PrintedVouchersList.Add(id);
                            break;
                        case "Stoplisted":
                            m_StoplistedVouchersList.Add(id);
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
            }
            catch
            {
                //Network error
            }
            finally
            {
                btnBlock.Enabled = m_PendingVouchersList.Count != 0 || m_PrintedVouchersList.Count != 0;
                btnPrint.Enabled = m_PendingVouchersList.Count != 0;
                btnRePrint.Enabled = m_PrintedVouchersList.Count != 0;
            }
        }

        private void DispatchVoucher(int id, string method, DateTime date, bool priority)
        {
            var voucherNumberAllocation = new VoucherNumberingAllocationPrinting.VoucherNumberingAllocationPrinting();

            voucherNumberAllocation.SetVoucherDispatchMedthod(id, method, date);

            if (date == DateTime.MinValue)
            {
                voucherNumberAllocation.SetVoucherAllocationPriority(id, priority);
            }
            else
            {
                voucherNumberAllocation.SetVoucherAllocationPriority(id, false);
            }
        }

        private void ClearDataGridViewItems(IList<int> idlist)
        {
            this.Invoke(new Action<IList>((lst) =>
            {
                lock (((ICollection)dgvAllocations.Rows).SyncRoot)
                {
                    foreach (int id in idlist)
                        foreach (var item in dgvAllocations.Rows.Find<int>("Id", new Predicate<int>((v) => v == id)))
                            item.Cells["Select"].Value = false;

                    idlist.Clear();
                }
            }), idlist);
        }

        private static string GetRetailerName(int countryId, int retailerId)
        {
            var partyManagement = new PartyManagement.PartyManagement();
            var details = partyManagement.RetrieveRetailerDetail(countryId, retailerId);
            return details.Name ?? string.Empty;
        }

        private void ListRetailers(Func<Retailer, bool> funct, IEnumerable<int> headofficeIds)
        {
            #region Populate Retailer Drop Down List

            var partyManagement = new PartyManagement.PartyManagement();

            var table = new VoucherDS.RetailerDataTable();
            table.AddRetailerRow("Please Select", null);

            foreach (var headofficeId in headofficeIds) //m_HeadOfficeId
            {
                var retailerList = partyManagement.RetrieveRetailerList(Program.currentUser.CountryID, headofficeId);
                foreach (var retailer in retailerList)
                {
                    if (funct == null || funct(retailer))
                    {
                        var name = string.Format("{0}-{1} {2}: id({3})",
                            retailer.Name,
                            retailer.RetailAddress.Line1, retailer.RetailAddress.Line5,
                            retailer.Id);
                        table.AddRetailerRow(name, retailer.Id.ToString());
                    }
                }
            }

            this.InvokeSf(new MethodInvoker(() =>
            {
                cmbRetailer.DataSource = table;
                cmbRetailer.DisplayMember = "Name";
                cmbRetailer.ValueMember = "Id";
            }));

            #endregion
        }

        private void ShowAllocationList(int headofficeId, int retailerId, char printed, bool dispatched)
        {
            bool doubleSell = rdoDoubleSale.Checked;
            var voucherPrinting = new VoucherNumberingAllocationPrinting.VoucherNumberingAllocationPrinting();
            var allocationArray = voucherPrinting.RetrieveRetailerRange(Program.currentUser.CountryID, headofficeId, retailerId, printed, doubleSell, dispatched);
            BuildDataGridView(allocationArray);
        }

        private void InitializeLayout()
        {
#if LOAD_FROM_FILE
            this.Text = this.Text.concat("  <Load from file>");
#endif

#if PRINT_TO_FILE
#warning PRINT_TO_FILE
            this.Text = this.Text.concat("  <Print to file>");
#endif
            printdemoToolStripMenuItem.Visible =
            toolStripMenuItem2.Visible =
            toolStripMenuItem3.Visible =
            saveToolStripMenuItem.Visible = Program.IsAdmin;
        }

        #endregion //PRIVATE METHODS

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Image Store");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.label1 = new System.Windows.Forms.Label();
            this.cmbRetailer = new System.Windows.Forms.ComboBox();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnBlock = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbHeadoffice = new System.Windows.Forms.ComboBox();
            this.btnShowRetailer = new System.Windows.Forms.Button();
            this.btnShowUnprintedAllocatedVouchers = new System.Windows.Forms.Button();
            this.rdoSingleSale = new System.Windows.Forms.RadioButton();
            this.rdoDoubleSale = new System.Windows.Forms.RadioButton();
            this.btnNotDispatched = new System.Windows.Forms.Button();
            this.cboSelectAllNone = new System.Windows.Forms.CheckBox();
            this.dgvAllocations = new System.Windows.Forms.DataGridView();
            this.printContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.printdemoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mapPrinterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.simulatePrintToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.assignFormatToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiEmptyCache = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripSeparator();
            this.updateVersionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lblPrinted = new System.Windows.Forms.Label();
            this.lblPending = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.btnRePrint = new System.Windows.Forms.Button();
            this.btnReprintFromCache = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tpPrint = new System.Windows.Forms.TabPage();
            this.tpScan = new System.Windows.Forms.TabPage();
            this.gbScanHeader = new System.Windows.Forms.GroupBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tvFolders = new System.Windows.Forms.TreeView();
            this.folderMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tbAddNew = new System.Windows.Forms.ToolStripMenuItem();
            this.tbDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
            this.tbRename = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList2 = new System.Windows.Forms.ImageList(this.components);
            this.lpScannedFiles = new System.Windows.Forms.FlowLayoutPanel();
            this.scanContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiSend = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiScanningShow = new System.Windows.Forms.ToolStripMenuItem();
            this.showSignedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem8 = new System.Windows.Forms.ToolStripSeparator();
            this.scheduleStartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem11 = new System.Windows.Forms.ToolStripSeparator();
            this.nameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiScanningDetailes = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem10 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiScaningDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiScaningDeleteAll = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiScanningClear = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiScanningForce = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiScanningForceAll = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiScanningIgnore = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem9 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiScanningAddToTran = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem7 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiExit = new System.Windows.Forms.ToolStripMenuItem();
            this.ssScaningStatus = new System.Windows.Forms.StatusStrip();
            this.lblScanned = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblSent = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblItemsWithErr = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblMessage = new System.Windows.Forms.ToolStripStatusLabel();
            this.pbScanProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.ScanningSettingPanel = new System.Windows.Forms.Panel();
            this.toggleButtonControl2 = new VPrinting.Controls.ToggleButtonControl();
            this.lblNext = new System.Windows.Forms.LinkLabel();
            this.lblPrev = new System.Windows.Forms.LinkLabel();
            this.btnFileBrowser = new System.Windows.Forms.Button();
            this.toggleButtonControl1 = new VPrinting.Controls.ToggleButtonControl();
            this.cbVoucherMustExist = new System.Windows.Forms.CheckBox();
            this.cbCoversheet = new System.Windows.Forms.CheckBox();
            this.btnShowMonitor = new System.Windows.Forms.Button();
            this.btnCover = new System.Windows.Forms.Button();
            this.llblOpenFolder = new System.Windows.Forms.LinkLabel();
            this.lblWarningMessage = new VPrinting.Controls.BliningLabel();
            this.btnShowHide = new VPrinting.Controls.ArrowButton.ArrowButton();
            this.btnScan = new GaryPerkin.UserControls.Buttons.RoundButton();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.tbTransferFile = new System.Windows.Forms.TextBox();
            this.btnBrowseForExprFile = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnRun = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.tbScanDirectory = new System.Windows.Forms.TextBox();
            this.btnBrowseForScanDir = new System.Windows.Forms.Button();
            this.tbSearch = new System.Windows.Forms.TabPage();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabVoucher = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.bliningLabel1 = new VPrinting.Controls.BliningLabel();
            this.btSearch = new System.Windows.Forms.Button();
            this.dtTo = new VPrinting.Controls.DateTimePicker2();
            this.dtFrom = new VPrinting.Controls.DateTimePicker2();
            this.label8 = new System.Windows.Forms.Label();
            this.tbVoucherId = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.cbCountryId = new System.Windows.Forms.ComboBox();
            this.tbBrId = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tabHistory = new System.Windows.Forms.TabPage();
            this.pnlDataGrid = new System.Windows.Forms.Panel();
            this.bliningLabel2 = new VPrinting.Controls.BliningLabel();
            this.cbHistoryType = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.historyToTime = new System.Windows.Forms.DateTimePicker();
            this.historyFromTime = new System.Windows.Forms.DateTimePicker();
            this.btnShowHistory = new System.Windows.Forms.Button();
            this.dgvSearchData = new System.Windows.Forms.DataGridView();
            this.historyMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiSort = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiShow = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tslItemsCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.ToolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnCCCover = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAllocations)).BeginInit();
            this.printContextMenu.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tpPrint.SuspendLayout();
            this.tpScan.SuspendLayout();
            this.gbScanHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.folderMenuStrip.SuspendLayout();
            this.scanContextMenuStrip.SuspendLayout();
            this.ssScaningStatus.SuspendLayout();
            this.ScanningSettingPanel.SuspendLayout();
            this.tbSearch.SuspendLayout();
            this.groupBox6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tabVoucher.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabHistory.SuspendLayout();
            this.pnlDataGrid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSearchData)).BeginInit();
            this.historyMenuStrip.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(54, 53);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Retailer :";
            // 
            // cmbRetailer
            // 
            this.cmbRetailer.Location = new System.Drawing.Point(119, 50);
            this.cmbRetailer.MaxDropDownItems = 24;
            this.cmbRetailer.Name = "cmbRetailer";
            this.cmbRetailer.Size = new System.Drawing.Size(240, 21);
            this.cmbRetailer.TabIndex = 9;
            this.cmbRetailer.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Retailer_KeyDown);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRefresh.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRefresh.Location = new System.Drawing.Point(8, 35);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(103, 24);
            this.btnRefresh.TabIndex = 11;
            this.btnRefresh.Text = "Show &Allocations";
            this.btnRefresh.Click += new System.EventHandler(this.Refresh_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.ForeColor = System.Drawing.Color.Navy;
            this.lblStatus.Location = new System.Drawing.Point(587, 108);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(376, 23);
            this.lblStatus.TabIndex = 12;
            // 
            // btnBlock
            // 
            this.btnBlock.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBlock.Enabled = false;
            this.btnBlock.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBlock.Location = new System.Drawing.Point(8, 60);
            this.btnBlock.Name = "btnBlock";
            this.btnBlock.Size = new System.Drawing.Size(103, 24);
            this.btnBlock.TabIndex = 14;
            this.btnBlock.Text = "Stoplist &Voucher";
            this.btnBlock.Click += new System.EventHandler(this.Block_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPrint.Enabled = false;
            this.btnPrint.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPrint.Location = new System.Drawing.Point(90, 60);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(126, 24);
            this.btnPrint.TabIndex = 13;
            this.btnPrint.Text = "&Print Range";
            this.btnPrint.Click += new System.EventHandler(this.Print_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(34, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 13);
            this.label2.TabIndex = 16;
            this.label2.Text = "HeadOffice :";
            // 
            // cmbHeadoffice
            // 
            this.cmbHeadoffice.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.cmbHeadoffice.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbHeadoffice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbHeadoffice.Location = new System.Drawing.Point(119, 23);
            this.cmbHeadoffice.MaxDropDownItems = 24;
            this.cmbHeadoffice.Name = "cmbHeadoffice";
            this.cmbHeadoffice.Size = new System.Drawing.Size(240, 21);
            this.cmbHeadoffice.TabIndex = 15;
            this.cmbHeadoffice.SelectedIndexChanged += new System.EventHandler(this.Headoffice_SelectedIndexChanged);
            // 
            // btnShowRetailer
            // 
            this.btnShowRetailer.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnShowRetailer.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnShowRetailer.Location = new System.Drawing.Point(8, 11);
            this.btnShowRetailer.Name = "btnShowRetailer";
            this.btnShowRetailer.Size = new System.Drawing.Size(103, 24);
            this.btnShowRetailer.TabIndex = 17;
            this.btnShowRetailer.Text = "Show Re&tailers";
            this.btnShowRetailer.Click += new System.EventHandler(this.ShowRetailer_Click);
            // 
            // btnShowUnprintedAllocatedVouchers
            // 
            this.btnShowUnprintedAllocatedVouchers.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnShowUnprintedAllocatedVouchers.Location = new System.Drawing.Point(90, 16);
            this.btnShowUnprintedAllocatedVouchers.Name = "btnShowUnprintedAllocatedVouchers";
            this.btnShowUnprintedAllocatedVouchers.Size = new System.Drawing.Size(126, 36);
            this.btnShowUnprintedAllocatedVouchers.TabIndex = 18;
            this.btnShowUnprintedAllocatedVouchers.Text = "Show &Unprinted Allocated Vouchers";
            this.btnShowUnprintedAllocatedVouchers.UseVisualStyleBackColor = true;
            this.btnShowUnprintedAllocatedVouchers.Click += new System.EventHandler(this.ShowUnprintedAllocatedVouchers_Click);
            // 
            // rdoSingleSale
            // 
            this.rdoSingleSale.AutoSize = true;
            this.rdoSingleSale.Location = new System.Drawing.Point(6, 35);
            this.rdoSingleSale.Name = "rdoSingleSale";
            this.rdoSingleSale.Size = new System.Drawing.Size(78, 17);
            this.rdoSingleSale.TabIndex = 19;
            this.rdoSingleSale.Text = "Single Sale";
            this.rdoSingleSale.UseVisualStyleBackColor = true;
            // 
            // rdoDoubleSale
            // 
            this.rdoDoubleSale.AutoSize = true;
            this.rdoDoubleSale.Checked = true;
            this.rdoDoubleSale.Location = new System.Drawing.Point(6, 60);
            this.rdoDoubleSale.Name = "rdoDoubleSale";
            this.rdoDoubleSale.Size = new System.Drawing.Size(83, 17);
            this.rdoDoubleSale.TabIndex = 20;
            this.rdoDoubleSale.TabStop = true;
            this.rdoDoubleSale.Text = "Double Sale";
            this.rdoDoubleSale.UseVisualStyleBackColor = true;
            // 
            // btnNotDispatched
            // 
            this.btnNotDispatched.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNotDispatched.Location = new System.Drawing.Point(117, 14);
            this.btnNotDispatched.Name = "btnNotDispatched";
            this.btnNotDispatched.Size = new System.Drawing.Size(162, 21);
            this.btnNotDispatched.TabIndex = 21;
            this.btnNotDispatched.Text = "Show Printed Not &Dispatched";
            this.btnNotDispatched.UseVisualStyleBackColor = true;
            this.btnNotDispatched.Click += new System.EventHandler(this.NotDispatched_Click);
            // 
            // cboSelectAllNone
            // 
            this.cboSelectAllNone.AutoSize = true;
            this.cboSelectAllNone.Location = new System.Drawing.Point(16, 90);
            this.cboSelectAllNone.Name = "cboSelectAllNone";
            this.cboSelectAllNone.Size = new System.Drawing.Size(107, 17);
            this.cboSelectAllNone.TabIndex = 22;
            this.cboSelectAllNone.Text = "Select All / None";
            this.cboSelectAllNone.UseVisualStyleBackColor = true;
            this.cboSelectAllNone.Click += new System.EventHandler(this.SelectAll_Click);
            // 
            // dgvAllocations
            // 
            this.dgvAllocations.AllowUserToAddRows = false;
            this.dgvAllocations.AllowUserToDeleteRows = false;
            this.dgvAllocations.ContextMenuStrip = this.printContextMenu;
            this.dgvAllocations.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvAllocations.Location = new System.Drawing.Point(3, 147);
            this.dgvAllocations.Name = "dgvAllocations";
            this.dgvAllocations.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvAllocations.Size = new System.Drawing.Size(1116, 382);
            this.dgvAllocations.TabIndex = 1;
            this.dgvAllocations.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGrid_CellEndEdit);
            this.dgvAllocations.SelectionChanged += new System.EventHandler(this.DataGrid_SelectionChanged);
            // 
            // printContextMenu
            // 
            this.printContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.printdemoToolStripMenuItem,
            this.mapPrinterToolStripMenuItem,
            this.simulatePrintToolStripMenuItem,
            this.toolStripMenuItem2,
            this.saveToolStripMenuItem,
            this.assignFormatToolStripMenuItem,
            this.toolStripMenuItem3,
            this.tsmiEmptyCache,
            this.toolStripMenuItem6,
            this.updateVersionToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem});
            this.printContextMenu.Name = "contextMenuStrip1";
            this.printContextMenu.Size = new System.Drawing.Size(148, 204);
            this.printContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.PrintContextMenu_Opening);
            // 
            // printdemoToolStripMenuItem
            // 
            this.printdemoToolStripMenuItem.Name = "printdemoToolStripMenuItem";
            this.printdemoToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.printdemoToolStripMenuItem.Text = "Print &demo";
            this.printdemoToolStripMenuItem.ToolTipText = "Print demo voucher";
            this.printdemoToolStripMenuItem.Click += new System.EventHandler(this.PrintDemo_Click);
            // 
            // mapPrinterToolStripMenuItem
            // 
            this.mapPrinterToolStripMenuItem.Name = "mapPrinterToolStripMenuItem";
            this.mapPrinterToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.mapPrinterToolStripMenuItem.Text = "&Map Printer";
            this.mapPrinterToolStripMenuItem.Click += new System.EventHandler(this.MapPrinterToolStripMenuItem_Click);
            // 
            // simulatePrintToolStripMenuItem
            // 
            this.simulatePrintToolStripMenuItem.Name = "simulatePrintToolStripMenuItem";
            this.simulatePrintToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.simulatePrintToolStripMenuItem.Text = "&Simulate Print";
            this.simulatePrintToolStripMenuItem.ToolTipText = "Simulate print voucher";
            this.simulatePrintToolStripMenuItem.Click += new System.EventHandler(this.SimulatePrint_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(144, 6);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.saveToolStripMenuItem.Text = "&Create Layout";
            this.saveToolStripMenuItem.ToolTipText = "Create voucher layout";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.SaveToolStripMenuItem_Click);
            // 
            // assignFormatToolStripMenuItem
            // 
            this.assignFormatToolStripMenuItem.Name = "assignFormatToolStripMenuItem";
            this.assignFormatToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.assignFormatToolStripMenuItem.Text = "&Assign Format";
            this.assignFormatToolStripMenuItem.Click += new System.EventHandler(this.AssignFormatToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(144, 6);
            // 
            // tsmiEmptyCache
            // 
            this.tsmiEmptyCache.Name = "tsmiEmptyCache";
            this.tsmiEmptyCache.Size = new System.Drawing.Size(147, 22);
            this.tsmiEmptyCache.Text = "Empty &Cache";
            this.tsmiEmptyCache.ToolTipText = "Empty local format cache";
            this.tsmiEmptyCache.Click += new System.EventHandler(this.EmptyCache_Click);
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(144, 6);
            // 
            // updateVersionToolStripMenuItem
            // 
            this.updateVersionToolStripMenuItem.Name = "updateVersionToolStripMenuItem";
            this.updateVersionToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.updateVersionToolStripMenuItem.Text = "&Update Version";
            this.updateVersionToolStripMenuItem.ToolTipText = "Update application version";
            this.updateVersionToolStripMenuItem.Click += new System.EventHandler(this.UpdateVersion_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(144, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.ToolTipText = "Exit application";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.Exit_Click);
            // 
            // lblPrinted
            // 
            this.lblPrinted.AutoSize = true;
            this.lblPrinted.Location = new System.Drawing.Point(275, 108);
            this.lblPrinted.Name = "lblPrinted";
            this.lblPrinted.Size = new System.Drawing.Size(0, 13);
            this.lblPrinted.TabIndex = 23;
            // 
            // lblPending
            // 
            this.lblPending.AutoSize = true;
            this.lblPending.Location = new System.Drawing.Point(331, 108);
            this.lblPending.Name = "lblPending";
            this.lblPending.Size = new System.Drawing.Size(0, 13);
            this.lblPending.TabIndex = 24;
            // 
            // groupBox1
            // 
            this.groupBox1.ContextMenuStrip = this.printContextMenu;
            this.groupBox1.Controls.Add(this.groupBox4);
            this.groupBox1.Controls.Add(this.btnShowRetailer);
            this.groupBox1.Controls.Add(this.btnBlock);
            this.groupBox1.Controls.Add(this.btnNotDispatched);
            this.groupBox1.Controls.Add(this.btnRefresh);
            this.groupBox1.Location = new System.Drawing.Point(365, 9);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(389, 96);
            this.groupBox1.TabIndex = 25;
            this.groupBox1.TabStop = false;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.btnRePrint);
            this.groupBox4.Controls.Add(this.btnReprintFromCache);
            this.groupBox4.Location = new System.Drawing.Point(117, 41);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(266, 49);
            this.groupBox4.TabIndex = 30;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Reprint";
            // 
            // btnRePrint
            // 
            this.btnRePrint.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRePrint.Location = new System.Drawing.Point(37, 19);
            this.btnRePrint.Name = "btnRePrint";
            this.btnRePrint.Size = new System.Drawing.Size(80, 24);
            this.btnRePrint.TabIndex = 27;
            this.btnRePrint.Text = "&Reprint";
            this.btnRePrint.UseVisualStyleBackColor = true;
            this.btnRePrint.Click += new System.EventHandler(this.RePrint_Click);
            // 
            // btnReprintFromCache
            // 
            this.btnReprintFromCache.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnReprintFromCache.Location = new System.Drawing.Point(145, 19);
            this.btnReprintFromCache.Name = "btnReprintFromCache";
            this.btnReprintFromCache.Size = new System.Drawing.Size(86, 24);
            this.btnReprintFromCache.TabIndex = 29;
            this.btnReprintFromCache.Text = "&Cache";
            this.btnReprintFromCache.UseVisualStyleBackColor = true;
            this.btnReprintFromCache.Click += new System.EventHandler(this.ReprintFromCache_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.ContextMenuStrip = this.printContextMenu;
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.rdoSingleSale);
            this.groupBox2.Controls.Add(this.rdoDoubleSale);
            this.groupBox2.Controls.Add(this.btnShowUnprintedAllocatedVouchers);
            this.groupBox2.Controls.Add(this.btnPrint);
            this.groupBox2.Location = new System.Drawing.Point(760, 9);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(225, 96);
            this.groupBox2.TabIndex = 26;
            this.groupBox2.TabStop = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 14);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(70, 13);
            this.label4.TabIndex = 21;
            this.label4.Text = "Voucher type";
            // 
            // groupBox3
            // 
            this.groupBox3.ContextMenuStrip = this.printContextMenu;
            this.groupBox3.Controls.Add(this.lblPending);
            this.groupBox3.Controls.Add(this.lblStatus);
            this.groupBox3.Controls.Add(this.lblPrinted);
            this.groupBox3.Controls.Add(this.groupBox2);
            this.groupBox3.Controls.Add(this.groupBox1);
            this.groupBox3.Controls.Add(this.cmbRetailer);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.cboSelectAllNone);
            this.groupBox3.Controls.Add(this.cmbHeadoffice);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox3.Location = new System.Drawing.Point(3, 16);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(1116, 131);
            this.groupBox3.TabIndex = 27;
            this.groupBox3.TabStop = false;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.dgvAllocations);
            this.groupBox5.Controls.Add(this.groupBox3);
            this.groupBox5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox5.Location = new System.Drawing.Point(3, 3);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(1122, 532);
            this.groupBox5.TabIndex = 28;
            this.groupBox5.TabStop = false;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tpPrint);
            this.tabControl1.Controls.Add(this.tpScan);
            this.tabControl1.Controls.Add(this.tbSearch);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(10, 10);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1136, 564);
            this.tabControl1.TabIndex = 29;
            // 
            // tpPrint
            // 
            this.tpPrint.Controls.Add(this.groupBox5);
            this.tpPrint.Location = new System.Drawing.Point(4, 22);
            this.tpPrint.Name = "tpPrint";
            this.tpPrint.Padding = new System.Windows.Forms.Padding(3);
            this.tpPrint.Size = new System.Drawing.Size(1128, 538);
            this.tpPrint.TabIndex = 0;
            this.tpPrint.Text = "           Print          ";
            this.tpPrint.UseVisualStyleBackColor = true;
            // 
            // tpScan
            // 
            this.tpScan.Controls.Add(this.gbScanHeader);
            this.tpScan.Location = new System.Drawing.Point(4, 22);
            this.tpScan.Name = "tpScan";
            this.tpScan.Padding = new System.Windows.Forms.Padding(3);
            this.tpScan.Size = new System.Drawing.Size(1128, 538);
            this.tpScan.TabIndex = 1;
            this.tpScan.Text = "            Scan           ";
            this.tpScan.UseVisualStyleBackColor = true;
            // 
            // gbScanHeader
            // 
            this.gbScanHeader.Controls.Add(this.splitContainer1);
            this.gbScanHeader.Controls.Add(this.ssScaningStatus);
            this.gbScanHeader.Controls.Add(this.ScanningSettingPanel);
            this.gbScanHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbScanHeader.Location = new System.Drawing.Point(3, 3);
            this.gbScanHeader.Name = "gbScanHeader";
            this.gbScanHeader.Size = new System.Drawing.Size(1122, 532);
            this.gbScanHeader.TabIndex = 0;
            this.gbScanHeader.TabStop = false;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 121);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tvFolders);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.lpScannedFiles);
            this.splitContainer1.Size = new System.Drawing.Size(1116, 386);
            this.splitContainer1.SplitterDistance = 171;
            this.splitContainer1.TabIndex = 4;
            // 
            // tvFolders
            // 
            this.tvFolders.ContextMenuStrip = this.folderMenuStrip;
            this.tvFolders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvFolders.ImageIndex = 18;
            this.tvFolders.ImageList = this.imageList2;
            this.tvFolders.Location = new System.Drawing.Point(0, 0);
            this.tvFolders.Name = "tvFolders";
            treeNode1.Name = "nRoot";
            treeNode1.Text = "Image Store";
            this.tvFolders.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1});
            this.tvFolders.SelectedImageKey = "folderopen.ico";
            this.tvFolders.Size = new System.Drawing.Size(171, 386);
            this.tvFolders.TabIndex = 0;
            this.tvFolders.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.ImageNode_Click);
            // 
            // folderMenuStrip
            // 
            this.folderMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tbAddNew,
            this.tbDelete,
            this.deleteAllToolStripMenuItem,
            this.toolStripMenuItem5,
            this.tbRename});
            this.folderMenuStrip.Name = "folderMenuStrip";
            this.folderMenuStrip.Size = new System.Drawing.Size(120, 98);
            this.folderMenuStrip.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.FolderMenu_ItemClicked);
            // 
            // tbAddNew
            // 
            this.tbAddNew.Name = "tbAddNew";
            this.tbAddNew.Size = new System.Drawing.Size(119, 22);
            this.tbAddNew.Text = "Add &New";
            // 
            // tbDelete
            // 
            this.tbDelete.Name = "tbDelete";
            this.tbDelete.Size = new System.Drawing.Size(119, 22);
            this.tbDelete.Text = "&Delete";
            // 
            // deleteAllToolStripMenuItem
            // 
            this.deleteAllToolStripMenuItem.Name = "deleteAllToolStripMenuItem";
            this.deleteAllToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
            this.deleteAllToolStripMenuItem.Text = "Delete &All";
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(116, 6);
            // 
            // tbRename
            // 
            this.tbRename.Name = "tbRename";
            this.tbRename.Size = new System.Drawing.Size(119, 22);
            this.tbRename.Text = "&Rename";
            // 
            // imageList2
            // 
            this.imageList2.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList2.ImageStream")));
            this.imageList2.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList2.Images.SetKeyName(0, "app.ico");
            this.imageList2.Images.SetKeyName(1, "document.ico");
            this.imageList2.Images.SetKeyName(2, "folderopen.ico");
            this.imageList2.Images.SetKeyName(3, "fonfile.ico");
            this.imageList2.Images.SetKeyName(4, "fonfont.ico");
            this.imageList2.Images.SetKeyName(5, "GenericPicDoc.ico");
            this.imageList2.Images.SetKeyName(6, "help.ico");
            this.imageList2.Images.SetKeyName(7, "ICS client.ico");
            this.imageList2.Images.SetKeyName(8, "idr_dll.ico");
            this.imageList2.Images.SetKeyName(9, "keys.ico");
            this.imageList2.Images.SetKeyName(10, "newfolder.ico");
            this.imageList2.Images.SetKeyName(11, "otheroptions.ico");
            this.imageList2.Images.SetKeyName(12, "scanner.ico");
            this.imageList2.Images.SetKeyName(13, "security.ico");
            this.imageList2.Images.SetKeyName(14, "sharedocuments.ico");
            this.imageList2.Images.SetKeyName(15, "sound.ico");
            this.imageList2.Images.SetKeyName(16, "user.ico");
            this.imageList2.Images.SetKeyName(17, "users.ico");
            this.imageList2.Images.SetKeyName(18, "Folder.ico");
            this.imageList2.Images.SetKeyName(19, "recycle_bin_f.ico");
            // 
            // lpScannedFiles
            // 
            this.lpScannedFiles.AutoScroll = true;
            this.lpScannedFiles.BackColor = System.Drawing.Color.White;
            this.lpScannedFiles.ContextMenuStrip = this.scanContextMenuStrip;
            this.lpScannedFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lpScannedFiles.Location = new System.Drawing.Point(0, 0);
            this.lpScannedFiles.Name = "lpScannedFiles";
            this.lpScannedFiles.Size = new System.Drawing.Size(941, 386);
            this.lpScannedFiles.TabIndex = 2;
            // 
            // scanContextMenuStrip
            // 
            this.scanContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiSend,
            this.tsmiScanningShow,
            this.showSignedToolStripMenuItem,
            this.toolStripMenuItem8,
            this.scheduleStartToolStripMenuItem,
            this.toolStripMenuItem11,
            this.nameToolStripMenuItem,
            this.tsmiScanningDetailes,
            this.toolStripMenuItem10,
            this.tsmiScaningDelete,
            this.tsmiScaningDeleteAll,
            this.tsmiScanningClear,
            this.toolStripMenuItem4,
            this.tsmiScanningForce,
            this.tsmiScanningForceAll,
            this.tsmiScanningIgnore,
            this.toolStripMenuItem9,
            this.tsmiScanningAddToTran,
            this.toolStripMenuItem7,
            this.tsmiExit});
            this.scanContextMenuStrip.Name = "scanContextMenuStrip";
            this.scanContextMenuStrip.Size = new System.Drawing.Size(159, 348);
            // 
            // tsmiSend
            // 
            this.tsmiSend.Name = "tsmiSend";
            this.tsmiSend.Size = new System.Drawing.Size(158, 22);
            this.tsmiSend.Text = "&Send";
            this.tsmiSend.ToolTipText = "Send item to image store";
            this.tsmiSend.Click += new System.EventHandler(this.Send_Click);
            // 
            // tsmiScanningShow
            // 
            this.tsmiScanningShow.Name = "tsmiScanningShow";
            this.tsmiScanningShow.Size = new System.Drawing.Size(158, 22);
            this.tsmiScanningShow.Text = "S&how";
            this.tsmiScanningShow.ToolTipText = "Download and show voucher";
            this.tsmiScanningShow.Click += new System.EventHandler(this.DownloadShow_Click);
            // 
            // showSignedToolStripMenuItem
            // 
            this.showSignedToolStripMenuItem.Name = "showSignedToolStripMenuItem";
            this.showSignedToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.showSignedToolStripMenuItem.Text = "Show Si&gned";
            this.showSignedToolStripMenuItem.Click += new System.EventHandler(this.DownloadShow_Click);
            // 
            // toolStripMenuItem8
            // 
            this.toolStripMenuItem8.Name = "toolStripMenuItem8";
            this.toolStripMenuItem8.Size = new System.Drawing.Size(155, 6);
            // 
            // scheduleStartToolStripMenuItem
            // 
            this.scheduleStartToolStripMenuItem.Name = "scheduleStartToolStripMenuItem";
            this.scheduleStartToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.scheduleStartToolStripMenuItem.Text = "Schedule Start At";
            this.scheduleStartToolStripMenuItem.Click += new System.EventHandler(this.StartScheduler_Click);
            // 
            // toolStripMenuItem11
            // 
            this.toolStripMenuItem11.Name = "toolStripMenuItem11";
            this.toolStripMenuItem11.Size = new System.Drawing.Size(155, 6);
            // 
            // nameToolStripMenuItem
            // 
            this.nameToolStripMenuItem.Name = "nameToolStripMenuItem";
            this.nameToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.nameToolStripMenuItem.Text = "&Rename";
            this.nameToolStripMenuItem.Click += new System.EventHandler(this.NameMenuItem_Click);
            // 
            // tsmiScanningDetailes
            // 
            this.tsmiScanningDetailes.Name = "tsmiScanningDetailes";
            this.tsmiScanningDetailes.Size = new System.Drawing.Size(158, 22);
            this.tsmiScanningDetailes.Text = "De&tails";
            this.tsmiScanningDetailes.ToolTipText = "Show voucher details";
            this.tsmiScanningDetailes.Click += new System.EventHandler(this.Details_Click);
            // 
            // toolStripMenuItem10
            // 
            this.toolStripMenuItem10.Name = "toolStripMenuItem10";
            this.toolStripMenuItem10.Size = new System.Drawing.Size(155, 6);
            // 
            // tsmiScaningDelete
            // 
            this.tsmiScaningDelete.Name = "tsmiScaningDelete";
            this.tsmiScaningDelete.Size = new System.Drawing.Size(158, 22);
            this.tsmiScaningDelete.Text = "&Delete";
            this.tsmiScaningDelete.ToolTipText = "Delete sigle item from selected folder of image store";
            this.tsmiScaningDelete.Click += new System.EventHandler(this.DeleteFile_Click);
            // 
            // tsmiScaningDeleteAll
            // 
            this.tsmiScaningDeleteAll.Name = "tsmiScaningDeleteAll";
            this.tsmiScaningDeleteAll.Size = new System.Drawing.Size(158, 22);
            this.tsmiScaningDeleteAll.Text = "Delete &All";
            this.tsmiScaningDeleteAll.ToolTipText = "Delete all items from selected folder of image store";
            this.tsmiScaningDeleteAll.Click += new System.EventHandler(this.DeleteAllFilesInFolder_Click);
            // 
            // tsmiScanningClear
            // 
            this.tsmiScanningClear.Name = "tsmiScanningClear";
            this.tsmiScanningClear.Size = new System.Drawing.Size(158, 22);
            this.tsmiScanningClear.Text = "&Clear";
            this.tsmiScanningClear.ToolTipText = "Clear desktop";
            this.tsmiScanningClear.Click += new System.EventHandler(this.ClearDesktop_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(155, 6);
            // 
            // tsmiScanningForce
            // 
            this.tsmiScanningForce.Name = "tsmiScanningForce";
            this.tsmiScanningForce.Size = new System.Drawing.Size(158, 22);
            this.tsmiScanningForce.Text = "&Force";
            this.tsmiScanningForce.ToolTipText = "Force insert voucher. Ignore barcode checks";
            this.tsmiScanningForce.Click += new System.EventHandler(this.Forse_Click);
            // 
            // tsmiScanningForceAll
            // 
            this.tsmiScanningForceAll.Name = "tsmiScanningForceAll";
            this.tsmiScanningForceAll.Size = new System.Drawing.Size(158, 22);
            this.tsmiScanningForceAll.Text = "F&orce All";
            this.tsmiScanningForceAll.ToolTipText = "Force inszert all vouchers. Ignore barcode checks";
            this.tsmiScanningForceAll.Click += new System.EventHandler(this.ForceAll_Click);
            // 
            // tsmiScanningIgnore
            // 
            this.tsmiScanningIgnore.Name = "tsmiScanningIgnore";
            this.tsmiScanningIgnore.Size = new System.Drawing.Size(158, 22);
            this.tsmiScanningIgnore.Text = "&Ignore";
            this.tsmiScanningIgnore.ToolTipText = "Ignore selected voucher.";
            this.tsmiScanningIgnore.Click += new System.EventHandler(this.Ignore_Click);
            // 
            // toolStripMenuItem9
            // 
            this.toolStripMenuItem9.Name = "toolStripMenuItem9";
            this.toolStripMenuItem9.Size = new System.Drawing.Size(155, 6);
            // 
            // tsmiScanningAddToTran
            // 
            this.tsmiScanningAddToTran.Name = "tsmiScanningAddToTran";
            this.tsmiScanningAddToTran.Size = new System.Drawing.Size(158, 22);
            this.tsmiScanningAddToTran.Text = "&Add to tran";
            this.tsmiScanningAddToTran.ToolTipText = "Add voucher to transfer file";
            this.tsmiScanningAddToTran.Click += new System.EventHandler(this.AddToTran_Click);
            // 
            // toolStripMenuItem7
            // 
            this.toolStripMenuItem7.Name = "toolStripMenuItem7";
            this.toolStripMenuItem7.Size = new System.Drawing.Size(155, 6);
            // 
            // tsmiExit
            // 
            this.tsmiExit.Name = "tsmiExit";
            this.tsmiExit.Size = new System.Drawing.Size(158, 22);
            this.tsmiExit.Text = "&Exit";
            this.tsmiExit.ToolTipText = "Exit application";
            this.tsmiExit.Click += new System.EventHandler(this.Exit_Click);
            // 
            // ssScaningStatus
            // 
            this.ssScaningStatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblScanned,
            this.lblSent,
            this.lblItemsWithErr,
            this.lblMessage,
            this.pbScanProgress});
            this.ssScaningStatus.Location = new System.Drawing.Point(3, 507);
            this.ssScaningStatus.Name = "ssScaningStatus";
            this.ssScaningStatus.Size = new System.Drawing.Size(1116, 22);
            this.ssScaningStatus.TabIndex = 3;
            this.ssScaningStatus.Text = "statusStrip1";
            // 
            // lblScanned
            // 
            this.lblScanned.AutoSize = false;
            this.lblScanned.Name = "lblScanned";
            this.lblScanned.Size = new System.Drawing.Size(130, 17);
            // 
            // lblSent
            // 
            this.lblSent.AutoSize = false;
            this.lblSent.Name = "lblSent";
            this.lblSent.Size = new System.Drawing.Size(137, 17);
            // 
            // lblItemsWithErr
            // 
            this.lblItemsWithErr.AutoSize = false;
            this.lblItemsWithErr.ForeColor = System.Drawing.Color.Red;
            this.lblItemsWithErr.Name = "lblItemsWithErr";
            this.lblItemsWithErr.Size = new System.Drawing.Size(109, 17);
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = false;
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(400, 17);
            // 
            // pbScanProgress
            // 
            this.pbScanProgress.Name = "pbScanProgress";
            this.pbScanProgress.Size = new System.Drawing.Size(200, 16);
            // 
            // ScanningSettingPanel
            // 
            this.ScanningSettingPanel.BackColor = System.Drawing.Color.LightGray;
            this.ScanningSettingPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ScanningSettingPanel.Controls.Add(this.btnCCCover);
            this.ScanningSettingPanel.Controls.Add(this.toggleButtonControl2);
            this.ScanningSettingPanel.Controls.Add(this.lblNext);
            this.ScanningSettingPanel.Controls.Add(this.lblPrev);
            this.ScanningSettingPanel.Controls.Add(this.btnFileBrowser);
            this.ScanningSettingPanel.Controls.Add(this.toggleButtonControl1);
            this.ScanningSettingPanel.Controls.Add(this.cbVoucherMustExist);
            this.ScanningSettingPanel.Controls.Add(this.cbCoversheet);
            this.ScanningSettingPanel.Controls.Add(this.btnShowMonitor);
            this.ScanningSettingPanel.Controls.Add(this.btnCover);
            this.ScanningSettingPanel.Controls.Add(this.llblOpenFolder);
            this.ScanningSettingPanel.Controls.Add(this.lblWarningMessage);
            this.ScanningSettingPanel.Controls.Add(this.btnShowHide);
            this.ScanningSettingPanel.Controls.Add(this.btnScan);
            this.ScanningSettingPanel.Controls.Add(this.textBox3);
            this.ScanningSettingPanel.Controls.Add(this.tbTransferFile);
            this.ScanningSettingPanel.Controls.Add(this.btnBrowseForExprFile);
            this.ScanningSettingPanel.Controls.Add(this.btnClear);
            this.ScanningSettingPanel.Controls.Add(this.btnRun);
            this.ScanningSettingPanel.Controls.Add(this.textBox1);
            this.ScanningSettingPanel.Controls.Add(this.tbScanDirectory);
            this.ScanningSettingPanel.Controls.Add(this.btnBrowseForScanDir);
            this.ScanningSettingPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.ScanningSettingPanel.Location = new System.Drawing.Point(3, 16);
            this.ScanningSettingPanel.Margin = new System.Windows.Forms.Padding(0);
            this.ScanningSettingPanel.Name = "ScanningSettingPanel";
            this.ScanningSettingPanel.Size = new System.Drawing.Size(1116, 105);
            this.ScanningSettingPanel.TabIndex = 0;
            // 
            // toggleButtonControl2
            // 
            this.toggleButtonControl2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.toggleButtonControl2.ColorOff = System.Drawing.Color.LightGray;
            this.toggleButtonControl2.ColorOn = System.Drawing.Color.Lime;
            this.toggleButtonControl2.Location = new System.Drawing.Point(827, 3);
            this.toggleButtonControl2.Margin = new System.Windows.Forms.Padding(1);
            this.toggleButtonControl2.Name = "toggleButtonControl2";
            this.toggleButtonControl2.Padding = new System.Windows.Forms.Padding(1);
            this.toggleButtonControl2.Size = new System.Drawing.Size(149, 47);
            this.toggleButtonControl2.TabIndex = 38;
            this.toggleButtonControl2.Text1 = new string[] {
        "P1",
        "P1"};
            this.toggleButtonControl2.Text2 = new string[] {
        "P1/5",
        "P1/5"};
            this.toggleButtonControl2.Text3 = new string[] {
        "P2",
        "P2"};
            // 
            // lblNext
            // 
            this.lblNext.AutoSize = true;
            this.lblNext.Location = new System.Drawing.Point(991, 64);
            this.lblNext.Name = "lblNext";
            this.lblNext.Size = new System.Drawing.Size(51, 13);
            this.lblNext.TabIndex = 37;
            this.lblNext.TabStop = true;
            this.lblNext.Text = "NEXT >>";
            this.lblNext.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.PrevNextClicked);
            // 
            // lblPrev
            // 
            this.lblPrev.AutoSize = true;
            this.lblPrev.Location = new System.Drawing.Point(937, 64);
            this.lblPrev.Name = "lblPrev";
            this.lblPrev.Size = new System.Drawing.Size(48, 13);
            this.lblPrev.TabIndex = 36;
            this.lblPrev.TabStop = true;
            this.lblPrev.Text = "<<PREV";
            this.lblPrev.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.PrevNextClicked);
            // 
            // btnFileBrowser
            // 
            this.btnFileBrowser.Location = new System.Drawing.Point(679, 68);
            this.btnFileBrowser.Name = "btnFileBrowser";
            this.btnFileBrowser.Size = new System.Drawing.Size(75, 23);
            this.btnFileBrowser.TabIndex = 35;
            this.btnFileBrowser.Text = "File Browser";
            this.btnFileBrowser.UseVisualStyleBackColor = true;
            this.btnFileBrowser.Click += new System.EventHandler(this.FileBrowser_Click);
            // 
            // toggleButtonControl1
            // 
            this.toggleButtonControl1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.toggleButtonControl1.ColorOff = System.Drawing.Color.LightGray;
            this.toggleButtonControl1.ColorOn = System.Drawing.Color.Lime;
            this.toggleButtonControl1.Location = new System.Drawing.Point(613, 3);
            this.toggleButtonControl1.Margin = new System.Windows.Forms.Padding(1);
            this.toggleButtonControl1.Name = "toggleButtonControl1";
            this.toggleButtonControl1.Padding = new System.Windows.Forms.Padding(1);
            this.toggleButtonControl1.Size = new System.Drawing.Size(202, 47);
            this.toggleButtonControl1.TabIndex = 34;
            this.toggleButtonControl1.Text1 = new string[] {
        "Barcode",
        "Barcode"};
            this.toggleButtonControl1.Text2 = new string[] {
        "Tras file",
        "Tras file"};
            this.toggleButtonControl1.Text3 = new string[] {
        "Sitecode",
        "Sitecode"};
            // 
            // cbVoucherMustExist
            // 
            this.cbVoucherMustExist.Appearance = System.Windows.Forms.Appearance.Button;
            this.cbVoucherMustExist.BackColor = System.Drawing.Color.LightGray;
            this.cbVoucherMustExist.Location = new System.Drawing.Point(516, 68);
            this.cbVoucherMustExist.Name = "cbVoucherMustExist";
            this.cbVoucherMustExist.Size = new System.Drawing.Size(75, 23);
            this.cbVoucherMustExist.TabIndex = 33;
            this.cbVoucherMustExist.Text = "Must exist";
            this.cbVoucherMustExist.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ToolTip1.SetToolTip(this.cbVoucherMustExist, "Must exist");
            this.cbVoucherMustExist.UseVisualStyleBackColor = false;
            this.cbVoucherMustExist.CheckedChanged += new System.EventHandler(this.VoucherMustExist_CheckedChanged);
            // 
            // cbCoversheet
            // 
            this.cbCoversheet.Appearance = System.Windows.Forms.Appearance.Button;
            this.cbCoversheet.AutoSize = true;
            this.cbCoversheet.Location = new System.Drawing.Point(597, 68);
            this.cbCoversheet.Name = "cbCoversheet";
            this.cbCoversheet.Size = new System.Drawing.Size(76, 23);
            this.cbCoversheet.TabIndex = 29;
            this.cbCoversheet.Text = "Cover &Sheet";
            this.ToolTip1.SetToolTip(this.cbCoversheet, "Click before scanning cover sheet");
            this.cbCoversheet.UseVisualStyleBackColor = true;
            this.cbCoversheet.CheckedChanged += new System.EventHandler(this.Coversheet_CheckedChanged);
            // 
            // btnShowMonitor
            // 
            this.btnShowMonitor.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnShowMonitor.Image = ((System.Drawing.Image)(resources.GetObject("btnShowMonitor.Image")));
            this.btnShowMonitor.Location = new System.Drawing.Point(422, 56);
            this.btnShowMonitor.Name = "btnShowMonitor";
            this.btnShowMonitor.Size = new System.Drawing.Size(75, 42);
            this.btnShowMonitor.TabIndex = 28;
            this.ToolTip1.SetToolTip(this.btnShowMonitor, "Click to open voucher monitor");
            this.btnShowMonitor.UseVisualStyleBackColor = true;
            this.btnShowMonitor.Click += new System.EventHandler(this.ShowMonitor_Click);
            // 
            // btnCover
            // 
            this.btnCover.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCover.Location = new System.Drawing.Point(994, 15);
            this.btnCover.Name = "btnCover";
            this.btnCover.Size = new System.Drawing.Size(108, 23);
            this.btnCover.TabIndex = 27;
            this.btnCover.Text = "Credit Card Cover";
            this.ToolTip1.SetToolTip(this.btnCover, "Gonerate cretid card cover");
            this.btnCover.UseVisualStyleBackColor = true;
            this.btnCover.Visible = false;
            this.btnCover.Click += new System.EventHandler(this.CoverSetup_Click);
            // 
            // llblOpenFolder
            // 
            this.llblOpenFolder.AutoSize = true;
            this.llblOpenFolder.Location = new System.Drawing.Point(396, 32);
            this.llblOpenFolder.Name = "llblOpenFolder";
            this.llblOpenFolder.Size = new System.Drawing.Size(16, 13);
            this.llblOpenFolder.TabIndex = 26;
            this.llblOpenFolder.TabStop = true;
            this.llblOpenFolder.Text = "...";
            this.llblOpenFolder.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OpenFolder_LinkClicked);
            // 
            // lblWarningMessage
            // 
            this.lblWarningMessage.AutoSize = true;
            this.lblWarningMessage.ForeColor = System.Drawing.Color.Red;
            this.lblWarningMessage.IntervalLow = System.TimeSpan.Parse("00:00:00.1000000");
            this.lblWarningMessage.IntervalUp = System.TimeSpan.Parse("00:00:00.3000000");
            this.lblWarningMessage.Location = new System.Drawing.Point(869, 85);
            this.lblWarningMessage.Name = "lblWarningMessage";
            this.lblWarningMessage.Size = new System.Drawing.Size(123, 13);
            this.lblWarningMessage.TabIndex = 25;
            this.lblWarningMessage.Text = "Last voucher processing";
            this.lblWarningMessage.Visible = false;
            // 
            // btnShowHide
            // 
            this.btnShowHide.ArrowEnabled = true;
            this.btnShowHide.HoverEndColor = System.Drawing.Color.DimGray;
            this.btnShowHide.HoverStartColor = System.Drawing.Color.WhiteSmoke;
            this.btnShowHide.Location = new System.Drawing.Point(13, 3);
            this.btnShowHide.Name = "btnShowHide";
            this.btnShowHide.NormalEndColor = System.Drawing.Color.DarkGray;
            this.btnShowHide.NormalStartColor = System.Drawing.Color.WhiteSmoke;
            this.btnShowHide.Rotation = 0;
            this.btnShowHide.Size = new System.Drawing.Size(24, 24);
            this.btnShowHide.TabIndex = 24;
            this.btnShowHide.Click += new System.EventHandler(this.ShowHide_Click);
            // 
            // btnScan
            // 
            this.btnScan.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnScan.BackColor = System.Drawing.Color.Silver;
            this.btnScan.BevelDepth = 5;
            this.btnScan.BevelHeight = 5;
            this.btnScan.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnScan.Dome = true;
            this.btnScan.Location = new System.Drawing.Point(980, 22);
            this.btnScan.Name = "btnScan";
            this.btnScan.RecessDepth = 0;
            this.btnScan.Size = new System.Drawing.Size(122, 69);
            this.btnScan.TabIndex = 20;
            this.btnScan.Text = "Scan";
            this.ToolTip1.SetToolTip(this.btnScan, "Scan button");
            this.btnScan.UseVisualStyleBackColor = false;
            this.btnScan.Click += new System.EventHandler(this.Scan_Click);
            // 
            // textBox3
            // 
            this.textBox3.BackColor = System.Drawing.Color.LightGray;
            this.textBox3.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox3.Location = new System.Drawing.Point(10, 57);
            this.textBox3.Multiline = true;
            this.textBox3.Name = "textBox3";
            this.textBox3.ReadOnly = true;
            this.textBox3.Size = new System.Drawing.Size(87, 20);
            this.textBox3.TabIndex = 15;
            this.textBox3.Text = "Transfer File:";
            this.textBox3.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // tbTransferFile
            // 
            this.tbTransferFile.Location = new System.Drawing.Point(101, 56);
            this.tbTransferFile.Name = "tbTransferFile";
            this.tbTransferFile.ReadOnly = true;
            this.tbTransferFile.Size = new System.Drawing.Size(254, 20);
            this.tbTransferFile.TabIndex = 14;
            this.ToolTip1.SetToolTip(this.tbTransferFile, "Voucher transfer file");
            // 
            // btnBrowseForExprFile
            // 
            this.btnBrowseForExprFile.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBrowseForExprFile.Location = new System.Drawing.Point(361, 56);
            this.btnBrowseForExprFile.Name = "btnBrowseForExprFile";
            this.btnBrowseForExprFile.Size = new System.Drawing.Size(29, 23);
            this.btnBrowseForExprFile.TabIndex = 13;
            this.btnBrowseForExprFile.Text = "...";
            this.ToolTip1.SetToolTip(this.btnBrowseForExprFile, "Browse button");
            this.btnBrowseForExprFile.UseVisualStyleBackColor = true;
            // 
            // btnClear
            // 
            this.btnClear.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClear.Image = ((System.Drawing.Image)(resources.GetObject("btnClear.Image")));
            this.btnClear.Location = new System.Drawing.Point(422, 3);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 47);
            this.btnClear.TabIndex = 8;
            this.ToolTip1.SetToolTip(this.btnClear, "Clear results");
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.Clear_Click);
            // 
            // btnRun
            // 
            this.btnRun.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRun.Image = ((System.Drawing.Image)(resources.GetObject("btnRun.Image")));
            this.btnRun.Location = new System.Drawing.Point(516, 3);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(75, 47);
            this.btnRun.TabIndex = 7;
            this.ToolTip1.SetToolTip(this.btnRun, "Process Run button");
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.RunScan_Click);
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.Color.LightGray;
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(17, 32);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(82, 16);
            this.textBox1.TabIndex = 3;
            this.textBox1.Text = "Scan Folder:";
            this.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // tbScanDirectory
            // 
            this.tbScanDirectory.Location = new System.Drawing.Point(101, 29);
            this.tbScanDirectory.Name = "tbScanDirectory";
            this.tbScanDirectory.Size = new System.Drawing.Size(254, 20);
            this.tbScanDirectory.TabIndex = 1;
            this.tbScanDirectory.Text = "M:\\Scans";
            this.ToolTip1.SetToolTip(this.tbScanDirectory, "Scan directory path");
            this.tbScanDirectory.TextChanged += new System.EventHandler(this.ScanDirectory_TextChanged);
            // 
            // btnBrowseForScanDir
            // 
            this.btnBrowseForScanDir.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBrowseForScanDir.Location = new System.Drawing.Point(361, 27);
            this.btnBrowseForScanDir.Name = "btnBrowseForScanDir";
            this.btnBrowseForScanDir.Size = new System.Drawing.Size(29, 23);
            this.btnBrowseForScanDir.TabIndex = 0;
            this.btnBrowseForScanDir.Text = "...";
            this.ToolTip1.SetToolTip(this.btnBrowseForScanDir, "Browse button");
            this.btnBrowseForScanDir.UseVisualStyleBackColor = true;
            // 
            // tbSearch
            // 
            this.tbSearch.Controls.Add(this.groupBox6);
            this.tbSearch.Controls.Add(this.statusStrip1);
            this.tbSearch.Location = new System.Drawing.Point(4, 22);
            this.tbSearch.Name = "tbSearch";
            this.tbSearch.Padding = new System.Windows.Forms.Padding(3);
            this.tbSearch.Size = new System.Drawing.Size(1128, 538);
            this.tbSearch.TabIndex = 2;
            this.tbSearch.Text = "          Search         ";
            this.tbSearch.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.splitContainer2);
            this.groupBox6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox6.Location = new System.Drawing.Point(3, 3);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(1122, 510);
            this.groupBox6.TabIndex = 0;
            this.groupBox6.TabStop = false;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(3, 16);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.tabControl2);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.dgvSearchData);
            this.splitContainer2.Size = new System.Drawing.Size(1116, 491);
            this.splitContainer2.SplitterDistance = 95;
            this.splitContainer2.TabIndex = 2;
            // 
            // tabControl2
            // 
            this.tabControl2.Controls.Add(this.tabVoucher);
            this.tabControl2.Controls.Add(this.tabHistory);
            this.tabControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl2.Location = new System.Drawing.Point(0, 0);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(1116, 95);
            this.tabControl2.TabIndex = 0;
            // 
            // tabVoucher
            // 
            this.tabVoucher.Controls.Add(this.panel1);
            this.tabVoucher.Location = new System.Drawing.Point(4, 22);
            this.tabVoucher.Name = "tabVoucher";
            this.tabVoucher.Padding = new System.Windows.Forms.Padding(3);
            this.tabVoucher.Size = new System.Drawing.Size(1108, 69);
            this.tabVoucher.TabIndex = 1;
            this.tabVoucher.Text = "Voucher";
            this.tabVoucher.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.bliningLabel1);
            this.panel1.Controls.Add(this.btSearch);
            this.panel1.Controls.Add(this.dtTo);
            this.panel1.Controls.Add(this.dtFrom);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.tbVoucherId);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.cbCountryId);
            this.panel1.Controls.Add(this.tbBrId);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1102, 63);
            this.panel1.TabIndex = 0;
            // 
            // bliningLabel1
            // 
            this.bliningLabel1.AutoSize = true;
            this.bliningLabel1.ForeColor = System.Drawing.Color.Red;
            this.bliningLabel1.IntervalLow = System.TimeSpan.Parse("00:00:00.1000000");
            this.bliningLabel1.IntervalUp = System.TimeSpan.Parse("00:00:00.3000000");
            this.bliningLabel1.Location = new System.Drawing.Point(58, 42);
            this.bliningLabel1.Name = "bliningLabel1";
            this.bliningLabel1.Size = new System.Drawing.Size(220, 13);
            this.bliningLabel1.TabIndex = 26;
            this.bliningLabel1.Text = "Note: number of results are limited up to 1000";
            this.bliningLabel1.Visible = false;
            // 
            // btSearch
            // 
            this.btSearch.Location = new System.Drawing.Point(901, 10);
            this.btSearch.Name = "btSearch";
            this.btSearch.Size = new System.Drawing.Size(75, 23);
            this.btSearch.TabIndex = 10;
            this.btSearch.Text = "Search";
            this.ToolTip1.SetToolTip(this.btSearch, "Search button");
            this.btSearch.UseVisualStyleBackColor = true;
            this.btSearch.Click += new System.EventHandler(this.Search_Click);
            // 
            // dtTo
            // 
            this.dtTo.Location = new System.Drawing.Point(668, 7);
            this.dtTo.Message = "To";
            this.dtTo.Name = "dtTo";
            this.dtTo.Size = new System.Drawing.Size(215, 30);
            this.dtTo.TabIndex = 7;
            this.ToolTip1.SetToolTip(this.dtTo, "To date");
            this.dtTo.Value = null;
            // 
            // dtFrom
            // 
            this.dtFrom.Location = new System.Drawing.Point(461, 7);
            this.dtFrom.Message = "From";
            this.dtFrom.Name = "dtFrom";
            this.dtFrom.Size = new System.Drawing.Size(209, 32);
            this.dtFrom.TabIndex = 6;
            this.ToolTip1.SetToolTip(this.dtFrom, "From date");
            this.dtFrom.Value = null;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(315, 15);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(47, 13);
            this.label8.TabIndex = 5;
            this.label8.Text = "Voucher";
            // 
            // tbVoucherId
            // 
            this.tbVoucherId.Location = new System.Drawing.Point(368, 12);
            this.tbVoucherId.Name = "tbVoucherId";
            this.tbVoucherId.Size = new System.Drawing.Size(91, 20);
            this.tbVoucherId.TabIndex = 4;
            this.ToolTip1.SetToolTip(this.tbVoucherId, "Voucher code");
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(160, 15);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(43, 13);
            this.label7.TabIndex = 3;
            this.label7.Text = "Retailer";
            // 
            // cbCountryId
            // 
            this.cbCountryId.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCountryId.FormattingEnabled = true;
            this.cbCountryId.Location = new System.Drawing.Point(60, 12);
            this.cbCountryId.Name = "cbCountryId";
            this.cbCountryId.Size = new System.Drawing.Size(94, 21);
            this.cbCountryId.TabIndex = 2;
            this.ToolTip1.SetToolTip(this.cbCountryId, "CountryID");
            // 
            // tbBrId
            // 
            this.tbBrId.Location = new System.Drawing.Point(209, 12);
            this.tbBrId.Name = "tbBrId";
            this.tbBrId.Size = new System.Drawing.Size(91, 20);
            this.tbBrId.TabIndex = 1;
            this.ToolTip1.SetToolTip(this.tbBrId, "Retailer code");
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Country";
            // 
            // tabHistory
            // 
            this.tabHistory.Controls.Add(this.pnlDataGrid);
            this.tabHistory.Location = new System.Drawing.Point(4, 22);
            this.tabHistory.Name = "tabHistory";
            this.tabHistory.Padding = new System.Windows.Forms.Padding(3);
            this.tabHistory.Size = new System.Drawing.Size(1108, 69);
            this.tabHistory.TabIndex = 0;
            this.tabHistory.Text = "History";
            this.tabHistory.UseVisualStyleBackColor = true;
            // 
            // pnlDataGrid
            // 
            this.pnlDataGrid.Controls.Add(this.bliningLabel2);
            this.pnlDataGrid.Controls.Add(this.cbHistoryType);
            this.pnlDataGrid.Controls.Add(this.label6);
            this.pnlDataGrid.Controls.Add(this.label5);
            this.pnlDataGrid.Controls.Add(this.historyToTime);
            this.pnlDataGrid.Controls.Add(this.historyFromTime);
            this.pnlDataGrid.Controls.Add(this.btnShowHistory);
            this.pnlDataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlDataGrid.Location = new System.Drawing.Point(3, 3);
            this.pnlDataGrid.Name = "pnlDataGrid";
            this.pnlDataGrid.Size = new System.Drawing.Size(1102, 63);
            this.pnlDataGrid.TabIndex = 0;
            // 
            // bliningLabel2
            // 
            this.bliningLabel2.AutoSize = true;
            this.bliningLabel2.ForeColor = System.Drawing.Color.Red;
            this.bliningLabel2.IntervalLow = System.TimeSpan.Parse("00:00:00.1000000");
            this.bliningLabel2.IntervalUp = System.TimeSpan.Parse("00:00:00.3000000");
            this.bliningLabel2.Location = new System.Drawing.Point(47, 43);
            this.bliningLabel2.Name = "bliningLabel2";
            this.bliningLabel2.Size = new System.Drawing.Size(220, 13);
            this.bliningLabel2.TabIndex = 27;
            this.bliningLabel2.Text = "Note: number of results are limited up to 1000";
            this.bliningLabel2.Visible = false;
            // 
            // cbHistoryType
            // 
            this.cbHistoryType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbHistoryType.FormattingEnabled = true;
            this.cbHistoryType.Location = new System.Drawing.Point(348, 14);
            this.cbHistoryType.Name = "cbHistoryType";
            this.cbHistoryType.Size = new System.Drawing.Size(139, 21);
            this.cbHistoryType.TabIndex = 5;
            this.ToolTip1.SetToolTip(this.cbHistoryType, "Action");
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(180, 17);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(20, 13);
            this.label6.TabIndex = 4;
            this.label6.Text = "To";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 17);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(30, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "From";
            // 
            // historyToTime
            // 
            this.historyToTime.Location = new System.Drawing.Point(206, 15);
            this.historyToTime.Name = "historyToTime";
            this.historyToTime.Size = new System.Drawing.Size(119, 20);
            this.historyToTime.TabIndex = 2;
            this.ToolTip1.SetToolTip(this.historyToTime, "To date");
            // 
            // historyFromTime
            // 
            this.historyFromTime.Location = new System.Drawing.Point(45, 15);
            this.historyFromTime.Name = "historyFromTime";
            this.historyFromTime.Size = new System.Drawing.Size(119, 20);
            this.historyFromTime.TabIndex = 1;
            this.ToolTip1.SetToolTip(this.historyFromTime, "From date");
            // 
            // btnShowHistory
            // 
            this.btnShowHistory.Location = new System.Drawing.Point(504, 12);
            this.btnShowHistory.Name = "btnShowHistory";
            this.btnShowHistory.Size = new System.Drawing.Size(75, 23);
            this.btnShowHistory.TabIndex = 0;
            this.btnShowHistory.Text = "Show";
            this.ToolTip1.SetToolTip(this.btnShowHistory, "Show button");
            this.btnShowHistory.UseVisualStyleBackColor = true;
            this.btnShowHistory.Click += new System.EventHandler(this.ShowHistory_Click);
            // 
            // dgvSearchData
            // 
            this.dgvSearchData.AllowUserToAddRows = false;
            this.dgvSearchData.AllowUserToDeleteRows = false;
            this.dgvSearchData.AllowUserToOrderColumns = true;
            this.dgvSearchData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSearchData.ContextMenuStrip = this.historyMenuStrip;
            this.dgvSearchData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvSearchData.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvSearchData.Location = new System.Drawing.Point(0, 0);
            this.dgvSearchData.Name = "dgvSearchData";
            this.dgvSearchData.Size = new System.Drawing.Size(1116, 392);
            this.dgvSearchData.TabIndex = 1;
            this.dgvSearchData.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.SearchData_RowsAdded);
            this.dgvSearchData.DoubleClick += new System.EventHandler(this.SearchData_DoubleClick);
            // 
            // historyMenuStrip
            // 
            this.historyMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiSort,
            this.tsmiShow,
            this.exitToolStripMenuItem1});
            this.historyMenuStrip.Name = "historyMenuStrip";
            this.historyMenuStrip.Size = new System.Drawing.Size(101, 70);
            // 
            // tsmiSort
            // 
            this.tsmiSort.Name = "tsmiSort";
            this.tsmiSort.Size = new System.Drawing.Size(100, 22);
            this.tsmiSort.Text = "&Sort";
            this.tsmiSort.ToolTipText = "Sort grid by selected column";
            this.tsmiSort.Click += new System.EventHandler(this.SearchSort_Click);
            // 
            // tsmiShow
            // 
            this.tsmiShow.Name = "tsmiShow";
            this.tsmiShow.Size = new System.Drawing.Size(100, 22);
            this.tsmiShow.Text = "S&how";
            this.tsmiShow.ToolTipText = "Download and show item from image store";
            this.tsmiShow.Click += new System.EventHandler(this.SearchShow_Click);
            // 
            // exitToolStripMenuItem1
            // 
            this.exitToolStripMenuItem1.Name = "exitToolStripMenuItem1";
            this.exitToolStripMenuItem1.Size = new System.Drawing.Size(100, 22);
            this.exitToolStripMenuItem1.Text = "&Exit";
            this.exitToolStripMenuItem1.ToolTipText = "Exit application";
            this.exitToolStripMenuItem1.Click += new System.EventHandler(this.Exit_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.tslItemsCount});
            this.statusStrip1.Location = new System.Drawing.Point(3, 513);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1122, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(36, 17);
            this.toolStripStatusLabel1.Text = "items.";
            // 
            // tslItemsCount
            // 
            this.tslItemsCount.Name = "tslItemsCount";
            this.tslItemsCount.Size = new System.Drawing.Size(11, 17);
            this.tslItemsCount.Text = ".";
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "Document-icon.png");
            this.imageList1.Images.SetKeyName(1, "File-Delete-icon.png");
            // 
            // btnCCCover
            // 
            this.btnCCCover.Location = new System.Drawing.Point(760, 68);
            this.btnCCCover.Name = "btnCCCover";
            this.btnCCCover.Size = new System.Drawing.Size(72, 23);
            this.btnCCCover.TabIndex = 39;
            this.btnCCCover.Text = "Cover CC";
            this.btnCCCover.UseVisualStyleBackColor = true;
            this.btnCCCover.Click += new System.EventHandler(this.btnCCCover_Click);
            // 
            // MainForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(1156, 584);
            this.ContextMenuStrip = this.printContextMenu;
            this.Controls.Add(this.tabControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(1022, 351);
            this.Name = "MainForm";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.Text = "Voucher Printing / Scanning";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Scanning_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAllocations)).EndInit();
            this.printContextMenu.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tpPrint.ResumeLayout(false);
            this.tpScan.ResumeLayout(false);
            this.gbScanHeader.ResumeLayout(false);
            this.gbScanHeader.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.folderMenuStrip.ResumeLayout(false);
            this.scanContextMenuStrip.ResumeLayout(false);
            this.ssScaningStatus.ResumeLayout(false);
            this.ssScaningStatus.PerformLayout();
            this.ScanningSettingPanel.ResumeLayout(false);
            this.ScanningSettingPanel.PerformLayout();
            this.tbSearch.ResumeLayout(false);
            this.tbSearch.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.tabControl2.ResumeLayout(false);
            this.tabVoucher.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tabHistory.ResumeLayout(false);
            this.pnlDataGrid.ResumeLayout(false);
            this.pnlDataGrid.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSearchData)).EndInit();
            this.historyMenuStrip.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        #region UI Controls
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbRetailer;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnBlock;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbHeadoffice;
        private System.Windows.Forms.Button btnShowRetailer;
        private Button btnShowUnprintedAllocatedVouchers;
        private RadioButton rdoSingleSale;
        private RadioButton rdoDoubleSale;
        private Button btnNotDispatched;
        private CheckBox cboSelectAllNone;
        private DataGridView dgvAllocations;

        private Label lblPrinted;
        private Label lblPending;

        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private Button btnRePrint;
        private ContextMenuStrip printContextMenu;
        private ToolStripMenuItem printdemoToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem1;
        private ToolStripMenuItem exitToolStripMenuItem;
        private Label label4;
        private System.ComponentModel.IContainer components;

        #endregion

        #region PRINTING

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.IsAdmin)
            {
                using (CreateFormatForm form = new CreateFormatForm())
                    form.ShowDialog(this);
            }
        }

        private void AssignFormatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.IsAdmin)
            {
                using (FormAssignFormat form = new FormAssignFormat())
                    form.ShowDialog(this);
            }
        }

        private void MapPrinterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.IsAdmin)
            {
                using (MapPrinterForm form = new MapPrinterForm())
                    form.ShowDialog(this);
            }
        }

        private void SimulatePrint_Click(object sender, EventArgs e)
        {
            MainForm.ms_SimulatePrint = !MainForm.ms_SimulatePrint;
        }

        private void PrintContextMenu_Opening(object sender, CancelEventArgs e)
        {
            simulatePrintToolStripMenuItem.Checked = MainForm.ms_SimulatePrint;
        }

        #endregion

        #region SCANNING

        #region CALLBACKS

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data">StateManager.Item</param>
        public void ShowItemScannedCallback(object data)
        {
            if (this.IsDisposed)
                return;

            var item = (StateManager.Item)data;
            if (item != null)
                lblMessage.Text = string.Format(Messages.VoucherStatus_2, item.SessionID, item.State);

            lblScanned.Text = string.Format(Messages.ItemsProcessed_1, m_ScanFileOrganizer.ProcessedItems + 1);
            pbScanProgress.ProgressBar.Value = Math.Min((int)(((double)m_SendFileOrganizer.ProcessedItems) /
                m_ScanFileOrganizer.ProcessedItems.Replace(0, 1) * 100), 100);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data">StateManager.Item</param>
        private void ShowItemCommitedCallback(object data)
        {
            if (this.IsDisposed)
                return;

            var item = (StateManager.Item)data;
            if (item != null)
                lblMessage.Text = string.Format(Messages.VoucherStatus_2, item.SessionID, item.State);

            lblSent.Text = string.Format(Messages.ItemsSent_1, m_SendFileOrganizer.ProcessedItems);
            pbScanProgress.ProgressBar.Value = Math.Min((int)(((double)m_SendFileOrganizer.ProcessedItems) /
                m_ScanFileOrganizer.ProcessedItems.Replace(0, 1) * 100), 100);
        }

        public void ShowItemsWithErrCallback(object data)
        {
            if (this.IsDisposed || data == null)
                return;

            var item = (int)data;
            lblItemsWithErr.Text = (item <= 0) ? null : string.Concat("Items with err ", item);
        }

        #endregion

        #region PRIVATE METHODS

        private readonly ManualResetEvent m_BuildFilesExit = new ManualResetEvent(false);

        private void RefreshVoucherPanel()
        {
            var tvNode = this.tvFolders.SelectedNode;
            if (tvNode == null)
                return;

            m_BuildFilesExit.Set();

            if (tvNode.Nodes.Count == 0)
                BuildNode(tvNode);

            var ninfo = (FolderInfo)tvNode.Tag;
            if (ninfo != null)
            {
                Global.FolderID = (int?)ninfo.Id;
                BuildFiles(ninfo.Id);
            }
            else
                m_StateManager.Clear();
        }

        private void ImageNode_Click(object sender, TreeNodeMouseClickEventArgs e)
        {
            m_CurrentVoucher = 0;

            m_BuildFilesExit.Set();

            if (e.Node.Nodes.Count == 0)
                BuildNode(e.Node);

            var ninfo = (FolderInfo)e.Node.Tag;
            if (ninfo != null)
            {
                Global.FolderID = (int?)ninfo.Id;
                BuildFiles(ninfo.Id);
            }
            else
                m_StateManager.Clear();
        }

        private void BuildNode(TreeNode node)
        {
            var ninfo = (FolderInfo)node.Tag;

            var list = ServiceDataAccess.Instance.ReadFolderList((ninfo != null ? ninfo.Id : (int?)null), Program.currentUser.CountryID);

            foreach (var info in list)
            {
                var newNode = node.Nodes.Add(info.Name);
                newNode.Tag = info;
            }
        }

        private volatile int m_CurrentVoucher;

        private void BuildFiles(int folderId)
        {
            Task.Factory.StartNew((o) =>
            {
                m_BuildFilesExit.Reset();
                m_StateManager.Clear();

                var fId = Convert.ToInt32(o);
                var files = ServiceDataAccess.Instance.ReadCoverList(fId);
                var vouchers = ServiceDataAccess.Instance.ReadFileList(fId, m_CurrentVoucher);

                foreach (var file in files)
                {
                    if (m_BuildFilesExit.WaitOne(10))
                        return;

                    Guid session = Guid.Empty;
                    Guid.TryParse(file.SessionId, out session);
                    m_StateManager.AddItem(file.Id, file.CountryID, StateManager.eState.COVER, session, file.Name);
                }

                foreach (var voucher in vouchers)
                {
                    if (m_BuildFilesExit.WaitOne(10))
                        return;

                    Guid session = Guid.Empty;
                    Guid.TryParse(voucher.SessionId, out session);
                    m_StateManager.AddVoucherItem(voucher.Id, voucher.CountryId, voucher.RetailerId, voucher.VoucherId,
                        StateManager.eState.VOUCHER, session, voucher.SiteCode, voucher.Name);
                }
            }, folderId);
        }

        private void EnableDisableScanPanel(bool enable)
        {
            toggleButtonControl1.Enabled =
            toggleButtonControl2.Enabled =
            tbScanDirectory.Enabled =
            tbTransferFile.Enabled =
            btnClear.Enabled =
            btnBrowseForExprFile.Enabled =
            btnBrowseForScanDir.Enabled = enable;
        }

        #endregion

        #region SEARCH

        private void SearchShow_Click(object sender, EventArgs e)
        {
            if (dgvSearchData.SelectedCells.Count == 0)
                return;
            if (dgvSearchData.SelectedCells[0].RowIndex == -1)
                return;

            switch (tabControl2.SelectedIndex)
            {
                case 0:
                    {
                        int id = dgvSearchData.Rows[dgvSearchData.SelectedCells[0].RowIndex].Cells[0].Value.cast<int>();
                        var item = m_StateManager.ProcessItem_Begin(true);
                        item.Id = id;
                        m_DownloadFileOrganizer.RunTask(
                        new TaskProcessOrganizer<StateManager.Item>.TaskItem(item, new Action<TaskProcessOrganizer<StateManager.Item>.TaskItem>((i) =>
                        {
                            var downloadDir = MainForm.GetAppSubFolder(Strings.DOWNLOAD);

                            string fullFileName = ServiceDataAccess.Instance.ReceiveFile(i.Item.Id, true, false, downloadDir.FullName);
                            m_DownloadFileOrganizer.Data[i.Item] = fullFileName;
                        })));
                    }
                    break;
                case 1:
                default:
                    break;
            }
        }

        private void UpdateVersion_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
            //var version = Assembly.GetEntryAssembly().GetName().Version;
            //Global.Instance.VersionUpdate(version.ToString(),
            //        () => this.InvokeSf(() => { this.ShowInfo("Download completed"); }),
            //        () => this.InvokeSf(() => { this.ShowInfo("Download failed"); }));
        }

        #endregion

        #region EVENT HANDLERS

        private void BrowseForFolder_Click(object sender, EventArgs e)
        {
            using (var dlg = new FolderBrowserDialog())
            {
                TextBox tb = (TextBox)((Button)sender).Tag;
                Debug.Assert(tb != null);

                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    tb.Text = dlg.SelectedPath;
                    StateSaver.Default.Set(Strings.tbScanDirectory, dlg.SelectedPath);
                }
            }
        }

        private void RunScan_Click(object sender, EventArgs e)
        {
            if (btnRun.IsOn())
            {
                if ((!m_ScanFileOrganizer.HasItems() ||
                        this.ShowQuestion(Messages.UnprocessedItemsCloseAnyway, MessageBoxButtons.YesNo, DialogResult.Yes) &&
                        (!m_SendFileOrganizer.HasItems() ||
                        this.ShowQuestion(Messages.UnsentItemsCloseAnyway, MessageBoxButtons.YesNo, DialogResult.Yes))))
                {
                    tvFolders.Enabled = true;
                    EnableDisableScanPanel(true);
                    btnRun.Set(false);
                    m_FileSysWatchers.ForEach(f => f.EnableRaisingEvents = false);
                }
            }
            else
            {
                if (m_StateManager.InDocumentMode && !m_StateManager.HasUnProcItems)
                {
                    this.ShowExclamation(Messages.NoDocumenLoadedLoadIt);
                }
                else if (!Global.FolderID.HasValue)
                {
                    this.ShowExclamation(Messages.DontScanToRootFolder);
                }
                else
                {
                    tvFolders.Enabled = false;
                    EnableDisableScanPanel(false);

                    Global.IgnoreList.Clear();

                    if (StateSaver.Default.Get<bool>(Strings.ClearScanDirectory, false))
                        ClearScanDirectory();

                    btnRun.Set(true);
                    m_FileSysWatchers.ForEach(f => f.Path = tbScanDirectory.Text);

                    for (int i = 0; i < SUPPORTED_FILE_EXTENTIONS.Length; i++)
                        m_FileSysWatchers[i].Filter = SUPPORTED_FILE_EXTENTIONS[i];

                    m_FileSysWatchers.ForEach(f => f.IncludeSubdirectories = true);
                    m_FileSysWatchers.ForEach(f => f.EnableRaisingEvents = true);
                }
            }
        }

        private void Clear_Click(object sender, EventArgs e)
        {
            if ((!m_ScanFileOrganizer.HasItems() ||
                this.ShowQuestion(Messages.UnprocessedItemsCloseAnyway, MessageBoxButtons.YesNo, DialogResult.Yes) &&
                (!m_SendFileOrganizer.HasItems() ||
                this.ShowQuestion(Messages.UnsentItemsCloseAnyway, MessageBoxButtons.YesNo, DialogResult.Yes))))
            {
                m_StateManager.Clear();
                m_ScanFileOrganizer.Clear();
                m_SendFileOrganizer.Clear();
                if (StateSaver.Default.Get<bool>(Strings.ClearScanDirectory, false))
                    ClearScanDirectory();
                tbTransferFile.Clear();
            }
        }

        private void Scanning_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2 && m_StateManager.Mode == StateManager.eMode.Barcode)
            {
                if (m_StateManager.CanCreate)
                    m_StateManager.CreateNewItem_AnyNoDoc();
                else
                    this.ShowExclamation("Please complete current item,\r\nbefore starting a new one.");
            }
        }

        private void Scan_Click(object sender, EventArgs e)
        {
            if (btnRun.IsOn())
                this.ShowExclamation(Messages.ScanningNotRun);

            if (string.IsNullOrWhiteSpace(tbScanDirectory.Text))
                this.ShowExclamation(Messages.ScanDirMayNotbeEmpty);

            if (!Directory.Exists(tbScanDirectory.Text))
                this.ShowExclamation(Messages.CanNotFindDir + tbScanDirectory.Text);
        }

        private void Send_Click(object sender, EventArgs e)
        {
            ItemControl item = scanContextMenuStrip.SourceControl as ItemControl;
            if (item != null && item.Item != null)
            {
                item.Item.State = StateManager.eState.NA;
                SendToServerAsync(item.Item);
            }
        }

        private void DownloadShow_Click(object sender, EventArgs e)
        {
            bool isSigned = sender == this.showSignedToolStripMenuItem;
            ItemControl selectedItemCnt = scanContextMenuStrip.SourceControl as ItemControl;
            if (selectedItemCnt != null)
            {
                selectedItemCnt.PlayEffect();

                var item = selectedItemCnt.Item;
                if (item == null)
                    return;

                if ((item.State == StateManager.eState.VOUCHER || item.State == StateManager.eState.COVER) && item.FileInfoList.Empty())
                {
                    var itm = item;
                    selectedItemCnt.Lock();

                    m_DownloadFileOrganizer.Data[item.SessionID] = selectedItemCnt;

                    m_DownloadFileOrganizer.RunTask(
                        new TaskProcessOrganizer<StateManager.Item>.TaskItem(itm, new Action<TaskProcessOrganizer<StateManager.Item>.TaskItem>((i) =>
                        {
                            bool isVoucher = itm.State == StateManager.eState.VOUCHER;
                            var versionDir = MainForm.GetAppSubFolder("DOWNLOAD");
                            string fullFileName = ServiceDataAccess.Instance.ReceiveFile(i.Item.Id, isVoucher, isSigned, versionDir.FullName);
                            m_DownloadFileOrganizer.Data[i.Item] = fullFileName;
                        })));
                }
                else
                {
                    SelectFilesForm.ShowFiles(this, item.FileInfoList);
                }
            }
        }

        private void DeleteFile_Click(object sender, EventArgs e)
        {
            ItemControl item = scanContextMenuStrip.SourceControl as ItemControl;
            if (item != null && item.Item != null)
            {
                if (this.ShowQuestion("You are going to delete an item.\r\nAre you sure?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if (item.Item.State.In(StateManager.eState.VOUCHER, StateManager.eState.COVER))
                    {
                        var srv = ServiceDataAccess.Instance;
                        srv.DeleteFile(item.Item.Id, item.Item.State == StateManager.eState.VOUCHER);
                        srv.SaveHistory(OperationHistory.FileDeleted, item.Item.ToString());
                        m_StateManager.Remove(item.Item);
                    }
                    else if (item.Item.State == StateManager.eState.NA)
                    {
                        m_StateManager.Remove(item.Item);
                    }
                }
            }
        }

        private void DeleteAllFilesInFolder_Click(object sender, EventArgs e)
        {
            if (Global.FolderID.HasValue)
            {
                if (this.ShowQuestion("You are about to delete all items\r\nin this folder.\r\nAre you sure.", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    var srv = ServiceDataAccess.Instance;
                    srv.DeleteAllFilesInFolder(Global.FolderID.GetValueOrDefault());
                    srv.SaveHistory(OperationHistory.AllFilesDeleted, Global.FolderID.GetValueOrDefault().ToString());
                    m_StateManager.Clear();
                }
            }
        }

        private void Forse_Click(object sender, EventArgs e)
        {
            ItemControl item = scanContextMenuStrip.SourceControl as ItemControl;
            if (item != null && item.Item != null && item.Item.State.In(StateManager.eState.NA))
                item.Item.Forsed = !item.Item.Forsed;
        }

        private void Ignore_Click(object sender, EventArgs e)
        {
            ItemControl item = scanContextMenuStrip.SourceControl as ItemControl;
            if (item != null && item.Item != null && item.Item.State.In(StateManager.eState.NA))
                item.Item.Ignored = !item.Item.Ignored;
        }

        private void ClearDesktop_Click(object sender, EventArgs e)
        {
            m_StateManager.Clear();
            if (StateSaver.Default.Get<bool>(Strings.ClearScanDirectory, false))
                ClearScanDirectory();
            tbTransferFile.Clear();
        }

        private void CoverSetup_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Filter = Strings.ImageFilter;

                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    var form = new SetupForm();
                    form.Img = Image.FromFile(dlg.FileName);
                    form.Selection = StateSaver.Default.Get<Rectangle>(Strings.VOUCHERCOVERREGION);
                    form.FormClosed += new FormClosedEventHandler(CoverSetup_FormClosed);
                    form.Show(this);
                    //Don't do that
                    //form.FormClosed -= new FormClosedEventHandler(CoverSetup_FormClosed);
                }
            }
        }

        private void CoverSetup_FormClosed(object sender, FormClosedEventArgs e)
        {
            SetupForm form = (SetupForm)sender;
            form.Img.DisposeSf();
            form.FormClosed -= new FormClosedEventHandler(CoverSetup_FormClosed);

            if (form.IsFirst && form.DialogResult == DialogResult.OK)
                StateSaver.Default.Set(Strings.VOUCHERCOVERREGION, form.Selection);
        }

        private void ShowHide_Click(object sender, EventArgs e)
        {
            ScanningSettingPanel.Height = (ScanningSettingPanel.Height == 26) ? 105 : 26;
            btnShowHide.Rotation = (ScanningSettingPanel.Height == 26) ? 180 : 0;
            btnClear.Visible = btnRun.Visible = toggleButtonControl1.Visible =
                btnShowMonitor.Visible = cbVoucherMustExist.Visible = cbCoversheet.Visible =
                (ScanningSettingPanel.Height != 26);
            //pnlScanMode.Visible = btnScan.Visible = (ScanningSettingPanel.Height != 26);
        }

        private void OpenFolder_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start(tbScanDirectory.Text);
            }
            catch (Exception ex)
            {
                this.ShowExclamation(ex.Message);
            }
        }

        private void FolderMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (tvFolders.SelectedNode == null)
                return;

            var ninfo = (FolderInfo)tvFolders.SelectedNode.Tag;
            if (e.ClickedItem == tbAddNew)
            {
                int? parentId = (ninfo != null) ? (int?)ninfo.Id : null;
                string name = null;
                if (InputForm.show(this, ref name, "Add New", "Name") && !string.IsNullOrWhiteSpace(name))
                {
                    var srv = ServiceDataAccess.Instance;
                    srv.AddFolder(parentId, name);
                    tvFolders.SelectedNode.Nodes.Clear();
                    BuildNode(tvFolders.SelectedNode);
                    srv.SaveHistory(OperationHistory.FolderAdded, name);
                }
            }
            else if (e.ClickedItem == tbRename)
            {
                if (ninfo != null)
                {
                    string name = null;
                    if (InputForm.show(this, ref name, "Rename", "New name") && !string.IsNullOrWhiteSpace(name))
                    {
                        var srv = ServiceDataAccess.Instance;
                        srv.RenameFolder(ninfo.Id, name);
                        srv.SaveHistory(OperationHistory.FolderRenamed, tvFolders.SelectedNode.Text.concat(" => ", name));
                        tvFolders.SelectedNode.Text = name;
                    }
                }
                else
                    this.ShowExclamation("Cannot rename this node");
            }
            else if (e.ClickedItem == tbDelete)
            {
                if (ninfo != null)
                {
                    if (this.ShowQuestion("You are about delete selected folder?\r\nThis will delete all vouchers in it.\r\nAre you sure?",
                        MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
                    {
                        var srv = ServiceDataAccess.Instance;
                        srv.DeleteFolder(ninfo.Id);
                        srv.SaveHistory(OperationHistory.FolderDeleted, tvFolders.SelectedNode.Text);
                        tvFolders.SelectedNode.Remove();
                    }
                }
                else
                    this.ShowExclamation("Cannot delete this node");
            }
        }

        private void ShowMonitor_Click(object sender, EventArgs e)
        {
            MonitorForm.show();
        }

        private void Coversheet_CheckedChanged(object sender, EventArgs e)
        {
            MainForm.ms_ImportCoversheet = cbCoversheet.Checked;
            cbCoversheet.BackColor = cbCoversheet.Checked ? Color.Red : SystemColors.Control;
        }

        private void BrowseForExprFile_Click(object sender, EventArgs e)
        {
            int countryId = 0, from = 0, to = 0;
            string sitecode = null;

            if (TransferForm.show(this, ref countryId, ref from, ref to, ref sitecode))
            {
                this.Enabled = false;
                Cursor.Current = Cursors.WaitCursor;

                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        var result1 = ServiceDataAccess.Instance.GetTransferFile(countryId, from, to, sitecode).ToList();
                        var result2 = result1.ConvertAll((t) => new JobItem(t, countryId));
                        m_StateManager.Load(result2);
                        MainForm.ms_DefaultCountryId = countryId;

                        this.InvokeSf(() =>
                        {
                            tbTransferFile.Text =
                                string.Format("Country: {0} Site Code: {1} From: {2} To: {3}", countryId, sitecode, from, to);
                        });
                    }
                    catch (Exception ex)
                    {
                        Program.OnThreadException(this, new ThreadExceptionEventArgs(ex));
                    }
                    finally
                    {
                        this.InvokeSf(() =>
                        {
                            this.Enabled = true;
                            Cursor.Current = Cursors.Default;
                        });
                    }
                });
            }
        }

        private void ForceAll_Click(object sender, EventArgs e)
        {
            m_StateManager.ForceAll();
        }

        private void Details_Click(object sender, EventArgs e)
        {
            ItemControl item = scanContextMenuStrip.SourceControl as ItemControl;
            if (item != null && item.Item != null)
                this.ShowInfo(item.Item.ToString());
        }

        private void NameMenuItem_Click(object sender, EventArgs e)
        {
            ItemControl item = scanContextMenuStrip.SourceControl as ItemControl;
            if (item != null && item.Item != null && item.Item.Id > 0)
            {
                string name = null;
                if (InputForm.show(this, ref name, "Name", "Name"))
                {
                    bool isVoucher = item.Item is StateManager.VoucherItem;
                    string setSql = isVoucher ? string.Format("v_name = '{0}'", name) : string.Format("f_name = '{0}'", name);
                    string whereClause = isVoucher ? string.Format("id = {0}", item.Item.Id) : string.Format("f_id = {0}", item.Item.Id);
                    ServiceDataAccess.Instance.UpdateVouchersOrFilesBySql(setSql, whereClause, isVoucher);
                    item.Item.Name = name;
                    item.Item.FireUpdated();
                }
            }
        }

        private void AddToTran_Click(object sender, EventArgs e)
        {
            if (Global.FolderID.HasValue)
            {
                int countryId = 0, retailerId = 0, voucherId = 0; string siteCode = "", name = "";

                if (AddVoucherItemForm.show(this, ref countryId, ref retailerId, ref voucherId, ref siteCode))
                    m_StateManager.AddVoucherItem(0, countryId, retailerId, voucherId, StateManager.eState.NA, Guid.NewGuid(), siteCode, name);
            }
            else
                this.ShowExclamation("Don't scan to the image root\r\nOpen tree and create folder");
        }

        #endregion //EVENT HANDLERS

        #region PUBLIC STATIC VOLATILE FIELDS

        public static volatile bool ms_SimulatePrint;

        public static volatile bool ms_MiltuPagePrint;

        public static volatile bool ms_ImportCoversheet;

        public static volatile int ms_DefaultCountryId;

        //public static volatile BarcodeConfig ms_BarcodeConfig;

        #endregion

        private void toggleButtonControl1_ActiveChanged(object sender, ValueEventArgs<int> e)
        {
            StateManager.eMode mode = (StateManager.eMode)(e.Value + 1);
            m_StateManager.Mode = mode; ///cbUseTransferFile.Checked ? StateManager.eMode.TransferFile : StateManager.eMode.Barcode;
        }

        private void toggleButtonControl2_ActiveChanged(object sender, ValueEventArgs<int> e)
        {
            StateManager.vMode mode = (StateManager.vMode)(e.Value + 1);
            m_StateManager.PartMode = mode; ///P1 = 1, P15 = 2, P2 = 3
        }

        private void VoucherMustExist_CheckedChanged(object sender, EventArgs e)
        {
            cbVoucherMustExist.Text = cbVoucherMustExist.Checked ? "Must Exist" : "Brand New";
            cbVoucherMustExist.BackColor = cbVoucherMustExist.Checked ? OnColor : OffColor;
            m_StateManager.VoucherMustExist = cbVoucherMustExist.Checked;
        }

        #endregion //SCANNING

        private void FileBrowser_Click(object sender, EventArgs e)
        {
            var explorer = new Explorer();
            explorer.Show(this);
        }

        private void ScanDirectory_TextChanged(object sender, EventArgs e)
        {
            StateSaver.Default.Set(Strings.tbScanDirectory, tbScanDirectory.Text);
        }

        private void PrevNextClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (sender == lblPrev)
            {
                lblNext.Visible = true;

                if (m_CurrentVoucher > 0)
                {
                    m_CurrentVoucher -= Program.ITEMS_SHOWN;
                    RefreshVoucherPanel();
                }
                else
                {
                    lblPrev.Visible = false;
                }
            }
            else
            {
                lblPrev.Visible = true;

                if (m_StateManager.Count > 0)
                {
                    m_CurrentVoucher += Program.ITEMS_SHOWN;
                    RefreshVoucherPanel();
                }
                else
                {
                    lblNext.Visible = false;
                }
            }
        }

        private void StartScheduler_Click(object sender, EventArgs e)
        {
            using (var formn = new SchedulerForm())
                formn.ShowDialog(this);
        }

        private void btnCCCover_Click(object sender, EventArgs e)
        {
            var useCover = StateSaver.Default.Set<bool>(Strings.USE_VCOVER, !StateSaver.Default.Get<bool>(Strings.USE_VCOVER, false));
            btnCCCover.Set(useCover);
        }
    }
}