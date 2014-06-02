namespace WinDbLst
{
    partial class MainForm
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
            this.pbBarCode = new System.Windows.Forms.PictureBox();
            this.pbVoucherImage = new System.Windows.Forms.PictureBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.processMessageQueueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tbVoucherText = new System.Windows.Forms.TextBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            ((System.ComponentModel.ISupportInitialize)(this.pbBarCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbVoucherImage)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pbBarCode
            // 
            this.pbBarCode.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pbBarCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbBarCode.Location = new System.Drawing.Point(260, 3);
            this.pbBarCode.Name = "pbBarCode";
            this.pbBarCode.Size = new System.Drawing.Size(353, 120);
            this.pbBarCode.TabIndex = 0;
            this.pbBarCode.TabStop = false;
            // 
            // pbVoucherImage
            // 
            this.pbVoucherImage.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pbVoucherImage.ContextMenuStrip = this.contextMenuStrip1;
            this.pbVoucherImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbVoucherImage.Location = new System.Drawing.Point(0, 0);
            this.pbVoucherImage.Name = "pbVoucherImage";
            this.pbVoucherImage.Size = new System.Drawing.Size(616, 476);
            this.pbVoucherImage.TabIndex = 1;
            this.pbVoucherImage.TabStop = false;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.processMessageQueueToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(203, 26);
            // 
            // processMessageQueueToolStripMenuItem
            // 
            this.processMessageQueueToolStripMenuItem.Name = "processMessageQueueToolStripMenuItem";
            this.processMessageQueueToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
            this.processMessageQueueToolStripMenuItem.Text = "Process Message Queue";
            this.processMessageQueueToolStripMenuItem.Click += new System.EventHandler(this.processMessageQueueToolStripMenuItem_Click);
            // 
            // tbVoucherText
            // 
            this.tbVoucherText.Dock = System.Windows.Forms.DockStyle.Left;
            this.tbVoucherText.Location = new System.Drawing.Point(3, 3);
            this.tbVoucherText.Multiline = true;
            this.tbVoucherText.Name = "tbVoucherText";
            this.tbVoucherText.ReadOnly = true;
            this.tbVoucherText.Size = new System.Drawing.Size(257, 120);
            this.tbVoucherText.TabIndex = 2;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(10, 10);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.pbBarCode);
            this.splitContainer1.Panel1.Controls.Add(this.tbVoucherText);
            this.splitContainer1.Panel1.Padding = new System.Windows.Forms.Padding(3);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.pbVoucherImage);
            this.splitContainer1.Size = new System.Drawing.Size(616, 606);
            this.splitContainer1.SplitterDistance = 126;
            this.splitContainer1.TabIndex = 3;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(636, 626);
            this.Controls.Add(this.splitContainer1);
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(388, 219);
            this.Name = "MainForm";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.Text = "MSSQL data Listener";
            ((System.ComponentModel.ISupportInitialize)(this.pbBarCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbVoucherImage)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pbBarCode;
        private System.Windows.Forms.PictureBox pbVoucherImage;
        private System.Windows.Forms.TextBox tbVoucherText;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem processMessageQueueToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
    }
}

