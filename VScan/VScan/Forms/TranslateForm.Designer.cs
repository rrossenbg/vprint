namespace PremierTaxFree.Forms
{
    partial class TranslateForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TranslateForm));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tbFrom = new System.Windows.Forms.TextBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiCut = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.cbFromLang = new System.Windows.Forms.ToolStripComboBox();
            this.cbToLang = new System.Windows.Forms.ToolStripComboBox();
            this.tsmiTranslate = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiExit = new System.Windows.Forms.ToolStripMenuItem();
            this.tbTo = new System.Windows.Forms.TextBox();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
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
            this.splitContainer1.Panel1.Controls.Add(this.tbFrom);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tbTo);
            this.splitContainer1.Size = new System.Drawing.Size(370, 334);
            this.splitContainer1.SplitterDistance = 157;
            this.splitContainer1.TabIndex = 0;
            // 
            // tbFrom
            // 
            this.tbFrom.ContextMenuStrip = this.contextMenuStrip1;
            this.tbFrom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbFrom.Location = new System.Drawing.Point(0, 0);
            this.tbFrom.Multiline = true;
            this.tbFrom.Name = "tbFrom";
            this.tbFrom.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbFrom.Size = new System.Drawing.Size(370, 157);
            this.tbFrom.TabIndex = 0;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiCut,
            this.tsmiCopy,
            this.tsmiPaste,
            this.toolStripMenuItem3,
            this.cbFromLang,
            this.cbToLang,
            this.tsmiTranslate,
            this.toolStripMenuItem1,
            this.tsmiExit});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(182, 176);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.ContextMenu_Opening);
            // 
            // tsmiCut
            // 
            this.tsmiCut.Name = "tsmiCut";
            this.tsmiCut.Size = new System.Drawing.Size(181, 22);
            this.tsmiCut.Text = "C&ut";
            this.tsmiCut.Click += new System.EventHandler(this.CutCopyPaste_Click);
            // 
            // tsmiCopy
            // 
            this.tsmiCopy.Name = "tsmiCopy";
            this.tsmiCopy.Size = new System.Drawing.Size(181, 22);
            this.tsmiCopy.Text = "&Copy";
            this.tsmiCopy.Click += new System.EventHandler(this.CutCopyPaste_Click);
            // 
            // tsmiPaste
            // 
            this.tsmiPaste.Name = "tsmiPaste";
            this.tsmiPaste.Size = new System.Drawing.Size(181, 22);
            this.tsmiPaste.Text = "&Paste";
            this.tsmiPaste.Click += new System.EventHandler(this.CutCopyPaste_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(178, 6);
            // 
            // cbFromLang
            // 
            this.cbFromLang.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFromLang.Name = "cbFromLang";
            this.cbFromLang.Size = new System.Drawing.Size(121, 21);
            this.cbFromLang.ToolTipText = "From language";
            // 
            // cbToLang
            // 
            this.cbToLang.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbToLang.Name = "cbToLang";
            this.cbToLang.Size = new System.Drawing.Size(121, 21);
            this.cbToLang.ToolTipText = "To language";
            // 
            // tsmiTranslate
            // 
            this.tsmiTranslate.Name = "tsmiTranslate";
            this.tsmiTranslate.Size = new System.Drawing.Size(181, 22);
            this.tsmiTranslate.Text = "&Translate";
            this.tsmiTranslate.Click += new System.EventHandler(this.Translate_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(178, 6);
            // 
            // tsmiExit
            // 
            this.tsmiExit.Name = "tsmiExit";
            this.tsmiExit.Size = new System.Drawing.Size(181, 22);
            this.tsmiExit.Text = "&Exit";
            this.tsmiExit.Click += new System.EventHandler(this.Exit_Click);
            // 
            // tbTo
            // 
            this.tbTo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.tbTo.ContextMenuStrip = this.contextMenuStrip1;
            this.tbTo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbTo.Location = new System.Drawing.Point(0, 0);
            this.tbTo.Multiline = true;
            this.tbTo.Name = "tbTo";
            this.tbTo.ReadOnly = true;
            this.tbTo.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbTo.Size = new System.Drawing.Size(370, 173);
            this.tbTo.TabIndex = 0;
            // 
            // TranslateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(390, 354);
            this.ContextMenuStrip = this.contextMenuStrip1;
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(159, 148);
            this.Name = "TranslateForm";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.Text = "Translate Form";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TextBox tbFrom;
        private System.Windows.Forms.TextBox tbTo;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsmiTranslate;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem tsmiExit;
        private System.Windows.Forms.ToolStripComboBox cbFromLang;
        private System.Windows.Forms.ToolStripComboBox cbToLang;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem tsmiCopy;
        private System.Windows.Forms.ToolStripMenuItem tsmiPaste;
        private System.Windows.Forms.ToolStripMenuItem tsmiCut;
    }
}