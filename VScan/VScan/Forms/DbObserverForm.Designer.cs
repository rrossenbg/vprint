using PremierTaxFree.Controls;
namespace PremierTaxFree.Forms
{
    partial class DbObserverForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DbObserverForm));
            this.splitContMain = new System.Windows.Forms.SplitContainer();
            this.dgvData = new System.Windows.Forms.DataGridView();
            this.colSiteID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRetailerID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colVoucherID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiRefresh = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContInner = new System.Windows.Forms.SplitContainer();
            this.pbBarCode = new PremierTaxFree.Controls.PictureBoxEx();
            this.btnDown = new System.Windows.Forms.Styled.VistaButton();
            this.btnUp = new System.Windows.Forms.Styled.VistaButton();
            this.pbVoucher = new PremierTaxFree.Controls.PictureBoxEx();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.splitContMain.Panel1.SuspendLayout();
            this.splitContMain.Panel2.SuspendLayout();
            this.splitContMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvData)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.splitContInner.Panel1.SuspendLayout();
            this.splitContInner.Panel2.SuspendLayout();
            this.splitContInner.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContMain
            // 
            this.splitContMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContMain.Location = new System.Drawing.Point(15, 40);
            this.splitContMain.Name = "splitContMain";
            // 
            // splitContMain.Panel1
            // 
            this.splitContMain.Panel1.Controls.Add(this.dgvData);
            // 
            // splitContMain.Panel2
            // 
            this.splitContMain.Panel2.Controls.Add(this.splitContInner);
            this.splitContMain.Size = new System.Drawing.Size(763, 564);
            this.splitContMain.SplitterDistance = 262;
            this.splitContMain.TabIndex = 0;
            // 
            // dgvData
            // 
            this.dgvData.AllowUserToAddRows = false;
            this.dgvData.AllowUserToDeleteRows = false;
            this.dgvData.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvData.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvData.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colSiteID,
            this.colRetailerID,
            this.colVoucherID});
            this.dgvData.ContextMenuStrip = this.contextMenuStrip1;
            this.dgvData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvData.Location = new System.Drawing.Point(0, 0);
            this.dgvData.MultiSelect = false;
            this.dgvData.Name = "dgvData";
            this.dgvData.RowHeadersVisible = false;
            this.dgvData.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvData.Size = new System.Drawing.Size(262, 564);
            this.dgvData.TabIndex = 0;
            this.dgvData.SelectionChanged += new System.EventHandler(this.DataView_SelectionChanged);
            // 
            // colSiteID
            // 
            this.colSiteID.HeaderText = "SiteID";
            this.colSiteID.Name = "colSiteID";
            this.colSiteID.ReadOnly = true;
            this.colSiteID.Width = 66;
            // 
            // colRetailerID
            // 
            this.colRetailerID.FillWeight = 160F;
            this.colRetailerID.HeaderText = "RetailerID";
            this.colRetailerID.Name = "colRetailerID";
            this.colRetailerID.ReadOnly = true;
            this.colRetailerID.Width = 89;
            // 
            // colVoucherID
            // 
            this.colVoucherID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colVoucherID.HeaderText = "VoucherID";
            this.colVoucherID.Name = "colVoucherID";
            this.colVoucherID.ReadOnly = true;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiRefresh});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(124, 26);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.Menu_Opening);
            // 
            // tsmiRefresh
            // 
            this.tsmiRefresh.Name = "tsmiRefresh";
            this.tsmiRefresh.Size = new System.Drawing.Size(152, 22);
            this.tsmiRefresh.Text = "&Refresh";
            this.tsmiRefresh.Click += new System.EventHandler(this.Refresh_Click);
            // 
            // splitContInner
            // 
            this.splitContInner.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContInner.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContInner.Location = new System.Drawing.Point(0, 0);
            this.splitContInner.Name = "splitContInner";
            this.splitContInner.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContInner.Panel1
            // 
            this.splitContInner.Panel1.Controls.Add(this.pbBarCode);
            // 
            // splitContInner.Panel2
            // 
            this.splitContInner.Panel2.Controls.Add(this.btnDown);
            this.splitContInner.Panel2.Controls.Add(this.btnUp);
            this.splitContInner.Panel2.Controls.Add(this.pbVoucher);
            this.splitContInner.Size = new System.Drawing.Size(497, 564);
            this.splitContInner.SplitterDistance = 90;
            this.splitContInner.TabIndex = 0;
            // 
            // pbBarCode
            // 
            this.pbBarCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbBarCode.Location = new System.Drawing.Point(0, 0);
            this.pbBarCode.Name = "pbBarCode";
            this.pbBarCode.Picture = null;
            this.pbBarCode.ShowCurrent = false;
            this.pbBarCode.Size = new System.Drawing.Size(493, 86);
            this.pbBarCode.TabIndex = 0;
            this.pbBarCode.TabStop = false;
            // 
            // btnDown
            // 
            this.btnDown.BackColor = System.Drawing.Color.Transparent;
            this.btnDown.BackImage = ((System.Drawing.Image)(resources.GetObject("btnDown.BackImage")));
            this.btnDown.ButtonColor = System.Drawing.Color.Transparent;
            this.btnDown.ButtonText = null;
            this.btnDown.Location = new System.Drawing.Point(214, 445);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(100, 25);
            this.btnDown.TabIndex = 2;
            this.btnDown.DoubleClick += new System.EventHandler(this.Button_DoubleClick);
            this.btnDown.Click += new System.EventHandler(this.Button_Click);
            // 
            // btnUp
            // 
            this.btnUp.BackColor = System.Drawing.Color.Transparent;
            this.btnUp.BackImage = ((System.Drawing.Image)(resources.GetObject("btnUp.BackImage")));
            this.btnUp.ButtonColor = System.Drawing.Color.Transparent;
            this.btnUp.ButtonText = null;
            this.btnUp.Location = new System.Drawing.Point(214, 0);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(100, 25);
            this.btnUp.TabIndex = 1;
            this.btnUp.DoubleClick += new System.EventHandler(this.Button_DoubleClick);
            this.btnUp.Click += new System.EventHandler(this.Button_Click);
            // 
            // pbVoucher
            // 
            this.pbVoucher.ContextMenuStrip = this.contextMenuStrip1;
            this.pbVoucher.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbVoucher.Location = new System.Drawing.Point(0, 0);
            this.pbVoucher.Name = "pbVoucher";
            this.pbVoucher.Picture = null;
            this.pbVoucher.ShowCurrent = false;
            this.pbVoucher.Size = new System.Drawing.Size(493, 466);
            this.pbVoucher.TabIndex = 0;
            this.pbVoucher.TabStop = false;
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.Timer_Tick);
            // 
            // DbObserverForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(793, 639);
            this.ContextMenuStrip = this.contextMenuStrip1;
            this.Controls.Add(this.splitContMain);
            this.MaximumSize = new System.Drawing.Size(793, 639);
            this.MinimumSize = new System.Drawing.Size(793, 639);
            this.Name = "DbObserverForm";
            this.Padding = new System.Windows.Forms.Padding(15, 40, 15, 35);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Data Observer";
            this.splitContMain.Panel1.ResumeLayout(false);
            this.splitContMain.Panel2.ResumeLayout(false);
            this.splitContMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvData)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.splitContInner.Panel1.ResumeLayout(false);
            this.splitContInner.Panel2.ResumeLayout(false);
            this.splitContInner.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContMain;
        private System.Windows.Forms.SplitContainer splitContInner;
        private PictureBoxEx pbVoucher;
        private PictureBoxEx pbBarCode;
        private System.Windows.Forms.DataGridView dgvData;
        private System.Windows.Forms.Styled.VistaButton btnDown;
        private System.Windows.Forms.Styled.VistaButton btnUp;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSiteID;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRetailerID;
        private System.Windows.Forms.DataGridViewTextBoxColumn colVoucherID;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsmiRefresh;
        private System.Windows.Forms.Timer timer1;
    }
}