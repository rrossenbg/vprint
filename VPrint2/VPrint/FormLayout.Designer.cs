namespace VPrinting
{
    partial class FormLayout
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
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiAdd = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiClear = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiLoadLayout = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSaveLayout = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiLoadBackground = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPrint = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPrintPreview = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiExit = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiAdd,
            this.tsmiDelete,
            this.tsmiEdit,
            this.toolStripMenuItem3,
            this.tsmiClear,
            this.tsmiLoadLayout,
            this.tsmiSaveLayout,
            this.toolStripMenuItem1,
            this.tsmiLoadBackground,
            this.tsmiPrint,
            this.tsmiPrintPreview,
            this.toolStripMenuItem2,
            this.tsmiExit});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(168, 264);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.ContextMenu_Opening);
            // 
            // tsmiAdd
            // 
            this.tsmiAdd.Name = "tsmiAdd";
            this.tsmiAdd.Size = new System.Drawing.Size(167, 22);
            this.tsmiAdd.Text = "&Add Object";
            this.tsmiAdd.Click += new System.EventHandler(this.AddDelete_Click);
            // 
            // tsmiDelete
            // 
            this.tsmiDelete.Name = "tsmiDelete";
            this.tsmiDelete.Size = new System.Drawing.Size(167, 22);
            this.tsmiDelete.Text = "&Delete Object";
            this.tsmiDelete.Click += new System.EventHandler(this.AddDelete_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(164, 6);
            // 
            // tsmiClear
            // 
            this.tsmiClear.Name = "tsmiClear";
            this.tsmiClear.Size = new System.Drawing.Size(167, 22);
            this.tsmiClear.Text = "&Clear";
            this.tsmiClear.Click += new System.EventHandler(this.Clear_Click);
            // 
            // tsmiLoadLayout
            // 
            this.tsmiLoadLayout.Name = "tsmiLoadLayout";
            this.tsmiLoadLayout.Size = new System.Drawing.Size(167, 22);
            this.tsmiLoadLayout.Text = "&Load Layout";
            this.tsmiLoadLayout.Click += new System.EventHandler(this.LoadLayout_Click);
            // 
            // tsmiSaveLayout
            // 
            this.tsmiSaveLayout.Name = "tsmiSaveLayout";
            this.tsmiSaveLayout.Size = new System.Drawing.Size(167, 22);
            this.tsmiSaveLayout.Text = "&Save Layout";
            this.tsmiSaveLayout.Click += new System.EventHandler(this.SaveLayout_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(164, 6);
            // 
            // tsmiLoadBackground
            // 
            this.tsmiLoadBackground.Name = "tsmiLoadBackground";
            this.tsmiLoadBackground.Size = new System.Drawing.Size(167, 22);
            this.tsmiLoadBackground.Text = "Load &Background";
            this.tsmiLoadBackground.Click += new System.EventHandler(this.LoadImage_Click);
            // 
            // tsmiPrint
            // 
            this.tsmiPrint.Name = "tsmiPrint";
            this.tsmiPrint.Size = new System.Drawing.Size(167, 22);
            this.tsmiPrint.Text = "&Print";
            this.tsmiPrint.Click += new System.EventHandler(this.Print_Click);
            // 
            // tsmiPrintPreview
            // 
            this.tsmiPrintPreview.Name = "tsmiPrintPreview";
            this.tsmiPrintPreview.Size = new System.Drawing.Size(167, 22);
            this.tsmiPrintPreview.Text = "Print Pre&view";
            this.tsmiPrintPreview.Click += new System.EventHandler(this.PrintPreview_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(164, 6);
            // 
            // tsmiExit
            // 
            this.tsmiExit.Name = "tsmiExit";
            this.tsmiExit.Size = new System.Drawing.Size(167, 22);
            this.tsmiExit.Text = "&Exit";
            this.tsmiExit.Click += new System.EventHandler(this.Exit_Click);
            // 
            // tsmiEdit
            // 
            this.tsmiEdit.Name = "tsmiEdit";
            this.tsmiEdit.Size = new System.Drawing.Size(167, 22);
            this.tsmiEdit.Text = "&Edit Object";
            this.tsmiEdit.Click += new System.EventHandler(this.AddDelete_Click);
            // 
            // FormLayout
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(893, 533);
            this.ContextMenuStrip = this.contextMenuStrip1;
            this.KeyPreview = true;
            this.Name = "FormLayout";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Voucher Layout";
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsmiLoadBackground;
        private System.Windows.Forms.ToolStripMenuItem tsmiPrint;
        private System.Windows.Forms.ToolStripMenuItem tsmiDelete;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem tsmiAdd;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem tsmiExit;
        private System.Windows.Forms.ToolStripMenuItem tsmiPrintPreview;
        private System.Windows.Forms.ToolStripMenuItem tsmiSaveLayout;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem tsmiLoadLayout;
        private System.Windows.Forms.ToolStripMenuItem tsmiClear;
        private System.Windows.Forms.ToolStripMenuItem tsmiEdit;
    }
}

