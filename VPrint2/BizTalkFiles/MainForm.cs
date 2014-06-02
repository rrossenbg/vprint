using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using BizTalkFiles.Properties;

namespace BizTalkFiles
{
    public class MainForm : Form
    {
        #region CONTROLS

        private Button btnClose;
        private Button btnStartParser;
        private IContainer components;
        private Label label1;
        private Label label2;
        private Label lblCount;
        private Label lblFvFIn;
        private Label lblFvFQty;
        private Label lblMoveFolder;
        private Label lblParsedQty;
        private readonly FvFinParserWorker m_FvFinParserWorker = new FvFinParserWorker();
        private readonly BizTalkFeederWorker m_BizTalkFeederWorker = new BizTalkFeederWorker();
        private TextBox txtErrorFolder;
        private TextBox txtSplitterFilesCount;
        private TextBox txtFvFinIn;
        private TextBox txtMaxBizTalFileCount;
        private TextBox txtFvFinOutFolder;
        private Label label3;
        private Button btnStartFeeder;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem menuToolStripMenuItem;
        private ToolStripMenuItem connectionStringToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private FontDialog fontDialog1;
        private Button btnTestBizTalk;
        private TextBox txtBizTalkFilesCount;
        private Label label4;
        private TextBox txtMaxDehydratedFilesCount;
        private Label label5;
        private TextBox txtArchiveFolder;
        private Label label6;
        private TextBox txtParsedErrorFolder;
        private ComboBox ddlOrchestrationName;
        private Label label7;
        private LinkLabel linkLabel1;
        private LinkLabel linkLabel2;
        private LinkLabel linkLabel3;
        private LinkLabel linkLabel4;
        private LinkLabel linkLabel5;
        private LinkLabel linkLabel6;
        private ToolTip toolTip1;
        private Panel pnlFVLLight;
        private Panel pnlFeederLight;
        private TextBox txtBizTalkParsedFolder;

        #endregion

        #region MEMBERS

        private SynchronizationContext m_Context;
        private NotifyIcon notify;
        private ContextMenuStrip contextMenu;
        private ToolStripMenuItem openMenuItem1;
        private ToolStripSeparator menuItem2;
        private ToolStripMenuItem exitMenuItem;
        private FileSystemWatcher Watcher1;
        private FileSystemWatcher Watcher2;

        private readonly AppInfoHolder m_FvInfoHolder = new AppInfoHolder();

        private const string XPATH = @"C:\DiData\PTF\Payments\BACS";
        private const string XPATH2 = @"C:\DiData\PTF\Locations\FvFin\ReTry";
        /// <summary>
        /// *.xml
        /// </summary>
        private const string EXT = "*.xml";

        #endregion

        public MainForm()
        {
            InitializeComponent();

            m_Context = SynchronizationContext.Current;

            m_FvFinParserWorker.SleepTime = TimeSpan.FromSeconds(5.0);
            m_FvFinParserWorker.Step += new EventHandler(Worker_Step);
            m_FvFinParserWorker.Error += new ThreadExceptionEventHandler(Program.OnException);

            m_BizTalkFeederWorker.SleepTime = TimeSpan.FromSeconds(5.0);
            m_BizTalkFeederWorker.Step += new EventHandler(Worker_Step);
            m_BizTalkFeederWorker.Error += new ThreadExceptionEventHandler(Program.OnException);

            linkLabel1.Tag = txtFvFinIn;
            linkLabel2.Tag = txtFvFinOutFolder;
            linkLabel3.Tag = txtBizTalkParsedFolder;
            linkLabel4.Tag = txtArchiveFolder;
            linkLabel5.Tag = txtErrorFolder;
            linkLabel6.Tag = txtParsedErrorFolder;


            notify.Icon = Resources.Deleket_Adobe_Cs4_File_Adobe_Dreamweaver_XML_01;
            notify.Visible = true;
            notify.Text = Application.ProductName;
            notify.BalloonTipText = string.Format("{0} should be closed explicitly.", Application.ProductName);
            notify.BalloonTipIcon = ToolTipIcon.Info;
            notify.BalloonTipTitle = Application.ProductName;
            notify.ShowBalloonTip(500);

            notify.ContextMenuStrip = this.contextMenu;
            notify.DoubleClick += new EventHandler(NotifyIcon_Click);

            Application.ThreadExit += new EventHandler(Application_ThreadExit);

            if (Directory.Exists(XPATH))
            {
                Watcher1.Filter = EXT;
                Watcher1.Path = XPATH;
                Watcher1.EnableRaisingEvents = true;

                Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(500);
                    var files = Directory.GetFiles(XPATH, EXT);
                    foreach (var file in files)
                        new Action<string>((f) => File.AppendAllText(f, " ")).RunSafe(file);
                });
            }
            else
            {
                Watcher1.EnableRaisingEvents = false;
            }

