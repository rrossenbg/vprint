namespace VPrinting.Controls
{
    partial class DateTimePicker2
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblMessage = new System.Windows.Forms.Label();
            this.dtPicker = new System.Windows.Forms.DateTimePicker();
            this.cbEnabled = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.Location = new System.Drawing.Point(10, 4);
            this.lblMessage.Name = "label1";
            this.lblMessage.Size = new System.Drawing.Size(30, 13);
            this.lblMessage.TabIndex = 0;
            this.lblMessage.Text = "Date";
            // 
            // dtPicker
            // 
            this.dtPicker.Location = new System.Drawing.Point(46, 4);
            this.dtPicker.Name = "dtPicker";
            this.dtPicker.Size = new System.Drawing.Size(126, 20);
            this.dtPicker.TabIndex = 1;
            // 
            // cbEnable
            // 
            this.cbEnabled.Appearance = System.Windows.Forms.Appearance.Button;
            this.cbEnabled.AutoSize = true;
            this.cbEnabled.Location = new System.Drawing.Point(179, 4);
            this.cbEnabled.Name = "cbEnable";
            this.cbEnabled.Size = new System.Drawing.Size(23, 23);
            this.cbEnabled.TabIndex = 2;
            this.cbEnabled.Text = "..";
            this.cbEnabled.UseVisualStyleBackColor = true;
            this.cbEnabled.CheckedChanged += new System.EventHandler(this.Enable_CheckedChanged);
            // 
            // DateTimePicker2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cbEnabled);
            this.Controls.Add(this.dtPicker);
            this.Controls.Add(this.lblMessage);
            this.Name = "DateTimePicker2";
            this.Size = new System.Drawing.Size(207, 31);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.DateTimePicker dtPicker;
        private System.Windows.Forms.CheckBox cbEnabled;
    }
}
