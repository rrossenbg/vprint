namespace VCover
{
    partial class MatchForm
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
            this.loadImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadTemplateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createTemplateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.addHiddenAreaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.matchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveMatchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearMatchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.vScroll = new System.Windows.Forms.VScrollBar();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadImageToolStripMenuItem,
            this.loadTemplateToolStripMenuItem,
            this.createTemplateToolStripMenuItem,
            this.toolStripMenuItem2,
            this.addHiddenAreaToolStripMenuItem,
            this.clearToolStripMenuItem,
            this.toolStripMenuItem1,
            this.matchToolStripMenuItem,
            this.saveMatchToolStripMenuItem,
            this.testToolStripMenuItem,
            this.clearMatchToolStripMenuItem,
            this.toolStripMenuItem3,
            this.closeToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(167, 264);
            // 
            // loadImageToolStripMenuItem
            // 
            this.loadImageToolStripMenuItem.Name = "loadImageToolStripMenuItem";
            this.loadImageToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.loadImageToolStripMenuItem.Text = "Load &Image";
            this.loadImageToolStripMenuItem.Click += new System.EventHandler(this.LoadImage_MenuItem_Click);
            // 
            // loadTemplateToolStripMenuItem
            // 
            this.loadTemplateToolStripMenuItem.Name = "loadTemplateToolStripMenuItem";
            this.loadTemplateToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.loadTemplateToolStripMenuItem.Text = "Load &Template";
            this.loadTemplateToolStripMenuItem.Click += new System.EventHandler(this.LoadTemplate_MenuItem_Click);
            // 
            // createTemplateToolStripMenuItem
            // 
            this.createTemplateToolStripMenuItem.Name = "createTemplateToolStripMenuItem";
            this.createTemplateToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.createTemplateToolStripMenuItem.Text = "C&reate Template";
            this.createTemplateToolStripMenuItem.Click += new System.EventHandler(this.CreateTemplateMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(163, 6);
            // 
            // addHiddenAreaToolStripMenuItem
            // 
            this.addHiddenAreaToolStripMenuItem.Name = "addHiddenAreaToolStripMenuItem";
            this.addHiddenAreaToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.addHiddenAreaToolStripMenuItem.Text = "Add Hidden Area";
            this.addHiddenAreaToolStripMenuItem.Click += new System.EventHandler(this.AddHiddenAreaMenuItem_Click);
            // 
            // clearToolStripMenuItem
            // 
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            this.clearToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.clearToolStripMenuItem.Text = "Clea&r Hidden Areas";
            this.clearToolStripMenuItem.Click += new System.EventHandler(this.ClearMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(163, 6);
            // 
            // matchToolStripMenuItem
            // 
            this.matchToolStripMenuItem.Name = "matchToolStripMenuItem";
            this.matchToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.matchToolStripMenuItem.Text = "&Match";
            this.matchToolStripMenuItem.Click += new System.EventHandler(this.Match_MenuItem_Click);
            // 
            // saveMatchToolStripMenuItem
            // 
            this.saveMatchToolStripMenuItem.Name = "saveMatchToolStripMenuItem";
            this.saveMatchToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.saveMatchToolStripMenuItem.Text = "&Save Match";
            this.saveMatchToolStripMenuItem.Click += new System.EventHandler(this.SaveMatchMenuItem_Click);
            // 
            // testToolStripMenuItem
            // 
            this.testToolStripMenuItem.Name = "testToolStripMenuItem";
            this.testToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.testToolStripMenuItem.Text = "&Test";
            this.testToolStripMenuItem.Click += new System.EventHandler(this.TestMenuItem_Click);
            // 
            // clearMatchToolStripMenuItem
            // 
            this.clearMatchToolStripMenuItem.Name = "clearMatchToolStripMenuItem";
            this.clearMatchToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.clearMatchToolStripMenuItem.Text = "C&lear Match";
            this.clearMatchToolStripMenuItem.Click += new System.EventHandler(this.ClearMatchMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(163, 6);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.closeToolStripMenuItem.Text = "&Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.CloseMenuItem_Click);
            // 
            // vScroll
            // 
            this.vScroll.Dock = System.Windows.Forms.DockStyle.Right;
            this.vScroll.Location = new System.Drawing.Point(583, 0);
            this.vScroll.Name = "vScroll";
            this.vScroll.Size = new System.Drawing.Size(16, 756);
            this.vScroll.TabIndex = 1;
            this.vScroll.Scroll += new System.Windows.Forms.ScrollEventHandler(this.ScrollBar1_Scroll);
            // 
            // MatchForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(599, 756);
            this.ContextMenuStrip = this.contextMenuStrip1;
            this.Controls.Add(this.vScroll);
            this.Name = "MatchForm";
            this.Text = "Match Form";
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem loadImageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadTemplateToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem matchToolStripMenuItem;
        private System.Windows.Forms.VScrollBar vScroll;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem addHiddenAreaToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveMatchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createTemplateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearMatchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testToolStripMenuItem;

    }
}