            if (Directory.Exists(XPATH2))
            {
                Watcher2.Path = XPATH2;
                Watcher2.EnableRaisingEvents = true;
            }
            else
            {
                Watcher2.EnableRaisingEvents = false;
            }
        }


        private void Application_ThreadExit(object sender, EventArgs e)
        {
            m_FvFinParserWorker.Stop();
            m_BizTalkFeederWorker.Stop();
        }

        private void Worker_Step(object sender, EventArgs e)
        {
            if (this.IsReady())
            {
                if (sender == m_FvFinParserWorker)
                {
                    m_Context.Post(LightIndicator, pnlFVLLight);
                }
                else if (sender == m_BizTalkFeederWorker)
                {
                    m_Context.Post(LightIndicator, pnlFeederLight);
                }
            }
        }

        private void LightIndicator(object state)
        {
            Control cnt = (Control)state;
            cnt.SwitchBackColor(Color.Green, Color.YellowGreen);
        }

        private void Close_Click(object sender, EventArgs e)
        {
            Close();
        }

        public void ParserStart_Click(object sender, EventArgs e)
        {
            if (!this.m_FvFinParserWorker.Running)
            {
                int processFileCount = 0;

                if (string.IsNullOrEmpty(this.txtFvFinIn.Text))
                {
                    this.ShowInformation("FvFinFolder is not correct");
                }
                else if (string.IsNullOrEmpty(this.txtFvFinOutFolder.Text))
                {
                    this.ShowInformation("FvFin ParsedFolder is not correct");
                }
                else if (string.IsNullOrEmpty(this.txtErrorFolder.Text))
                {
                    this.ShowInformation("ErrorFolder is not correct");
                }
                else if (string.IsNullOrEmpty(this.txtArchiveFolder.Text))
                {
                    this.ShowInformation("ArchiveFolder is not correct");
                }
                else if (string.IsNullOrEmpty(this.txtParsedErrorFolder.Text))
                {
                    this.ShowInformation("ParsedErrFolder is not correct");
                }
                else if ((!int.TryParse(this.txtSplitterFilesCount.Text, out processFileCount) || (processFileCount <= 0)) || (processFileCount > 500))
                {
                    this.ShowInformation("Max FilesCount should be a valid number (1-500)");
                }
                else
                {
                    EnSureDirectories();

                    m_FvInfoHolder.FvFinInputFolder =
                    this.m_FvFinParserWorker.FvFinInputFolder = this.txtFvFinIn.Text;

                    m_FvInfoHolder.FvFinParsedFolder =
                    this.m_FvFinParserWorker.FvFinParsedFolder = this.txtFvFinOutFolder.Text;

                    m_FvInfoHolder.ErrorFolder =
                    this.m_FvFinParserWorker.ErrorFolder = this.txtErrorFolder.Text;
                    
                    this.m_FvFinParserWorker.MaxProcessFilesCount = processFileCount;

                    m_FvInfoHolder.ArchiveFolder =
                    this.m_FvFinParserWorker.ArchiveFolder = txtArchiveFolder.Text;

                    m_FvInfoHolder.ParsedErrFolder =
                    this.m_FvFinParserWorker.ParsedErrFolder = txtParsedErrorFolder.Text;

                    this.m_FvFinParserWorker.Start(ThreadPriority.Lowest, "FvFinParserWorker");
                }
            }
            else
            {
                this.m_FvFinParserWorker.Stop();
            }

            this.btnStartParser.BackColor = this.m_FvFinParserWorker.Running ? Color.Green : Color.Red;
            this.btnStartParser.Text = this.m_FvFinParserWorker.Running ? "Stop FVL Parser" : "Start FVL Parser";
        }

        private void StartFeeder_Click(object sender, EventArgs e)
        {
            if (!m_BizTalkFeederWorker.Running)
            {
                int bizTalkFilesCount = 0, processFileCount = 0, bizTalkDehydratedFilesCount = 0;

                if (string.IsNullOrEmpty(this.txtFvFinOutFolder.Text))
                {
                    this.ShowInformation("FvFin out Folder is not correct");
                }
                else if (string.IsNullOrEmpty(this.txtBizTalkParsedFolder.Text))
                {
                    this.ShowInformation("BizTalk Parsed Folder is not correct");
                }
                else if (string.IsNullOrEmpty(this.txtErrorFolder.Text))
                {
                    this.ShowInformation("ErrorFolder is not correct");
                }
                else if (!int.TryParse(this.txtBizTalkFilesCount.Text, out processFileCount) || (processFileCount <= 0) || (processFileCount > 500))
                {
                    this.ShowInformation("Max Process FilesCount should be a valid number (1-500)");
                }
                else if (!int.TryParse(this.txtMaxBizTalFileCount.Text, out bizTalkFilesCount) || (bizTalkFilesCount <= 0) || (bizTalkFilesCount > 1000))
                {
                    this.ShowInformation("Max BizTalk Files Count should be a valid number (1-1000)");
                }
                else if (!int.TryParse(this.txtMaxDehydratedFilesCount.Text, out bizTalkDehydratedFilesCount) || (bizTalkDehydratedFilesCount <= 0) || (bizTalkDehydratedFilesCount > 100))
                {
                    this.ShowInformation("Max BizTalk Dehydrated Files should be a valid number (1-100)");
                }
                else if (string.IsNullOrEmpty(ddlOrchestrationName.Text))
                {
                    this.ShowInformation("BizTalk orchestration name couldn't be empty");
                }
                else
                {
                    EnSureDirectories();
                    SqlServerHelper.OrechestrationName = ddlOrchestrationName.Text;
                    this.m_BizTalkFeederWorker.FvFinOutFolder = this.txtFvFinOutFolder.Text;
                    this.m_BizTalkFeederWorker.BizTalkFolder = this.txtBizTalkParsedFolder.Text;
                    this.m_BizTalkFeederWorker.MaxProcessFilesCount = processFileCount;
                    this.m_BizTalkFeederWorker.MaxBizTalkFilesCount = bizTalkFilesCount;
                    this.m_BizTalkFeederWorker.TalkDehydratedFilesCount = bizTalkDehydratedFilesCount;
                    this.m_BizTalkFeederWorker.Start(ThreadPriority.Lowest, "BizTalkFeederWorker");
                }
            }
            else
            {
                this.m_BizTalkFeederWorker.Stop();
            }

            this.btnStartFeeder.BackColor = this.m_BizTalkFeederWorker.Running ? Color.Green : Color.Red;
            this.btnStartFeeder.Text = this.m_BizTalkFeederWorker.Running ? "Stop BizTalk Feeder" : "Start BizTalk Feeder";
        }

        private void EnSureDirectories()
        {
            if (!Directory.Exists(txtFvFinIn.Text))
                Directory.CreateDirectory(txtFvFinIn.Text);

            if (!Directory.Exists(txtFvFinOutFolder.Text))
                Directory.CreateDirectory(txtFvFinOutFolder.Text);

            if (!Directory.Exists(txtErrorFolder.Text))
                Directory.CreateDirectory(txtErrorFolder.Text);

            if (!Directory.Exists(txtArchiveFolder.Text))
                Directory.CreateDirectory(txtArchiveFolder.Text);

            if (!Directory.Exists(txtParsedErrorFolder.Text))
                Directory.CreateDirectory(txtParsedErrorFolder.Text);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.btnStartParser = new System.Windows.Forms.Button();
            this.lblFvFIn = new System.Windows.Forms.Label();
            this.txtFvFinIn = new System.Windows.Forms.TextBox();
            this.lblFvFQty = new System.Windows.Forms.Label();
            this.lblParsedQty = new System.Windows.Forms.Label();
            this.txtBizTalkParsedFolder = new System.Windows.Forms.TextBox();
            this.lblMoveFolder = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.lblCount = new System.Windows.Forms.Label();
            this.txtSplitterFilesCount = new System.Windows.Forms.TextBox();
            this.txtErrorFolder = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtMaxBizTalFileCount = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtFvFinOutFolder = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnStartFeeder = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.menuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectionStringToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fontDialog1 = new System.Windows.Forms.FontDialog();
            this.btnTestBizTalk = new System.Windows.Forms.Button();
            this.txtBizTalkFilesCount = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtMaxDehydratedFilesCount = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtArchiveFolder = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtParsedErrorFolder = new System.Windows.Forms.TextBox();
            this.ddlOrchestrationName = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.linkLabel2 = new System.Windows.Forms.LinkLabel();
            this.linkLabel3 = new System.Windows.Forms.LinkLabel();
            this.linkLabel4 = new System.Windows.Forms.LinkLabel();
            this.linkLabel5 = new System.Windows.Forms.LinkLabel();
            this.linkLabel6 = new System.Windows.Forms.LinkLabel();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.pnlFVLLight = new System.Windows.Forms.Panel();
            this.pnlFeederLight = new System.Windows.Forms.Panel();
            this.Watcher1 = new System.IO.FileSystemWatcher();
            this.notify = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Watcher2 = new System.IO.FileSystemWatcher();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Watcher1)).BeginInit();
            this.contextMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Watcher2)).BeginInit();
            this.SuspendLayout();
            // 
            // btnStartParser
            // 
            this.btnStartParser.Location = new System.Drawing.Point(124, 328);
            this.btnStartParser.Name = "btnStartParser";
            this.btnStartParser.Size = new System.Drawing.Size(121, 23);
            this.btnStartParser.TabIndex = 0;
            this.btnStartParser.Text = "Start FVL Splitter";
            this.btnStartParser.UseVisualStyleBackColor = true;
            this.btnStartParser.Click += new System.EventHandler(this.ParserStart_Click);
            // 
            // lblFvFIn
            // 
            this.lblFvFIn.AutoSize = true;
            this.lblFvFIn.Location = new System.Drawing.Point(14, 40);
            this.lblFvFIn.Name = "lblFvFIn";
            this.lblFvFIn.Size = new System.Drawing.Size(78, 13);
            this.lblFvFIn.TabIndex = 1;
            this.lblFvFIn.Text = "FvFIn In Folder";
            // 
            // txtFvFinIn
            // 
            this.txtFvFinIn.Location = new System.Drawing.Point(115, 37);
            this.txtFvFinIn.Name = "txtFvFinIn";
            this.txtFvFinIn.Size = new System.Drawing.Size(265, 20);
            this.txtFvFinIn.TabIndex = 3;
            this.txtFvFinIn.Text = "C:\\DiData\\PTF\\Locations\\FvFin";
            // 
            // lblFvFQty
            // 
            this.lblFvFQty.AutoSize = true;
            this.lblFvFQty.Location = new System.Drawing.Point(386, 38);
            this.lblFvFQty.Name = "lblFvFQty";
            this.lblFvFQty.Size = new System.Drawing.Size(0, 13);
            this.lblFvFQty.TabIndex = 5;
            // 
            // lblParsedQty
            // 
            this.lblParsedQty.AutoSize = true;
            this.lblParsedQty.Location = new System.Drawing.Point(389, 86);
            this.lblParsedQty.Name = "lblParsedQty";
            this.lblParsedQty.Size = new System.Drawing.Size(0, 13);
            this.lblParsedQty.TabIndex = 6;
            // 
            // txtBizTalkParsedFolder
            // 
            this.txtBizTalkParsedFolder.Location = new System.Drawing.Point(115, 83);
            this.txtBizTalkParsedFolder.Name = "txtBizTalkParsedFolder";
            this.txtBizTalkParsedFolder.Size = new System.Drawing.Size(265, 20);
            this.txtBizTalkParsedFolder.TabIndex = 10;
            this.txtBizTalkParsedFolder.Text = "C:\\DiData\\PTF\\Locations\\InCommon\\Parsed";
            // 
            // lblMoveFolder
            // 
            this.lblMoveFolder.AutoSize = true;
            this.lblMoveFolder.Location = new System.Drawing.Point(14, 86);
            this.lblMoveFolder.Name = "lblMoveFolder";
            this.lblMoveFolder.Size = new System.Drawing.Size(86, 13);
            this.lblMoveFolder.TabIndex = 11;
            this.lblMoveFolder.Text = "BizTalk In Folder";
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(307, 361);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 12;
            this.btnClose.Text = "Hide";
            this.toolTip1.SetToolTip(this.btnClose, "Close Application");
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.Close_Click);
            // 
            // lblCount
            // 
            this.lblCount.AutoSize = true;
            this.lblCount.Location = new System.Drawing.Point(14, 285);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(106, 26);
            this.lblCount.TabIndex = 13;
            this.lblCount.Text = "Max Processed Files\r\n on one go     (1-500)";
            // 
            // txtSplitterFilesCount
            // 
            this.txtSplitterFilesCount.Location = new System.Drawing.Point(175, 291);
            this.txtSplitterFilesCount.Name = "txtSplitterFilesCount";
            this.txtSplitterFilesCount.Size = new System.Drawing.Size(70, 20);
            this.txtSplitterFilesCount.TabIndex = 14;
            this.txtSplitterFilesCount.Text = "5";
            // 
            // txtErrorFolder
            // 
            this.txtErrorFolder.Location = new System.Drawing.Point(115, 145);
            this.txtErrorFolder.Name = "txtErrorFolder";
            this.txtErrorFolder.Size = new System.Drawing.Size(265, 20);
            this.txtErrorFolder.TabIndex = 15;
            this.txtErrorFolder.Text = "C:\\DiData\\PTF\\Locations\\FvFin\\Error";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 148);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 16;
            this.label1.Text = "Error Folder";
            // 
            // txtMaxBizTalFileCount
            // 
            this.txtMaxBizTalFileCount.Location = new System.Drawing.Point(307, 234);
            this.txtMaxBizTalFileCount.Name = "txtMaxBizTalFileCount";
            this.txtMaxBizTalFileCount.Size = new System.Drawing.Size(75, 20);
            this.txtMaxBizTalFileCount.TabIndex = 17;
            this.txtMaxBizTalFileCount.Text = "50";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 237);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(177, 13);
            this.label2.TabIndex = 18;
            this.label2.Text = "Max Files in BizTalk Folder  (1-1000)";
            // 
            // txtFvFinOutFolder
            // 
            this.txtFvFinOutFolder.Location = new System.Drawing.Point(115, 60);
            this.txtFvFinOutFolder.Name = "txtFvFinOutFolder";
            this.txtFvFinOutFolder.Size = new System.Drawing.Size(265, 20);
            this.txtFvFinOutFolder.TabIndex = 19;
            this.txtFvFinOutFolder.Text = "C:\\DiData\\PTF\\Locations\\FvFin\\Parsed";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 63);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(90, 13);
            this.label3.TabIndex = 20;
            this.label3.Text = "FvFIn Out Parsed";
            // 
            // btnStartFeeder
            // 
            this.btnStartFeeder.Location = new System.Drawing.Point(265, 328);
            this.btnStartFeeder.Name = "btnStartFeeder";
            this.btnStartFeeder.Size = new System.Drawing.Size(121, 23);
            this.btnStartFeeder.TabIndex = 21;
            this.btnStartFeeder.Text = "Start BizTalk Feeder";
            this.btnStartFeeder.UseVisualStyleBackColor = true;
            this.btnStartFeeder.Click += new System.EventHandler(this.StartFeeder_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(402, 24);
            this.menuStrip1.TabIndex = 22;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // menuToolStripMenuItem
            // 
            this.menuToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connectionStringToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.menuToolStripMenuItem.Name = "menuToolStripMenuItem";
            this.menuToolStripMenuItem.Size = new System.Drawing.Size(45, 20);
            this.menuToolStripMenuItem.Text = "Menu";
            // 
            // connectionStringToolStripMenuItem
            // 
            this.connectionStringToolStripMenuItem.Name = "connectionStringToolStripMenuItem";
            this.connectionStringToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.connectionStringToolStripMenuItem.Text = "Connection String";
            this.connectionStringToolStripMenuItem.Click += new System.EventHandler(this.ConnectionString_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.Exit_Click);
            // 
            // fontDialog1
            // 
            this.fontDialog1.Color = System.Drawing.SystemColors.ControlText;
            // 
            // btnTestBizTalk
            // 
            this.btnTestBizTalk.Location = new System.Drawing.Point(17, 328);
            this.btnTestBizTalk.Name = "btnTestBizTalk";
            this.btnTestBizTalk.Size = new System.Drawing.Size(81, 23);
            this.btnTestBizTalk.TabIndex = 23;
            this.btnTestBizTalk.Text = "Test BizTalk";
            this.btnTestBizTalk.UseVisualStyleBackColor = true;
            this.btnTestBizTalk.Click += new System.EventHandler(this.TestBizTalk_Click);
            // 
            // txtBizTalkFilesCount
            // 
            this.txtBizTalkFilesCount.Location = new System.Drawing.Point(307, 291);
            this.txtBizTalkFilesCount.Name = "txtBizTalkFilesCount";
            this.txtBizTalkFilesCount.Size = new System.Drawing.Size(75, 20);
            this.txtBizTalkFilesCount.TabIndex = 24;
            this.txtBizTalkFilesCount.Text = "50";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 264);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(197, 13);
            this.label4.TabIndex = 26;
            this.label4.Text = "Max Dehydrated Files in BizTalk  (1-100)";
            // 
            // txtMaxDehydratedFilesCount
            // 
            this.txtMaxDehydratedFilesCount.Location = new System.Drawing.Point(307, 261);
            this.txtMaxDehydratedFilesCount.Name = "txtMaxDehydratedFilesCount";
            this.txtMaxDehydratedFilesCount.Size = new System.Drawing.Size(75, 20);
            this.txtMaxDehydratedFilesCount.TabIndex = 25;
            this.txtMaxDehydratedFilesCount.Text = "20";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 112);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(75, 13);
            this.label5.TabIndex = 28;
            this.label5.Text = "Archive Folder";
            // 
            // txtArchiveFolder
            // 
            this.txtArchiveFolder.Location = new System.Drawing.Point(115, 109);
            this.txtArchiveFolder.Name = "txtArchiveFolder";
            this.txtArchiveFolder.Size = new System.Drawing.Size(265, 20);
            this.txtArchiveFolder.TabIndex = 27;
            this.txtArchiveFolder.Text = "C:\\DiData\\PTF\\Locations\\FvFIn\\Archive";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(14, 174);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(97, 13);
            this.label6.TabIndex = 30;
            this.label6.Text = "Parsed Error Folder";
            // 
            // txtParsedErrorFolder
            // 
            this.txtParsedErrorFolder.Location = new System.Drawing.Point(115, 171);
            this.txtParsedErrorFolder.Name = "txtParsedErrorFolder";
            this.txtParsedErrorFolder.Size = new System.Drawing.Size(265, 20);
            this.txtParsedErrorFolder.TabIndex = 29;
            this.txtParsedErrorFolder.Text = "C:\\DiData\\PTF\\Locations\\FvFin\\Parsed_Error ";
            // 
            // ddlOrchestrationName
            // 
            this.ddlOrchestrationName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlOrchestrationName.FormattingEnabled = true;
            this.ddlOrchestrationName.Items.AddRange(new object[] {
            "CommonOrchestration",
            "VoucherIndividualProcessing"});
            this.ddlOrchestrationName.Location = new System.Drawing.Point(154, 198);
            this.ddlOrchestrationName.Name = "ddlOrchestrationName";
            this.ddlOrchestrationName.Size = new System.Drawing.Size(228, 21);
            this.ddlOrchestrationName.TabIndex = 31;
            this.toolTip1.SetToolTip(this.ddlOrchestrationName, "Set orchestration Name");
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(14, 201);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(101, 13);
            this.label7.TabIndex = 32;
            this.label7.Text = "Orchestration Name";
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(386, 40);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(16, 13);
            this.linkLabel1.TabIndex = 33;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "...";
            this.toolTip1.SetToolTip(this.linkLabel1, "Click to open path");
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabel_LinkClicked);
            // 
            // linkLabel2
            // 
            this.linkLabel2.AutoSize = true;
            this.linkLabel2.Location = new System.Drawing.Point(386, 63);
            this.linkLabel2.Name = "linkLabel2";
            this.linkLabel2.Size = new System.Drawing.Size(16, 13);
            this.linkLabel2.TabIndex = 34;
            this.linkLabel2.TabStop = true;
            this.linkLabel2.Text = "...";
            this.toolTip1.SetToolTip(this.linkLabel2, "Click to open path");
            this.linkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabel_LinkClicked);
            // 
            // linkLabel3
            // 
            this.linkLabel3.AutoSize = true;
            this.linkLabel3.Location = new System.Drawing.Point(386, 86);
            this.linkLabel3.Name = "linkLabel3";
            this.linkLabel3.Size = new System.Drawing.Size(16, 13);
            this.linkLabel3.TabIndex = 35;
            this.linkLabel3.TabStop = true;
            this.linkLabel3.Text = "...";
            this.toolTip1.SetToolTip(this.linkLabel3, "Click to open path");
            this.linkLabel3.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabel_LinkClicked);
            // 
            // linkLabel4
            // 
            this.linkLabel4.AutoSize = true;
            this.linkLabel4.Location = new System.Drawing.Point(386, 112);
            this.linkLabel4.Name = "linkLabel4";
            this.linkLabel4.Size = new System.Drawing.Size(16, 13);
            this.linkLabel4.TabIndex = 36;
            this.linkLabel4.TabStop = true;
            this.linkLabel4.Text = "...";
            this.toolTip1.SetToolTip(this.linkLabel4, "Click to open path");
            this.linkLabel4.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabel_LinkClicked);
            // 
            // linkLabel5
            // 
            this.linkLabel5.AutoSize = true;
            this.linkLabel5.Location = new System.Drawing.Point(386, 148);
            this.linkLabel5.Name = "linkLabel5";
            this.linkLabel5.Size = new System.Drawing.Size(16, 13);
            this.linkLabel5.TabIndex = 37;
            this.linkLabel5.TabStop = true;
            this.linkLabel5.Text = "...";
            this.toolTip1.SetToolTip(this.linkLabel5, "Click to open path");
            this.linkLabel5.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabel_LinkClicked);
            // 
            // linkLabel6
            // 
            this.linkLabel6.AutoSize = true;
            this.linkLabel6.Location = new System.Drawing.Point(386, 174);
            this.linkLabel6.Name = "linkLabel6";
            this.linkLabel6.Size = new System.Drawing.Size(16, 13);
            this.linkLabel6.TabIndex = 38;
            this.linkLabel6.TabStop = true;
            this.linkLabel6.Text = "...";
            this.toolTip1.SetToolTip(this.linkLabel6, "Click to open path");
            this.linkLabel6.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabel_LinkClicked);
            // 
            // pnlFVLLight
            // 
            this.pnlFVLLight.Location = new System.Drawing.Point(127, 331);
            this.pnlFVLLight.Name = "pnlFVLLight";
            this.pnlFVLLight.Size = new System.Drawing.Size(5, 5);
            this.pnlFVLLight.TabIndex = 39;
            // 
            // pnlFeederLight
            // 
            this.pnlFeederLight.Location = new System.Drawing.Point(268, 331);
            this.pnlFeederLight.Name = "pnlFeederLight";
            this.pnlFeederLight.Size = new System.Drawing.Size(5, 5);
            this.pnlFeederLight.TabIndex = 40;
            // 
            // Watcher1
            // 
            
            this.Watcher1.SynchronizingObject = this;
            this.Watcher1.Changed += new System.IO.FileSystemEventHandler(this.BAC_File_CreatedChanged);
            this.Watcher1.Created += new System.IO.FileSystemEventHandler(this.BAC_File_CreatedChanged);
            // 
            // notify
            // 
            this.notify.Text = "notifyIcon1";
            this.notify.Visible = true;
            // 
            // contextMenu
            // 
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openMenuItem1,
            this.menuItem2,
            this.exitMenuItem});
            this.contextMenu.Name = "contextMenuStrip1";
            this.contextMenu.Size = new System.Drawing.Size(101, 54);
            // 
            // openMenuItem1
            // 
            this.openMenuItem1.Name = "openMenuItem1";
            this.openMenuItem1.Size = new System.Drawing.Size(100, 22);
            this.openMenuItem1.Text = "Show";
            this.openMenuItem1.Click += new System.EventHandler(this.NotifyIcon_Click);
            // 
            // menuItem2
            // 
            this.menuItem2.Name = "menuItem2";
            this.menuItem2.Size = new System.Drawing.Size(97, 6);
            // 
            // exitMenuItem
            // 
            this.exitMenuItem.Name = "exitMenuItem";
            this.exitMenuItem.Size = new System.Drawing.Size(100, 22);
            this.exitMenuItem.Text = "Exit";
            this.exitMenuItem.Click += new System.EventHandler(this.Exit2_Click);
            // 
            // Watcher2
            // 
            this.Watcher2.EnableRaisingEvents = true;
            this.Watcher2.Filter = "*.xml";
            this.Watcher2.SynchronizingObject = this;
            this.Watcher2.Created += new System.IO.FileSystemEventHandler(this.Retry_File_CreatedChanged);
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(402, 389);
            this.Controls.Add(this.pnlFeederLight);
            this.Controls.Add(this.pnlFVLLight);
            this.Controls.Add(this.linkLabel6);
            this.Controls.Add(this.linkLabel5);
            this.Controls.Add(this.linkLabel4);
            this.Controls.Add(this.linkLabel3);
            this.Controls.Add(this.linkLabel2);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.ddlOrchestrationName);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtParsedErrorFolder);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtArchiveFolder);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtMaxDehydratedFilesCount);
            this.Controls.Add(this.txtBizTalkFilesCount);
            this.Controls.Add(this.btnTestBizTalk);
            this.Controls.Add(this.btnStartFeeder);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtFvFinOutFolder);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtMaxBizTalFileCount);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtErrorFolder);
            this.Controls.Add(this.txtSplitterFilesCount);
            this.Controls.Add(this.lblCount);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.lblMoveFolder);
            this.Controls.Add(this.txtBizTalkParsedFolder);
            this.Controls.Add(this.lblParsedQty);
            this.Controls.Add(this.lblFvFQty);
            this.Controls.Add(this.txtFvFinIn);
            this.Controls.Add(this.lblFvFIn);
            this.Controls.Add(this.btnStartParser);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(410, 416);
            this.MinimumSize = new System.Drawing.Size(410, 416);
            this.Name = "MainForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BizTalk Files v.8";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Watcher1)).EndInit();
            this.contextMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Watcher2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void ConnectionString_Click(object sender, EventArgs e)
        {
            string connStr = SqlServerHelper.ConnectionString;
            InputForm.ShowDialog(this, ref connStr);
            SqlServerHelper.ConnectionString = connStr;
        }

        private void TestBizTalk_Click(object sender, EventArgs e)
        {
            int bizTalkDehydratedFilesCount = 0;
            if (!int.TryParse(this.txtMaxDehydratedFilesCount.Text, out bizTalkDehydratedFilesCount) || (bizTalkDehydratedFilesCount <= 0) || (bizTalkDehydratedFilesCount > 100))
            {
                this.ShowInformation("Max BizTalk Dehydrated Files should be a valid number (1-100)");
            }
            else
            {
                bool ready = SqlServerHelper.IsBizTalkReady(bizTalkDehydratedFilesCount);
                btnTestBizTalk.BackColor = ready ? Color.Green : Color.Red;
            }
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void LinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            TextBox txt = (TextBox)((LinkLabel)sender).Tag;
            try
            {
                Process.Start(txt.Text);
            }
            catch (Exception ex)
            {
                this.ShowError(ex.Message);
            }
        }

        private void NotifyIcon_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Normal;
                this.Activate();
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            e.Cancel = true;
            base.OnClosing(e);
        }

        private void BAC_File_CreatedChanged(object sender, FileSystemEventArgs e)
        {
            Task.Factory.StartNew((o) =>
            {
                Thread.Sleep(500);

                FileSystemEventArgs args = (FileSystemEventArgs)o;

                try
                {

                    var xml = XElement.Load(args.FullPath);

                    StringBuilder b = new StringBuilder();

                    foreach (var node in xml.Nodes())
                    {
                        var elem = ((XElement)node);

                        foreach (var att in elem.Attributes())
                        {
                            if (att == elem.FirstAttribute)
                            {
                                b.AppendFormat("\"{0}\",", att.Value.TrimSafe());
                            }
                            else if (att == elem.LastAttribute)
                            {
                                b.AppendLine(att.Value.TrimSafe());
                            }
                            else //if (att != elem.LastAttribute)
                            {
                                b.AppendFormat("{0},", att.Value.TrimSafe());
                            }
                        }
                    }

                    var newpath = Path.ChangeExtension(args.FullPath, "csv");
                    if (File.Exists(newpath))
                        File.Delete(newpath);
                    File.WriteAllText(newpath, b.ToString());
                    if (File.Exists(args.FullPath))
                        File.Delete(args.FullPath);
                }
                catch
                {
                    //
                }
            }, e);
        }

        private void Retry_File_CreatedChanged(object sender, FileSystemEventArgs e)
        {
            if (m_FvInfoHolder.IsInitialized)
            {
                Task.Factory.StartNew((o) =>
                {
                    Thread.Sleep(500);

                    FileSystemEventArgs args = (FileSystemEventArgs)o;

                    var file = new FileInfo(args.FullPath);

                    var xml = XElement.Load(args.FullPath);

                    RootElementProcessor processor = new RootElementProcessor(m_FvInfoHolder);
                    processor.Process(xml, file);

                    file.Delete();
                }, e);
            }
        }

        private void Exit2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}

