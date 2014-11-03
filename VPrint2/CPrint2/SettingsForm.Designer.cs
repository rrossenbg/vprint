namespace CPrint2
{
    partial class SettingsForm
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
            this.highS = new System.Windows.Forms.TrackBar();
            this.minWidth = new System.Windows.Forms.TrackBar();
            this.minHeight = new System.Windows.Forms.TrackBar();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton4 = new System.Windows.Forms.RadioButton();
            this.lblhighS = new System.Windows.Forms.Label();
            this.lblminWidth = new System.Windows.Forms.Label();
            this.lblminHeight = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.highS)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.minWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.minHeight)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // highS
            // 
            this.highS.Dock = System.Windows.Forms.DockStyle.Top;
            this.highS.Location = new System.Drawing.Point(0, 0);
            this.highS.Maximum = 255;
            this.highS.Name = "highS";
            this.highS.Size = new System.Drawing.Size(244, 42);
            this.highS.TabIndex = 0;
            this.highS.Tag = "High S";
            this.toolTip1.SetToolTip(this.highS, "High S");
            this.highS.Value = 50;
            this.highS.ValueChanged += new System.EventHandler(this.Any_ValueChanged);
            // 
            // minWidth
            // 
            this.minWidth.Dock = System.Windows.Forms.DockStyle.Top;
            this.minWidth.Location = new System.Drawing.Point(0, 55);
            this.minWidth.Maximum = 300;
            this.minWidth.Name = "minWidth";
            this.minWidth.Size = new System.Drawing.Size(244, 42);
            this.minWidth.TabIndex = 1;
            this.minWidth.Tag = "min Width";
            this.toolTip1.SetToolTip(this.minWidth, "min Width");
            this.minWidth.Value = 50;
            this.minWidth.ValueChanged += new System.EventHandler(this.Any_ValueChanged);
            // 
            // minHeight
            // 
            this.minHeight.Dock = System.Windows.Forms.DockStyle.Top;
            this.minHeight.Location = new System.Drawing.Point(0, 110);
            this.minHeight.Maximum = 300;
            this.minHeight.Name = "minHeight";
            this.minHeight.Size = new System.Drawing.Size(244, 42);
            this.minHeight.TabIndex = 2;
            this.minHeight.Tag = "min Height";
            this.toolTip1.SetToolTip(this.minHeight, "min Height");
            this.minHeight.Value = 50;
            this.minHeight.ValueChanged += new System.EventHandler(this.Any_ValueChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButton3);
            this.groupBox1.Controls.Add(this.radioButton2);
            this.groupBox1.Controls.Add(this.radioButton1);
            this.groupBox1.Controls.Add(this.radioButton4);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 165);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(244, 95);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Show mode";
            // 
            // radioButton3
            // 
            this.radioButton3.AutoSize = true;
            this.radioButton3.Dock = System.Windows.Forms.DockStyle.Top;
            this.radioButton3.Location = new System.Drawing.Point(3, 67);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(238, 17);
            this.radioButton3.TabIndex = 2;
            this.radioButton3.TabStop = true;
            this.radioButton3.Text = "Hsv";
            this.radioButton3.UseVisualStyleBackColor = true;
            this.radioButton3.CheckedChanged += new System.EventHandler(this.Radio_ValueChanged);
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Dock = System.Windows.Forms.DockStyle.Top;
            this.radioButton2.Location = new System.Drawing.Point(3, 50);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(238, 17);
            this.radioButton2.TabIndex = 1;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "Thresholded";
            this.radioButton2.UseVisualStyleBackColor = true;
            this.radioButton2.CheckedChanged += new System.EventHandler(this.Radio_ValueChanged);
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Dock = System.Windows.Forms.DockStyle.Top;
            this.radioButton1.Location = new System.Drawing.Point(3, 33);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(238, 17);
            this.radioButton1.TabIndex = 0;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "Original";
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.CheckedChanged += new System.EventHandler(this.Radio_ValueChanged);
            // 
            // radioButton4
            // 
            this.radioButton4.AutoSize = true;
            this.radioButton4.Dock = System.Windows.Forms.DockStyle.Top;
            this.radioButton4.Location = new System.Drawing.Point(3, 16);
            this.radioButton4.Name = "radioButton4";
            this.radioButton4.Size = new System.Drawing.Size(238, 17);
            this.radioButton4.TabIndex = 3;
            this.radioButton4.TabStop = true;
            this.radioButton4.Text = "Normal";
            this.radioButton4.UseVisualStyleBackColor = true;
            this.radioButton4.CheckedChanged += new System.EventHandler(this.Radio_ValueChanged);
            // 
            // lblhighS
            // 
            this.lblhighS.AutoSize = true;
            this.lblhighS.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblhighS.Location = new System.Drawing.Point(0, 42);
            this.lblhighS.Name = "lblhighS";
            this.lblhighS.Size = new System.Drawing.Size(35, 13);
            this.lblhighS.TabIndex = 4;
            this.lblhighS.Text = "label1";
            // 
            // lblminWidth
            // 
            this.lblminWidth.AutoSize = true;
            this.lblminWidth.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblminWidth.Location = new System.Drawing.Point(0, 97);
            this.lblminWidth.Name = "lblminWidth";
            this.lblminWidth.Size = new System.Drawing.Size(35, 13);
            this.lblminWidth.TabIndex = 5;
            this.lblminWidth.Text = "label2";
            // 
            // lblminHeight
            // 
            this.lblminHeight.AutoSize = true;
            this.lblminHeight.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblminHeight.Location = new System.Drawing.Point(0, 152);
            this.lblminHeight.Name = "lblminHeight";
            this.lblminHeight.Size = new System.Drawing.Size(35, 13);
            this.lblminHeight.TabIndex = 6;
            this.lblminHeight.Text = "label3";
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(244, 299);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lblminHeight);
            this.Controls.Add(this.minHeight);
            this.Controls.Add(this.lblminWidth);
            this.Controls.Add(this.minWidth);
            this.Controls.Add(this.lblhighS);
            this.Controls.Add(this.highS);
            this.MinimumSize = new System.Drawing.Size(129, 290);
            this.Name = "SettingsForm";
            this.Text = "Settings";
            ((System.ComponentModel.ISupportInitialize)(this.highS)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.minWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.minHeight)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TrackBar highS;
        private System.Windows.Forms.TrackBar minWidth;
        private System.Windows.Forms.TrackBar minHeight;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButton3;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.Label lblhighS;
        private System.Windows.Forms.Label lblminWidth;
        private System.Windows.Forms.Label lblminHeight;
        private System.Windows.Forms.RadioButton radioButton4;
    }
}